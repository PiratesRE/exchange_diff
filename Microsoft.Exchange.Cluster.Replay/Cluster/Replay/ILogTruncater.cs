using System;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal interface ILogTruncater : IStartStop
	{
		void RecordReplayGeneration(long genRequired);

		void RecordInspectorGeneration(long genInspected);

		void StopTruncation();
	}
}
