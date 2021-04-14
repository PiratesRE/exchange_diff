using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmListMdbStatusMonitor
	{
		public void SetTestKillStoreDelegate(AmListMdbStatusMonitor.KillStoreDelegate killStore)
		{
			this.killStoreDelegate = killStore;
		}

		public static AmListMdbStatusMonitor Instance
		{
			get
			{
				if (AmListMdbStatusMonitor.sm_instance == null)
				{
					AmListMdbStatusMonitor.sm_instance = new AmListMdbStatusMonitor();
				}
				return AmListMdbStatusMonitor.sm_instance;
			}
		}

		public void RecordFailure(AmServerName failingServer)
		{
			if (RegistryParameters.ListMdbStatusMonitorDisabled)
			{
				AmListMdbStatusMonitor.Tracer.TraceDebug(0L, "AmListMdbStatusMonitor disabled via regkey");
				return;
			}
			AmListMdbStatusMonitor.StatusRecord statusRecord = null;
			if (this.records.TryGetValue(failingServer, out statusRecord))
			{
				statusRecord.FailCount++;
				if (statusRecord.TimeOfFirstFailure == null)
				{
					statusRecord.TimeOfFirstFailure = new DateTime?(DateTime.UtcNow);
					return;
				}
				if (DateTime.UtcNow > statusRecord.TimeOfFirstFailure.Value + this.FailureSuppressionWindow)
				{
					this.AttemptRecovery(statusRecord);
					return;
				}
			}
			else
			{
				statusRecord = new AmListMdbStatusMonitor.StatusRecord();
				statusRecord.ServerName = failingServer;
				statusRecord.FailCount = 1;
				statusRecord.TimeOfFirstFailure = new DateTime?(DateTime.UtcNow);
				this.records[failingServer] = statusRecord;
			}
		}

		public void RecordSuccess(AmServerName succeedingServer)
		{
			AmListMdbStatusMonitor.StatusRecord statusRecord = null;
			if (this.records.TryGetValue(succeedingServer, out statusRecord))
			{
				statusRecord.FailCount = 0;
				statusRecord.TimeOfFirstFailure = null;
			}
		}

		private void AttemptRecovery(AmListMdbStatusMonitor.StatusRecord rec)
		{
			DateTime curTime = DateTime.UtcNow;
			if (rec.TimeOfLastRecoveryAction == null || curTime > rec.TimeOfLastRecoveryAction.Value + TimeSpan.FromSeconds((double)RegistryParameters.ListMdbStatusRecoveryLimitInSec))
			{
				rec.TimeOfLastRecoveryAction = new DateTime?(curTime);
				if (rec.ServerName.IsLocalComputerName)
				{
					ReplayCrimsonEvents.AmListMdbStatusMonitorLocalRecoveryStarts.Log();
					ThreadPool.QueueUserWorkItem(delegate(object param0)
					{
						this.killStoreDelegate(curTime, "ListMdbStatusMonitor");
					});
					return;
				}
			}
			else
			{
				AmListMdbStatusMonitor.Tracer.TraceDebug(0L, "AmListMdbStatusMonitor skipped recovery");
			}
		}

		public static readonly Trace Tracer = ExTraceGlobals.ActiveManagerTracer;

		private static AmListMdbStatusMonitor sm_instance;

		private AmListMdbStatusMonitor.KillStoreDelegate killStoreDelegate = new AmListMdbStatusMonitor.KillStoreDelegate(AmStoreServiceMonitor.KillStoreIfRunningBefore);

		private Dictionary<AmServerName, AmListMdbStatusMonitor.StatusRecord> records = new Dictionary<AmServerName, AmListMdbStatusMonitor.StatusRecord>();

		public TimeSpan FailureSuppressionWindow = TimeSpan.FromSeconds((double)RegistryParameters.ListMdbStatusFailureSuppressionWindowInSec);

		public delegate Exception KillStoreDelegate(DateTime limitTimeUtc, string reason);

		private class StatusRecord
		{
			public AmServerName ServerName { get; set; }

			public DateTime? TimeOfLastRecoveryAction { get; set; }

			public DateTime? TimeOfFirstFailure { get; set; }

			public int FailCount { get; set; }
		}
	}
}
