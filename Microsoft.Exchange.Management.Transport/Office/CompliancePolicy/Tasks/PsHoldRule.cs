using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.UnifiedPolicy;
using Microsoft.Exchange.Management.Transport;
using Microsoft.Office.CompliancePolicy.PolicyEvaluation;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	[Serializable]
	public sealed class PsHoldRule : PsComplianceRuleBase
	{
		public PsHoldRule()
		{
		}

		public PsHoldRule(RuleStorage ruleStorage) : base(ruleStorage)
		{
		}

		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return PsHoldRule.schema;
			}
		}

		public DateTime? ContentDateFrom
		{
			get
			{
				return (DateTime?)this[PsHoldRuleSchema.ContentDateFrom];
			}
			set
			{
				this[PsHoldRuleSchema.ContentDateFrom] = value;
			}
		}

		public DateTime? ContentDateTo
		{
			get
			{
				return (DateTime?)this[PsHoldRuleSchema.ContentDateTo];
			}
			set
			{
				this[PsHoldRuleSchema.ContentDateTo] = value;
			}
		}

		public HoldDurationHint HoldDurationDisplayHint
		{
			get
			{
				return (HoldDurationHint)this[PsHoldRuleSchema.HoldDurationDisplayHint];
			}
			set
			{
				this[PsHoldRuleSchema.HoldDurationDisplayHint] = value;
			}
		}

		public Unlimited<int>? HoldContent
		{
			get
			{
				int? num = (int?)this[PsHoldRuleSchema.HoldContent];
				if (num == null)
				{
					return null;
				}
				return new Unlimited<int>?((num.Value == 0) ? Unlimited<int>.UnlimitedValue : num.Value);
			}
			set
			{
				this[PsHoldRuleSchema.HoldContent] = ((value != null) ? new int?(value.Value.IsUnlimited ? 0 : value.Value.Value) : null);
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		internal override void PopulateTaskProperties(Task task, IConfigurationSession configurationSession)
		{
			base.PopulateTaskProperties(task, configurationSession);
			if (!string.IsNullOrEmpty(base.RuleBlob))
			{
				PolicyRule policyRuleFromRuleBlob = this.GetPolicyRuleFromRuleBlob();
				if (policyRuleFromRuleBlob.IsTooAdvancedToParse)
				{
					base.ReadOnly = true;
				}
				else
				{
					this.SetTaskConditions(PsHoldRule.ConvertEngineConditionToTaskConditions(policyRuleFromRuleBlob.Condition));
					this.SetTaskActions(PsHoldRule.ConvertEngineActionsToTaskActions(policyRuleFromRuleBlob.Actions));
				}
				base.ResetChangeTracking();
			}
		}

		internal override void UpdateStorageProperties(Task task, IConfigurationSession configurationSession, bool isNewRule)
		{
			base.UpdateStorageProperties(task, configurationSession, isNewRule);
			if (base.ObjectState != ObjectState.Unchanged)
			{
				PolicyRule policyRule = new PolicyRule
				{
					Condition = PsHoldRule.ConvertTaskConditionsToEngineCondition(this.GetTaskConditions()),
					Actions = PsHoldRule.ConvertTaskActionsToEngineActions(this.GetTaskActions()),
					Comments = base.Comment,
					Enabled = (base.Enabled ? RuleState.Enabled : RuleState.Disabled),
					ImmutableId = base.Guid,
					Name = base.Name
				};
				base.RuleBlob = this.GetRuleXmlFromPolicyRule(policyRule);
			}
		}

		internal override PolicyRule GetPolicyRuleFromRuleBlob()
		{
			RuleParser ruleParser = new RuleParser(new SimplePolicyParserFactory());
			return ruleParser.GetRule(base.RuleBlob);
		}

		private IEnumerable<PsComplianceRulePredicateBase> GetTaskConditions()
		{
			List<PsComplianceRulePredicateBase> list = new List<PsComplianceRulePredicateBase>();
			if (this.ContentDateFrom != null)
			{
				list.Add(new PsContentDateFromPredicate(this.ContentDateFrom.Value));
			}
			if (this.ContentDateTo != null)
			{
				list.Add(new PsContentDateToPredicate(this.ContentDateTo.Value));
			}
			if (!string.IsNullOrEmpty(base.ContentMatchQuery))
			{
				list.Add(new PsContentMatchQueryPredicate(base.ContentMatchQuery));
			}
			return list;
		}

		private void SetTaskConditions(IEnumerable<PsComplianceRulePredicateBase> conditions)
		{
			foreach (PsComplianceRulePredicateBase psComplianceRulePredicateBase in conditions)
			{
				if (psComplianceRulePredicateBase is PsContentMatchQueryPredicate)
				{
					base.ContentMatchQuery = (psComplianceRulePredicateBase as PsContentMatchQueryPredicate).TextQuery;
				}
				else if (psComplianceRulePredicateBase is PsContentDateFromPredicate)
				{
					PsContentDateFromPredicate psContentDateFromPredicate = psComplianceRulePredicateBase as PsContentDateFromPredicate;
					this.ContentDateFrom = new DateTime?(psContentDateFromPredicate.PropertyValue);
				}
				else
				{
					if (!(psComplianceRulePredicateBase is PsContentDateToPredicate))
					{
						throw new UnexpectedConditionOrActionDetectedException();
					}
					PsContentDateToPredicate psContentDateToPredicate = psComplianceRulePredicateBase as PsContentDateToPredicate;
					this.ContentDateTo = new DateTime?(psContentDateToPredicate.PropertyValue);
				}
			}
		}

		internal IEnumerable<PsComplianceRuleActionBase> GetTaskActions()
		{
			List<PsComplianceRuleActionBase> list = new List<PsComplianceRuleActionBase>();
			if (this.HoldContent != null)
			{
				PsHoldContentAction item = new PsHoldContentAction(this.HoldContent.Value.IsUnlimited ? 0 : this.HoldContent.Value.Value, this.HoldDurationDisplayHint);
				list.Add(item);
			}
			return list;
		}

		private void SetTaskActions(IEnumerable<PsComplianceRuleActionBase> actions)
		{
			foreach (PsComplianceRuleActionBase psComplianceRuleActionBase in actions)
			{
				if (!(psComplianceRuleActionBase is PsHoldContentAction))
				{
					throw new UnexpectedConditionOrActionDetectedException();
				}
				PsHoldContentAction psHoldContentAction = psComplianceRuleActionBase as PsHoldContentAction;
				this.HoldContent = new Unlimited<int>?((psHoldContentAction.HoldDurationDays == 0) ? Unlimited<int>.UnlimitedValue : psHoldContentAction.HoldDurationDays);
				this.HoldDurationDisplayHint = psHoldContentAction.HoldDurationDisplayHint;
			}
		}

		internal static IEnumerable<PsComplianceRulePredicateBase> ConvertEngineConditionToTaskConditions(Microsoft.Office.CompliancePolicy.PolicyEvaluation.Condition condition)
		{
			List<PsComplianceRulePredicateBase> list = new List<PsComplianceRulePredicateBase>();
			QueryPredicate queryPredicate = condition as QueryPredicate;
			if (queryPredicate != null)
			{
				AndCondition andCondition = queryPredicate.SubCondition as AndCondition;
				if (andCondition == null)
				{
					return list;
				}
				using (List<Microsoft.Office.CompliancePolicy.PolicyEvaluation.Condition>.Enumerator enumerator = andCondition.SubConditions.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Microsoft.Office.CompliancePolicy.PolicyEvaluation.Condition condition2 = enumerator.Current;
						list.AddRange(PsHoldRule.ConvertEngineConditionToTaskConditions(condition2));
					}
					return list;
				}
			}
			if (condition is PredicateCondition)
			{
				PredicateCondition predicate = condition as PredicateCondition;
				list.Add(PsComplianceRulePredicateBase.FromEnginePredicate(predicate));
			}
			else if (!(condition is TrueCondition))
			{
				throw new UnexpectedConditionOrActionDetectedException();
			}
			return list;
		}

		internal static Microsoft.Office.CompliancePolicy.PolicyEvaluation.Condition ConvertTaskConditionsToEngineCondition(IEnumerable<PsComplianceRulePredicateBase> predicates)
		{
			AndCondition andCondition = new AndCondition();
			foreach (PsComplianceRulePredicateBase psComplianceRulePredicateBase in predicates)
			{
				andCondition.SubConditions.Add(psComplianceRulePredicateBase.ToEnginePredicate());
			}
			if (!andCondition.SubConditions.Any<Microsoft.Office.CompliancePolicy.PolicyEvaluation.Condition>())
			{
				andCondition.SubConditions.Add(Microsoft.Office.CompliancePolicy.PolicyEvaluation.Condition.True);
			}
			return new QueryPredicate(andCondition);
		}

		internal static IEnumerable<PsComplianceRuleActionBase> ConvertEngineActionsToTaskActions(IList<Microsoft.Office.CompliancePolicy.PolicyEvaluation.Action> actions)
		{
			return actions.Select(new Func<Microsoft.Office.CompliancePolicy.PolicyEvaluation.Action, PsComplianceRuleActionBase>(PsComplianceRuleActionBase.FromEngineAction));
		}

		internal static IList<Microsoft.Office.CompliancePolicy.PolicyEvaluation.Action> ConvertTaskActionsToEngineActions(IEnumerable<PsComplianceRuleActionBase> actions)
		{
			return (from action in actions
			select action.ToEngineAction()).ToList<Microsoft.Office.CompliancePolicy.PolicyEvaluation.Action>();
		}

		private static readonly PsHoldRuleSchema schema = ObjectSchema.GetInstance<PsHoldRuleSchema>();
	}
}
