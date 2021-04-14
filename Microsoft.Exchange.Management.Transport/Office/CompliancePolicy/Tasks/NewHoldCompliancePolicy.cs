using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("New", "HoldCompliancePolicy", SupportsShouldProcess = true)]
	public sealed class NewHoldCompliancePolicy : NewCompliancePolicyBase
	{
		public NewHoldCompliancePolicy() : base(PolicyScenario.Hold)
		{
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.PsPolicyPresentationObject = new PsHoldCompliancePolicy(this.DataObject)
			{
				Name = base.Name,
				Comment = base.Comment,
				Workload = base.TenantWorkloadConfig,
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
			PsHoldCompliancePolicy psHoldCompliancePolicy = new PsHoldCompliancePolicy(dataObject as PolicyStorage)
			{
				StorageBindings = Utils.LoadBindingStoragesByPolicy(base.DataSession, dataObject as PolicyStorage)
			};
			psHoldCompliancePolicy.PopulateTaskProperties(this, base.DataSession as IConfigurationSession);
			base.WriteResult(psHoldCompliancePolicy);
		}
	}
}
