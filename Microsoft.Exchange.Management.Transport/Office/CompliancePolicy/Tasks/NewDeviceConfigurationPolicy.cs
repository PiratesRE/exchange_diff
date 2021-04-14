using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("New", "DeviceConfigurationPolicy", SupportsShouldProcess = true)]
	public sealed class NewDeviceConfigurationPolicy : NewDevicePolicyBase
	{
		public NewDeviceConfigurationPolicy() : base(PolicyScenario.DeviceSettings)
		{
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.PsPolicyPresentationObject = new DevicePolicy(this.DataObject)
			{
				Name = base.Name,
				Comment = base.Comment,
				Workload = Workload.Intune,
				Enabled = base.Enabled,
				Mode = Mode.Enforce,
				ExchangeBinding = base.InternalExchangeBindings,
				SharePointBinding = base.InternalSharePointBindings
			};
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			DevicePolicy devicePolicy = new DevicePolicy(dataObject as PolicyStorage)
			{
				StorageBindings = Utils.LoadBindingStoragesByPolicy(base.DataSession, dataObject as PolicyStorage)
			};
			devicePolicy.PopulateTaskProperties(this, base.DataSession as IConfigurationSession);
			base.WriteResult(devicePolicy);
		}
	}
}
