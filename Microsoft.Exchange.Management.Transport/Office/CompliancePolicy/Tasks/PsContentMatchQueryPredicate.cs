using System;
using Microsoft.Office.CompliancePolicy.PolicyEvaluation;

namespace Microsoft.Office.CompliancePolicy.Tasks
{
	public sealed class PsContentMatchQueryPredicate : PsComplianceRulePredicateBase
	{
		public PsContentMatchQueryPredicate(string textQuery)
		{
			this.TextQuery = textQuery;
		}

		public string TextQuery { get; private set; }

		internal override PredicateCondition ToEnginePredicate()
		{
			return new TextQueryPredicate(this.TextQuery);
		}

		internal static PsContentMatchQueryPredicate FromEnginePredicate(TextQueryPredicate condition)
		{
			return new PsContentMatchQueryPredicate(condition.TextQuery);
		}
	}
}
