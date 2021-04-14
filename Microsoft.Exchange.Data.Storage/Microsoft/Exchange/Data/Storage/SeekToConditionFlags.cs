using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum SeekToConditionFlags
	{
		None = 0,
		AllowExtendedFilters = 1,
		AllowExtendedSeekReferences = 2,
		KeepCursorPositionWhenNoMatch = 4
	}
}
