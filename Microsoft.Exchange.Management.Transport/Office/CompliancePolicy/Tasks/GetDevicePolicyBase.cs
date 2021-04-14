using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	public class GetDevicePolicyBase : GetCompliancePolicyBase
	{
		protected GetDevicePolicyBase()
		{
		}

		protected GetDevicePolicyBase(PolicyScenario scenario) : base(scenario)
		{
			DevicePolicyUtility.ValidateDeviceScenarioArgument(scenario);
		}

		private PsCompliancePolicyBase CreatePolicyByScenario(PolicyStorage policyStorage)
		{
			if (policyStorage.Scenario == PolicyScenario.DeviceSettings)
			{
				return new DevicePolicy(policyStorage);
			}
			if (policyStorage.Scenario == PolicyScenario.DeviceConditionalAccess)
			{
				return new DeviceConditionalAccessPolicy(policyStorage);
			}
			if (policyStorage.Scenario == PolicyScenario.DeviceTenantConditionalAccess)
			{
				return new DeviceTenantPolicy(policyStorage);
			}
			return null;
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			PsCompliancePolicyBase psCompliancePolicyBase = this.CreatePolicyByScenario(dataObject as PolicyStorage);
			if (psCompliancePolicyBase != null)
			{
				psCompliancePolicyBase.StorageBindings = Utils.LoadBindingStoragesByPolicy(base.DataSession, dataObject as PolicyStorage);
				foreach (BindingStorage bindingStorage in psCompliancePolicyBase.StorageBindings)
				{
					base.WriteVerbose(Strings.VerboseLoadBindingStorageObjects(bindingStorage.ToString(), psCompliancePolicyBase.ToString()));
				}
				psCompliancePolicyBase.PopulateTaskProperties(this, base.DataSession as IConfigurationSession);
				if (psCompliancePolicyBase.ReadOnly)
				{
					this.WriteWarning(Strings.WarningTaskPolicyIsTooAdvancedToRead(psCompliancePolicyBase.Name));
				}
				PolicySettingStatusHelpers.PopulatePolicyDistributionStatus(psCompliancePolicyBase, dataObject as PolicyStorage, base.DataSession, this, this.executionLogger);
				base.WriteResult(psCompliancePolicyBase);
			}
		}
	}
}
