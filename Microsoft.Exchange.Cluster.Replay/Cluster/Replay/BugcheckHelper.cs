using System;
using System.Threading;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class BugcheckHelper
	{
		public static void TriggerBugcheckIfRequired(DateTime bugcheckRequestTimeUtc, string bugcheckReason)
		{
			if (RegistryParameters.DisableBugcheckOnHungIo != 0)
			{
				ReplayCrimsonEvents.BugcheckAttemptSkipped.Log<string>(bugcheckReason);
				return;
			}
			if (Interlocked.CompareExchange(ref BugcheckHelper.s_numThreadsInBugcheck, 1, 0) == 1)
			{
				ReplayCrimsonEvents.BugcheckAttemptSkippedInProgress.Log<string>(bugcheckReason);
				return;
			}
			try
			{
				DateTime utcNow = DateTime.UtcNow;
				DateTime localBootTime = AmHelper.GetLocalBootTime();
				if (bugcheckRequestTimeUtc > localBootTime || localBootTime >= utcNow)
				{
					ReplayCrimsonEvents.BugCheckAttemptTriggered.Log<string, string>(localBootTime.ToString("s"), bugcheckReason);
					bool flag = false;
					ExTraceGlobals.FaultInjectionTracer.TraceTest<bool>(4291177789U, ref flag);
					Exception ex = null;
					if (!flag)
					{
						ex = BugcheckHelper.KillWinInitProcess();
					}
					else
					{
						AmTrace.Error("Skipping KillProcess() of 'winint.exe' due to Fault Injection.", new object[0]);
					}
					if (ex != null)
					{
						ReplayCrimsonEvents.BugcheckAttemptFailed.Log<string, string, string>(ex.Message, localBootTime.ToString("s"), bugcheckReason);
					}
				}
			}
			finally
			{
				if (Interlocked.CompareExchange(ref BugcheckHelper.s_numThreadsInBugcheck, 0, 1) == 0)
				{
					DiagCore.RetailAssert(false, "We should not have more than 1 thread in TriggerBugcheckIfRequired()", new object[0]);
				}
			}
		}

		internal static Exception KillWinInitProcess()
		{
			return ServiceOperations.KillProcess("wininit", true);
		}

		private const string WinInitProcess = "wininit";

		private static int s_numThreadsInBugcheck;
	}
}
