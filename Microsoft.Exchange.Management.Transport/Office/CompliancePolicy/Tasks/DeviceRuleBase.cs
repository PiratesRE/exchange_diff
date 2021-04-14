using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Office.CompliancePolicy.PolicyEvaluation;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Serializable]
	public abstract class DeviceRuleBase : PsComplianceRuleBase
	{
		protected DeviceRuleBase()
		{
		}

		protected abstract IEnumerable<Microsoft.Office.CompliancePolicy.PolicyEvaluation.Condition> GetTaskConditions();

		protected abstract void SetTaskConditions(IEnumerable<Microsoft.Office.CompliancePolicy.PolicyEvaluation.Condition> conditions);

		protected DeviceRuleBase(RuleStorage ruleStorage) : base(ruleStorage)
		{
		}

		public MultiValuedProperty<Guid> TargetGroups { get; set; }

		internal override void PopulateTaskProperties(Task task, IConfigurationSession configurationSession)
		{
			base.PopulateTaskProperties(task, configurationSession);
			if (!string.IsNullOrEmpty(base.RuleBlob))
			{
				this.SetTaskConditions(this.ConvertEngineConditionToTaskConditions(this.GetPolicyRuleFromRuleBlob().Condition));
			}
		}

		internal override void UpdateStorageProperties(Task task, IConfigurationSession configurationSession, bool isNewRule)
		{
			base.UpdateStorageProperties(task, configurationSession, isNewRule);
			PolicyRule policyRule = new PolicyRule
			{
				Condition = this.ConvertTaskConditionsToEngineCondition(this.GetTaskConditions()),
				Actions = DeviceRuleBase.EmptyActionList,
				Comments = string.Empty,
				Enabled = RuleState.Disabled,
				ImmutableId = base.Guid,
				Name = base.Name
			};
			base.RuleBlob = this.GetRuleXmlFromPolicyRule(policyRule);
		}

		internal override PolicyRule GetPolicyRuleFromRuleBlob()
		{
			return new RuleParser(new SimplePolicyParserFactory()).GetRule(base.RuleBlob);
		}

		internal IEnumerable<Microsoft.Office.CompliancePolicy.PolicyEvaluation.Condition> ConvertEngineConditionToTaskConditions(Microsoft.Office.CompliancePolicy.PolicyEvaluation.Condition condition)
		{
			AndCondition andCondition = condition as AndCondition;
			if (andCondition == null)
			{
				return Enumerable.Empty<Microsoft.Office.CompliancePolicy.PolicyEvaluation.Condition>();
			}
			return andCondition.SubConditions;
		}

		internal Microsoft.Office.CompliancePolicy.PolicyEvaluation.Condition ConvertTaskConditionsToEngineCondition(IEnumerable<Microsoft.Office.CompliancePolicy.PolicyEvaluation.Condition> predicates)
		{
			AndCondition andCondition = new AndCondition();
			andCondition.SubConditions.AddRange(predicates);
			return andCondition;
		}

		private static readonly List<Microsoft.Office.CompliancePolicy.PolicyEvaluation.Action> EmptyActionList = new List<Microsoft.Office.CompliancePolicy.PolicyEvaluation.Action>();
	}
}
