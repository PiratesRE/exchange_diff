using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("disable", "AddressListPaging", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class DisableAddressListPaging : AddressListPagingTaskBase
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageDisableAddressListPaging((this.Identity != null) ? this.Identity.ToString() : "Current Oganization");
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
			OrganizationId currentOrganizationId = (this.Identity != null) ? this.DataObject.OrganizationId : base.CurrentOrganizationId;
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, currentOrganizationId, base.ExecutingUserOrganizationId, false);
			configurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(configurationSession.DomainController, false, ConsistencyMode.PartiallyConsistent, configurationSession.NetworkCredential, sessionSettings, 62, "InternalProcessRecord", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\organization\\DisableAddressListPaging.cs");
			Organization orgContainer = configurationSession.GetOrgContainer();
			if (orgContainer.IsAddressListPagingEnabled)
			{
				orgContainer.IsAddressListPagingEnabled = false;
				base.WriteVerbose(TaskVerboseStringHelper.GetSaveObjectVerboseString(orgContainer, configurationSession, typeof(Organization)));
				configurationSession.Save(orgContainer);
			}
		}
	}
}
