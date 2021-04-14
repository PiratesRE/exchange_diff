using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public class RuleEvaluationResult
	{
		public RuleEvaluationResult()
		{
			this.Predicates = new List<PredicateEvaluationResult>();
			this.Actions = new List<PolicyHistoryResult>();
		}

		public RuleEvaluationResult(Guid ruleId) : this()
		{
			this.RuleId = ruleId;
		}

		public bool IsMatch
		{
			get
			{
				return this.Predicates.All((PredicateEvaluationResult p) => p.IsMatch);
			}
		}

		public Guid RuleId { get; set; }

		public IList<PredicateEvaluationResult> Predicates { get; set; }

		public IList<PolicyHistoryResult> Actions { get; set; }

		public TimeSpan TimeSpent
		{
			get
			{
				TimeSpan timeSpan = default(TimeSpan);
				timeSpan = this.Predicates.Aggregate(timeSpan, (TimeSpan current, PredicateEvaluationResult predicate) => current + predicate.TimeSpent);
				return timeSpan + this.Actions.Aggregate(timeSpan, (TimeSpan current, PolicyHistoryResult action) => current + action.TimeSpent);
			}
		}
	}
}
