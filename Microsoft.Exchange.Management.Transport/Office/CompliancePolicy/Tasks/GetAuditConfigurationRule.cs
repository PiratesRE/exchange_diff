using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Data.Storage.UnifiedPolicy;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Cmdlet("Get", "AuditConfigurationRule", DefaultParameterSetName = "Identity")]
	public sealed class GetAuditConfigurationRule : GetComplianceRuleBase
	{
		public GetAuditConfigurationRule() : base(PolicyScenario.AuditSettings)
		{
		}

		protected override IEnumerable<RuleStorage> GetPagedData()
		{
			return from p in base.GetPagedData()
			where p.Scenario == base.Scenario || AuditPolicyUtility.IsAuditConfigurationRule(p.Name)
			select p;
		}

		protected override void InternalValidate()
		{
			if (this.Identity != null && !AuditPolicyUtility.IsAuditConfigurationRule(this.Identity.ToString()))
			{
				base.WriteError(new ArgumentException(Strings.CanOnlyManipulateAuditConfigurationRule), ErrorCategory.InvalidArgument, null);
			}
			base.InternalValidate();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			AuditConfigurationRule auditConfigurationRule = new AuditConfigurationRule(dataObject as RuleStorage);
			auditConfigurationRule.PopulateTaskProperties();
			base.WriteResult(auditConfigurationRule);
		}
	}
}
