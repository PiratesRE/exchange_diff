using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Win32;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class ServerLevelDatabaseRedundancyAlert : MonitoringAlert
	{
		public ServerLevelDatabaseRedundancyAlert(string serverName, Guid serverGuid, DatabaseAlertInfoTable<DatabaseRedundancyAlert> oneCopyAlerts) : base(serverName, serverGuid)
		{
			this.m_oneCopyAlerts = oneCopyAlerts;
		}

		protected override TimeSpan DatabaseHealthCheckGreenTransitionSuppression
		{
			get
			{
				return TimeSpan.Zero;
			}
		}

		protected override TimeSpan DatabaseHealthCheckRedTransitionSuppression
		{
			get
			{
				return TimeSpan.Zero;
			}
		}

		protected override bool IsValidationSuccessful(IHealthValidationResultMinimal serverValidationResult)
		{
			bool flag = false;
			bool flag2 = false;
			StringBuilder stringBuilder = new StringBuilder(1024);
			foreach (KeyValuePair<Guid, DatabaseRedundancyAlert> keyValuePair in ((IEnumerable<KeyValuePair<Guid, DatabaseRedundancyAlert>>)this.m_oneCopyAlerts))
			{
				if (keyValuePair.Value.CurrentAlertState == TransientErrorInfo.ErrorType.Failure)
				{
					if (!flag)
					{
						stringBuilder.Append(keyValuePair.Value.Identity);
						flag = true;
					}
					else
					{
						stringBuilder.AppendFormat(", {0}", keyValuePair.Value.Identity);
					}
				}
				else if (keyValuePair.Value.CurrentAlertState == TransientErrorInfo.ErrorType.Success)
				{
					flag2 = true;
				}
			}
			if (flag)
			{
				stringBuilder.AppendLine();
				stringBuilder.AppendLine();
				foreach (KeyValuePair<Guid, DatabaseRedundancyAlert> keyValuePair2 in ((IEnumerable<KeyValuePair<Guid, DatabaseRedundancyAlert>>)this.m_oneCopyAlerts))
				{
					if (keyValuePair2.Value.CurrentAlertState == TransientErrorInfo.ErrorType.Failure)
					{
						stringBuilder.AppendFormat("{0} - {1}", keyValuePair2.Value.Identity, keyValuePair2.Value.ErrorMessageWithoutFullStatus);
						stringBuilder.AppendLine();
					}
				}
				serverValidationResult.ErrorMessage = stringBuilder.ToString();
				serverValidationResult.ErrorMessageWithoutFullStatus = serverValidationResult.ErrorMessage;
			}
			this.m_suppressEventsDueToStartup = (!flag2 && !flag);
			return !flag;
		}

		protected override void RaiseGreenEvent(IHealthValidationResultMinimal result)
		{
			if (!this.m_suppressEventsDueToStartup)
			{
				ReplayEventLogConstants.Tuple_MonitoringDatabaseRedundancyServerCheckPassed.LogEvent(null, new object[]
				{
					base.Identity
				});
				new EventNotificationItem("msexchangerepl", "DatabaseRedundancy", "OneCopyServerEvent", ResultSeverityLevel.Informational)
				{
					StateAttribute1 = base.Identity,
					StateAttribute2 = string.Empty
				}.Publish(false);
			}
			this.WriteLastRunTime();
		}

		protected override void RaiseRedEvent(IHealthValidationResultMinimal result)
		{
			if (!this.m_suppressEventsDueToStartup)
			{
				ReplayEventLogConstants.Tuple_MonitoringDatabaseRedundancyServerCheckFailed.LogEvent(null, new object[]
				{
					base.Identity,
					EventUtil.TruncateStringInput(result.ErrorMessage, 32766)
				});
				new EventNotificationItem("msexchangerepl", "DatabaseRedundancy", "OneCopyServerEvent", ResultSeverityLevel.Critical)
				{
					StateAttribute1 = base.Identity,
					StateAttribute2 = EventUtil.TruncateStringInput(result.ErrorMessage, 16383)
				}.Publish(false);
			}
			this.WriteLastRunTime();
		}

		private void WriteLastRunTime()
		{
			Exception ex = RegistryUtil.RunRegistryFunction(delegate()
			{
				using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\HighAvailability\\States"))
				{
					registryKey.SetValue("OneCopyMonitorLastRun", DateTime.UtcNow.ToString());
				}
			});
			if (ex != null)
			{
				MonitoringAlert.Tracer.TraceError<Exception>((long)this.GetHashCode(), "ServerLevelDatabaseRedundancyAlert: WriteLastRunTime() failed with: {0}", ex);
				ReplayCrimsonEvents.MonitoringServerCheckFailedToWriteLastRunTime.LogPeriodic<string, Exception>(this.GetHashCode(), DiagCore.DefaultEventSuppressionInterval, ex.Message, ex);
			}
		}

		private const string MonitorStateValueName = "OneCopyMonitorLastRun";

		private DatabaseAlertInfoTable<DatabaseRedundancyAlert> m_oneCopyAlerts;

		private bool m_suppressEventsDueToStartup;
	}
}
