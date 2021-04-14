using System;

namespace Microsoft.Exchange.Data.Search
{
	[Flags]
	internal enum WordBreakType
	{
		WORDREP_BREAK_EOW = 0,
		WORDREP_BREAK_EOS = 1,
		WORDREP_BREAK_EOP = 2,
		WORDREP_BREAK_EOC = 3
	}
}
