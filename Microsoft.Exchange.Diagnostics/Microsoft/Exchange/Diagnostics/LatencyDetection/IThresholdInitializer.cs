using System;

namespace Microsoft.Exchange.Diagnostics.LatencyDetection
{
	internal interface IThresholdInitializer
	{
		void SetThresholdFromConfiguration(LatencyDetectionLocation location, LoggingType type);
	}
}
