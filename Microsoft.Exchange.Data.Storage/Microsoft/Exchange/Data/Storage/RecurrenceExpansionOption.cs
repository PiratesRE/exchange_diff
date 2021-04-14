using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	public enum RecurrenceExpansionOption
	{
		None = 0,
		IncludeMaster = 1,
		IncludeRegularOccurrences = 2,
		IncludeAll = 3,
		TruncateMaster = 4
	}
}
