using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Monitoring.ServiceContextProvider;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders
{
	public class OnlineResponder : ResponderWorkItem
	{
		public static ResponderDefinition CreateDefinition()
		{
			return new ResponderDefinition
			{
				AssemblyPath = OnlineResponder.AssemblyPath,
				TypeName = OnlineResponder.TypeName,
				Name = "OnlineResponder",
				ServiceName = ExchangeComponent.DataProtection.Name,
				AlertTypeId = "*",
				AlertMask = "*",
				RecurrenceIntervalSeconds = 300,
				WaitIntervalSeconds = 30,
				TimeoutSeconds = 300,
				MaxRetryAttempts = 3,
				Enabled = true,
				StartTime = DateTime.UtcNow
			};
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			ServerComponentStateManager.SyncAdState();
			if (DatacenterRegistry.IsForefrontForOffice())
			{
				ServiceContextProvider.Instance.NotifyRecoveryCompletion(ServerComponentEnum.ServerWideOffline.ToString(), true, "");
			}
			IDataAccessQuery<ResponderResult> lastSuccessfulResponderResult = base.Broker.GetLastSuccessfulResponderResult(base.Definition);
			Task<ResponderResult> task = lastSuccessfulResponderResult.ExecuteAsync(cancellationToken, base.TraceContext);
			task.Continue(delegate(ResponderResult lastResponderResult)
			{
				DateTime startTime = DateTime.MinValue;
				if (lastResponderResult != null)
				{
					startTime = lastResponderResult.ExecutionStartTime;
				}
				IDataAccessQuery<MonitorResult> successfulMonitorResults = this.Broker.GetSuccessfulMonitorResults(startTime, this.Result.ExecutionStartTime);
				Dictionary<ServerComponentEnum, OnlineResponder.RedGreenRecord> results = new Dictionary<ServerComponentEnum, OnlineResponder.RedGreenRecord>();
				this.Broker.AsDataAccessQuery<MonitorResult>(successfulMonitorResults).ExecuteAsync(delegate(MonitorResult monitorResult)
				{
					ServerComponentEnum key2;
					if (this.MonitorAffectsComponent(monitorResult, out key2))
					{
						bool isAlert = monitorResult.IsAlert;
						Dictionary<ServerComponentEnum, OnlineResponder.RedGreenRecord> results;
						lock (results)
						{
							OnlineResponder.RedGreenRecord redGreenRecord;
							if (!results.TryGetValue(key2, out redGreenRecord))
							{
								redGreenRecord = new OnlineResponder.RedGreenRecord();
								results.Add(key2, redGreenRecord);
							}
							if (isAlert)
							{
								redGreenRecord.RedCount++;
							}
							else
							{
								redGreenRecord.GreenCount++;
							}
						}
					}
				}, cancellationToken, this.TraceContext);
				foreach (KeyValuePair<ServerComponentEnum, OnlineResponder.RedGreenRecord> keyValuePair in results)
				{
					ServerComponentEnum key = keyValuePair.Key;
					if (keyValuePair.Value.RedCount == 0 && keyValuePair.Value.GreenCount > 0)
					{
						string text = key.ToString();
						ServerComponentStateManager.SetOnline(key);
						if (DatacenterRegistry.IsForefrontForOffice())
						{
							ServiceContextProvider.Instance.NotifyRecoveryCompletion(text, true, "");
						}
						ManagedAvailabilityCrimsonEvents.ComponentSetOnline.LogPeriodic<string, string, string, string, string, string>(text, OnlineResponder.defaultEventSuppression, RecoveryActionId.TakeComponentOnline.ToString(), text, this.Definition.Name, string.Empty, string.Empty, string.Empty);
					}
				}
			}, cancellationToken, TaskContinuationOptions.AttachedToParent);
		}

		private bool MonitorAffectsComponent(MonitorResult result, out ServerComponentEnum componentId)
		{
			componentId = ServerComponentEnum.None;
			if (result != null && result.IsHaImpacting)
			{
				componentId = result.Component.ServerComponent;
			}
			return ServerComponentEnum.None != componentId;
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(OnlineResponder).FullName;

		private static readonly TimeSpan defaultEventSuppression = TimeSpan.FromMinutes(5.0);

		private class RedGreenRecord
		{
			public int RedCount { get; set; }

			public int GreenCount { get; set; }
		}
	}
}
