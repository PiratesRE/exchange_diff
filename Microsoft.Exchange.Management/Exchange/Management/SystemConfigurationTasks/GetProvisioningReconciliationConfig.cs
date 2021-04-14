using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.ProvisioningReconciliation;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "ProvisioningReconciliationConfig")]
	public sealed class GetProvisioningReconciliationConfig : GetSingletonSystemConfigurationObjectTask<ProvisioningReconciliationConfig>
	{
		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		internal new Fqdn DomainController { get; set; }

		protected override void WriteResult(IConfigurable dataObject)
		{
			if (dataObject != null)
			{
				ProvisioningReconciliationConfig provisioningReconciliationConfig = (ProvisioningReconciliationConfig)dataObject;
				if (provisioningReconciliationConfig != null)
				{
					provisioningReconciliationConfig.ReconciliationCookieForCurrentCycle = ProvisioningReconciliationHelper.GetReconciliationCookie(provisioningReconciliationConfig, new Task.TaskErrorLoggingDelegate(base.WriteError));
					provisioningReconciliationConfig.ReconciliationCookiesForNextCycle = ProvisioningReconciliationHelper.GetReconciliationCookiesForNextCycle(provisioningReconciliationConfig.ReconciliationCookieForCurrentCycle.DCHostName, new Task.TaskErrorLoggingDelegate(base.WriteError));
				}
				base.WriteResult(provisioningReconciliationConfig);
				return;
			}
			base.WriteResult(dataObject);
		}
	}
}
