using System;

namespace Microsoft.Exchange.Diagnostics.LatencyDetection
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IPerformanceDataProvider
	{
		string Name { get; }

		bool ThreadLocal { get; }

		string Operations { get; }

		bool IsSnapshotInProgress { get; }

		PerformanceData TakeSnapshot(bool begin);

		void ResetOperations();
	}
}
