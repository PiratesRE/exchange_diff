using System;

namespace Microsoft.Exchange.Data
{
	public enum SortOrder
	{
		[LocDescription(DataStrings.IDs.Ascending)]
		Ascending,
		[LocDescription(DataStrings.IDs.Descending)]
		Descending
	}
}
