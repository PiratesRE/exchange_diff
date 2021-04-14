using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("get", "RetentionPolicy", DefaultParameterSetName = "Identity")]
	public sealed class GetRetentionPolicy : GetMailboxPolicyBase<RetentionPolicy>
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter IgnoreDehydratedFlag { get; set; }

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				if (!this.IgnoreDehydratedFlag)
				{
					return SharedTenantConfigurationMode.Dehydrateable;
				}
				return SharedTenantConfigurationMode.NotShared;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			IConfigurationSession configurationSession;
			if (!this.IgnoreDehydratedFlag && SharedConfiguration.IsDehydratedConfiguration(base.CurrentOrganizationId))
			{
				configurationSession = SharedConfiguration.CreateScopedToSharedConfigADSession(base.CurrentOrganizationId);
				return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(false, ConsistencyMode.IgnoreInvalid, configurationSession.SessionSettings, 527, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxPolicies\\RetentionPolicyTasks.cs");
			}
			if (!MobileDeviceTaskHelper.IsRunningUnderMyOptionsRole(this, base.TenantGlobalCatalogSession, base.SessionSettings))
			{
				configurationSession = (IConfigurationSession)base.CreateSession();
			}
			else
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
				configurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, sessionSettings, 546, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxPolicies\\RetentionPolicyTasks.cs");
			}
			return configurationSession;
		}
	}
}
