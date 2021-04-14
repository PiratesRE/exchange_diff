using System;

namespace Microsoft.Exchange.DxStore.Common
{
	[Flags]
	public enum LogOptions
	{
		None = 0,
		LogException = 1,
		LogStart = 2,
		LogSuccess = 4,
		LogPeriodic = 8,
		LogAll = 7,
		LogExceptionFull = 16,
		LogAlways = 32768,
		LogNever = 16384
	}
}
