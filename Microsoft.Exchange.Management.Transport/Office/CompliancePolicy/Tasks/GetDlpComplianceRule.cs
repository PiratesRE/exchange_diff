using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Get", "DlpComplianceRule", DefaultParameterSetName = "Identity")]
	public sealed class GetDlpComplianceRule : GetComplianceRuleBase
	{
		public GetDlpComplianceRule() : base(PolicyScenario.Dlp)
		{
		}

		protected override IEnumerable<RuleStorage> GetPagedData()
		{
			return from p in base.GetPagedData()
			where p.Scenario == base.Scenario
			select p;
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			PsDlpComplianceRule psDlpComplianceRule = new PsDlpComplianceRule(dataObject as RuleStorage);
			psDlpComplianceRule.PopulateTaskProperties(this, base.DataSession as IConfigurationSession);
			if (psDlpComplianceRule.ReadOnly)
			{
				this.WriteWarning(Strings.WarningTaskRuleIsTooAdvancedToRead(psDlpComplianceRule.Name));
			}
			base.WriteResult(psDlpComplianceRule);
		}
	}
}
