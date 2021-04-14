using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public class RulesEvaluationHistory
	{
		internal RulesEvaluationHistory()
		{
			this.History = new List<RuleEvaluationResult>();
		}

		public IList<RuleEvaluationResult> History { get; private set; }

		public TimeSpan TimeSpent
		{
			get
			{
				TimeSpan seed = default(TimeSpan);
				return this.History.Aggregate(seed, (TimeSpan current, RuleEvaluationResult result) => current + result.TimeSpent);
			}
		}

		public void AddRuleEvaluationResult(PolicyEvaluationContext context)
		{
			if (context != null && context.CurrentRule != null && context.ComplianceItemPagedReader == null)
			{
				this.History.Add(new RuleEvaluationResult(context.CurrentRule.ImmutableId));
			}
		}

		public PredicateEvaluationResult AddPredicateEvaluationResult(PolicyEvaluationContext context)
		{
			if (context != null && context.CurrentRule != null && context.ComplianceItemPagedReader == null)
			{
				RuleEvaluationResult ruleEvaluationResult = this.History.FirstOrDefault((RuleEvaluationResult h) => h.RuleId == context.CurrentRule.ImmutableId);
				if (ruleEvaluationResult != null)
				{
					PredicateEvaluationResult predicateEvaluationResult = new PredicateEvaluationResult();
					ruleEvaluationResult.Predicates.Add(predicateEvaluationResult);
					return predicateEvaluationResult;
				}
			}
			return null;
		}

		internal RuleEvaluationResult GetCurrentRuleResult(PolicyEvaluationContext context)
		{
			RuleEvaluationResult result = null;
			if (context != null && context.CurrentRule != null && context.ComplianceItemPagedReader == null)
			{
				result = this.History.FirstOrDefault((RuleEvaluationResult h) => h.RuleId == context.CurrentRule.ImmutableId);
			}
			return result;
		}
	}
}
