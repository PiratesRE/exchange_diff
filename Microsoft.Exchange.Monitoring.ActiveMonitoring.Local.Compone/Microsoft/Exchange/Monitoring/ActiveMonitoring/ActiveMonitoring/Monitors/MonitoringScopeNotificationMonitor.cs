using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Monitors
{
	public sealed class MonitoringScopeNotificationMonitor : ScopeNotificationMonitor
	{
		public static MonitorDefinition CreateDefinition(string name, ScopeNotificationMonitor.InstanceType sourceInstanceType)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentException("Cannot be null or empty string", "name");
			}
			MonitorDefinition monitorDefinition = new MonitorDefinition();
			monitorDefinition.AssemblyPath = MonitoringScopeNotificationMonitor.AssemblyPath;
			monitorDefinition.TypeName = MonitoringScopeNotificationMonitor.TypeName;
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
			string sourceInstanceType = this.ReadAttribute("SourceInstanceType", null);
			if (string.IsNullOrWhiteSpace(sourceInstanceType))
			{
				throw new InvalidOperationException("SourceInstanceType cannot be null or empty string.");
			}
			Dictionary<int, MonitorResult> monitorResults = new Dictionary<int, MonitorResult>();
			int totalCount = 0;
			IDataAccessQuery<MonitorResult> successfulMonitorResults = base.Broker.GetSuccessfulMonitorResults(startTime, base.Result.ExecutionStartTime);
			Task<int> task = base.Broker.AsDataAccessQuery<MonitorResult>(successfulMonitorResults).ExecuteAsync(delegate(MonitorResult monitorResult)
			{
				MonitorResult monitorResult2 = null;
				if (!monitorResults.TryGetValue(monitorResult.WorkItemId, out monitorResult2) || monitorResult.ExecutionStartTime > monitorResult2.ExecutionStartTime)
				{
					monitorResults[monitorResult.WorkItemId] = monitorResult;
				}
				totalCount++;
			}, cancellationToken, base.TraceContext);
			task.ContinueWith(delegate(Task<int> t)
			{
				WTFDiagnostics.TraceDebug<int, int>(ExTraceGlobals.CommonComponentsTracer, this.TraceContext, "Total Results found: {0}, Unique results found: {1}", totalCount, monitorResults.Count, null, "AddScopeNotification", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Monitors\\MonitoringScopeNotificationMonitor.cs", 128);
				foreach (MonitorResult monitorResult in monitorResults.Values)
				{
					if (!string.IsNullOrWhiteSpace(monitorResult.TargetScopes))
					{
						ProbeResult probeResult = null;
						if (monitorResult.LastFailedProbeResultId > 0)
						{
							probeResult = this.Broker.GetProbeResult(monitorResult.LastFailedProbeId, monitorResult.LastFailedProbeResultId).ExecuteAsync(cancellationToken, this.TraceContext).Result;
						}
						Component component = monitorResult.Component;
						if (component == null)
						{
							component = new Component(monitorResult.ComponentName);
						}
						bool isMultiSourceInstance = !this.Broker.IsLocal() || monitorResult.ResultName.StartsWith("HealthManagerObserverMonitor", StringComparison.InvariantCultureIgnoreCase);
						string data = string.Format("<Data><StateAttribute6>{0}</StateAttribute6><StateAttribute7>{1}</StateAttribute7><StateAttribute8>{2}</StateAttribute8><StateAttribute9>{3}</StateAttribute9><StateAttribute10>{4}</StateAttribute10></Data>", new object[]
						{
							monitorResult.StateAttribute6,
							monitorResult.StateAttribute7,
							monitorResult.StateAttribute8,
							monitorResult.StateAttribute9,
							monitorResult.StateAttribute10
						});
						string[] array = monitorResult.TargetScopes.Split(new char[]
						{
							';'
						});
						string[] array2 = array;
						int i = 0;
						while (i < array2.Length)
						{
							string text = array2[i];
							string text2 = text;
							if (!this.Broker.IsLocal())
							{
								goto IL_245;
							}
							Match m = Regex.Match(text2, "^{(?<ScopeType>\\w+)}$", RegexOptions.Compiled);
							if (!m.Success || ScopeMappingEndpointManager.Instance.GetEndpoint().DefaultScopes == null || ScopeMappingEndpointManager.Instance.GetEndpoint().DefaultScopes.Count <= 0)
							{
								goto IL_245;
							}
							IEnumerable<KeyValuePair<string, ScopeMapping>> source = from s in ScopeMappingEndpointManager.Instance.GetEndpoint().DefaultScopes
							where s.Value.ScopeType.Equals(m.Groups["ScopeType"].Value)
							select s;
							if (source.Count<KeyValuePair<string, ScopeMapping>>() > 0)
							{
								text2 = source.First<KeyValuePair<string, ScopeMapping>>().Value.ScopeName;
								goto IL_245;
							}
							IL_361:
							i++;
							continue;
							IL_245:
							ScopeNotificationCache.Instance.AddScopeNotificationRawData(new ScopeNotificationRawData
							{
								NotificationName = monitorResult.ResultName,
								ScopeName = text2,
								HealthSetName = component.Name,
								HealthState = (int)this.TranslateHealthState(monitorResult.HealthState),
								MachineName = Environment.MachineName,
								SourceInstanceName = ((!string.IsNullOrWhiteSpace(Settings.InstanceName)) ? Settings.InstanceName : Environment.MachineName),
								SourceInstanceType = sourceInstanceType,
								IsMultiSourceInstance = isMultiSourceInstance,
								ExecutionStartTime = (monitorResult.FirstAlertObservedTime ?? monitorResult.ExecutionStartTime),
								ExecutionEndTime = monitorResult.ExecutionEndTime,
								Error = ((probeResult != null) ? probeResult.Error : null),
								Exception = ((probeResult != null) ? probeResult.Exception : null),
								ExecutionContext = ((probeResult != null) ? probeResult.ExecutionContext : null),
								FailureContext = ((probeResult != null) ? probeResult.FailureContext : null),
								Data = data
							});
							goto IL_361;
						}
					}
				}
			}, cancellationToken, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled, TaskScheduler.Current);
		}

		internal const string HealthManagerObserverMonitorName = "HealthManagerObserverMonitor";

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(MonitoringScopeNotificationMonitor).FullName;
	}
}
