using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class RuleEvaluationResult
	{
		internal bool IsMatch { get; set; }

		internal IList<PredicateEvaluationResult> Predicates { get; set; }

		internal IList<string> Actions { get; set; }

		internal RuleEvaluationResult()
		{
			this.Predicates = new List<PredicateEvaluationResult>();
			this.Actions = new List<string>();
		}

		internal static IList<PredicateEvaluationResult> GetPredicateEvaluationResult(Type predicateType, IList<PredicateEvaluationResult> predicates)
		{
			return (from predicateEvaluationResult in predicates
			where predicateEvaluationResult.Type == predicateType
			select predicateEvaluationResult).ToList<PredicateEvaluationResult>();
		}
	}
}
