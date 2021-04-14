using System;

namespace Microsoft.Exchange.Diagnostics.LatencyDetection
{
	[Flags]
	public enum TriggerOptions
	{
		None = 0,
		DoNotTrigger = 1
	}
}
