using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class PredicateEvaluationResult
	{
		internal List<string> MatchResults { get; private set; }

		internal bool IsMatch { get; set; }

		internal int SupplementalInfo { get; set; }

		internal Type Type { get; private set; }

		internal string PropertyName { get; private set; }

		internal PredicateEvaluationResult()
		{
			this.Type = typeof(object);
			this.MatchResults = new List<string>();
			this.SupplementalInfo = 0;
			this.IsMatch = false;
		}

		internal PredicateEvaluationResult(Type predicateType, bool isMatch, IEnumerable<string> matchingValues, int supplementalInfo)
		{
			this.IsMatch = isMatch;
			this.Type = predicateType;
			this.MatchResults = new List<string>(matchingValues);
			this.SupplementalInfo = supplementalInfo;
		}
	}
}
