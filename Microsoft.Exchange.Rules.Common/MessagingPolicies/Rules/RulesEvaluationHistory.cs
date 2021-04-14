using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class RulesEvaluationHistory
	{
		internal Dictionary<Guid, RuleEvaluationResult> History { get; private set; }

		internal RulesEvaluationHistory()
		{
			this.History = new Dictionary<Guid, RuleEvaluationResult>();
		}

		internal void AddRuleEvaluationResult(RulesEvaluationContext context)
		{
			if (context != null && context.CurrentRule != null && !this.History.ContainsKey(context.CurrentRule.ImmutableId))
			{
				this.History.Add(context.CurrentRule.ImmutableId, new RuleEvaluationResult());
			}
		}

		internal PredicateEvaluationResult AddPredicateEvaluationResult(RulesEvaluationContext context)
		{
			RuleEvaluationResult ruleEvaluationResult;
			if (context != null && context.CurrentRule != null && this.History.TryGetValue(context.CurrentRule.ImmutableId, out ruleEvaluationResult))
			{
				PredicateEvaluationResult predicateEvaluationResult = new PredicateEvaluationResult();
				ruleEvaluationResult.Predicates.Add(predicateEvaluationResult);
				return predicateEvaluationResult;
			}
			return null;
		}

		internal PredicateEvaluationResult AddPredicateEvaluationResult(RulesEvaluationContext context, Type predicateType, bool isMatch, IList<string> matchingValues, int supplementalInfo)
		{
			RuleEvaluationResult ruleEvaluationResult;
			if (context != null && context.CurrentRule != null && this.History.TryGetValue(context.CurrentRule.ImmutableId, out ruleEvaluationResult))
			{
				PredicateEvaluationResult predicateEvaluationResult = new PredicateEvaluationResult(predicateType, isMatch, matchingValues, supplementalInfo);
				ruleEvaluationResult.Predicates.Add(predicateEvaluationResult);
				return predicateEvaluationResult;
			}
			return null;
		}

		internal void SetCurrentRuleIsMatch(RulesEvaluationContext context, bool isMatch)
		{
			RuleEvaluationResult currentRuleResult = this.GetCurrentRuleResult(context);
			if (currentRuleResult != null)
			{
				currentRuleResult.IsMatch = isMatch;
			}
		}

		internal RuleEvaluationResult GetCurrentRuleResult(RulesEvaluationContext context)
		{
			RuleEvaluationResult result = null;
			if (context != null && context.Rules != null && context.CurrentRule != null)
			{
				this.History.TryGetValue(context.CurrentRule.ImmutableId, out result);
			}
			return result;
		}
	}
}
