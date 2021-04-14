using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Security.RightsManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IRmsLatencyTracker
	{
		void BeginTrackRmsLatency(RmsOperationType operation);

		void EndTrackRmsLatency(RmsOperationType operation);

		void EndAndBeginTrackRmsLatency(RmsOperationType endOperation, RmsOperationType beginOperation);
	}
}
