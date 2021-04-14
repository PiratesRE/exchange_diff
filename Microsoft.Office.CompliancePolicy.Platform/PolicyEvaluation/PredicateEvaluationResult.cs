using System;
using System.Collections.Generic;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public class PredicateEvaluationResult : PolicyHistoryResult
	{
		public PredicateEvaluationResult()
		{
			this.IsMatch = false;
		}

		public PredicateEvaluationResult(Type predicateType, bool isMatch, IEnumerable<string> matchingValues, int supplementalInfo, TimeSpan timeSpent) : base(predicateType, matchingValues, supplementalInfo, timeSpent)
		{
			this.IsMatch = isMatch;
		}

		public bool IsMatch { get; set; }
	}
}
