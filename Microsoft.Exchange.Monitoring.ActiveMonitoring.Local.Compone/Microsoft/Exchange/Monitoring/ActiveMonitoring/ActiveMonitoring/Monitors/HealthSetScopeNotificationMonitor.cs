using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Data.Storage.ActiveMonitoring;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring.Management.Common;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Monitors
{
	public class HealthSetScopeNotificationMonitor : ScopeNotificationMonitor
	{
		public static MonitorDefinition CreateDefinition(string name, ScopeNotificationMonitor.InstanceType sourceInstanceType)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("Cannot be null or empty string", "name");
			}
			MonitorDefinition monitorDefinition = new MonitorDefinition();
			monitorDefinition.AssemblyPath = HealthSetScopeNotificationMonitor.AssemblyPath;
			monitorDefinition.TypeName = HealthSetScopeNotificationMonitor.TypeName;
			monitorDefinition.Name = name;
			monitorDefinition.Component = ExchangeComponent.Monitoring;
			monitorDefinition.ServiceName = ExchangeComponent.Monitoring.Name;
			monitorDefinition.ServicePriority = 0;
			monitorDefinition.SampleMask = "*";
			monitorDefinition.RecurrenceIntervalSeconds = 300;
			monitorDefinition.MonitoringIntervalSeconds = 300;
			monitorDefinition.TimeoutSeconds = 240;
			monitorDefinition.MaxRetryAttempts = 3;
			monitorDefinition.Attributes["SourceInstanceType"] = sourceInstanceType.ToString();
			monitorDefinition.Enabled = true;
			return monitorDefinition;
		}

		protected override void AddScopeNotification(DateTime startTime, CancellationToken cancellationToken)
		{
			if (ScopeMappingEndpointManager.Instance.GetEndpoint().DefaultScopes == null || ScopeMappingEndpointManager.Instance.GetEndpoint().DefaultScopes.Count == 0)
			{
				WTFDiagnostics.TraceWarning<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "No default scopes found for machine: {0}", Environment.MachineName, null, "AddScopeNotification", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Monitors\\HealthSetScopeNotificationMonitor.cs", 93);
				return;
			}
			string sourceInstanceType = this.ReadAttribute("SourceInstanceType", null);
			if (string.IsNullOrWhiteSpace(sourceInstanceType))
			{
				throw new InvalidOperationException("SourceInstanceType cannot be null or empty string.");
			}
			List<RpcGetMonitorHealthStatus.RpcMonitorHealthEntry> list = RpcGetMonitorHealthStatus.Invoke(Environment.MachineName, 30000);
			if (list != null && list.Count > 0)
			{
				WTFDiagnostics.TraceDebug<int>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Total HealthSets found: {0}", list.Count, null, "AddScopeNotification", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Monitors\\HealthSetScopeNotificationMonitor.cs", 113);
				HealthReportHelper healthReportHelper = new HealthReportHelper();
				foreach (RpcGetMonitorHealthStatus.RpcMonitorHealthEntry rpcMonitorHealthEntry in list)
				{
					if (!string.Equals(rpcMonitorHealthEntry.Name, "HealthManagerHeartbeatMonitor", StringComparison.OrdinalIgnoreCase))
					{
						MonitorHealthEntry healthEntry = new MonitorHealthEntry(Environment.MachineName, rpcMonitorHealthEntry);
						healthReportHelper.ProcessEntry(healthEntry);
					}
				}
				bool isMultiSourceInstance = !base.Broker.IsLocal();
				healthReportHelper.ProcessHealth(delegate(ConsolidatedHealth consolidatedHealth)
				{
					string text = null;
					if (consolidatedHealth.Entries != null)
					{
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.AppendFormat("<b>States of all monitors within the health set:</b><br>", new object[0]);
						stringBuilder.AppendFormat("Note: Data may be stale. To get current data, run: Get-ServerHealth -Identity '{0}' -HealthSet '{1}'<br><br>", Environment.MachineName, consolidatedHealth.HealthSet);
						string[] headers = new string[]
						{
							"Name",
							"TargetResource",
							"State",
							"LastTransitionTime"
						};
						List<string[]> list2 = new List<string[]>(consolidatedHealth.Entries.Count);
						foreach (MonitorHealthEntry monitorHealthEntry in consolidatedHealth.Entries)
						{
							list2.Add(new string[]
							{
								monitorHealthEntry.Name,
								monitorHealthEntry.TargetResource,
								monitorHealthEntry.AlertValue.ToString(),
								monitorHealthEntry.LastTransitionTime.ToString()
							});
						}
						stringBuilder.AppendFormat("{0}", TableDecorator.CreateTable(headers, list2));
						text = string.Format("<EscalationSubject><![CDATA[{0} Health Set state on '{1}']]></EscalationSubject><EscalationMessage><![CDATA[{2}]]></EscalationMessage>", consolidatedHealth.HealthSet, Environment.MachineName, stringBuilder.ToString());
					}
					text = string.Format("<Data>{0}</Data>", text);
					foreach (KeyValuePair<string, ScopeMapping> keyValuePair in ScopeMappingEndpointManager.Instance.GetEndpoint().DefaultScopes)
					{
						string key = keyValuePair.Key;
						ScopeNotificationCache.Instance.AddScopeNotificationRawData(new ScopeNotificationRawData
						{
							NotificationName = string.Format("HealthSet/{0}", consolidatedHealth.HealthSet),
							ScopeName = key,
							HealthSetName = consolidatedHealth.HealthSet,
							HealthState = (int)this.TranslateHealthState(consolidatedHealth.AlertValue),
							MachineName = Environment.MachineName,
							SourceInstanceName = ((!string.IsNullOrWhiteSpace(Settings.InstanceName)) ? Settings.InstanceName : Environment.MachineName),
							SourceInstanceType = sourceInstanceType,
							IsMultiSourceInstance = isMultiSourceInstance,
							ExecutionStartTime = consolidatedHealth.LastTransitionTime,
							ExecutionEndTime = this.Result.ExecutionStartTime,
							Error = null,
							Exception = null,
							ExecutionContext = null,
							FailureContext = null,
							Data = text
						});
					}
				});
				return;
			}
			WTFDiagnostics.TraceDebug(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "No HealthSets found.", null, "AddScopeNotification", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Monitors\\HealthSetScopeNotificationMonitor.cs", 121);
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(HealthSetScopeNotificationMonitor).FullName;
	}
}
