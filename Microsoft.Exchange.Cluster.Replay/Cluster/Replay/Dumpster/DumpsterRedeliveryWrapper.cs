using System;

namespace Microsoft.Exchange.Cluster.Replay.Dumpster
{
	internal static class DumpsterRedeliveryWrapper
	{
		public static void MarkRedeliveryRequired(ReplayConfiguration configuration, DateTime inspectorTime, long lastLogGenBeforeActivation, long numLogsLost)
		{
			SafetyNetRedelivery.MarkRedeliveryRequired(configuration, inspectorTime, lastLogGenBeforeActivation, numLogsLost);
		}

		public static void MarkRedeliveryRequired(ReplayConfiguration configuration, DateTime failoverTimeUtc, DateTime startTimeUtc, DateTime endTimeUtc, long lastLogGenBeforeActivation, long numLogsLost)
		{
			SafetyNetRedelivery.MarkRedeliveryRequired(configuration, failoverTimeUtc, startTimeUtc, endTimeUtc, lastLogGenBeforeActivation, numLogsLost);
		}

		public static void DoRedeliveryIfRequired(object replayConfig)
		{
			SafetyNetRedelivery.DoRedeliveryIfRequired(replayConfig);
		}

		public static bool IsRedeliveryRequired(ReplayConfiguration replayConfig)
		{
			SafetyNetInfoCache safetyNetTable = replayConfig.ReplayState.GetSafetyNetTable();
			return safetyNetTable.IsRedeliveryRequired(true, true);
		}
	}
}
