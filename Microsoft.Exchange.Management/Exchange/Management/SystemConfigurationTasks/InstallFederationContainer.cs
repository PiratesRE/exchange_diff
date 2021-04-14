using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Install", "FederationContainer")]
	public sealed class InstallFederationContainer : NewMultitenancyFixedNameSystemConfigurationObjectTask<FederatedOrganizationId>
	{
		protected override IConfigurable PrepareDataObject()
		{
			TaskLogger.LogEnter();
			FederatedOrganizationId federatedOrganizationId = null;
			IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
			ADObjectId orgContainerId = configurationSession.GetOrgContainerId();
			configurationSession.SessionSettings.IsSharedConfigChecked = true;
			FederatedOrganizationId[] array = configurationSession.Find<FederatedOrganizationId>(orgContainerId, QueryScope.SubTree, ADObject.ObjectClassFilter("msExchFedOrgId"), null, 1);
			if (array != null && array.Length == 1)
			{
				federatedOrganizationId = array[0];
			}
			if (federatedOrganizationId != null)
			{
				if (!federatedOrganizationId.Name.Equals("Federation", StringComparison.OrdinalIgnoreCase))
				{
					this.containerHasBeenRenamed = true;
				}
			}
			else
			{
				federatedOrganizationId = (FederatedOrganizationId)base.PrepareDataObject();
			}
			federatedOrganizationId.Name = "Federation";
			federatedOrganizationId.SetId((IConfigurationSession)base.DataSession, federatedOrganizationId.Name);
			TaskLogger.LogExit();
			return federatedOrganizationId;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (this.containerHasBeenRenamed || base.DataSession.Read<FederatedOrganizationId>(this.DataObject.Id) == null)
			{
				base.InternalProcessRecord();
			}
			TaskLogger.LogExit();
		}

		private bool containerHasBeenRenamed;
	}
}
