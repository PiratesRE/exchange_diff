using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Get", "DlpCompliancePolicy", DefaultParameterSetName = "Identity")]
	public sealed class GetDlpCompliancePolicy : GetCompliancePolicyBase
	{
		public GetDlpCompliancePolicy() : base(PolicyScenario.Dlp)
		{
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			PsDlpCompliancePolicy psDlpCompliancePolicy = new PsDlpCompliancePolicy(dataObject as PolicyStorage)
			{
				StorageBindings = Utils.LoadBindingStoragesByPolicy(base.DataSession, dataObject as PolicyStorage)
			};
			foreach (BindingStorage bindingStorage in psDlpCompliancePolicy.StorageBindings)
			{
				base.WriteVerbose(Strings.VerboseLoadBindingStorageObjects(bindingStorage.ToString(), psDlpCompliancePolicy.ToString()));
			}
			psDlpCompliancePolicy.PopulateTaskProperties(this, base.DataSession as IConfigurationSession);
			if (psDlpCompliancePolicy.ReadOnly)
			{
				this.WriteWarning(Strings.WarningTaskPolicyIsTooAdvancedToRead(psDlpCompliancePolicy.Name));
			}
			base.PopulateDistributionStatus(psDlpCompliancePolicy, dataObject as PolicyStorage);
			base.WriteResult(psDlpCompliancePolicy);
		}
	}
}
