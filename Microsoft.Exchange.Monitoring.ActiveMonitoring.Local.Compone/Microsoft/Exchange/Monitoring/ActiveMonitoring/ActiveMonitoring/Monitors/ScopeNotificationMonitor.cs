using System;
using System.Data.SqlTypes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Monitors
{
	public abstract class ScopeNotificationMonitor : MonitorWorkItem
	{
		protected override void DoMonitorWork(CancellationToken cancellationToken)
		{
			IDataAccessQuery<MonitorResult> lastSuccessfulMonitorResult = base.Broker.GetLastSuccessfulMonitorResult(base.Definition);
			Task<MonitorResult> task = lastSuccessfulMonitorResult.ExecuteAsync(cancellationToken, base.TraceContext);
			task.Continue(delegate(MonitorResult lastMonitorResult)
			{
				DateTime startTime = SqlDateTime.MinValue.Value;
				if (lastMonitorResult != null)
				{
					startTime = lastMonitorResult.ExecutionStartTime;
				}
				this.AddScopeNotification(startTime, cancellationToken);
			}, cancellationToken, TaskContinuationOptions.AttachedToParent);
		}

		protected abstract void AddScopeNotification(DateTime startTime, CancellationToken cancellationToken);

		protected ResultType TranslateHealthState(object healthStateValue)
		{
			if (healthStateValue is ServiceHealthStatus)
			{
				switch ((ServiceHealthStatus)healthStateValue)
				{
				case ServiceHealthStatus.None:
				case ServiceHealthStatus.Healthy:
					return ResultType.Succeeded;
				default:
					return ResultType.Failed;
				}
			}
			else
			{
				if (healthStateValue is MonitorAlertState)
				{
					MonitorAlertState monitorAlertState = (MonitorAlertState)healthStateValue;
					switch (monitorAlertState)
					{
					case MonitorAlertState.Unknown:
					case MonitorAlertState.Healthy:
						break;
					default:
						if (monitorAlertState != MonitorAlertState.Disabled)
						{
							return ResultType.Failed;
						}
						break;
					}
					return ResultType.Succeeded;
				}
				throw new NotSupportedException(string.Format("healthStateValue '{0}' of type '{1}' is not supported.", healthStateValue, healthStateValue.GetType().FullName));
			}
		}

		protected const string SourceInstanceType = "SourceInstanceType";

		public enum InstanceType
		{
			LAM,
			SM,
			XAM
		}
	}
}
