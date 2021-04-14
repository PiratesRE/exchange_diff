using System;
using Microsoft.Exchange.Data.Metering;
using Microsoft.Exchange.Data.Metering.Throttling;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal interface IMeteringComponent : ITransportComponent
	{
		ICountTracker<MeteredEntity, MeteredCount> Metering { get; }

		ICountTrackerDiagnostics<MeteredEntity, MeteredCount> MeteringDiagnostics { get; }

		void SetLoadtimeDependencies(ICountTrackerConfig config, Trace tracer);
	}
}
