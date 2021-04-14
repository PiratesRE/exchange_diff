using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Hygiene.Data.Spam
{
	public class PredicateData
	{
		public long? RuleID { get; set; }

		public PredicateType PredicateType { get; set; }

		public List<PredicateData> ChildPredicate { get; set; }

		public long? ProcessorID { get; set; }

		public int? Sequence { get; set; }

		public int? MinOccurs { get; set; }

		public int? MaxOccurs { get; set; }

		public string Target { get; set; }

		public long? Value { get; set; }

		public NumericOperationType? Operation { get; set; }
	}
}
