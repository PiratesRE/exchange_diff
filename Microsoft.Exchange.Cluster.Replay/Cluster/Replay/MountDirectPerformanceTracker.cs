using System;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Data.HA.DirectoryServices;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class MountDirectPerformanceTracker : FailoverPerformanceTrackerBase<MountDatabaseDirectOperation>
	{
		public MountDirectPerformanceTracker(Guid dbGuid) : base("MountDirectPerf")
		{
			this.m_dbGuid = dbGuid;
			this.m_dbName = this.LookupDatabaseName();
		}

		public long LastAcllLossAmount { private get; set; }

		public bool LastAcllRunWithSkipHealthChecks { private get; set; }

		public long HighestLogGenBefore { private get; set; }

		public long HighestLogGenAfter { private get; set; }

		public bool IsLossyMountEnabled { private get; set; }

		public bool IsDismountInProgress { private get; set; }

		public override void LogEvent()
		{
			ReplayCrimsonEvents.MountDirectPerformance.Log<string, Guid, long, TimeSpan, TimeSpan, TimeSpan, TimeSpan, TimeSpan, TimeSpan, TimeSpan, TimeSpan, TimeSpan, TimeSpan, TimeSpan, TimeSpan, TimeSpan, TimeSpan, bool, long, long, bool, bool>(this.m_dbName, this.m_dbGuid, this.LastAcllLossAmount, base.GetDuration(MountDatabaseDirectOperation.AmPreMountCallback), base.GetDuration(MountDatabaseDirectOperation.RegistryReplicatorCopy), base.GetDuration(MountDatabaseDirectOperation.StoreMount), base.GetDuration(MountDatabaseDirectOperation.PreMountQueuedOpStart), base.GetDuration(MountDatabaseDirectOperation.PreMountQueuedOpExecution), base.GetDuration(MountDatabaseDirectOperation.PreventMountIfNecessary), base.GetDuration(MountDatabaseDirectOperation.ResumeActiveCopy), base.GetDuration(MountDatabaseDirectOperation.UpdateLastLogGenOnMount), base.GetDuration(MountDatabaseDirectOperation.GetRunningReplicaInstance), base.GetDuration(MountDatabaseDirectOperation.ConfirmLogReset), base.GetDuration(MountDatabaseDirectOperation.LowestGenerationInDirectory), base.GetDuration(MountDatabaseDirectOperation.HighestGenerationInDirectory), base.GetDuration(MountDatabaseDirectOperation.GenerationAvailableInDirectory), base.GetDuration(MountDatabaseDirectOperation.UpdateLastLogGeneratedInClusDB), this.IsLossyMountEnabled, this.HighestLogGenBefore, this.HighestLogGenAfter, this.LastAcllRunWithSkipHealthChecks, this.IsDismountInProgress);
		}

		private string LookupDatabaseName()
		{
			string text = null;
			IADDatabase iaddatabase = Dependencies.ReplayAdObjectLookup.DatabaseLookup.FindAdObjectByGuid(this.m_dbGuid);
			if (iaddatabase != null)
			{
				text = iaddatabase.Name;
			}
			if (string.IsNullOrEmpty(text))
			{
				text = this.m_dbGuid.ToString();
			}
			return text;
		}

		private Guid m_dbGuid;

		private string m_dbName;
	}
}
