using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Install", "ProvisioningReconciliationConfig")]
	public sealed class InstallProvisioningReconciliationConfig : NewFixedNameSystemConfigurationObjectTask<ProvisioningReconciliationConfig>
	{
		protected override IConfigurable PrepareDataObject()
		{
			ProvisioningReconciliationConfig provisioningReconciliationConfig = (ProvisioningReconciliationConfig)base.PrepareDataObject();
			provisioningReconciliationConfig.SetId((IConfigurationSession)base.DataSession, ProvisioningReconciliationConfig.CanonicalName);
			return provisioningReconciliationConfig;
		}

		protected override void InternalProcessRecord()
		{
			ProvisioningReconciliationConfig[] array = this.ConfigurationSession.Find<ProvisioningReconciliationConfig>(null, QueryScope.SubTree, null, null, 1);
			if (array == null || array.Length == 0)
			{
				base.InternalProcessRecord();
			}
		}
	}
}
