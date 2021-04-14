using System;

namespace Microsoft.Exchange.Diagnostics.LatencyDetection
{
	internal struct PROCESSOR_POWER_INFORMATION
	{
		public uint Number;

		public uint MaxMhz;

		public uint CurrentMhz;

		public uint MhzLimit;

		public uint MaxIdleState;

		public uint CurrentIdleState;
	}
}
