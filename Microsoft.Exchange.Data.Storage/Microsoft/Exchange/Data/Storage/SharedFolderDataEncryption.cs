using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Xml;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Authentication;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.SoapWebClient;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SharedFolderDataEncryption
	{
		public SharedFolderDataEncryption(ExternalAuthentication externalAuthentication)
		{
			if (!externalAuthentication.Enabled)
			{
				SharedFolderDataEncryption.Tracer.TraceError<SharedFolderDataEncryption>((long)this.GetHashCode(), "{0}: The organization is not federated.", this);
				throw new OrganizationNotFederatedException();
			}
			this.externalAuthentication = externalAuthentication;
		}

		private bool IsMultitenancyEnabled
		{
			get
			{
				return VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled;
			}
		}

		public EncryptionResults Encrypt(IExchangePrincipal mailboxOwner, ExternalUserCollection externalUserCollection, string[] recipients, string sender, string containerClass, string folderId, IFrontEndLocator frontEndLocator, string domainController = null)
		{
			IRecipientSession session = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(domainController, true, ConsistencyMode.IgnoreInvalid, null, mailboxOwner.MailboxInfo.OrganizationId.ToADSessionSettings(), ConfigScopes.TenantSubTree, 121, "Encrypt", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Sharing\\SharedFolderDataEncryption.cs");
			ValidRecipient[] recipients2 = Array.ConvertAll<string, ValidRecipient>(recipients, (string recipient) => new ValidRecipient(recipient, session.FindByProxyAddress(new SmtpProxyAddress(recipient, false))));
			return this.Encrypt(mailboxOwner, session, externalUserCollection, recipients2, sender, containerClass, folderId, frontEndLocator);
		}

		public EncryptionResults Encrypt(IExchangePrincipal mailboxOwner, IRecipientSession recipientSession, ExternalUserCollection externalUserCollection, ValidRecipient[] recipients, string sender, string containerClass, string folderId, IFrontEndLocator frontEndLocator)
		{
			SharingDataType sharingDataType = SharingDataType.FromContainerClass(containerClass);
			if (sharingDataType == null || !sharingDataType.IsExternallySharable)
			{
				throw new ArgumentOutOfRangeException("containerClass");
			}
			ADUser aduser = DirectoryHelper.ReadADRecipient(mailboxOwner.MailboxInfo.MailboxGuid, mailboxOwner.MailboxInfo.IsArchive, recipientSession) as ADUser;
			if (aduser == null)
			{
				SharedFolderDataEncryption.Tracer.TraceError<SharedFolderDataEncryption, string>((long)this.GetHashCode(), "{0}: The Active Directory user was not found. Sender={1}.", this, sender);
				throw new ObjectNotFoundException(ServerStrings.ADUserNotFound);
			}
			ProxyAddress item = new SmtpProxyAddress(sender, false);
			if (!aduser.EmailAddresses.Contains(item))
			{
				SharedFolderDataEncryption.Tracer.TraceError<SharedFolderDataEncryption, string>((long)this.GetHashCode(), "{0}: The SMTP address was not found in the user AD object for this mailbox. Sender={1}.", this, sender);
				throw new ObjectNotFoundException(ServerStrings.ADUserNotFound);
			}
			SharingPolicy sharingPolicy = DirectoryHelper.ReadSharingPolicy(mailboxOwner.MailboxInfo.MailboxGuid, mailboxOwner.MailboxInfo.IsArchive, recipientSession);
			SharedFolderDataEncryption.Tracer.TraceDebug<SharedFolderDataEncryption, object>((long)this.GetHashCode(), "{0}: Sharing policy to be applied to this user: {1}", this, (sharingPolicy == null) ? "<null>" : sharingPolicy.Id);
			SharingPolicyAction sharingPolicyActions = SharedFolderDataEncryption.GetSharingPolicyActions(sharingDataType.StoreObjectType);
			SharedFolderDataRecipient[] externalIdentities = SharedFolderDataEncryption.GetExternalIdentities(externalUserCollection, recipients);
			List<InvalidRecipient> list = new List<InvalidRecipient>();
			Dictionary<TokenTarget, List<SharedFolderDataRecipient>> dictionary = new Dictionary<TokenTarget, List<SharedFolderDataRecipient>>(externalIdentities.Length, SharedFolderDataEncryption.TokenTargetComparer);
			for (int i = 0; i < recipients.Length; i++)
			{
				SharedFolderDataRecipient item2 = externalIdentities[i];
				ValidRecipient validRecipient = recipients[i];
				SmtpAddress smtpAddress = new SmtpAddress(validRecipient.SmtpAddress);
				string domain = smtpAddress.Domain;
				if (sharingPolicy == null || !sharingPolicy.IsAllowedForAnySharing(domain, sharingPolicyActions))
				{
					SharedFolderDataEncryption.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Sharing policy does not allow user to share with domain {0}", domain);
					list.Add(new InvalidRecipient(validRecipient.SmtpAddress, InvalidRecipientResponseCodeType.SystemPolicyBlocksSharingWithThisRecipient));
				}
				else
				{
					SmtpAddress smtpAddress2 = new SmtpAddress(validRecipient.SmtpAddressForEncryption);
					TokenTarget tokenTarget = TargetUriResolver.Resolve(smtpAddress2.Domain, aduser.OrganizationId);
					if (tokenTarget == null)
					{
						list.Add(new InvalidRecipient(validRecipient.SmtpAddress, InvalidRecipientResponseCodeType.RecipientOrganizationNotFederated));
					}
					else
					{
						List<SharedFolderDataRecipient> list2;
						if (!dictionary.TryGetValue(tokenTarget, out list2))
						{
							list2 = new List<SharedFolderDataRecipient>(1);
							dictionary.Add(tokenTarget, list2);
						}
						list2.Add(item2);
					}
				}
			}
			List<EncryptedSharedFolderData> list3 = new List<EncryptedSharedFolderData>(dictionary.Count);
			SharedFolderData sharedFolderData = new SharedFolderData();
			sharedFolderData.DataType = sharingDataType.ExternalName;
			sharedFolderData.FolderId = folderId;
			sharedFolderData.SharingUrl = this.GetSharingUrl(aduser, frontEndLocator);
			sharedFolderData.FederationUri = this.externalAuthentication.TokenValidator.TargetUri.ToString();
			sharedFolderData.SenderSmtpAddress = sender;
			DelegationTokenRequest delegationTokenRequest = new DelegationTokenRequest
			{
				FederatedIdentity = aduser.GetFederatedIdentity(),
				EmailAddress = aduser.GetFederatedSmtpAddress(new SmtpAddress(sender)).ToString(),
				Offer = Offer.SharingInviteMessage
			};
			SecurityTokenService securityTokenService = this.externalAuthentication.GetSecurityTokenService(aduser.OrganizationId);
			foreach (KeyValuePair<TokenTarget, List<SharedFolderDataRecipient>> keyValuePair in dictionary)
			{
				delegationTokenRequest.Target = keyValuePair.Key;
				sharedFolderData.Recipients = keyValuePair.Value.ToArray();
				try
				{
					RequestedToken requestedToken = securityTokenService.IssueToken(delegationTokenRequest);
					list3.Add(this.Encrypt(requestedToken, sharedFolderData));
				}
				catch (WSTrustException ex)
				{
					foreach (SharedFolderDataRecipient sharedFolderDataRecipient in sharedFolderData.Recipients)
					{
						list.Add(new InvalidRecipient(sharedFolderDataRecipient.SmtpAddress, SharedFolderDataEncryption.GetResponseCodeFromException(ex), ex.ToString()));
					}
				}
			}
			return new EncryptionResults(list3.ToArray(), list.ToArray());
		}

		public SharedFolderData Decrypt(EncryptedSharedFolderData[] encryptedSharedFolderDataCollection)
		{
			foreach (EncryptedSharedFolderData encryptedSharedFolderData in encryptedSharedFolderDataCollection)
			{
				SharedFolderData sharedFolderData = this.TryDecrypt(encryptedSharedFolderData);
				if (sharedFolderData != null)
				{
					return sharedFolderData;
				}
			}
			throw new InvalidEncryptedSharedFolderDataException();
		}

		public SharedFolderData TryDecrypt(EncryptedSharedFolderData encryptedSharedFolderData)
		{
			if (encryptedSharedFolderData.Token == null)
			{
				SharedFolderDataEncryption.Tracer.TraceError<SharedFolderDataEncryption>((long)this.GetHashCode(), "{0}: EncryptedSharedFolderData is missing Token element.", this);
				return null;
			}
			if (encryptedSharedFolderData.Token.EncryptedData == null)
			{
				SharedFolderDataEncryption.Tracer.TraceError<SharedFolderDataEncryption>((long)this.GetHashCode(), "{0}: EncryptedSharedFolderData.Token is missing <EncryptedData> element.", this);
				return null;
			}
			if (encryptedSharedFolderData.Data == null)
			{
				SharedFolderDataEncryption.Tracer.TraceError<SharedFolderDataEncryption>((long)this.GetHashCode(), "{0}: EncryptedSharedFolderData is missing <Data> element.", this);
				return null;
			}
			if (encryptedSharedFolderData.Data.EncryptedData == null)
			{
				SharedFolderDataEncryption.Tracer.TraceError<SharedFolderDataEncryption>((long)this.GetHashCode(), "{0}: EncryptedSharedFolderData.Data is missing <EncryptedData> element.", this);
				return null;
			}
			TokenValidationResults tokenValidationResults = this.externalAuthentication.TokenValidator.ValidateToken(encryptedSharedFolderData.Token.EncryptedData, Offer.SharingInviteMessage);
			if (tokenValidationResults.Result != TokenValidationResult.Valid)
			{
				SharedFolderDataEncryption.Tracer.TraceError<SharedFolderDataEncryption, TokenValidationResults>((long)this.GetHashCode(), "{0}: Token is not valid. TokenValidationResults={1}", this, tokenValidationResults);
				return null;
			}
			SymmetricSecurityKey proofToken = tokenValidationResults.ProofToken;
			if (proofToken == null)
			{
				SharedFolderDataEncryption.Tracer.TraceError<SharedFolderDataEncryption>((long)this.GetHashCode(), "{0}: Unable to retrieve the security key from the token.", this);
				return null;
			}
			XmlElement xmlElement;
			try
			{
				xmlElement = SymmetricEncryptedXml.Decrypt(encryptedSharedFolderData.Data.EncryptedData, proofToken);
			}
			catch (CryptographicException arg)
			{
				SharedFolderDataEncryption.Tracer.TraceError<SharedFolderDataEncryption, CryptographicException>((long)this.GetHashCode(), "{0}: Unable to decrypt the data element. Exception={1}", this, arg);
				return null;
			}
			SharedFolderData result;
			try
			{
				result = SharedFolderData.DeserializeFromXmlELement(xmlElement);
			}
			catch (InvalidOperationException arg2)
			{
				SharedFolderDataEncryption.Tracer.TraceError<SharedFolderDataEncryption, InvalidOperationException>((long)this.GetHashCode(), "{0}: Unable to deserialize the data element. InvalidOperationException={1}", this, arg2);
				result = null;
			}
			catch (XmlException arg3)
			{
				SharedFolderDataEncryption.Tracer.TraceError<SharedFolderDataEncryption, XmlException>((long)this.GetHashCode(), "{0}: Unable to deserialize the data element. XmlException={1}", this, arg3);
				result = null;
			}
			return result;
		}

		private static InvalidRecipientResponseCodeType GetResponseCodeFromException(WSTrustException exception)
		{
			if (exception is UnknownTokenIssuerException)
			{
				return InvalidRecipientResponseCodeType.RecipientOrganizationFederatedWithUnknownTokenIssuer;
			}
			if (exception is SoapFaultException || exception is RequestSecurityTokenResponseException)
			{
				return InvalidRecipientResponseCodeType.RecipientOrganizationNotFederated;
			}
			return InvalidRecipientResponseCodeType.OtherError;
		}

		private static SharingPolicyAction GetSharingPolicyActions(StoreObjectType storeObjectType)
		{
			switch (storeObjectType)
			{
			case StoreObjectType.CalendarFolder:
				return SharingPolicyAction.CalendarSharingFreeBusySimple | SharingPolicyAction.CalendarSharingFreeBusyDetail | SharingPolicyAction.CalendarSharingFreeBusyReviewer;
			case StoreObjectType.ContactsFolder:
				return SharingPolicyAction.ContactsSharing;
			default:
				throw new ArgumentOutOfRangeException("storeObjectType");
			}
		}

		private static SharedFolderDataRecipient[] GetExternalIdentities(ExternalUserCollection externalUserCollection, ValidRecipient[] recipients)
		{
			SharedFolderDataRecipient[] result = Array.ConvertAll<ValidRecipient, SharedFolderDataRecipient>(recipients, (ValidRecipient recipient) => SharedFolderDataEncryption.GetExternalIdentity(externalUserCollection, recipient));
			externalUserCollection.Save();
			return result;
		}

		private static SharedFolderDataRecipient GetExternalIdentity(ExternalUserCollection externalUserCollection, ValidRecipient recipient)
		{
			if (!SmtpAddress.IsValidSmtpAddress(recipient.SmtpAddress))
			{
				throw new ArgumentOutOfRangeException(ServerStrings.InvalidSmtpAddress(recipient.SmtpAddress));
			}
			SmtpAddress smtpAddress = new SmtpAddress(recipient.SmtpAddress);
			ExternalUser externalUser = externalUserCollection.FindFederatedUserWithOriginalSmtpAddress(smtpAddress);
			if (externalUser == null)
			{
				externalUser = externalUserCollection.AddFederatedUser(smtpAddress);
			}
			return new SharedFolderDataRecipient
			{
				SmtpAddress = recipient.SmtpAddressForEncryption,
				SharingKey = externalUser.ExternalId.ToString()
			};
		}

		private string GetSharingUrl(ADUser user, IFrontEndLocator frontEndLocator)
		{
			ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromADUser(user, null);
			if (exchangePrincipal.MailboxInfo.Location.ServerVersion >= Server.E15MinVersion && this.IsMultitenancyEnabled)
			{
				return this.GetE15MultitenancySharingUrl(exchangePrincipal, frontEndLocator);
			}
			return this.GetEnterpriseOrE14SharingUrl(exchangePrincipal);
		}

		private string GetE15MultitenancySharingUrl(ExchangePrincipal exchangePrincipal, IFrontEndLocator frontEndLocator)
		{
			SharedFolderDataEncryption.Tracer.TraceDebug<ExchangePrincipal>((long)this.GetHashCode(), "Entering GetE15MultitenancySharingUrl for mailbox {0}", exchangePrincipal);
			Uri uri = null;
			Exception ex = null;
			try
			{
				uri = frontEndLocator.GetWebServicesUrl(exchangePrincipal);
			}
			catch (ServerNotFoundException ex2)
			{
				ex = ex2;
			}
			catch (ADTransientException ex3)
			{
				ex = ex3;
			}
			catch (DataSourceOperationException ex4)
			{
				ex = ex4;
			}
			catch (DataValidationException ex5)
			{
				ex = ex5;
			}
			finally
			{
				if (ex != null)
				{
					throw new NoExternalEwsAvailableException(ex);
				}
			}
			string text = uri.ToString();
			SharedFolderDataEncryption.Tracer.TraceDebug<string>((long)this.GetHashCode(), "GetE15MultitenancySharingUrl - EWS url '{0}'", text);
			string text2 = EwsWsSecurityUrl.Fix(text);
			SharedFolderDataEncryption.Tracer.TraceDebug<string>((long)this.GetHashCode(), "GetE15MultitenancySharingUrl - fixed EWS url '{0}'", text2);
			return text2;
		}

		private string GetEnterpriseOrE14SharingUrl(ExchangePrincipal exchangePrincipal)
		{
			SharedFolderDataEncryption.Tracer.TraceDebug<ExchangePrincipal, bool>((long)this.GetHashCode(), "Entering GetEnterpriseOrE14SharingUrl - mailbox {0}, isMultitenancyEnabled={1}", exchangePrincipal, this.IsMultitenancyEnabled);
			ServiceTopology serviceTopology = this.IsMultitenancyEnabled ? ServiceTopology.GetCurrentLegacyServiceTopology("f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Sharing\\SharedFolderDataEncryption.cs", "GetEnterpriseOrE14SharingUrl", 655) : ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Sharing\\SharedFolderDataEncryption.cs", "GetEnterpriseOrE14SharingUrl", 655);
			IList<WebServicesService> list = serviceTopology.FindAll<WebServicesService>(exchangePrincipal, ClientAccessType.External, SharedFolderDataEncryption.serviceVersionFilter, "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Sharing\\SharedFolderDataEncryption.cs", "GetEnterpriseOrE14SharingUrl", 658);
			if (list.Count != 0)
			{
				return EwsWsSecurityUrl.Fix(list[0].Url.ToString());
			}
			SharedFolderDataEncryption.Tracer.TraceDebug<ExchangePrincipal>((long)this.GetHashCode(), "Unable to find a CAS with external access in same site of user {0}. Trying other sites.", exchangePrincipal);
			WebServicesService webServicesService = serviceTopology.FindAny<WebServicesService>(ClientAccessType.External, SharedFolderDataEncryption.serviceVersionFilter, "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Sharing\\SharedFolderDataEncryption.cs", "GetEnterpriseOrE14SharingUrl", 671);
			if (webServicesService == null)
			{
				throw new NoExternalEwsAvailableException();
			}
			return EwsWsSecurityUrl.Fix(webServicesService.Url.ToString());
		}

		private EncryptedSharedFolderData Encrypt(RequestedToken requestedToken, SharedFolderData sharedFolderData)
		{
			XmlElement xmlElement;
			try
			{
				xmlElement = sharedFolderData.SerializeToXmlElement();
			}
			catch (InvalidOperationException innerException)
			{
				throw new UnableToGenerateEncryptedSharedFolderDataException(innerException);
			}
			XmlElement encryptedData;
			try
			{
				encryptedData = SymmetricEncryptedXml.Encrypt(xmlElement, requestedToken.ProofToken);
			}
			catch (CryptographicException innerException2)
			{
				throw new UnableToGenerateEncryptedSharedFolderDataException(innerException2);
			}
			return new EncryptedSharedFolderData
			{
				Token = new EncryptedDataContainer
				{
					EncryptedData = requestedToken.SecurityToken
				},
				Data = new EncryptedDataContainer
				{
					EncryptedData = encryptedData
				}
			};
		}

		private const SharingPolicyAction SharingPolicyCalendarActions = SharingPolicyAction.CalendarSharingFreeBusySimple | SharingPolicyAction.CalendarSharingFreeBusyDetail | SharingPolicyAction.CalendarSharingFreeBusyReviewer;

		private const SharingPolicyAction SharingPolicyContactsActions = SharingPolicyAction.ContactsSharing;

		private static readonly Trace Tracer = ExTraceGlobals.SharingTracer;

		private static readonly Predicate<WebServicesService> serviceVersionFilter = (WebServicesService service) => service.ServerVersionNumber >= Server.E14MinVersion;

		private static readonly SharedFolderDataEncryption.TokenTargetEqualityComparer TokenTargetComparer = new SharedFolderDataEncryption.TokenTargetEqualityComparer();

		private ExternalAuthentication externalAuthentication;

		private sealed class TokenTargetEqualityComparer : IEqualityComparer<TokenTarget>
		{
			public bool Equals(TokenTarget x, TokenTarget y)
			{
				return x == y || (x != null && y != null && x.Uri.Equals(y.Uri));
			}

			public int GetHashCode(TokenTarget tokenTarget)
			{
				return tokenTarget.Uri.GetHashCode();
			}
		}
	}
}
