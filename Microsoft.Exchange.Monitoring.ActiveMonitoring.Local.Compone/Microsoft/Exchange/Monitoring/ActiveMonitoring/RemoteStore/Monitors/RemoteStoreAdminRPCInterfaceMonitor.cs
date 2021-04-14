using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.RemoteStore.Monitors
{
	public class RemoteStoreAdminRPCInterfaceMonitor : MonitorWorkItem
	{
		internal static MonitorDefinition CreateDefinition(string name, string sampleMask, string serviceName, Component component, string targetResource, int monitoringThreshold, TimeSpan recurrenceInterval, TimeSpan monitoringInterval, bool enabled = true)
		{
			MonitorDefinition monitorDefinition = new MonitorDefinition();
			monitorDefinition.Name = name;
			monitorDefinition.SampleMask = sampleMask;
			monitorDefinition.ServiceName = serviceName;
			monitorDefinition.Component = component;
			monitorDefinition.TargetResource = targetResource;
			monitorDefinition.MaxRetryAttempts = 3;
			monitorDefinition.Enabled = enabled;
			monitorDefinition.MonitoringThreshold = (double)monitoringThreshold;
			monitorDefinition.MonitoringIntervalSeconds = (int)monitoringInterval.TotalSeconds;
			monitorDefinition.RecurrenceIntervalSeconds = (int)recurrenceInterval.TotalSeconds;
			monitorDefinition.TimeoutSeconds = (int)RemoteStoreAdminRPCInterfaceMonitor.DefaultTimeout.TotalSeconds;
			monitorDefinition.InsufficientSamplesIntervalSeconds = Math.Max(5 * monitorDefinition.RecurrenceIntervalSeconds, Convert.ToInt32(ConfigurationManager.AppSettings["InsufficientSamplesIntervalInSeconds"]));
			monitorDefinition.AssemblyPath = RemoteStoreAdminRPCInterfaceMonitor.AssemblyPath;
			monitorDefinition.TypeName = RemoteStoreAdminRPCInterfaceMonitor.TypeName;
			return monitorDefinition;
		}

		protected override void DoMonitorWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.RemoteStoreTracer, base.TraceContext, "RemoteStoreAdminRPCInterfaceMonitor.DoMonitorWork: Starting StoreRemoteAdminRPCInterfaceMonitor", null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RemoteStore\\RemoteStoreAdminRPCInterfaceMonitor.cs", 90);
			List<ProbeResult> allProbeResults = WorkItemResultHelper.GetAllProbeResults(base.Broker, base.Result.ExecutionStartTime, base.Definition.SampleMask, base.MonitoringWindowStartTime, cancellationToken);
			if ((double)allProbeResults.Count >= base.Definition.MonitoringThreshold)
			{
				Dictionary<string, Dictionary<string, int>> dictionary = this.PopulateExceptionTypeCountGroupedByServers(allProbeResults);
				if (dictionary.Count > 0)
				{
					Dictionary<string, string> serversWithExceptionTypeUsingThresholds = this.GetServersWithExceptionTypeUsingThresholds(dictionary, base.Definition.MonitoringThreshold, base.Definition.SecondaryMonitoringThreshold);
					if (serversWithExceptionTypeUsingThresholds.Count > 0)
					{
						WTFDiagnostics.TraceInformation(ExTraceGlobals.RemoteStoreTracer, base.TraceContext, "RemoteStoreAdminRPCInterfaceMonitor.DoMonitorWork: Setting monitor to alert state", null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RemoteStore\\RemoteStoreAdminRPCInterfaceMonitor.cs", 125);
						base.Result.StateAttribute1 = string.Join<KeyValuePair<string, string>>(string.Empty, serversWithExceptionTypeUsingThresholds);
						base.Result.StateAttribute2 = "SettingMonitorIntoAlertState";
						base.Result.IsAlert = true;
						return;
					}
				}
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.RemoteStoreTracer, base.TraceContext, "RemoteStoreAdminRPCInterfaceMonitor.DoMonitorWork: Not enough evidence to set the monitor to alert state", null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\RemoteStore\\RemoteStoreAdminRPCInterfaceMonitor.cs", 139);
			base.Result.StateAttribute2 = "NotEnoughDataToCauseAlert";
			base.Result.IsAlert = false;
		}

		internal Dictionary<string, Dictionary<string, int>> PopulateExceptionTypeCountGroupedByServers(List<ProbeResult> probeResults)
		{
			Dictionary<string, Dictionary<string, int>> dictionary = new Dictionary<string, Dictionary<string, int>>(StringComparer.InvariantCultureIgnoreCase);
			if (probeResults != null)
			{
				for (int i = 0; i < probeResults.Count; i++)
				{
					string stateAttribute = probeResults[i].StateAttribute1;
					string stateAttribute2 = probeResults[i].StateAttribute2;
					if (!string.IsNullOrWhiteSpace(stateAttribute) && !string.IsNullOrWhiteSpace(stateAttribute2))
					{
						if (dictionary.ContainsKey(stateAttribute))
						{
							if (dictionary[stateAttribute].ContainsKey(stateAttribute2))
							{
								Dictionary<string, int> dictionary2;
								string key;
								(dictionary2 = dictionary[stateAttribute])[key = stateAttribute2] = dictionary2[key] + 1;
							}
							else
							{
								dictionary[stateAttribute].Add(stateAttribute2, 1);
							}
						}
						else
						{
							dictionary.Add(stateAttribute, new Dictionary<string, int>(StringComparer.InvariantCultureIgnoreCase));
							dictionary[stateAttribute].Add(stateAttribute2, 1);
						}
					}
				}
			}
			return dictionary;
		}

		internal Dictionary<string, string> GetServersWithExceptionTypeUsingThresholds(Dictionary<string, Dictionary<string, int>> exceptionTypeCountGroupedByServers, double totalFailuresThreshold, double individualExceptionTypeThreshold)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (exceptionTypeCountGroupedByServers != null)
			{
				foreach (KeyValuePair<string, Dictionary<string, int>> keyValuePair in exceptionTypeCountGroupedByServers)
				{
					int num = keyValuePair.Value.Sum((KeyValuePair<string, int> x) => x.Value);
					if ((double)num >= totalFailuresThreshold)
					{
						foreach (KeyValuePair<string, int> keyValuePair2 in keyValuePair.Value)
						{
							if ((double)keyValuePair2.Value >= individualExceptionTypeThreshold)
							{
								if (dictionary.ContainsKey(keyValuePair.Key))
								{
									dictionary[keyValuePair.Key] = string.Format("{0},{1}", dictionary[keyValuePair.Key], keyValuePair2.Key);
								}
								else
								{
									dictionary.Add(keyValuePair.Key, keyValuePair2.Key);
								}
							}
						}
					}
				}
			}
			return dictionary;
		}

		private static TimeSpan DefaultTimeout = TimeSpan.FromSeconds(60.0);

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(RemoteStoreAdminRPCInterfaceMonitor).FullName;
	}
}
