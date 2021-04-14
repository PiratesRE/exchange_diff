using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Data.Storage.UnifiedPolicy;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Get", "HoldComplianceRule", DefaultParameterSetName = "Identity")]
	public sealed class GetHoldComplianceRule : GetComplianceRuleBase
	{
		public GetHoldComplianceRule() : base(PolicyScenario.Hold)
		{
		}

		protected override IEnumerable<RuleStorage> GetPagedData()
		{
			return from p in base.GetPagedData()
			where p.Scenario == base.Scenario || (!AuditPolicyUtility.IsAuditConfigurationRule(p.Name) && !DevicePolicyUtility.IsDeviceConfigurationRule(p.Name) && !DevicePolicyUtility.IsDeviceConditionalAccessRule(p.Name) && !DevicePolicyUtility.IsDeviceTenantRule(p.Name) && p.Scenario != PolicyScenario.Dlp)
			select p;
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			PsHoldRule psHoldRule = new PsHoldRule(dataObject as RuleStorage);
			psHoldRule.PopulateTaskProperties(this, base.DataSession as IConfigurationSession);
			if (psHoldRule.ReadOnly)
			{
				this.WriteWarning(Strings.WarningTaskRuleIsTooAdvancedToRead(psHoldRule.Name));
			}
			base.WriteResult(psHoldRule);
		}
	}
}
