using System;

namespace System.Diagnostics
{
	[Serializable]
	internal enum AssertFilters
	{
		FailDebug,
		FailIgnore,
		FailTerminate,
		FailContinueFilter
	}
}
