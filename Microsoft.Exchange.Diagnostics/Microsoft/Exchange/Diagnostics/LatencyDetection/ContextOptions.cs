using System;

namespace Microsoft.Exchange.Diagnostics.LatencyDetection
{
	[Flags]
	internal enum ContextOptions
	{
		Default = 0,
		DoNotMeasureTime = 1,
		DoNotCreateReport = 2
	}
}
