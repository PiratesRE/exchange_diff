using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	public abstract class GetDeviceRuleBase : GetComplianceRuleBase
	{
		protected GetDeviceRuleBase(PolicyScenario scenario) : base(scenario)
		{
			DevicePolicyUtility.ValidateDeviceScenarioArgument(scenario);
		}

		protected abstract bool IsDeviceRule(string identity);

		protected abstract DeviceRuleBase CreateDeviceRuleObject(RuleStorage ruleStorage);

		protected abstract string GetCanOnlyManipulateErrorString();

		protected override IEnumerable<RuleStorage> GetPagedData()
		{
			return from p in base.GetPagedData()
			where p.Scenario == base.Scenario || this.IsDeviceRule(p.Name)
			select p;
		}

		protected override void InternalValidate()
		{
			if (this.Identity != null && !this.IsDeviceRule(this.Identity.ToString()))
			{
				base.WriteError(new ArgumentException(this.GetCanOnlyManipulateErrorString()), ErrorCategory.InvalidArgument, null);
			}
			base.InternalValidate();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			DeviceRuleBase deviceRuleBase = this.CreateDeviceRuleObject(dataObject as RuleStorage);
			deviceRuleBase.PopulateTaskProperties(this, base.DataSession as IConfigurationSession);
			base.WriteResult(deviceRuleBase);
		}
	}
}
