using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Provisioning;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ProvisioningTasks
{
	[Cmdlet("Get", "RecipientEnforcementProvisioningPolicy", DefaultParameterSetName = "Identity")]
	public sealed class GetRecipientEnforcementProvisioningPolicy : GetProvisioningPolicyBase<RecipientEnforcementProvisioningPolicyIdParameter, RecipientEnforcementProvisioningPolicy>
	{
		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				if (!this.IgnoreDehydratedFlag)
				{
					return SharedTenantConfigurationMode.Static;
				}
				return SharedTenantConfigurationMode.NotShared;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Status { get; set; }

		[Parameter(Mandatory = false)]
		public SwitchParameter IgnoreDehydratedFlag { get; set; }

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObject.Identity,
				dataObject
			});
			if (this.Status)
			{
				RecipientEnforcementProvisioningPolicy recipientEnforcementProvisioningPolicy = dataObject as RecipientEnforcementProvisioningPolicy;
				if (recipientEnforcementProvisioningPolicy != null)
				{
					ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, false);
					IConfigurationSession configSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(true, ConsistencyMode.PartiallyConsistent, sessionSettings, 74, "WriteResult", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\ProvisioningTasks\\Recipient\\GetRecipientEnforcementProvisioningPolicy.cs");
					recipientEnforcementProvisioningPolicy.MailboxCount = new int?(SystemAddressListMemberCount.GetCount(configSession, base.CurrentOrganizationId, "All Mailboxes(VLV)", false));
					recipientEnforcementProvisioningPolicy.MailUserCount = new int?(SystemAddressListMemberCount.GetCount(configSession, base.CurrentOrganizationId, "All Mail Users(VLV)", false));
					recipientEnforcementProvisioningPolicy.ContactCount = new int?(SystemAddressListMemberCount.GetCount(configSession, base.CurrentOrganizationId, "All Contacts(VLV)", false));
					recipientEnforcementProvisioningPolicy.DistributionListCount = new int?(SystemAddressListMemberCount.GetCount(configSession, base.CurrentOrganizationId, "All Groups(VLV)", false));
					try
					{
						recipientEnforcementProvisioningPolicy.TeamMailboxCount = new int?(SystemAddressListMemberCount.GetCount(configSession, base.CurrentOrganizationId, "TeamMailboxes(VLV)", false));
						recipientEnforcementProvisioningPolicy.PublicFolderMailboxCount = new int?(SystemAddressListMemberCount.GetCount(configSession, base.CurrentOrganizationId, "PublicFolderMailboxes(VLV)", false));
						recipientEnforcementProvisioningPolicy.MailPublicFolderCount = new int?(SystemAddressListMemberCount.GetCount(configSession, base.CurrentOrganizationId, "MailPublicFolders(VLV)", false));
					}
					catch (ADNoSuchObjectException)
					{
					}
				}
			}
			base.WriteResult(dataObject);
			TaskLogger.LogExit();
		}
	}
}
