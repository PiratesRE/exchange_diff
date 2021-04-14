using System;

namespace Microsoft.Exchange.Data
{
	[Flags]
	internal enum DsnRequestedFlags
	{
		Default = 0,
		Success = 1,
		Failure = 2,
		Delay = 4,
		Never = 8,
		AllFlags = 15
	}
}
