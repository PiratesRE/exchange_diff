using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Diagnostics.LatencyDetection
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface ILatencyDetectionLogger
	{
		LoggingType Type { get; }

		void Log(LatencyReportingThreshold threshold, LatencyDetectionContext trigger, ICollection<LatencyDetectionContext> context, LatencyDetectionException exception);
	}
}
