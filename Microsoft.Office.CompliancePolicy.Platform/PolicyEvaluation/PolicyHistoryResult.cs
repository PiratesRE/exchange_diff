using System;
using System.Collections.Generic;

namespace Microsoft.Office.CompliancePolicy.PolicyEvaluation
{
	public class PolicyHistoryResult
	{
		public PolicyHistoryResult()
		{
			this.Type = typeof(object);
			this.SupplementalInfo = 0;
			this.Results = new List<string>();
			this.TimeSpent = default(TimeSpan);
			this.Error = string.Empty;
		}

		public PolicyHistoryResult(Type type, IEnumerable<string> results, int supplementalInfo, TimeSpan timeSpent)
		{
			this.Type = type;
			this.Results = new List<string>(results);
			this.SupplementalInfo = supplementalInfo;
			this.TimeSpent = timeSpent;
			this.Error = string.Empty;
		}

		public string Name { get; private set; }

		public Type Type { get; private set; }

		public int SupplementalInfo { get; set; }

		public List<string> Results { get; private set; }

		public TimeSpan TimeSpent { get; set; }

		public string Error { get; private set; }
	}
}
