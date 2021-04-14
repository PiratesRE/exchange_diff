using System;

namespace Microsoft.Filtering
{
	[Flags]
	public enum RecoveryOptions
	{
		None = 0,
		Crash = 1,
		Timeout = 2
	}
}
