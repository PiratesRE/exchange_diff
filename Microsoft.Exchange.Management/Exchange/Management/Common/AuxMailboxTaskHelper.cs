using System;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Common
{
	internal static class AuxMailboxTaskHelper
	{
		internal static ADUser SplitArchiveFromPrimary(ADUser primaryUser, IRecipientSession recipientSession = null)
		{
			ADUser aduser = null;
			if (!primaryUser.ArchiveGuid.Equals(Guid.Empty))
			{
				aduser = new ADUser();
				aduser.StampPersistableDefaultValues();
				aduser.StampDefaultValues(RecipientType.UserMailbox);
				aduser.ResetChangeTracking();
				AuxMailboxTaskHelper.AuxMailboxStampDefaultValues(aduser);
				aduser.AddressListMembership = primaryUser.AddressListMembership;
				aduser.DisplayName = string.Format("{0} {1}", Strings.ArchiveNamePrefix, primaryUser.DisplayName);
				aduser.OrganizationId = primaryUser.OrganizationId;
				aduser.ExchangeGuid = primaryUser.ArchiveGuid;
				aduser.Database = primaryUser.ArchiveDatabase;
				aduser.Alias = RecipientTaskHelper.GenerateAlias(string.Format("{0}{1}", Strings.ArchiveNamePrefix, aduser.ExchangeGuid));
				aduser.Name = string.Format("{0}{1}", Strings.ArchiveNamePrefix, primaryUser.ExchangeGuid);
				if (recipientSession == null)
				{
					recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(false, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(primaryUser.OrganizationId), 74, "SplitArchiveFromPrimary", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Common\\recipient\\AuxMailboxTaskHelper.cs");
				}
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(false, ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(primaryUser.OrganizationId), 80, "SplitArchiveFromPrimary", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Common\\recipient\\AuxMailboxTaskHelper.cs");
				aduser.UserPrincipalName = RecipientTaskHelper.GenerateUniqueUserPrincipalName(recipientSession, aduser.Alias, tenantOrTopologyConfigurationSession.GetDefaultAcceptedDomain().DomainName.Domain, null);
				aduser.EmailAddresses.Add(new SmtpProxyAddress(aduser.UserPrincipalName, false));
				DatabaseLocationInfo serverForDatabase = ActiveManager.GetActiveManagerInstance().GetServerForDatabase(primaryUser.ArchiveDatabase.ObjectGuid);
				aduser.ServerLegacyDN = serverForDatabase.ServerLegacyDN;
				aduser.SetId(primaryUser.Id.Parent.GetChildId(aduser.Name));
			}
			return aduser;
		}

		internal static void AuxMailboxStampDefaultValues(ADUser user)
		{
			user.SetExchangeVersion(ADUser.GetMaximumSupportedExchangeObjectVersion(RecipientTypeDetails.UserMailbox, false));
			user.MasterAccountSid = new SecurityIdentifier(WellKnownSidType.SelfSid, null);
			user.HiddenFromAddressListsEnabled = true;
			user.IsAuxMailbox = true;
			MailboxTaskHelper.StampMailboxRecipientTypes(user, null);
		}
	}
}
