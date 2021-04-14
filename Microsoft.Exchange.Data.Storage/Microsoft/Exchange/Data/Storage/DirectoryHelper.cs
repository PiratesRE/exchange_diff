using System;
using System.Security;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class DirectoryHelper
	{
		public static ADObjectId GetGlobalAddressListFromAddressBookPolicy(ADObjectId addressBookPolicyId, IConfigurationSession configurationSession)
		{
			if (addressBookPolicyId != null)
			{
				AddressBookMailboxPolicy addressBookMailboxPolicy = configurationSession.Read<AddressBookMailboxPolicy>(addressBookPolicyId);
				if (addressBookMailboxPolicy != null)
				{
					return addressBookMailboxPolicy.GlobalAddressList;
				}
			}
			return null;
		}

		public static ExDateTime GetPasswordExpirationDate(ADObjectId adUserObjectId, IRecipientSession recipientSession)
		{
			if (adUserObjectId.IsNullOrEmpty())
			{
				throw new ArgumentException("ADUser object ID cannot be null or empty", "adUserObjectId");
			}
			ADUser aduser = recipientSession.Read(adUserObjectId) as ADUser;
			if (aduser == null)
			{
				throw new ObjectNotFoundException(ServerStrings.ADUserNotFound);
			}
			if ((aduser.UserAccountControl & UserAccountControlFlags.DoNotExpirePassword) != UserAccountControlFlags.None)
			{
				return ExDateTime.MaxValue;
			}
			ExDateTime? exDateTime = null;
			if (aduser.PasswordLastSet != null)
			{
				exDateTime = new ExDateTime?(new ExDateTime(ExTimeZone.UtcTimeZone, aduser.PasswordLastSet.Value));
			}
			if (exDateTime == null)
			{
				return ExDateTime.MaxValue;
			}
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromAccountPartitionRootOrgScopeSet(adUserObjectId.GetPartitionId()), 86, "GetPasswordExpirationDate", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\DirectoryHelper.cs");
			tenantOrTopologyConfigurationSession.UseConfigNC = false;
			ADDomain addomain = tenantOrTopologyConfigurationSession.Read<ADDomain>(aduser.Id.DomainId);
			if (addomain == null)
			{
				throw new ObjectNotFoundException(ServerStrings.ADUserNotFound);
			}
			EnhancedTimeSpan? maximumPasswordAge = addomain.MaximumPasswordAge;
			TimeSpan? timeSpan = (maximumPasswordAge != null) ? new TimeSpan?(maximumPasswordAge.GetValueOrDefault()) : null;
			if (timeSpan == null || timeSpan == TimeSpan.Zero)
			{
				return ExDateTime.MaxValue;
			}
			return exDateTime.Value.Add(timeSpan.Value);
		}

		public static bool ResetPassword(ADObjectId adUserObjectId, SecureString newPassword, IRecipientSession recipientSession)
		{
			if (newPassword == null)
			{
				throw new ArgumentNullException("newPassword");
			}
			if (adUserObjectId.IsNullOrEmpty())
			{
				throw new ArgumentException("ADUser object ID cannot be null or empty", "adUserObjectId");
			}
			recipientSession.SetPassword(adUserObjectId, newPassword);
			return true;
		}

		public static ADRecipient ReadADRecipient(Guid mailboxGuid, bool isArchive, IRecipientSession recipientSession)
		{
			if (isArchive)
			{
				return recipientSession.FindByExchangeGuidIncludingArchive(mailboxGuid);
			}
			return recipientSession.FindByExchangeGuid(mailboxGuid);
		}

		public static bool HasSharingPartnership(Guid mailboxGuid, bool isArchive, string externalId, IRecipientSession recipientSession)
		{
			SharingPartnerIdentityCollection sharingPartnerIdentityCollection = DirectoryHelper.ReadSharingPartnerIdentities(mailboxGuid, isArchive, recipientSession);
			return sharingPartnerIdentityCollection != null && sharingPartnerIdentityCollection.Count != 0 && sharingPartnerIdentityCollection.Contains(externalId);
		}

		private static SharingPartnerIdentityCollection ReadSharingPartnerIdentities(Guid mailboxGuid, bool isArchive, IRecipientSession recipientSession)
		{
			ADRecipient adrecipient = DirectoryHelper.ReadADRecipient(mailboxGuid, isArchive, recipientSession);
			if (adrecipient == null)
			{
				throw new ObjectNotFoundException(ServerStrings.ADUserNotFound);
			}
			ADUser aduser = adrecipient as ADUser;
			if (aduser == null)
			{
				ExTraceGlobals.StorageTracer.TraceDebug<ADRecipient>((long)mailboxGuid.GetHashCode(), "ExchangePrincipal::ReadSharingPartnerIdentities. This is not an ADUser so SharingPartnerIdentities doesn't apply. Recipient = {0}.", adrecipient);
				return null;
			}
			return aduser.SharingPartnerIdentities;
		}

		public static SharingPolicy ReadSharingPolicy(Guid mailboxGuid, bool isArchive, IRecipientSession recipientSession)
		{
			ADRecipient adrecipient = DirectoryHelper.ReadADRecipient(mailboxGuid, isArchive, recipientSession);
			if (adrecipient == null)
			{
				throw new ObjectNotFoundException(ServerStrings.ADUserNotFound);
			}
			ADUser aduser = adrecipient as ADUser;
			if (aduser == null)
			{
				ExTraceGlobals.StorageTracer.TraceError<ADRecipient>((long)mailboxGuid.GetHashCode(), "ExchangePrincipal::ReadSharingPolicy. This is not an ADUser so policy doesn't apply. Recipient = {0}.", adrecipient);
				return null;
			}
			ADObjectId adobjectId = aduser.SharingPolicy;
			ADSessionSettings adsessionSettings;
			if (SharedConfiguration.IsDehydratedConfiguration(aduser.OrganizationId))
			{
				SharedConfiguration sharedConfiguration = SharedConfiguration.GetSharedConfiguration(aduser.OrganizationId);
				adsessionSettings = sharedConfiguration.GetSharedConfigurationSessionSettings();
			}
			else
			{
				adsessionSettings = aduser.OrganizationId.ToADSessionSettings();
				adsessionSettings.IsSharedConfigChecked = true;
			}
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, adsessionSettings, 248, "ReadSharingPolicy", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\DirectoryHelper.cs");
			if (adobjectId == null)
			{
				FederatedOrganizationId federatedOrganizationId = tenantOrTopologyConfigurationSession.GetFederatedOrganizationId(tenantOrTopologyConfigurationSession.SessionSettings.CurrentOrganizationId);
				if (federatedOrganizationId != null)
				{
					adobjectId = federatedOrganizationId.DefaultSharingPolicyLink;
				}
			}
			SharingPolicy sharingPolicy = null;
			if (adobjectId != null)
			{
				sharingPolicy = tenantOrTopologyConfigurationSession.Read<SharingPolicy>(adobjectId);
				if (sharingPolicy == null)
				{
					throw new ObjectNotFoundException(ServerStrings.SharingPolicyNotFound(adobjectId.Name));
				}
			}
			return sharingPolicy;
		}

		public static void DoAdCallAndTranslateExceptions(Action call, string methodName)
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
