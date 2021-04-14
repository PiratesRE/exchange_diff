using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class ServerLevelDatabaseStaleStatusAlert : MonitoringAlert
	{
		public ServerLevelDatabaseStaleStatusAlert(string serverName, Guid serverGuid, DatabaseAlertInfoTable<DatabaseStaleStatusAlert> staleAlerts) : base(serverName, serverGuid)
		{
			this.m_staleAlerts = staleAlerts;
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
				return TimeSpan.FromSeconds((double)RegistryParameters.DatabaseHealthCheckStaleStatusServerLevelRedTransitionSuppressionInSec);
			}
		}

		protected override TimeSpan DatabaseHealthCheckGreenPeriodicInterval
		{
			get
			{
				return TimeSpan.FromSeconds((double)RegistryParameters.DatabaseHealthCheckStaleStatusGreenPeriodicIntervalInSec);
			}
		}

		protected override TimeSpan DatabaseHealthCheckRedPeriodicInterval
		{
			get
			{
				return TimeSpan.FromSeconds((double)RegistryParameters.DatabaseHealthCheckStaleStatusRedPeriodicIntervalInSec);
			}
		}

		protected override bool IsValidationSuccessful(IHealthValidationResultMinimal serverValidationResult)
		{
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder(1024);
			foreach (KeyValuePair<Guid, DatabaseStaleStatusAlert> keyValuePair in ((IEnumerable<KeyValuePair<Guid, DatabaseStaleStatusAlert>>)this.m_staleAlerts))
			{
				if (keyValuePair.Value.CurrentAlertState == TransientErrorInfo.ErrorType.Failure)
				{
					if (num == 0)
					{
						stringBuilder.Append(keyValuePair.Value.Identity);
					}
					else
					{
						stringBuilder.AppendFormat(", {0}", keyValuePair.Value.Identity);
					}
					num++;
				}
			}
			string text = stringBuilder.ToString();
			if (num >= RegistryParameters.DatabaseHealthCheckStaleStatusServerLevelMinStaleCopies)
			{
				MonitoringAlert.Tracer.TraceError<int, string>((long)this.GetHashCode(), "ServerLevelDatabaseStaleStatusAlert: IsValidationSuccessful() found {0} stale copy status cached entries. Affected dbs: {1}", num, text);
				serverValidationResult.ErrorMessage = text;
				serverValidationResult.ErrorMessageWithoutFullStatus = text;
				return false;
			}
			return true;
		}

		protected override void RaiseGreenEvent(IHealthValidationResultMinimal result)
		{
			MonitoringAlert.Tracer.TraceDebug((long)this.GetHashCode(), "ServerLevelDatabaseStaleStatusAlert: RaiseGreenEvent() called!");
		}

		protected override void RaiseRedEvent(IHealthValidationResultMinimal result)
		{
			ReplayCrimsonEvents.ServerLevelDatabaseStaleStatusCheckFailed.Log<string, string>(base.Identity, EventUtil.TruncateStringInput(result.ErrorMessage, 32766));
			MonitoringAlert.Tracer.TraceError((long)this.GetHashCode(), "ServerLevelDatabaseStaleStatusAlert: RaiseRedEvent() called! Recovery will be attempted via Bugcheck.");
			BugcheckHelper.TriggerBugcheckIfRequired(DateTime.UtcNow, "ServerLevelDatabaseStaleStatusAlert is attempting recovery via BugCheck due to hung GetCopyStatus() RPC.");
		}

		private DatabaseAlertInfoTable<DatabaseStaleStatusAlert> m_staleAlerts;
	}
}
