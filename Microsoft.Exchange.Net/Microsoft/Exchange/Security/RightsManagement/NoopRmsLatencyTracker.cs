using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Security.RightsManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class NoopRmsLatencyTracker : IRmsLatencyTracker
	{
		private NoopRmsLatencyTracker()
		{
		}

		public void BeginTrackRmsLatency(RmsOperationType operation)
		{
		}

		public void EndTrackRmsLatency(RmsOperationType operation)
		{
		}

		public void EndAndBeginTrackRmsLatency(RmsOperationType endOperation, RmsOperationType beginOperation)
		{
		}

		public static readonly NoopRmsLatencyTracker Instance = new NoopRmsLatencyTracker();
	}
}
