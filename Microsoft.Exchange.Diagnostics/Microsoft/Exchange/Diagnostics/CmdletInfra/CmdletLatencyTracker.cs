using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Diagnostics.CmdletInfra
{
	internal static class CmdletLatencyTracker
	{
		internal static LatencyTracker GetLatencyTracker(Guid cmdletUniqueId)
		{
			if (cmdletUniqueId == Guid.Empty && !CmdletThreadStaticData.TryGetCurrentCmdletUniqueId(out cmdletUniqueId))
			{
				return null;
			}
			LatencyTracker result;
			CmdletStaticDataWithUniqueId<LatencyTracker>.TryGet(cmdletUniqueId, out result);
			return result;
		}

		internal static void StartLatencyTracker(Guid cmdletUniqueId)
		{
			LatencyTracker latencyTracker = CmdletStaticDataWithUniqueId<LatencyTracker>.Get(cmdletUniqueId);
			if (latencyTracker != null)
			{
				CmdletLogger.SafeAppendGenericError(cmdletUniqueId, "StartLatencyTracker", string.Format("Latency tracker with this cmdlet {0} already exists. Override it anyway.", cmdletUniqueId), false);
			}
			latencyTracker = new LatencyTracker(cmdletUniqueId.ToString(), new Func<IActivityScope>(ActivityContext.GetCurrentActivityScope));
			latencyTracker.Start();
			CmdletStaticDataWithUniqueId<LatencyTracker>.Set(cmdletUniqueId, latencyTracker);
		}

		internal static long StopLatencyTracker(Guid cmdletUniqueId)
		{
			LatencyTracker latencyTracker = CmdletStaticDataWithUniqueId<LatencyTracker>.Get(cmdletUniqueId);
			if (latencyTracker == null)
			{
				return -1L;
			}
			return latencyTracker.Stop();
		}

		internal static void DisposeLatencyTracker(Guid cmdletUniqueId)
		{
			CmdletStaticDataWithUniqueId<LatencyTracker>.Remove(cmdletUniqueId);
		}

		internal static bool StartInternalTracking(Guid cmdletUniqueId, string funcName)
		{
			return CmdletLatencyTracker.StartInternalTracking(cmdletUniqueId, funcName, false);
		}

		internal static bool StartInternalTracking(Guid cmdletUniqueId, string funcName, bool logDetailsAlways)
		{
			return CmdletLatencyTracker.StartInternalTracking(cmdletUniqueId, funcName, funcName, logDetailsAlways);
		}

		internal static bool StartInternalTracking(Guid cmdletUniqueId, string groupName, string funcName, bool logDetailsAlways)
		{
			LatencyTracker latencyTracker = CmdletLatencyTracker.GetLatencyTracker(cmdletUniqueId);
			return latencyTracker != null && latencyTracker.StartInternalTracking(groupName, funcName, logDetailsAlways);
		}

		internal static void EndInternalTracking(Guid cmdletUniqueId, string funcName)
		{
			CmdletLatencyTracker.EndInternalTracking(cmdletUniqueId, funcName, funcName);
		}

		internal static void EndInternalTracking(Guid cmdletUniqueId, string groupName, string funcName)
		{
			LatencyTracker latencyTracker = CmdletLatencyTracker.GetLatencyTracker(cmdletUniqueId);
			if (latencyTracker == null)
			{
				return;
			}
			latencyTracker.EndInternalTracking(groupName, funcName);
		}

		internal static void PushLatencyDetailsToLog(Guid cmdletUniqueId, Dictionary<string, Enum> knownFuncNameToLogMetadataDic, Action<Enum, double> updateLatencyToLogger, Action<string, string> defaultLatencyLogger)
		{
			LatencyTracker latencyTracker = CmdletLatencyTracker.GetLatencyTracker(cmdletUniqueId);
			if (latencyTracker != null)
			{
				latencyTracker.PushLatencyDetailsToLog(knownFuncNameToLogMetadataDic, updateLatencyToLogger, defaultLatencyLogger);
				return;
			}
			if (defaultLatencyLogger != null)
			{
				defaultLatencyLogger("LatencyMissed", "latencyTracker is null");
				return;
			}
			if (updateLatencyToLogger != null)
			{
				updateLatencyToLogger(RpsCommonMetadata.GenericLatency, 0.0);
			}
		}
	}
}
