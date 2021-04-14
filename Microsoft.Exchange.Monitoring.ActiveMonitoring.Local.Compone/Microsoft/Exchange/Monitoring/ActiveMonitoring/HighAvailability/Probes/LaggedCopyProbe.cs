using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability.Probes
{
	public class LaggedCopyProbe : ProbeWorkItem
	{
		public static ProbeDefinition CreateDefinition(string name, string serviceName, MailboxDatabaseInfo targetDatabase, int recurrenceInterval, int timeout, int maxRetry)
		{
			ProbeDefinition probeDefinition = new ProbeDefinition();
			probeDefinition.AssemblyPath = Assembly.GetExecutingAssembly().Location;
			probeDefinition.ServiceName = serviceName;
			probeDefinition.TypeName = typeof(LaggedCopyProbe).FullName;
			probeDefinition.Name = name;
			probeDefinition.RecurrenceIntervalSeconds = recurrenceInterval;
			probeDefinition.TimeoutSeconds = timeout;
			probeDefinition.MaxRetryAttempts = maxRetry;
			probeDefinition.TargetResource = targetDatabase.MailboxDatabaseName;
			probeDefinition.Attributes["dbGuid"] = targetDatabase.MailboxDatabaseGuid.ToString();
			return probeDefinition;
		}

		public static ProbeDefinition CreateDefinition(string name, string serviceName, MailboxDatabaseInfo targetDatabase, int recurrenceInterval)
		{
			return LaggedCopyProbe.CreateDefinition(name, serviceName, targetDatabase, recurrenceInterval, recurrenceInterval / 2, 3);
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (HighAvailabilityUtility.CheckCancellationRequested(cancellationToken))
			{
				base.Result.StateAttribute1 = "Cancellation Requested!";
				return;
			}
			this.databaseGuid = new Guid(base.Definition.Attributes["dbGuid"]);
			base.Result.StateAttribute1 = base.Definition.Name;
			base.Result.StateAttribute2 = this.databaseGuid.ToString();
			List<CopyStatusClientCachedEntry> allCopyStatusesForDatabase = CachedDbStatusReader.Instance.GetAllCopyStatusesForDatabase(this.databaseGuid);
			bool flag = false;
			CopyStatusClientCachedEntry copyStatusClientCachedEntry = null;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (CopyStatusClientCachedEntry copyStatusClientCachedEntry2 in allCopyStatusesForDatabase)
			{
				if (copyStatusClientCachedEntry2.Result == CopyStatusRpcResult.Success && !copyStatusClientCachedEntry2.CopyStatus.IsActiveCopy())
				{
					stringBuilder.AppendFormat("Server={0}, Status={1}, ConfigLag={2} mins, LastLogPlayedTime={3}; ", new object[]
					{
						copyStatusClientCachedEntry2.CopyStatus.MailboxServer,
						copyStatusClientCachedEntry2.CopyStatus.CopyStatus,
						copyStatusClientCachedEntry2.CopyStatus.ConfiguredReplayLagTime.TotalMinutes,
						copyStatusClientCachedEntry2.CopyStatus.LastReplayedLogTime
					});
					if (copyStatusClientCachedEntry2.CopyStatus.ConfiguredReplayLagTime > TimeSpan.FromSeconds(0.0))
					{
						flag = true;
					}
					if (DateTime.UtcNow - copyStatusClientCachedEntry2.CopyStatus.LastReplayedLogTime.ToUniversalTime() > TimeSpan.FromDays(8.5))
					{
						copyStatusClientCachedEntry = copyStatusClientCachedEntry2;
					}
				}
			}
			base.Result.StateAttribute3 = stringBuilder.ToString();
			if (!flag || copyStatusClientCachedEntry != null)
			{
				string text = flag ? string.Empty : string.Format("No lag is configured for database {0}. ", base.Definition.TargetResource);
				if (copyStatusClientCachedEntry != null)
				{
					text += string.Format("One of the copies ({0}\\{1}) has last replay log time {2} and Lag is {3} on it.", new object[]
					{
						copyStatusClientCachedEntry.CopyStatus.DBName,
						copyStatusClientCachedEntry.CopyStatus.MailboxServer,
						copyStatusClientCachedEntry.CopyStatus.LastReplayedLogTime.ToString(),
						copyStatusClientCachedEntry.CopyStatus.ReplayLagEnabled
					});
				}
				throw new HighAvailabilityMAProbeRedResultException(text);
			}
		}

		private Guid databaseGuid;
	}
}
