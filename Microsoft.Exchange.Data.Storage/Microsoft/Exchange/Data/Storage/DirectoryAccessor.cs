using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DirectoryAccessor : IDirectoryAccessor, IADUserFinder
	{
		public IGenericADUser FindBySid(IRecipientSession recipientSession, SecurityIdentifier sid)
		{
			ArgumentValidator.ThrowIfNull("recipientSession", recipientSession);
			IGenericADUser adUser = null;
			this.DoAdCallAndTranslateExceptions(delegate
			{
				adUser = this.TranslateADRecipient(recipientSession.FindBySid(sid), false);
			}, "DirectoryAccessor::FindBySid");
			return adUser;
		}

		public IGenericADUser FindByProxyAddress(IRecipientSession recipientSession, ProxyAddress proxyAddress)
		{
			ArgumentValidator.ThrowIfNull("recipientSession", recipientSession);
			IGenericADUser adUser = null;
			this.DoAdCallAndTranslateExceptions(delegate
			{
				adUser = this.TranslateADRecipient(recipientSession.FindByProxyAddress(proxyAddress), false);
			}, "DirectoryAccessor::FindByProxyAddress");
			return adUser;
		}

		public IGenericADUser FindByExchangeGuid(IRecipientSession recipientSession, Guid mailboxGuid, bool includeSystemMailbox)
		{
			ArgumentValidator.ThrowIfNull("recipientSession", recipientSession);
			IGenericADUser adUser = null;
			this.DoAdCallAndTranslateExceptions(delegate
			{
				adUser = this.TranslateADRecipient(recipientSession.FindByExchangeGuidIncludingAlternate(mailboxGuid), includeSystemMailbox);
			}, "DirectoryAccessor::FindByExchangeGuid");
			return adUser;
		}

		public IGenericADUser FindByObjectId(IRecipientSession recipientSession, ADObjectId directoryEntry)
		{
			ArgumentValidator.ThrowIfNull("recipientSession", recipientSession);
			IGenericADUser adUser = null;
			this.DoAdCallAndTranslateExceptions(delegate
			{
				adUser = this.TranslateADRecipient(recipientSession.Read(directoryEntry), false);
			}, "DirectoryAccessor::FindByObjectId");
			return adUser;
		}

		public IGenericADUser FindByLegacyExchangeDn(IRecipientSession recipientSession, string legacyExchangeDn)
		{
			ArgumentValidator.ThrowIfNull("recipientSession", recipientSession);
			IGenericADUser adUser = null;
			this.DoAdCallAndTranslateExceptions(delegate
			{
				adUser = this.TranslateADRecipient(recipientSession.FindByLegacyExchangeDN(legacyExchangeDn), false);
			}, "DirectoryAccessor::FindByLegacyExchangeDn");
			return adUser;
		}

		public IGenericADUser FindMiniRecipientByProxyAddress(IRecipientSession recipientSession, ProxyAddress proxyAddress, PropertyDefinition[] miniRecipientProperties, out StorageMiniRecipient miniRecipient)
		{
			ArgumentValidator.ThrowIfNull("recipientSession", recipientSession);
			StorageMiniRecipient localMiniRecipient = null;
			this.DoAdCallAndTranslateExceptions(delegate
			{
				localMiniRecipient = recipientSession.FindMiniRecipientByProxyAddress<StorageMiniRecipient>(proxyAddress, miniRecipientProperties);
			}, "DirectoryAccessor::FindMiniRecipientByProxyAddress");
			miniRecipient = localMiniRecipient;
			return this.TranslateMiniRecipient(miniRecipient);
		}

		public SmtpAddress GetOrganizationFederatedMailboxIdentity(IConfigurationSession configurationSession)
		{
			ArgumentValidator.ThrowIfNull("configurationSession", configurationSession);
			SmtpAddress organizationFederatedMailboxIdentity = default(SmtpAddress);
			this.DoAdCallAndTranslateExceptions(delegate
			{
				organizationFederatedMailboxIdentity = configurationSession.FindSingletonConfigurationObject<TransportConfigContainer>().OrganizationFederatedMailbox;
			}, "DirectoryAccessor::GetOrganizationFederatedMailboxIdentity");
			return organizationFederatedMailboxIdentity;
		}

		public bool TryGetOrganizationContentConversionProperties(OrganizationId organizationId, out OrganizationContentConversionProperties organizationContentConversionProperties)
		{
			bool tryResult = false;
			OrganizationContentConversionProperties localOrganizationContentConversionProperties = null;
			this.DoAdCallAndTranslateExceptions(delegate
			{
				tryResult = OrganizationContentConversionCache.TryGetOrganizationContentConversionProperties(organizationId, out localOrganizationContentConversionProperties);
			}, "DirectoryAccessor::TryGetOrganizationContentConversionProperties");
			organizationContentConversionProperties = localOrganizationContentConversionProperties;
			return tryResult;
		}

		public OrganizationRelationship GetOrganizationRelationship(OrganizationId organizationId, string domain)
		{
			OrganizationRelationship relationship = null;
			this.DoAdCallAndTranslateExceptions(delegate
			{
				OrganizationIdCacheValue organizationIdCacheValue = OrganizationIdCache.Singleton.Get(organizationId);
				relationship = organizationIdCacheValue.GetOrganizationRelationship(domain);
			}, "DirectoryAccessor:GetOrganizationRelationship");
			return relationship;
		}

		public int? GetPrimaryGroupId(IRecipientSession recipientSession, SecurityIdentifier userSid)
		{
			ArgumentValidator.ThrowIfNull("recipientSession", recipientSession);
			ADUser adUser = null;
			this.DoAdCallAndTranslateExceptions(delegate
			{
				adUser = (recipientSession.FindBySid(userSid) as ADUser);
			}, "DirectoryAccessor::GetPrimaryGroupId");
			if (adUser == null)
			{
				return null;
			}
			return (int?)adUser[ADUserSchema.PrimaryGroupId];
		}

		public bool IsLicensingEnforcedInOrg(OrganizationId organizationId)
		{
			ArgumentValidator.ThrowIfNull("organizationId", organizationId);
			bool isLicensingEnforcedInOrg = false;
			this.DoAdCallAndTranslateExceptions(delegate
			{
				isLicensingEnforcedInOrg = CapabilityHelper.GetIsLicensingEnforcedInOrg(organizationId);
			}, "DirectoryAccessor::IsLicensingEnforcedInOrg");
			return isLicensingEnforcedInOrg;
		}

		public bool IsTenantAccessBlocked(OrganizationId organizationId)
		{
			bool isTenantAccessBlocked = false;
			if (organizationId != null)
			{
				this.DoAdCallAndTranslateExceptions(delegate
				{
					OrganizationProperties organizationProperties;
					if (OrganizationPropertyCache.TryGetOrganizationProperties(organizationId, out organizationProperties))
					{
						isTenantAccessBlocked = organizationProperties.IsTenantAccessBlocked;
					}
				}, "DirectoryAccessor::IsTenantAccessBlocked");
			}
			return isTenantAccessBlocked;
		}

		public Server GetLocalServer()
		{
			Server localServer = null;
			this.DoAdCallAndTranslateExceptions(delegate
			{
				localServer = LocalServer.GetServer();
			}, "DirectoryAccessor::GetLocalServer");
			return localServer;
		}

		private IGenericADUser TranslateADRecipient(ADRecipient recipient, bool checkForSystemMailbox = false)
		{
			if (recipient != null)
			{
				if (checkForSystemMailbox && recipient.RecipientType == RecipientType.SystemMailbox)
				{
					ADSystemMailbox adsystemMailbox = recipient as ADSystemMailbox;
					if (adsystemMailbox != null)
					{
						return new ADSystemMailboxGenericWrapper(adsystemMailbox);
					}
				}
				else
				{
					ADUser aduser = recipient as ADUser;
					if (aduser != null)
					{
						return new ADUserGenericWrapper(aduser);
					}
					ADGroup adgroup = recipient as ADGroup;
					if (adgroup != null && adgroup.RecipientType == RecipientType.MailUniversalDistributionGroup && adgroup.RecipientTypeDetails == RecipientTypeDetails.GroupMailbox)
					{
						return new ADGroupGenericWrapper(adgroup);
					}
				}
			}
			return null;
		}

		private IGenericADUser TranslateMiniRecipient(StorageMiniRecipient recipient)
		{
			IGenericADUser result = null;
			if (recipient != null)
			{
				result = new MiniRecipientGenericWrapper(recipient);
			}
			return result;
		}

		private void DoAdCallAndTranslateExceptions(Action call, string methodName)
		{
			try
			{
				call();
			}
			catch (DataValidationException innerException)
			{
				throw new ObjectNotFoundException(ServerStrings.ADUserNotFound, innerException);
			}
			catch (DataSourceOperationException ex)
			{
				throw StorageGlobals.TranslateDirectoryException(ServerStrings.ADException, ex, null, "{0}. Failed due to directory exception {1}.", new object[]
				{
					methodName,
					ex
				});
			}
			catch (DataSourceTransientException ex2)
			{
				throw StorageGlobals.TranslateDirectoryException(ServerStrings.ADException, ex2, null, "{0}. Failed due to directory exception {1}.", new object[]
				{
					methodName,
					ex2
				});
			}
		}
	}
}
