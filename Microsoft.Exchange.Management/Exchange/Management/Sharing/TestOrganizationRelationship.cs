using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Net;
using System.Text;
using System.Web.Services.Protocols;
using System.Xml;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Authentication;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.SoapWebClient;
using Microsoft.Exchange.SoapWebClient.AutoDiscover;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.Sharing
{
	[Cmdlet("Test", "OrganizationRelationship", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class TestOrganizationRelationship : GetMultitenancySystemConfigurationObjectTask<OrganizationRelationshipIdParameter, OrganizationRelationship>
	{
		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = true)]
		public RecipientIdParameter UserIdentity
		{
			get
			{
				return (RecipientIdParameter)base.Fields["UserIdentity"];
			}
			set
			{
				base.Fields["UserIdentity"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageTestOrganizationRelationship;
			}
		}

		private string[] TrustedHostnames
		{
			get
			{
				if (this.trustedHostnames == null)
				{
					this.trustedHostnames = base.RootOrgGlobalConfigSession.GetAutodiscoverTrustedHosters();
				}
				return this.trustedHostnames;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			IRecipientSession session = this.CreateTenantGlobalCatalogSession(base.SessionSettings);
			this.adUser = (base.GetDataObject<ADUser>(this.UserIdentity, session, this.RootId, null, null) as ADUser);
			if (this.adUser == null)
			{
				base.WriteError(new LocalizedException(Strings.ErrorObjectNotFound(this.UserIdentity.ToString())), ErrorCategory.InvalidArgument, null);
			}
			else if (this.Identity == null)
			{
				base.WriteError(new LocalizedException(Strings.ExceptionMandatoryParameter("Identity")), ErrorCategory.InvalidArgument, null);
			}
			this.organizationRelationship = (base.GetDataObject(this.Identity) as OrganizationRelationship);
			if (this.organizationRelationship == null)
			{
				base.WriteError(new LocalizedException(Strings.ErrorObjectNotFound(this.Identity.ToString())), ErrorCategory.InvalidArgument, null);
			}
			if (!this.adUser.OrganizationId.Equals(this.organizationRelationship.OrganizationId))
			{
				base.WriteError(new InvalidOperationException(Strings.MismatchedUser(this.adUser.Identity.ToString(), this.adUser.OrganizationId.ToString(), this.organizationRelationship.DistinguishedName, this.organizationRelationship.OrganizationId.ToString())), ErrorCategory.InvalidOperation, null);
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			bool flag = true;
			this.testOrganizationRelationshipTestResults = new List<TestOrganizationRelationshipResult>();
			this.ValidateAndSetUpBasicTestConfiguration();
			string msg = string.Format("Begin testing for organization relationship {0}, enabled state {1}.", this.organizationRelationship.DistinguishedName, this.organizationRelationship.Enabled.ToString());
			this.WriteStringPrependNewLine(msg);
			msg = string.Format("Exchange D-Auth Federation Authentication STS Client Identities are {0}", this.externalAuthentication.SecurityTokenServicesIdentifiers);
			this.WriteStringPrependNewLine(msg);
			if (this.externalAuthentication.SubFailureType == ExternalAuthentication.ExternalAuthenticationSubFailureType.WarningApplicationUriSkipped)
			{
				this.WriteWarning(Strings.ApplicationUrisSkipped);
			}
			this.PrintConfiguration();
			this.WriteStringPrependNewLine("STEP 1: Validating user configuration");
			flag = this.ValidateUserConfiguration();
			if (flag)
			{
				this.WriteStringPrependNewLine("RESULT: Success.");
				this.WriteStringPrependNewLine("STEP 2: Getting federation information from remote organization...");
				this.remoteFederatedDomainFromFedInfo = null;
				List<GetFederationInformationResult> federationInformation = this.GetFederationInformation();
				try
				{
					if (federationInformation == null || federationInformation.Count == 0)
					{
						this.WriteString("RESULT: Unable to retrieve federation information from remote organization.  Doing local testing only.");
						flag = this.TestOrganizationRelationshipLocalOnly();
					}
					else
					{
						this.WriteStringPrependNewLine("RESULT: Success.");
						flag = this.TestOrganizationRelationshipWithFederationInformation(federationInformation);
					}
				}
				catch (Exception ex)
				{
					this.WriteString("RESULT: Error.");
					this.WriteResults();
					throw ex;
				}
			}
			if (flag)
			{
				this.WriteString("RESULT: Success.");
			}
			else
			{
				this.WriteString("RESULT: Error.");
			}
			this.WriteStringPrependNewLine("LAST STEP: Writing results...");
			this.WriteResults();
			this.WriteStringPrependNewLine("COMPLETE." + Environment.NewLine);
			TaskLogger.LogExit();
		}

		private void ValidateAndSetUpBasicTestConfiguration()
		{
			bool enabled = VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled;
			ExternalAuthentication.ForceReset();
			this.externalAuthentication = ExternalAuthentication.GetCurrent();
			if (!this.externalAuthentication.Enabled)
			{
				base.WriteError(new InvalidOperationException(Strings.LocalFederationConfigurationError(this.externalAuthentication.FailureType.ToString(), this.externalAuthentication.SubFailureType.ToString())), ErrorCategory.InvalidOperation, null);
			}
			if (this.organizationRelationship.DomainNames == null || this.organizationRelationship.DomainNames.Count == 0)
			{
				base.WriteError(new InvalidOperationException(Strings.NoDomainsDefinedOnOrgRelationship(this.Identity.ToString())), ErrorCategory.InvalidOperation, null);
			}
			if (enabled)
			{
				SmtpAddress windowsLiveID = this.adUser.WindowsLiveID;
				if (this.adUser.WindowsLiveID.ToString().Length == 0)
				{
					base.WriteError(new InvalidOperationException(Strings.ErrorWindowsLiveIdRequired(this.adUser.Identity.ToString())), ErrorCategory.InvalidOperation, null);
				}
			}
			this.orIdName = ((this.adUser.OrganizationId == OrganizationId.ForestWideOrgId) ? "First Organization" : this.adUser.OrganizationId.ToString());
			this.userFederatedIdentity = this.adUser.GetFederatedIdentity();
			if (this.userFederatedIdentity == null || this.userFederatedIdentity.Identity == null)
			{
				base.WriteError(new InvalidOperationException(Strings.UserHasNoFederatedIdentity(this.UserIdentity.ToString())), ErrorCategory.InvalidOperation, null);
			}
			else
			{
				this.userFederatedDomainName = SmtpAddress.Parse(this.userFederatedIdentity.Identity).Domain;
				this.userFederatedDomainNameWOWellKnown = FederatedOrganizationId.RemoveHybridConfigurationWellKnownSubDomain(this.userFederatedDomainName);
			}
			this.securityTokenService = this.externalAuthentication.GetSecurityTokenService(this.adUser.OrganizationId);
			if (this.securityTokenService == null)
			{
				base.WriteError(new InvalidOperationException(Strings.CannotAcquireSTSClientForUser(this.adUser.Identity.ToString(), this.orIdName, this.externalAuthentication.FailureType.ToString(), this.externalAuthentication.SubFailureType.ToString())), ErrorCategory.InvalidOperation, null);
			}
			this.organizationIdCacheValue = OrganizationIdCache.Singleton.Get(this.organizationRelationship.OrganizationId);
			this.federatedOrganizationId = this.organizationIdCacheValue.FederatedOrganizationId;
			if (this.federatedOrganizationId.DelegationTrustLink == null)
			{
				base.WriteError(new InvalidOperationException(Strings.OrganizationHasNoFederation(this.orIdName)), ErrorCategory.InvalidOperation, null);
			}
			this.defaultFederatedDomain = (this.organizationIdCacheValue.DefaultFederatedDomain ?? string.Empty);
			this.accountNamespace = (this.federatedOrganizationId.AccountNamespace.Domain ?? string.Empty);
			if (!enabled && string.IsNullOrEmpty(this.accountNamespace))
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorNoTrustConfigured), ErrorCategory.InvalidOperation, null);
			}
			this.localFederatedDomainsFromFedInfo = new HashSet<string>(this.organizationIdCacheValue.FederatedDomains, StringComparer.OrdinalIgnoreCase);
			if (this.localFederatedDomainsFromFedInfo.Count == 0)
			{
				base.WriteError(new InvalidOperationException(Strings.OrganizationHasNoFederatedDomains(this.orIdName)), ErrorCategory.InvalidOperation, null);
			}
			if (!this.federatedOrganizationId.Enabled)
			{
				TestOrganizationRelationshipResult testOrganizationRelationshipResult = new TestOrganizationRelationshipResult
				{
					Id = TestOrganizationRelationshipResultId.FederatedOrganizationIdNotEnabled,
					Status = Strings.TestOrganizationRelationshipWarning,
					Description = Strings.WarningFederatedOrgIdNotEnabled
				};
				this.testOrganizationRelationshipTestResults.Add(testOrganizationRelationshipResult);
				base.WriteVerbose(testOrganizationRelationshipResult.Description);
			}
			this.localOrgRelDomainNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (SmtpDomain smtpDomain in this.organizationRelationship.DomainNames)
			{
				this.localOrgRelDomainNames.Add(smtpDomain.Domain);
			}
		}

		private bool TestOrganizationRelationshipLocalOnly()
		{
			this.WriteStringPrependNewLine("STEP 3: Requesting delegation token from the STS...");
			RequestedToken delegationToken = this.GetDelegationToken();
			if (delegationToken == null)
			{
				base.WriteVerbose(new LocalizedString("Error. Attempted to get delegation token, but token came back as null."));
				return false;
			}
			this.WriteStringPrependNewLine("RESULT: Success.");
			this.WriteString(string.Concat(new object[]
			{
				"Retrieved token for target ",
				this.organizationRelationship.TargetAutodiscoverEpr,
				" for offer ",
				Offer.Autodiscover.ToString()
			}));
			this.WriteStringPrependNewLine("STEP 4: Getting organization relationship settings from remote partner...");
			AutodiscoverResultData remoteOrganizationRelationshipSettings = this.GetRemoteOrganizationRelationshipSettings(delegationToken);
			if (remoteOrganizationRelationshipSettings != null)
			{
				this.WriteStringPrependNewLine("RESULT: Success.");
				this.WriteStringPrependNewLine("STEP 5: Validating organization relationships returned from remote partner");
				return this.VerifyOrganizationRelationships(remoteOrganizationRelationshipSettings);
			}
			this.WriteStringPrependNewLine("RESULT: Unable to retrieve organization relationships from remote organization.");
			return false;
		}

		private bool TestOrganizationRelationshipWithFederationInformation(List<GetFederationInformationResult> getFederationInformationResults)
		{
			this.WriteStringPrependNewLine("STEP 3: Validating consistency in returned federation information");
			this.remoteFederatedDomainFromFedInfo = this.FindValidRemoteFederationInformation(getFederationInformationResults);
			if (this.remoteFederatedDomainFromFedInfo == null)
			{
				return false;
			}
			this.WriteStringPrependNewLine("RESULT: Success.");
			List<string> list = new List<string>();
			foreach (SmtpDomain smtpDomain in this.organizationRelationship.DomainNames)
			{
				if (this.remoteFederatedDomainFromFedInfo != null)
				{
					if (this.remoteFederatedDomainFromFedInfo.Contains(smtpDomain.Domain))
					{
						list.Add(smtpDomain.Domain + " (Defined Both Local and Remote)");
					}
					else
					{
						list.Add(smtpDomain.Domain + " (Defined Local Only)");
					}
				}
				else
				{
					list.Add(smtpDomain.Domain + " (No remote info avail)");
				}
			}
			StringBuilder stringBuilder = new StringBuilder(1024);
			stringBuilder.AppendLine("Local organization relationship domain configuration...");
			stringBuilder.AppendLine("---------------------------------------------------------------------");
			stringBuilder.AppendLine(string.Join(", ", list) + Environment.NewLine);
			base.WriteVerbose(new LocalizedString(stringBuilder.ToString()));
			if (!this.remoteFederatedDomainFromFedInfo.Overlaps(this.localOrgRelDomainNames))
			{
				TestOrganizationRelationshipResult testOrganizationRelationshipResult = new TestOrganizationRelationshipResult
				{
					Id = TestOrganizationRelationshipResultId.NoRemoteFederatedDomainInLocalOrgRelationship,
					Status = Strings.TestOrganizationRelationshipWarning,
					Description = Strings.NoRemoteFederatedDomainInLocalOrganizationRelationship(this.organizationRelationship.Name)
				};
				this.testOrganizationRelationshipTestResults.Add(testOrganizationRelationshipResult);
				base.WriteVerbose(testOrganizationRelationshipResult.Description);
			}
			this.WriteStringPrependNewLine("STEP 4: Requesting delegation token from the STS...");
			RequestedToken delegationToken = this.GetDelegationToken();
			if (delegationToken == null)
			{
				base.WriteVerbose(new LocalizedString("Error. Attempted to get delegation token, but token came back as null."));
				return false;
			}
			this.WriteStringPrependNewLine("RESULT: Success.");
			this.WriteString(string.Concat(new object[]
			{
				"Retrieved token for target ",
				this.organizationRelationship.TargetAutodiscoverEpr,
				" for offer ",
				Offer.Autodiscover.ToString()
			}));
			this.WriteStringPrependNewLine("STEP 5: Getting organization relationship setting from remote partner...");
			AutodiscoverResultData remoteOrganizationRelationshipSettings = this.GetRemoteOrganizationRelationshipSettings(delegationToken);
			if (remoteOrganizationRelationshipSettings != null)
			{
				this.WriteStringPrependNewLine("RESULT: Success.");
				this.WriteStringPrependNewLine("STEP 6: Validating organization relationships returned from remote partner");
				return this.VerifyOrganizationRelationships(remoteOrganizationRelationshipSettings);
			}
			this.WriteStringPrependNewLine("RESULT: Unable to retrieve organization relationships from remote organization.");
			return false;
		}

		private List<GetFederationInformationResult> GetFederationInformation()
		{
			List<GetFederationInformationResult> list = new List<GetFederationInformationResult>(this.organizationRelationship.DomainNames.Count);
			Uri uri = this.organizationRelationship.TargetAutodiscoverEpr;
			if (uri != null)
			{
				uri = EwsWsSecurityUrl.FixForAnonymous(uri);
			}
			string value;
			using (AutodiscoverClient autodiscoverClient = new AutodiscoverClient())
			{
				this.ConfigureAutodiscoverClientBinding(autodiscoverClient);
				base.WriteVerbose(Strings.CallingGetFederationInformation);
				foreach (SmtpDomain smtpDomain in this.organizationRelationship.DomainNames)
				{
					if (uri != null)
					{
						value = string.Format("Getting Federation information for domain {0} from endpoint {1}.", smtpDomain.Domain, uri);
						base.WriteVerbose(new LocalizedString(value));
						try
						{
							list.Add(GetFederationInformationClient.Endpoint(autodiscoverClient, uri, smtpDomain.Domain));
							goto IL_141;
						}
						catch (Exception exception)
						{
							value = string.Format("Unable to retrieve federation information for domain {0} from endpoint {1}. Exception={2}", smtpDomain.Domain, uri, TestOrganizationRelationship.GetExtendedExceptionInformation(exception));
							base.WriteVerbose(new LocalizedString(value));
							goto IL_141;
						}
						goto IL_D5;
					}
					goto IL_D5;
					IL_141:
					value = string.Format("Get Federation Information complete for {0}.", smtpDomain.Domain);
					base.WriteVerbose(new LocalizedString(value));
					continue;
					IL_D5:
					this.WriteWarning(new LocalizedString("AutodiscoverUrl was null, so AutodiscoverClient will discover the autodiscover endpoint. This might produce undesirable results."));
					value = string.Format("Getting Federation information for domain {0} via standard autodiscover.", smtpDomain.Domain);
					base.WriteVerbose(new LocalizedString(value));
					try
					{
						list.AddRange(GetFederationInformationClient.Discover(autodiscoverClient, smtpDomain.Domain));
					}
					catch (Exception exception2)
					{
						value = string.Format("Unable to retrieve federation information for domain {0} via standard autodiscover. Exception={1}", smtpDomain.Domain, TestOrganizationRelationship.GetExtendedExceptionInformation(exception2));
						base.WriteVerbose(new LocalizedString(value));
					}
					goto IL_141;
				}
			}
			foreach (GetFederationInformationResult getFederationInformationResult in list)
			{
				if (getFederationInformationResult.Type == AutodiscoverResult.Success)
				{
					base.WriteVerbose(Strings.AutodiscoverServiceCallSucceeded(getFederationInformationResult.Url.ToString()));
					value = string.Format("Federation Information Result: {0}", getFederationInformationResult.ToString());
					base.WriteVerbose(new LocalizedString(value));
				}
				else
				{
					base.WriteVerbose(new LocalizedString("FailureToCallAutodiscoverService. In GetFederationInformation, at least one of the results in GetFederationInformationResults did not result in an AutodiscoverResult.Success. Check for issues with GetFederationInformationClient.Endpoint and GetFederationInformationClient.Discover called in GetFederationInformation()."));
					base.WriteVerbose(Strings.FailureToCallAutodiscoverService(getFederationInformationResult.Url.ToString(), TestOrganizationRelationship.GetExtendedExceptionInformation(getFederationInformationResult.Exception)));
				}
			}
			base.WriteVerbose(new LocalizedString("Returning only successful GetFederationInformation results"));
			list.RemoveAll((GetFederationInformationResult result) => result.Type != AutodiscoverResult.Success);
			value = string.Format("Number of valid results found: {0}", list.Count.ToString());
			base.WriteVerbose(new LocalizedString(value));
			return list;
		}

		private HashSet<string> FindValidRemoteFederationInformation(List<GetFederationInformationResult> getFederationInformationResults)
		{
			HashSet<string> result = null;
			if (this.GetValidRemoteFederatedInformation(getFederationInformationResults, out result))
			{
				base.WriteVerbose(new LocalizedString("The remote and local organizations are using compatible federation information."));
				return result;
			}
			string remoteTokenIssuerUris = string.Join(";", from fedInforesult in getFederationInformationResults
			select string.Join(",", fedInforesult.TokenIssuerUris));
			string remoteApplicationUris = string.Join(",", from fedInforesult in getFederationInformationResults
			select fedInforesult.ApplicationUri);
			StringBuilder stringBuilder = new StringBuilder(256);
			foreach (SecurityTokenService securityTokenService in this.externalAuthentication.SecurityTokenServices)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append(";");
				}
				stringBuilder.Append(string.Join(",", new string[]
				{
					securityTokenService.TokenIssuerUri.ToString()
				}));
			}
			LocalizedString localizedString = Strings.MismatchedFederation(stringBuilder.ToString(), remoteTokenIssuerUris, this.organizationRelationship.TargetApplicationUri.OriginalString, remoteApplicationUris);
			TestOrganizationRelationshipResult item = new TestOrganizationRelationshipResult
			{
				Id = TestOrganizationRelationshipResultId.MismatchedFederation,
				Status = Strings.TestOrganizationRelationshipWarning,
				Description = localizedString
			};
			this.testOrganizationRelationshipTestResults.Add(item);
			base.WriteVerbose(localizedString);
			return null;
		}

		private bool GetValidRemoteFederatedInformation(List<GetFederationInformationResult> getFederationInformationResults, out HashSet<string> remoteFederatedDomains)
		{
			bool result = false;
			remoteFederatedDomains = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (GetFederationInformationResult getFederationInformationResult in getFederationInformationResults)
			{
				bool flag = false;
				foreach (string strA in getFederationInformationResult.TokenIssuerUris)
				{
					foreach (SecurityTokenService securityTokenService in this.externalAuthentication.SecurityTokenServices)
					{
						if (string.Compare(strA, securityTokenService.TokenIssuerUri.ToString(), StringComparison.OrdinalIgnoreCase) == 0)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						break;
					}
				}
				if (flag)
				{
					if (string.IsNullOrEmpty(getFederationInformationResult.ApplicationUri))
					{
						TestOrganizationRelationshipResult testOrganizationRelationshipResult = new TestOrganizationRelationshipResult
						{
							Id = TestOrganizationRelationshipResultId.ApplicationUriMissing,
							Status = Strings.TestOrganizationRelationshipWarning,
							Description = Strings.ApplicationUrisMissingOnReturnedInformation
						};
						this.testOrganizationRelationshipTestResults.Add(testOrganizationRelationshipResult);
						base.WriteVerbose(testOrganizationRelationshipResult.Description);
					}
					else if (this.ApplicationUrisEqual(getFederationInformationResult.ApplicationUri, this.organizationRelationship.TargetApplicationUri.OriginalString))
					{
						result = true;
						foreach (string item in getFederationInformationResult.Domains)
						{
							remoteFederatedDomains.Add(item);
						}
					}
				}
			}
			return result;
		}

		private void PrintConfiguration()
		{
			StringBuilder stringBuilder = new StringBuilder(8192);
			bool enabled = VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled;
			string text = enabled ? "Online" : "On-Prem";
			stringBuilder.AppendLine(string.Concat(new object[]
			{
				Environment.NewLine,
				"Printing current configuration...",
				Environment.NewLine,
				"---------------------------------------------------------------------",
				Environment.NewLine,
				"Online or On-Prem: ",
				text,
				Environment.NewLine,
				Environment.NewLine,
				"Printing configuration information (from organizationRelationship):",
				Environment.NewLine,
				"---------------------------------------------------------------------",
				Environment.NewLine,
				"TargetAutodiscoverEpr: ",
				this.organizationRelationship.TargetAutodiscoverEpr,
				Environment.NewLine,
				"TargetApplicationUri: ",
				this.organizationRelationship.TargetApplicationUri.ToString()
			}));
			stringBuilder.AppendLine(string.Concat(new string[]
			{
				Environment.NewLine,
				"Printing configuration information (from federation):",
				Environment.NewLine,
				"---------------------------------------------------------------------",
				Environment.NewLine,
				"Local Application URI: ",
				this.externalAuthentication.ApplicationUri.ToString()
			}));
			stringBuilder.AppendLine("Organization Client Certificate Thumbprint: " + this.securityTokenService.Certificate.Thumbprint);
			stringBuilder.AppendLine("Action Offer: " + Offer.Autodiscover.ToString());
			stringBuilder.AppendLine("Default Federated Domain: " + this.defaultFederatedDomain);
			if (enabled)
			{
				if (!string.IsNullOrEmpty(this.accountNamespace))
				{
					stringBuilder.AppendLine("Informational: Account Namespace is set on online organization: " + this.accountNamespace);
				}
			}
			else
			{
				stringBuilder.AppendLine("Account Namespace: " + this.accountNamespace);
			}
			stringBuilder.AppendLine(Environment.NewLine + "Printing configuration information (from user):" + Environment.NewLine + "---------------------------------------------------------------------");
			stringBuilder.AppendLine("Federated Domain Name: " + this.userFederatedDomainName);
			if (this.localFederatedDomainsFromFedInfo.Contains(this.userFederatedDomainName) || this.localFederatedDomainsFromFedInfo.Contains(this.userFederatedDomainNameWOWellKnown))
			{
				stringBuilder.AppendLine("User Federated Domain is in organization's Federated Domain list");
			}
			stringBuilder.AppendLine("Organization ID: " + this.orIdName);
			stringBuilder.AppendLine(string.Concat(new object[]
			{
				"SID: ",
				this.adUser.Sid.ToString(),
				Environment.NewLine,
				"Primary SMTP: ",
				this.adUser.PrimarySmtpAddress.ToString(),
				Environment.NewLine,
				"ADUserObjectGuid: ",
				this.adUser.Guid.ToString(),
				Environment.NewLine,
				"ADUserObjectGuidBase64: ",
				Convert.ToBase64String(this.adUser.Guid.ToByteArray()),
				Environment.NewLine,
				"Federated Email: ",
				this.adUser.GetFederatedSmtpAddress().ToString(),
				Environment.NewLine,
				"RST Identity Type = FederatedIdentity Type: ",
				this.userFederatedIdentity.Type,
				Environment.NewLine,
				"RST Identity Value = FederatedIdentity Value: ",
				this.userFederatedIdentity.Identity,
				Environment.NewLine,
				"ImmutableId: ",
				this.adUser.ImmutableId.ToString()
			}));
			List<string> federatedEmailAddresses = this.adUser.GetFederatedEmailAddresses();
			if (federatedEmailAddresses != null && federatedEmailAddresses.Count > 0)
			{
				stringBuilder.AppendLine("Federated Email Addresses: " + string.Join(",", from emailaddress in federatedEmailAddresses
				select emailaddress));
			}
			if (enabled)
			{
				stringBuilder.AppendLine("WindowsLiveID: " + this.adUser.WindowsLiveID.ToString());
				stringBuilder.AppendLine("msExchOnPremiseObjectGuid: " + this.adUser.OnPremisesObjectId + Environment.NewLine);
				stringBuilder.AppendLine("Hint: For cloud FederatedIdentity is based upon WindowsLiveID domain name authentication type");
				stringBuilder.AppendLine("Hint: If WindowsLiveId domain name managed then WindowsLiveID directly used as Type UPN");
				stringBuilder.AppendLine("Hint: Otherwise if ImmutableId set then it is used as Type ImmutableId");
				stringBuilder.AppendLine("Hint: Otherwise msExchOnPremiseObjectGuid@WindowsLiveIdDomain it is used as Type ImmutableId");
			}
			else
			{
				stringBuilder.AppendLine(Environment.NewLine + "Hint: For on-premise FederatedIdentity is normally ImmutableId@AccountNamespace or ADUserObjectGuidBase64@AccountNamespace");
				stringBuilder.AppendLine("Hint: For on-premise if ImmutableId set it must include and match current AccountNamespace");
			}
			stringBuilder.AppendLine("Hint: For Federated Email, SMTP proxy address matching Default Federated Domain is first choice.");
			stringBuilder.AppendLine("Hint: Otherwise if primary SMTP address is Federated Domain for organization, then use it.");
			stringBuilder.AppendLine("Hint: Otherwise choose first SMTP address whose domain is federated.");
			stringBuilder.AppendLine("---------------------------------------------------------------------" + Environment.NewLine);
			base.WriteVerbose(new LocalizedString(stringBuilder.ToString()));
		}

		private bool ValidateUserConfiguration()
		{
			bool enabled = VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled;
			if (!this.localFederatedDomainsFromFedInfo.Contains(this.userFederatedDomainName) && !this.localFederatedDomainsFromFedInfo.Contains(this.userFederatedDomainNameWOWellKnown))
			{
				TestOrganizationRelationshipResult testOrganizationRelationshipResult = new TestOrganizationRelationshipResult
				{
					Id = TestOrganizationRelationshipResultId.UserFederatedIdentityIsNotFederatedDomain,
					Status = Strings.TestOrganizationRelationshipWarning,
					Description = Strings.FederatedDomainofUserNotOrgFederated(this.userFederatedDomainName)
				};
				this.testOrganizationRelationshipTestResults.Add(testOrganizationRelationshipResult);
				base.WriteVerbose(testOrganizationRelationshipResult.Description);
				return false;
			}
			if (this.localOrgRelDomainNames.Contains(this.userFederatedDomainName) || this.localOrgRelDomainNames.Contains(this.userFederatedDomainNameWOWellKnown))
			{
				this.WriteWarning(Strings.UserFederatedDomainInLocalOrgRelationship(this.userFederatedDomainName));
			}
			if (enabled)
			{
				AuthenticationType authenticationType;
				if (!this.organizationIdCacheValue.NamespaceAuthenticationTypeHash.TryGetValue(this.userFederatedDomainName, out authenticationType))
				{
					TestOrganizationRelationshipResult testOrganizationRelationshipResult2 = new TestOrganizationRelationshipResult
					{
						Id = TestOrganizationRelationshipResultId.UnknownFederationDomainAuthenticationType,
						Status = Strings.TestOrganizationRelationshipWarning,
						Description = Strings.UnknownFederationDomainAuthenticationType(this.userFederatedDomainName)
					};
					this.testOrganizationRelationshipTestResults.Add(testOrganizationRelationshipResult2);
					base.WriteVerbose(testOrganizationRelationshipResult2.Description);
					return false;
				}
				bool flag;
				switch (authenticationType)
				{
				case AuthenticationType.Managed:
					flag = false;
					break;
				case AuthenticationType.Federated:
					flag = true;
					break;
				default:
				{
					TestOrganizationRelationshipResult testOrganizationRelationshipResult3 = new TestOrganizationRelationshipResult
					{
						Id = TestOrganizationRelationshipResultId.UnknownFederationDomainAuthenticationType,
						Status = Strings.TestOrganizationRelationshipWarning,
						Description = Strings.UnknownFederationDomainAuthenticationType(this.userFederatedDomainName)
					};
					this.testOrganizationRelationshipTestResults.Add(testOrganizationRelationshipResult3);
					base.WriteVerbose(testOrganizationRelationshipResult3.Description);
					flag = false;
					break;
				}
				}
				if (flag)
				{
					base.WriteVerbose(new LocalizedString(Environment.NewLine + "VALIDATION CHECK: Checking if the RST Identity type is ImmutableId, and not UPN, since adUser is Federated Authentication..."));
					if (this.userFederatedIdentity.Type.ToString().Equals("UPN"))
					{
						TestOrganizationRelationshipResult testOrganizationRelationshipResult4 = new TestOrganizationRelationshipResult
						{
							Id = TestOrganizationRelationshipResultId.FederatedIdentityTypeMismatch,
							Status = Strings.TestOrganizationRelationshipWarning,
							Description = Strings.FederatedIdentityTypeMismatch("Federated", "UPN")
						};
						this.testOrganizationRelationshipTestResults.Add(testOrganizationRelationshipResult4);
						base.WriteVerbose(testOrganizationRelationshipResult4.Description);
						return false;
					}
					base.WriteVerbose(new LocalizedString(Environment.NewLine + "RESULT: Success."));
					base.WriteVerbose(new LocalizedString(Environment.NewLine + "VALIDATION CHECK: Checking if either msExchImmutableId or msExchOnPremiseObjectGuid is set, since at least one must be because adUser is federated..."));
					if ((this.adUser.OnPremisesObjectId == null || this.adUser.OnPremisesObjectId.ToString().Length == 0) && (this.adUser.ImmutableId == null || this.adUser.ImmutableId.ToString().Length == 0))
					{
						TestOrganizationRelationshipResult testOrganizationRelationshipResult5 = new TestOrganizationRelationshipResult
						{
							Id = TestOrganizationRelationshipResultId.RequiredIdentityInformationNotSet,
							Status = Strings.TestOrganizationRelationshipWarning,
							Description = Strings.RequiredIdentityInformationNotSet
						};
						this.testOrganizationRelationshipTestResults.Add(testOrganizationRelationshipResult5);
						base.WriteVerbose(testOrganizationRelationshipResult5.Description);
						return false;
					}
					base.WriteVerbose(new LocalizedString(Environment.NewLine + "RESULT: Success."));
				}
				else
				{
					base.WriteVerbose(new LocalizedString(Environment.NewLine + "VALIDATION CHECK: Checking if the RST Identity type is UPN, since adUser is Managed Authentication Type..."));
					if (!this.userFederatedIdentity.Type.ToString().Equals("UPN"))
					{
						TestOrganizationRelationshipResult testOrganizationRelationshipResult6 = new TestOrganizationRelationshipResult
						{
							Id = TestOrganizationRelationshipResultId.FederatedIdentityTypeMismatch,
							Status = Strings.TestOrganizationRelationshipWarning,
							Description = Strings.FederatedIdentityTypeMismatch("Managed", "ImmutableId")
						};
						this.testOrganizationRelationshipTestResults.Add(testOrganizationRelationshipResult6);
						base.WriteVerbose(testOrganizationRelationshipResult6.Description);
						return false;
					}
					base.WriteVerbose(new LocalizedString(Environment.NewLine + "RESULT: Success."));
				}
			}
			else
			{
				base.WriteVerbose(new LocalizedString(Environment.NewLine + "VALIDATION CHECK: Checking if the RST Identity type is ImmutableId for on-premise deployments."));
				if (this.userFederatedIdentity.Type.ToString().Equals("UPN"))
				{
					TestOrganizationRelationshipResult testOrganizationRelationshipResult7 = new TestOrganizationRelationshipResult
					{
						Id = TestOrganizationRelationshipResultId.FederatedIdentityTypeMismatch,
						Status = Strings.TestOrganizationRelationshipWarning,
						Description = Strings.FederatedIdentityTypeMismatch("Federated", "UPN")
					};
					this.testOrganizationRelationshipTestResults.Add(testOrganizationRelationshipResult7);
					base.WriteVerbose(testOrganizationRelationshipResult7.Description);
					return false;
				}
				base.WriteVerbose(new LocalizedString(Environment.NewLine + "VALIDATION CHECK: Checking if optional msExchImmutableId is set for this on-premises user."));
				if (this.adUser.ImmutableId != null && this.adUser.ImmutableId.ToString().Length > 0)
				{
					string str = string.Format("User has msExchImmutableId set to value {0}.", this.adUser.ImmutableId.ToString());
					base.WriteVerbose(new LocalizedString(Environment.NewLine + str));
				}
				else
				{
					base.WriteVerbose(new LocalizedString(Environment.NewLine + "User does not have msExchImmutableId set, however this is optional and Base 64 encoded value of AD user object GUID will be used instead."));
				}
				if (!this.userFederatedDomainName.Equals(this.accountNamespace, StringComparison.OrdinalIgnoreCase) && !this.userFederatedDomainNameWOWellKnown.Equals(this.accountNamespace, StringComparison.OrdinalIgnoreCase))
				{
					TestOrganizationRelationshipResult testOrganizationRelationshipResult8 = new TestOrganizationRelationshipResult
					{
						Id = TestOrganizationRelationshipResultId.UserFederatedDomainDoesNotMatchAccountNamespace,
						Status = Strings.TestOrganizationRelationshipWarning,
						Description = Strings.UserFederatedDomainDoesNotMatchAccountNamespace(this.userFederatedDomainName, this.accountNamespace)
					};
					this.testOrganizationRelationshipTestResults.Add(testOrganizationRelationshipResult8);
					base.WriteVerbose(testOrganizationRelationshipResult8.Description);
					return false;
				}
			}
			return true;
		}

		private RequestedToken GetDelegationToken()
		{
			RequestedToken requestedToken = null;
			base.WriteVerbose(Strings.GeneratingDelegationToken(this.adUser.PrimarySmtpAddress.ToString(), this.organizationRelationship.TargetApplicationUri.ToString()));
			Exception ex = null;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8))
				{
					xmlTextWriter.Namespaces = true;
					xmlTextWriter.Formatting = Formatting.Indented;
					try
					{
						DelegationTokenRequest delegationTokenRequest = new DelegationTokenRequest
						{
							FederatedIdentity = this.userFederatedIdentity,
							EmailAddress = this.adUser.GetFederatedSmtpAddress().ToString(),
							Target = this.organizationRelationship.GetTokenTarget(),
							Offer = Offer.Autodiscover,
							EmailAddresses = this.adUser.GetFederatedEmailAddresses()
						};
						requestedToken = this.securityTokenService.IssueToken(delegationTokenRequest, xmlTextWriter);
						this.DumpStreamToVerbose(memoryStream);
						memoryStream.Close();
						base.WriteVerbose(new LocalizedString(Environment.NewLine + "Requested Token: " + requestedToken.SecurityToken.ToString() + Environment.NewLine));
						base.WriteVerbose(Strings.TokenSuccessfullyGenerated);
						base.WriteVerbose(new LocalizedString(string.Concat(new object[]
						{
							"Got a token for target ",
							delegationTokenRequest.Target.ToString(),
							" for offer ",
							Offer.Autodiscover
						})));
					}
					catch (InvalidFederatedOrganizationIdException ex2)
					{
						ex = ex2;
					}
					catch (WSTrustException ex3)
					{
						ex = ex3;
					}
					if (ex != null)
					{
						this.DumpStreamToVerbose(memoryStream);
						memoryStream.Close();
						TestOrganizationRelationshipResult item = new TestOrganizationRelationshipResult
						{
							Id = TestOrganizationRelationshipResultId.FailureToGetDelegationToken,
							Status = Strings.TestOrganizationRelationshipError,
							Description = Strings.FailureToGetDelegationToken(ex.ToString())
						};
						this.testOrganizationRelationshipTestResults.Add(item);
						base.WriteVerbose(Strings.FailureToGetDelegationToken(ex.ToString()));
					}
				}
			}
			return requestedToken;
		}

		private void DumpStreamToVerbose(Stream stream)
		{
			if (!stream.CanRead)
			{
				return;
			}
			stream.Seek(0L, SeekOrigin.Begin);
			StreamReader streamReader = new StreamReader(stream);
			base.WriteVerbose(new LocalizedString(Environment.NewLine + streamReader.ReadToEnd()));
		}

		private AutodiscoverResultData GetRemoteOrganizationRelationshipSettings(RequestedToken token)
		{
			AutodiscoverResultData autodiscoverResultData = null;
			base.WriteVerbose(Strings.CallingGetOrganizationRelationshipSettings);
			using (MemoryStream memoryStream = new MemoryStream())
			{
				StreamWriter streamWriter = new StreamWriter(memoryStream, Encoding.UTF8);
				streamWriter.WriteLine("GetRemoteOrganizationRelationshipSettings network response details:");
				using (AutodiscoverClient autodiscoverClient = new AutodiscoverClient())
				{
					this.ConfigureAutodiscoverClientBinding(autodiscoverClient);
					autodiscoverClient.Authenticator = SoapHttpClientAuthenticator.Create(token);
					Uri targetAutodiscoverEpr = this.organizationRelationship.TargetAutodiscoverEpr;
					InvokeDelegate invokeDelegateForRemoteOrganizationRelationpSettings = this.GetInvokeDelegateForRemoteOrganizationRelationpSettings(streamWriter);
					if (targetAutodiscoverEpr != null)
					{
						base.WriteVerbose(Strings.AutodiscoverUsingUrl(targetAutodiscoverEpr.ToString()));
						try
						{
							autodiscoverResultData = autodiscoverClient.InvokeWithEndpoint(invokeDelegateForRemoteOrganizationRelationpSettings, targetAutodiscoverEpr);
						}
						catch (Exception exception)
						{
							string value = string.Format("Unable to retrieve organization relationship information from endpoint {0}. Exception={1}", targetAutodiscoverEpr.ToString(), TestOrganizationRelationship.GetExtendedExceptionInformation(exception));
							base.WriteVerbose(new LocalizedString(value));
						}
						this.WriteVerboseAutodiscoverResultData(autodiscoverResultData);
						if (autodiscoverResultData.Type != AutodiscoverResult.Success)
						{
							autodiscoverResultData = null;
						}
					}
					else
					{
						foreach (SmtpDomain smtpDomain in this.organizationRelationship.DomainNames)
						{
							base.WriteVerbose(Strings.AutodiscoverUsingDomain(smtpDomain.Domain));
							IEnumerable<AutodiscoverResultData> enumerable = null;
							try
							{
								enumerable = autodiscoverClient.InvokeWithDiscovery(invokeDelegateForRemoteOrganizationRelationpSettings, smtpDomain.Domain);
							}
							catch (Exception exception2)
							{
								string value = string.Format("Unable to retrieve organization relationship information for domain {0}. Exception={1}", smtpDomain.Domain, TestOrganizationRelationship.GetExtendedExceptionInformation(exception2));
								base.WriteVerbose(new LocalizedString(value));
							}
							foreach (AutodiscoverResultData autodiscoverResultData2 in enumerable)
							{
								this.WriteVerboseAutodiscoverResultData(autodiscoverResultData2);
								if (autodiscoverResultData2.Type == AutodiscoverResult.Success && autodiscoverResultData == null)
								{
									autodiscoverResultData = autodiscoverResultData2;
									break;
								}
							}
							if (autodiscoverResultData != null)
							{
								break;
							}
						}
					}
					streamWriter.WriteLine("GetRemoteOrganizationRelationshipSettings response complete.");
					this.DumpStreamToVerbose(memoryStream);
					memoryStream.Close();
				}
			}
			if (autodiscoverResultData == null)
			{
				TestOrganizationRelationshipResult testOrganizationRelationshipResult = new TestOrganizationRelationshipResult
				{
					Id = TestOrganizationRelationshipResultId.AutodiscoverServiceCallFailed,
					Status = Strings.TestOrganizationRelationshipError,
					Description = Strings.AutodiscoverServiceCallFailed
				};
				base.WriteVerbose(testOrganizationRelationshipResult.Description);
				this.testOrganizationRelationshipTestResults.Add(testOrganizationRelationshipResult);
			}
			return autodiscoverResultData;
		}

		private void WriteVerboseAutodiscoverResultData(AutodiscoverResultData autodiscoverResult)
		{
			switch (autodiscoverResult.Type)
			{
			case AutodiscoverResult.Success:
				base.WriteVerbose(Strings.AutodiscoverServiceCallSucceeded(autodiscoverResult.Url.ToString()));
				return;
			case AutodiscoverResult.Failure:
			case AutodiscoverResult.UnsecuredRedirect:
			case AutodiscoverResult.InvalidSslHostname:
				base.WriteVerbose(Strings.FailureToCallAutodiscoverService(autodiscoverResult.Url.ToString(), TestOrganizationRelationship.GetExtendedExceptionInformation(autodiscoverResult.Exception)));
				return;
			default:
				return;
			}
		}

		private bool VerifyOrganizationRelationships(AutodiscoverResultData autodiscoverResponse)
		{
			bool result = true;
			this.remoteOrganizationRelationshipForUserFedDomain = null;
			if (this.organizationRelationship.TargetAutodiscoverEpr == null)
			{
				TestOrganizationRelationshipResult testOrganizationRelationshipResult = new TestOrganizationRelationshipResult
				{
					Id = TestOrganizationRelationshipResultId.AutoDiscoverNotSetInOrgRelationship,
					Status = Strings.TestOrganizationRelationshipWarning,
					Description = Strings.AutoDiscoverIsNotSetInOrgRelationship
				};
				base.WriteVerbose(testOrganizationRelationshipResult.Description);
				this.testOrganizationRelationshipTestResults.Add(testOrganizationRelationshipResult);
			}
			else if (autodiscoverResponse.Url != null && EwsWsSecurityUrl.Fix(this.organizationRelationship.TargetAutodiscoverEpr) != EwsWsSecurityUrl.Fix(autodiscoverResponse.Url))
			{
				string localUrl = (this.organizationRelationship.TargetAutodiscoverEpr == null) ? string.Empty : this.organizationRelationship.TargetAutodiscoverEpr.ToString();
				string actualUrl = (autodiscoverResponse.Url == null) ? string.Empty : autodiscoverResponse.Url.ToString();
				TestOrganizationRelationshipResult testOrganizationRelationshipResult2 = new TestOrganizationRelationshipResult
				{
					Id = TestOrganizationRelationshipResultId.AutodiscoverUrlsDiffer,
					Status = Strings.TestOrganizationRelationshipWarning,
					Description = Strings.AutodiscoverUrlsDiffer(localUrl, actualUrl)
				};
				base.WriteVerbose(testOrganizationRelationshipResult2.Description);
				this.testOrganizationRelationshipTestResults.Add(testOrganizationRelationshipResult2);
			}
			GetOrganizationRelationshipSettingsResponse getOrganizationRelationshipSettingsResponse = autodiscoverResponse.Response as GetOrganizationRelationshipSettingsResponse;
			if (getOrganizationRelationshipSettingsResponse == null)
			{
				base.WriteVerbose(new LocalizedString("Detailed information on the remote organization relationship was not returned by the Autodiscover service"));
				return true;
			}
			if (getOrganizationRelationshipSettingsResponse.OrganizationRelationshipSettingsCollection == null || getOrganizationRelationshipSettingsResponse.OrganizationRelationshipSettingsCollection.Length == 0)
			{
				TestOrganizationRelationshipResult testOrganizationRelationshipResult3 = new TestOrganizationRelationshipResult
				{
					Id = TestOrganizationRelationshipResultId.NoOrganizationRelationshipInstancesWereReturnedByTheRemoteParty,
					Status = Strings.TestOrganizationRelationshipError,
					Description = Strings.NoOrganizationRelationshipInstancesWereReturnedByTheRemoteParty
				};
				base.WriteVerbose(testOrganizationRelationshipResult3.Description);
				this.testOrganizationRelationshipTestResults.Add(testOrganizationRelationshipResult3);
				result = false;
			}
			if (getOrganizationRelationshipSettingsResponse.OrganizationRelationshipSettingsCollection.Length > 1)
			{
				TestOrganizationRelationshipResult testOrganizationRelationshipResult4 = new TestOrganizationRelationshipResult
				{
					Id = TestOrganizationRelationshipResultId.MultipleOrganizationRelationshipInstancesReturnedByTheRemoteParty,
					Status = Strings.TestOrganizationRelationshipWarning,
					Description = Strings.MultipleOrganizationRelationshipInstancesReturnedByTheRemoteParty
				};
				base.WriteVerbose(testOrganizationRelationshipResult4.Description);
				this.testOrganizationRelationshipTestResults.Add(testOrganizationRelationshipResult4);
			}
			foreach (OrganizationRelationshipSettings organizationRelationshipSettings in getOrganizationRelationshipSettingsResponse.OrganizationRelationshipSettingsCollection)
			{
				if (!this.VerifyOrganizationRelationship(organizationRelationshipSettings, autodiscoverResponse.Url))
				{
					TestOrganizationRelationshipResult testOrganizationRelationshipResult5 = new TestOrganizationRelationshipResult
					{
						Id = TestOrganizationRelationshipResultId.VerificationOfRemoteOrganizationRelationshipFailed,
						Status = Strings.TestOrganizationRelationshipError,
						Description = Strings.VerificationOfRemoteOrganizationRelationshipFailed(organizationRelationshipSettings.Name)
					};
					base.WriteVerbose(testOrganizationRelationshipResult5.Description);
					this.testOrganizationRelationshipTestResults.Add(testOrganizationRelationshipResult5);
					result = false;
				}
			}
			if (this.remoteOrganizationRelationshipForUserFedDomain == null)
			{
				TestOrganizationRelationshipResult testOrganizationRelationshipResult6 = new TestOrganizationRelationshipResult
				{
					Id = TestOrganizationRelationshipResultId.UserFedDomainNotInRemoteOrgRelationship,
					Status = Strings.TestOrganizationRelationshipWarning,
					Description = Strings.UserFederatedDomainNotInRemoteOrgRelationship(this.userFederatedDomainName)
				};
				this.testOrganizationRelationshipTestResults.Add(testOrganizationRelationshipResult6);
				base.WriteVerbose(testOrganizationRelationshipResult6.Description);
			}
			return result;
		}

		private bool VerifyOrganizationRelationship(OrganizationRelationshipSettings remoteOrganizationRelationship, Uri autodiscoverUrl)
		{
			bool result = true;
			Uri applicationUri = this.externalAuthentication.ApplicationUri;
			HashSet<string> hashSet;
			if (remoteOrganizationRelationship.DomainNames == null || remoteOrganizationRelationship.DomainNames.Length == 0)
			{
				hashSet = new HashSet<string>();
				TestOrganizationRelationshipResult testOrganizationRelationshipResult = new TestOrganizationRelationshipResult
				{
					Id = TestOrganizationRelationshipResultId.RemoteOrgRelationshipHasNoDomainsDefined,
					Status = Strings.TestOrganizationRelationshipWarning,
					Description = Strings.NoDomainsDefinedOnOrgRelationship(remoteOrganizationRelationship.Name)
				};
				this.testOrganizationRelationshipTestResults.Add(testOrganizationRelationshipResult);
				base.WriteVerbose(testOrganizationRelationshipResult.Description);
				result = false;
			}
			else
			{
				hashSet = new HashSet<string>(remoteOrganizationRelationship.DomainNames, StringComparer.OrdinalIgnoreCase);
			}
			if (hashSet.Contains(this.userFederatedDomainName) || hashSet.Contains(this.userFederatedDomainNameWOWellKnown))
			{
				if (this.remoteOrganizationRelationshipForUserFedDomain != null)
				{
					TestOrganizationRelationshipResult testOrganizationRelationshipResult2 = new TestOrganizationRelationshipResult
					{
						Id = TestOrganizationRelationshipResultId.UserFedDomainInMultipleRemoteOrgRelationship,
						Status = Strings.TestOrganizationRelationshipWarning,
						Description = Strings.UserFederatedDomainInMultipleRemoteOrgRelationship(this.userFederatedDomainName, remoteOrganizationRelationship.Name)
					};
					this.testOrganizationRelationshipTestResults.Add(testOrganizationRelationshipResult2);
					base.WriteVerbose(testOrganizationRelationshipResult2.Description);
				}
				else
				{
					this.remoteOrganizationRelationshipForUserFedDomain = remoteOrganizationRelationship;
				}
			}
			if (string.IsNullOrEmpty(remoteOrganizationRelationship.TargetApplicationUri))
			{
				TestOrganizationRelationshipResult testOrganizationRelationshipResult3 = new TestOrganizationRelationshipResult
				{
					Id = TestOrganizationRelationshipResultId.ApplicationUriMissing,
					Status = Strings.TestOrganizationRelationshipWarning,
					Description = Strings.ApplicationUrisMissingOnReturnedInformation
				};
				this.testOrganizationRelationshipTestResults.Add(testOrganizationRelationshipResult3);
				base.WriteVerbose(testOrganizationRelationshipResult3.Description);
				result = false;
			}
			else if (!this.ApplicationUrisEqual(remoteOrganizationRelationship.TargetApplicationUri, applicationUri.OriginalString))
			{
				TestOrganizationRelationshipResult testOrganizationRelationshipResult4 = new TestOrganizationRelationshipResult
				{
					Id = TestOrganizationRelationshipResultId.ApplicationUrisDiffer,
					Status = Strings.TestOrganizationRelationshipWarning,
					Description = Strings.ApplicationUrisDiffer(this.externalAuthentication.TokenValidator.TargetUri.ToString(), remoteOrganizationRelationship.TargetApplicationUri.ToString())
				};
				this.testOrganizationRelationshipTestResults.Add(testOrganizationRelationshipResult4);
				base.WriteVerbose(testOrganizationRelationshipResult4.Description);
				result = false;
			}
			if (Enum.IsDefined(typeof(FreeBusyAccessLevel), remoteOrganizationRelationship.FreeBusyAccessLevel))
			{
				FreeBusyAccessLevel freeBusyAccessLevel = (FreeBusyAccessLevel)Enum.Parse(typeof(FreeBusyAccessLevel), remoteOrganizationRelationship.FreeBusyAccessLevel);
				if (this.organizationRelationship.FreeBusyAccessEnabled && freeBusyAccessLevel == FreeBusyAccessLevel.None)
				{
					TestOrganizationRelationshipResult testOrganizationRelationshipResult5 = new TestOrganizationRelationshipResult
					{
						Id = TestOrganizationRelationshipResultId.AccessMismatchLocalRemote,
						Status = Strings.TestOrganizationRelationshipError,
						Description = Strings.AccessMismatchLocalRemote(this.organizationRelationship.Name, "FreeBusyAccessEnabled", remoteOrganizationRelationship.Name, "FreeBusyAccessLevel")
					};
					this.testOrganizationRelationshipTestResults.Add(testOrganizationRelationshipResult5);
					base.WriteVerbose(testOrganizationRelationshipResult5.Description);
					result = false;
				}
			}
			else
			{
				TestOrganizationRelationshipResult testOrganizationRelationshipResult6 = new TestOrganizationRelationshipResult
				{
					Id = TestOrganizationRelationshipResultId.CouldNotParseRemoteValue,
					Status = Strings.TestOrganizationRelationshipError,
					Description = Strings.CouldNotParseRemoteValue("FreeBusyAccessLevel", remoteOrganizationRelationship.FreeBusyAccessLevel)
				};
				this.testOrganizationRelationshipTestResults.Add(testOrganizationRelationshipResult6);
				base.WriteVerbose(testOrganizationRelationshipResult6.Description);
				result = false;
			}
			if (remoteOrganizationRelationship.FreeBusyAccessEnabled && this.organizationRelationship.FreeBusyAccessLevel == FreeBusyAccessLevel.None)
			{
				TestOrganizationRelationshipResult testOrganizationRelationshipResult7 = new TestOrganizationRelationshipResult
				{
					Id = TestOrganizationRelationshipResultId.AccessMismatchRemoteLocal,
					Status = Strings.TestOrganizationRelationshipError,
					Description = Strings.AccessMismatchRemoteLocal(remoteOrganizationRelationship.Name, "FreeBusyAccessEnabled", this.organizationRelationship.Name, "FreeBusyAccessLevel")
				};
				this.testOrganizationRelationshipTestResults.Add(testOrganizationRelationshipResult7);
				base.WriteVerbose(testOrganizationRelationshipResult7.Description);
				result = false;
			}
			if (this.organizationRelationship.MailboxMoveEnabled != remoteOrganizationRelationship.MailboxMoveEnabled)
			{
				TestOrganizationRelationshipResult testOrganizationRelationshipResult8 = new TestOrganizationRelationshipResult
				{
					Id = TestOrganizationRelationshipResultId.PropertiesDiffer,
					Status = Strings.TestOrganizationRelationshipWarning,
					Description = Strings.PropertiesDiffer("MailboxMoveEnabled", this.organizationRelationship.Name, this.organizationRelationship.MailboxMoveEnabled.ToString(), remoteOrganizationRelationship.Name, remoteOrganizationRelationship.MailboxMoveEnabled.ToString())
				};
				base.WriteVerbose(testOrganizationRelationshipResult8.Description);
				this.testOrganizationRelationshipTestResults.Add(testOrganizationRelationshipResult8);
			}
			if (Enum.IsDefined(typeof(MailTipsAccessLevel), remoteOrganizationRelationship.MailTipsAccessLevel))
			{
				MailTipsAccessLevel mailTipsAccessLevel = (MailTipsAccessLevel)Enum.Parse(typeof(MailTipsAccessLevel), remoteOrganizationRelationship.MailTipsAccessLevel);
				if (this.organizationRelationship.MailTipsAccessEnabled && mailTipsAccessLevel == MailTipsAccessLevel.None)
				{
					TestOrganizationRelationshipResult testOrganizationRelationshipResult9 = new TestOrganizationRelationshipResult
					{
						Id = TestOrganizationRelationshipResultId.AccessMismatchLocalRemote,
						Status = Strings.TestOrganizationRelationshipError,
						Description = Strings.AccessMismatchLocalRemote(this.organizationRelationship.Name, "MailTipsAccessEnabled", remoteOrganizationRelationship.Name, "MailTipsAccessLevel")
					};
					this.testOrganizationRelationshipTestResults.Add(testOrganizationRelationshipResult9);
					base.WriteVerbose(testOrganizationRelationshipResult9.Description);
					result = false;
				}
			}
			else
			{
				TestOrganizationRelationshipResult testOrganizationRelationshipResult10 = new TestOrganizationRelationshipResult
				{
					Id = TestOrganizationRelationshipResultId.CouldNotParseRemoteValue,
					Status = Strings.TestOrganizationRelationshipError,
					Description = Strings.CouldNotParseRemoteValue("MailTipsAccessLevel", remoteOrganizationRelationship.MailTipsAccessLevel)
				};
				this.testOrganizationRelationshipTestResults.Add(testOrganizationRelationshipResult10);
				base.WriteVerbose(testOrganizationRelationshipResult10.Description);
				result = false;
			}
			if (remoteOrganizationRelationship.MailTipsAccessEnabled && this.organizationRelationship.MailTipsAccessLevel == MailTipsAccessLevel.None)
			{
				TestOrganizationRelationshipResult testOrganizationRelationshipResult11 = new TestOrganizationRelationshipResult
				{
					Id = TestOrganizationRelationshipResultId.AccessMismatchRemoteLocal,
					Status = Strings.TestOrganizationRelationshipError,
					Description = Strings.AccessMismatchRemoteLocal(remoteOrganizationRelationship.Name, "MailTipsEnabled", this.organizationRelationship.Name, "MailTipsAccessLevel")
				};
				this.testOrganizationRelationshipTestResults.Add(testOrganizationRelationshipResult11);
				base.WriteVerbose(testOrganizationRelationshipResult11.Description);
				result = false;
			}
			if (this.organizationRelationship.DeliveryReportEnabled != remoteOrganizationRelationship.DeliveryReportEnabled)
			{
				TestOrganizationRelationshipResult testOrganizationRelationshipResult12 = new TestOrganizationRelationshipResult
				{
					Id = TestOrganizationRelationshipResultId.PropertiesDiffer,
					Status = Strings.TestOrganizationRelationshipError,
					Description = Strings.PropertiesDiffer("DeliveryReportEnabled", this.organizationRelationship.Name, this.organizationRelationship.DeliveryReportEnabled.ToString(), remoteOrganizationRelationship.Name, remoteOrganizationRelationship.DeliveryReportEnabled.ToString())
				};
				this.testOrganizationRelationshipTestResults.Add(testOrganizationRelationshipResult12);
				base.WriteVerbose(testOrganizationRelationshipResult12.Description);
				result = false;
			}
			if (!this.localFederatedDomainsFromFedInfo.Overlaps(remoteOrganizationRelationship.DomainNames))
			{
				TestOrganizationRelationshipResult testOrganizationRelationshipResult13 = new TestOrganizationRelationshipResult
				{
					Id = TestOrganizationRelationshipResultId.NoDomainInTheRemoteOrganizationRelationshipIsFederatedLocally,
					Status = Strings.TestOrganizationRelationshipWarning,
					Description = Strings.NoDomainInTheRemoteOrganizationRelationshipIsFederatedLocally(remoteOrganizationRelationship.Name)
				};
				this.testOrganizationRelationshipTestResults.Add(testOrganizationRelationshipResult13);
				base.WriteVerbose(testOrganizationRelationshipResult13.Description);
			}
			else if (!this.localFederatedDomainsFromFedInfo.IsSubsetOf(remoteOrganizationRelationship.DomainNames))
			{
				TestOrganizationRelationshipResult testOrganizationRelationshipResult14 = new TestOrganizationRelationshipResult
				{
					Id = TestOrganizationRelationshipResultId.LocalFederatedDomainsAreMissingFromTheRemoteOrganizationRelationsipDomains,
					Status = Strings.TestOrganizationRelationshipWarning,
					Description = Strings.LocalFederatedDomainsAreMissingFromTheRemoteOrganizationRelationsipDomains
				};
				this.testOrganizationRelationshipTestResults.Add(testOrganizationRelationshipResult14);
				base.WriteVerbose(testOrganizationRelationshipResult14.Description);
			}
			Uri remoteTargetSharingEpr = null;
			if (remoteOrganizationRelationship.TargetSharingEpr != null && Uri.TryCreate(remoteOrganizationRelationship.TargetSharingEpr, UriKind.RelativeOrAbsolute, out remoteTargetSharingEpr))
			{
				bool foundWebServiceMatch = false;
				remoteTargetSharingEpr = EwsWsSecurityUrl.Fix(remoteTargetSharingEpr);
				if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
				{
					Uri uri = EwsWsSecurityUrl.Fix(FrontEndLocator.GetDatacenterFrontEndWebServicesUrl());
					foundWebServiceMatch = (remoteTargetSharingEpr == uri);
				}
				else
				{
					Action<WebServicesService> action = delegate(WebServicesService svc)
					{
						if (remoteTargetSharingEpr == EwsWsSecurityUrl.Fix(svc.Url))
						{
							foundWebServiceMatch = true;
						}
					};
					ServiceTopology currentServiceTopology = ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Federation\\TestOrganizationRelationship.cs", "VerifyOrganizationRelationship", 1835);
					currentServiceTopology.ForEach<WebServicesService>(action, "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Federation\\TestOrganizationRelationship.cs", "VerifyOrganizationRelationship", 1836);
				}
				if (!foundWebServiceMatch)
				{
					TestOrganizationRelationshipResult testOrganizationRelationshipResult15 = new TestOrganizationRelationshipResult
					{
						Id = TestOrganizationRelationshipResultId.TargetSharingEprDoesNotMatchAnyExternalURI,
						Status = Strings.TestOrganizationRelationshipError,
						Description = Strings.TargetSharingEprDoesNotMatchAnyExternalURI(remoteOrganizationRelationship.TargetSharingEpr.ToString())
					};
					this.testOrganizationRelationshipTestResults.Add(testOrganizationRelationshipResult15);
					base.WriteVerbose(testOrganizationRelationshipResult15.Description);
					result = false;
				}
			}
			return result;
		}

		private InvokeDelegate GetInvokeDelegateForRemoteOrganizationRelationpSettings(StreamWriter debugStream)
		{
			GetOrganizationRelationshipSettingsRequest request = new GetOrganizationRelationshipSettingsRequest();
			request.Domains = new string[this.organizationRelationship.DomainNames.Count];
			int num = 0;
			foreach (SmtpDomain smtpDomain in this.organizationRelationship.DomainNames)
			{
				request.Domains[num++] = smtpDomain.Domain;
			}
			return delegate(DefaultBinding_Autodiscover binding)
			{
				binding.Url = EwsWsSecurityUrl.Fix(binding.Url);
				GetOrganizationRelationshipSettingsResponse organizationRelationshipSettings = binding.GetOrganizationRelationshipSettings(request);
				if (debugStream != null)
				{
					debugStream.WriteLine("Received response for request to " + binding.Url);
					if (binding.ResponseHttpHeaders != null)
					{
						debugStream.WriteLine("HTTP response headers:");
						foreach (KeyValuePair<string, string> keyValuePair in binding.ResponseHttpHeaders)
						{
							debugStream.WriteLine(keyValuePair.Key + " : " + keyValuePair.Value);
						}
					}
					debugStream.Flush();
				}
				return organizationRelationshipSettings;
			};
		}

		private void ConfigureAutodiscoverClientBinding(AutodiscoverClient client)
		{
			client.RequestedServerVersion = DefaultBinding_Autodiscover.Exchange2010RequestedServerVersion;
			client.UserAgent = "TestOrganizationRelationship/1.1";
			Server localServer = LocalServerCache.LocalServer;
			if (localServer != null && localServer.InternetWebProxy != null)
			{
				client.Proxy = new WebProxy(localServer.InternetWebProxy);
				base.WriteVerbose(Strings.UsingProxy(localServer.InternetWebProxy.ToString()));
			}
			if (this.TrustedHostnames != null)
			{
				client.AllowedHostnames.AddRange(this.TrustedHostnames);
				base.WriteVerbose(Strings.GetFederationInformationTrustedHostnames(string.Join(",", this.TrustedHostnames)));
			}
		}

		private void WriteStringPrependNewLine(string msg)
		{
			this.WriteString(Environment.NewLine + msg);
		}

		private void WriteString(string msg)
		{
			base.WriteObject(msg);
		}

		private void WriteResults()
		{
			if (this.testOrganizationRelationshipTestResults.Count <= 0)
			{
				this.WriteString("No Significant Issues to Report");
				return;
			}
			foreach (TestOrganizationRelationshipResult dataObject in this.testOrganizationRelationshipTestResults)
			{
				this.WriteResult(dataObject);
			}
		}

		private static string GetExtendedExceptionInformation(Exception exception)
		{
			StringBuilder stringBuilder = new StringBuilder(1024);
			while (exception != null)
			{
				WebException ex = exception as WebException;
				if (ex != null)
				{
					stringBuilder.Append("WebException.Response = ");
					if (ex.Response != null)
					{
						using (Stream responseStream = ex.Response.GetResponseStream())
						{
							if (responseStream.CanRead)
							{
								if (responseStream.CanSeek)
								{
									responseStream.Seek(0L, SeekOrigin.Begin);
								}
								using (StreamReader streamReader = new StreamReader(responseStream))
								{
									stringBuilder.AppendLine();
									stringBuilder.AppendLine(streamReader.ReadToEnd());
									goto IL_8A;
								}
							}
							stringBuilder.AppendLine("<cannot read response stream>");
							IL_8A:
							goto IL_A2;
						}
					}
					stringBuilder.AppendLine("<Null>");
				}
				IL_A2:
				SoapException ex2 = exception as SoapException;
				if (ex2 != null)
				{
					if (ex2.Code != null)
					{
						stringBuilder.Append("SoapException.Code = ");
						stringBuilder.Append(ex2.Code);
						stringBuilder.AppendLine();
					}
					if (ex2.Detail != null)
					{
						stringBuilder.AppendLine("SoapException.Detail = ");
						stringBuilder.AppendLine(ex2.Detail.OuterXml);
					}
				}
				stringBuilder.AppendLine("Exception:");
				stringBuilder.AppendLine(exception.ToString());
				stringBuilder.AppendLine();
				exception = exception.InnerException;
			}
			string text = stringBuilder.ToString();
			if (string.IsNullOrEmpty(text))
			{
				text = "{}";
			}
			return text;
		}

		private bool ApplicationUrisEqual(string remoteApplicationUri, string localApplicationUri)
		{
			remoteApplicationUri = (remoteApplicationUri.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ? remoteApplicationUri : ("http://" + remoteApplicationUri));
			localApplicationUri = (localApplicationUri.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ? localApplicationUri : ("http://" + localApplicationUri));
			return remoteApplicationUri.Equals(localApplicationUri, StringComparison.OrdinalIgnoreCase);
		}

		private const string UserAgent = "TestOrganizationRelationship/1.1";

		private List<TestOrganizationRelationshipResult> testOrganizationRelationshipTestResults;

		private ADUser adUser;

		private OrganizationRelationship organizationRelationship;

		private FederatedOrganizationId federatedOrganizationId;

		private HashSet<string> localFederatedDomainsFromFedInfo;

		private HashSet<string> remoteFederatedDomainFromFedInfo;

		private HashSet<string> localOrgRelDomainNames;

		private FederatedIdentity userFederatedIdentity;

		private string accountNamespace;

		private string defaultFederatedDomain;

		private string userFederatedDomainName;

		private string userFederatedDomainNameWOWellKnown;

		private string orIdName;

		private SecurityTokenService securityTokenService;

		private OrganizationRelationshipSettings remoteOrganizationRelationshipForUserFedDomain;

		private ExternalAuthentication externalAuthentication;

		private string[] trustedHostnames;

		private OrganizationIdCacheValue organizationIdCacheValue;
	}
}
