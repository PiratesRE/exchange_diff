using System;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Cluster.Replay.Dumpster;
using Microsoft.Exchange.Cluster.Shared;

internal static class SafetyNetRequestBackoff
{
	static SafetyNetRequestBackoff()
	{
		SafetyNetRequestBackoff.s_backOffEntries[0] = new SafetyNetRequestBackoff.BackoffEntry(TimeSpan.FromMinutes(30.0), TimeSpan.FromSeconds(100.0));
		SafetyNetRequestBackoff.s_backOffEntries[1] = new SafetyNetRequestBackoff.BackoffEntry(TimeSpan.FromHours(4.0), TimeSpan.FromMinutes(15.0));
		SafetyNetRequestBackoff.s_backOffEntries[2] = new SafetyNetRequestBackoff.BackoffEntry(TimeSpan.Zero, TimeSpan.FromHours(1.0));
	}

	public static DateTime GetNextDueTime(SafetyNetRequestKey snKey, SafetyNetInfo snInfo, bool inPrimaryPhase)
	{
		if (RegistryParameters.DumpsterRedeliveryIgnoreBackoff)
		{
			return DateTime.UtcNow;
		}
		TimeSpan t = DateTimeHelper.SafeSubtract(DateTime.UtcNow, inPrimaryPhase ? snKey.RequestCreationTimeUtc : snInfo.ShadowRequestCreateTimeUtc);
		for (int i = 0; i < SafetyNetRequestBackoff.s_backOffEntries.Length; i++)
		{
			SafetyNetRequestBackoff.BackoffEntry backoffEntry = SafetyNetRequestBackoff.s_backOffEntries[i];
			if (i == SafetyNetRequestBackoff.s_backOffEntries.Length - 1)
			{
				return snInfo.RequestLastAttemptedTimeUtc.Add(backoffEntry.DueTime);
			}
			if (t <= backoffEntry.AgeLimit)
			{
				return snInfo.RequestLastAttemptedTimeUtc.Add(backoffEntry.DueTime);
			}
		}
		return DateTime.UtcNow;
	}

	private static SafetyNetRequestBackoff.BackoffEntry[] s_backOffEntries = new SafetyNetRequestBackoff.BackoffEntry[3];

	private struct BackoffEntry
	{
		public BackoffEntry(TimeSpan ageLimit, TimeSpan dueTime)
		{
			this.AgeLimit = ageLimit;
			this.DueTime = dueTime;
		}

		public readonly TimeSpan AgeLimit;

		public readonly TimeSpan DueTime;
	}
}
