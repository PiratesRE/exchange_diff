using System;
using Microsoft.Office.CompliancePolicy.ComplianceData;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.PolicyEvaluation;

namespace Microsoft.Office.CompliancePolicy.ComplianceTask
{
	public abstract class ComplianceServiceProvider
	{
		public abstract PolicyConfigProvider GetPolicyStore(ComplianceItemContainer rootContainer);

		public abstract PolicyConfigProvider GetPolicyStore(string tenantId);

		public abstract ExecutionLog GetExecutionLog();

		public abstract Auditor GetAuditor();

		public abstract ComplianceItemPagedReader GetPagedReader(ComplianceItemContainer container);

		public abstract RuleParser GetRuleParser();

		public abstract ComplianceItemContainer GetComplianceItemContainer(string tenantId, string scope);
	}
}
