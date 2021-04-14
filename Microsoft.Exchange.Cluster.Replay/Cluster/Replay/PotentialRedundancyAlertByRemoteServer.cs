using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class PotentialRedundancyAlertByRemoteServer : MonitoringAlert
	{
		public PotentialRedundancyAlertByRemoteServer(DatabaseAlertInfoTable<DatabasePotentialOneCopyAlert> potentialOneCopyAlert) : base(string.Empty, Guid.Empty)
		{
			this.m_potentialOneCopyAlert = potentialOneCopyAlert;
		}

		public string RemoteServer
		{
			get
			{
				return this.m_RemoteServer;
			}
		}

		protected override TimeSpan DatabaseHealthCheckGreenTransitionSuppression
		{
			get
			{
				return TimeSpan.FromSeconds((double)RegistryParameters.DatabaseHealthCheckServerLevelPotentialOneCopyGreenTransitionSuppressionInSec);
			}
		}

		protected override TimeSpan DatabaseHealthCheckRedTransitionSuppression
		{
			get
			{
				return TimeSpan.FromSeconds((double)RegistryParameters.DatabaseHealthCheckServerLevelPotentialOneCopyRedTransitionSuppressionInSec);
			}
		}

		protected override TimeSpan DatabaseHealthCheckRedPeriodicInterval
		{
			get
			{
				return TimeSpan.FromSeconds((double)RegistryParameters.DatabaseHealthCheckServerLevelPotentialOneCopyRedPeriodicIntervalInSec);
			}
		}

		protected override bool IsValidationSuccessful(IHealthValidationResultMinimal serverValidationResult)
		{
			bool flag = false;
			StringBuilder stringBuilder = new StringBuilder(1024);
			IEnumerable<DatabasePotentialOneCopyAlert> source = from a in this.m_potentialOneCopyAlert
			select a.Value;
			IEnumerable<DatabasePotentialOneCopyAlert> source2 = from r in source
			where r.CurrentAlertState == TransientErrorInfo.ErrorType.Failure
			select r;
			if (source2.Any<DatabasePotentialOneCopyAlert>())
			{
				IEnumerable<DatabasePotentialOneCopyAlert> source3 = from f in source2
				where !string.IsNullOrEmpty(f.TargetServer)
				select f;
				if (!source3.Any<DatabasePotentialOneCopyAlert>())
				{
					MonitoringAlert.Tracer.TraceWarning<int>((long)this.GetHashCode(), "ServerLevelDatabasePotentialRedundancyAlert: None of the DatabasePotentialOneCopyAlert ({0}) has valid server name", source2.Count<DatabasePotentialOneCopyAlert>());
				}
				else
				{
					IEnumerable<IGrouping<string, DatabasePotentialOneCopyAlert>> source4 = source3.GroupBy((DatabasePotentialOneCopyAlert a) => a.TargetServer, StringComparer.OrdinalIgnoreCase);
					IGrouping<string, DatabasePotentialOneCopyAlert> grouping = source4.FirstOrDefault<IGrouping<string, DatabasePotentialOneCopyAlert>>();
					string targetServer = grouping.Key;
					if (source4.Count<IGrouping<string, DatabasePotentialOneCopyAlert>>() > 1)
					{
						IEnumerable<string> values = from f in source4
						select f.Key;
						string text = string.Join(", ", values);
						MonitoringAlert.Tracer.TraceWarning<string, string>((long)this.GetHashCode(), "ServerLevelDatabasePotentialRedundancyAlert: Found two or more servers with potential redundancy issues: {0}, will notify Active Monitoring to do recovery actions on the first server '{1}'", text, targetServer);
						ReplayCrimsonEvents.PotentialRedundancyAlertByRemoteServerCheckTwoOrMoreServersFound.LogPeriodic<string, string>(text, TimeSpan.FromMinutes(30.0), text, targetServer);
					}
					flag = true;
					this.m_RemoteServer = targetServer;
					DatabasePotentialOneCopyAlert[] array = grouping.ToArray<DatabasePotentialOneCopyAlert>();
					stringBuilder.Append(string.Join(", ", from f in array
					select f.Identity));
					from r in source
					where r.TargetServer == targetServer
					select r.Identity;
					stringBuilder.AppendLine();
					stringBuilder.AppendLine();
					foreach (DatabasePotentialOneCopyAlert databasePotentialOneCopyAlert in array)
					{
						stringBuilder.AppendFormat("{0} - {1}", databasePotentialOneCopyAlert.Identity, databasePotentialOneCopyAlert.ErrorMessageWithoutFullStatus);
						stringBuilder.AppendLine();
					}
					serverValidationResult.ErrorMessage = stringBuilder.ToString();
					serverValidationResult.ErrorMessageWithoutFullStatus = serverValidationResult.ErrorMessage;
				}
			}
			IEnumerable<DatabasePotentialOneCopyAlert> source5 = from r in source
			where r.CurrentAlertState == TransientErrorInfo.ErrorType.Success
			select r;
			this.m_suppressEventsDueToStartup = (!source5.Any<DatabasePotentialOneCopyAlert>() && !flag);
			return !flag;
		}

		protected override void RaiseGreenEvent(IHealthValidationResultMinimal result)
		{
			if (!this.m_suppressEventsDueToStartup)
			{
				ReplayCrimsonEvents.PotentialRedundancyAlertByRemoteServerCheckPassed.Log<string>(Environment.MachineName);
				new EventNotificationItem("msexchangerepl", "DatabaseRedundancy", "PotentialOneCopyByRemoteServerEvent", ResultSeverityLevel.Informational)
				{
					StateAttribute1 = string.Empty,
					StateAttribute2 = string.Empty
				}.Publish(false);
			}
		}

		protected override void RaiseRedEvent(IHealthValidationResultMinimal result)
		{
			if (!this.m_suppressEventsDueToStartup)
			{
				ReplayCrimsonEvents.PotentialRedundancyAlertByRemoteServerCheckFailed.Log<string, string, string>(Environment.MachineName, this.RemoteServer, EventUtil.TruncateStringInput(result.ErrorMessage, 32766));
				new EventNotificationItem("msexchangerepl", "DatabaseRedundancy", "PotentialOneCopyByRemoteServerEvent", ResultSeverityLevel.Critical)
				{
					StateAttribute1 = this.RemoteServer,
					StateAttribute2 = EventUtil.TruncateStringInput(result.ErrorMessage, 16383)
				}.Publish(false);
			}
		}

		private string m_RemoteServer;

		private DatabaseAlertInfoTable<DatabasePotentialOneCopyAlert> m_potentialOneCopyAlert;

		private bool m_suppressEventsDueToStartup;
	}
}
