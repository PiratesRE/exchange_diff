using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("New", "DlpCompliancePolicy", SupportsShouldProcess = true)]
	public sealed class NewDlpCompliancePolicy : NewCompliancePolicyBase
	{
		public NewDlpCompliancePolicy() : base(PolicyScenario.Dlp)
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
				SharePointBinding = base.InternalSharePointBindings,
				OneDriveBinding = base.InternalOneDriveBindings
			};
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			PsDlpCompliancePolicy psDlpCompliancePolicy = new PsDlpCompliancePolicy(dataObject as PolicyStorage)
			{
				StorageBindings = Utils.LoadBindingStoragesByPolicy(base.DataSession, dataObject as PolicyStorage)
			};
			psDlpCompliancePolicy.PopulateTaskProperties(this, base.DataSession as IConfigurationSession);
			base.WriteResult(psDlpCompliancePolicy);
		}
	}
}
