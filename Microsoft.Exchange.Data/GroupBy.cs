using System;

namespace Microsoft.Exchange.Data
{
	public enum GroupBy
	{
		[LocDescription(DataStrings.IDs.GroupByDay)]
		Day,
		[LocDescription(DataStrings.IDs.GroupByMonth)]
		Month,
		[LocDescription(DataStrings.IDs.GroupByTotal)]
		Total
	}
}
