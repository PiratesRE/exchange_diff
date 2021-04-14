using System;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	public class SpamRuleData : RuleDataBase
	{
		public PredicateData Predicate { get; set; }

		public ResultData Result { get; set; }

		public int? AsfID { get; set; }

		public string ConditionMatchPhrase { get; set; }

		public string ConditionNotMatchPhrase { get; set; }

		public AuthoringData AuthoringProperties { get; set; }
	}
}
