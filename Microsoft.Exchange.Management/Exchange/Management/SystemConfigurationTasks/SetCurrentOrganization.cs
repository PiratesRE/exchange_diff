using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "CurrentOrganization", SupportsShouldProcess = true)]
	public sealed class SetCurrentOrganization : SystemConfigurationObjectActionTask<OrganizationIdParameter, ADOrganizationalUnit>
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter IgnoreSiteCheck
		{
			get
			{
				return (SwitchParameter)(base.Fields["IgnoreSiteCheck"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["IgnoreSiteCheck"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.SetCurrentOrganizationConfirmation(((ADObjectId)this.DataObject.Identity).Parent.Name);
			}
		}

		internal override IConfigurationSession CreateConfigurationSession(ADSessionSettings sessionSettings)
		{
			IConfigurationSession configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 46, "CreateConfigurationSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\organization\\SetCurrentOrganization.cs");
			configurationSession.UseConfigNC = false;
			return configurationSession;
		}

		protected override IConfigDataProvider CreateSession()
		{
			return this.CreateConfigurationSession(null);
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			this.IgnoreSiteCheck;
			base.ExchangeRunspaceConfig.SwitchCurrentPartnerOrganizationAndReloadRoleCmdletInfo(this.DataObject.Name);
			ExchangePropertyContainer.ResetPerOrganizationData(base.SessionState);
			TaskLogger.LogExit();
		}
	}
}
