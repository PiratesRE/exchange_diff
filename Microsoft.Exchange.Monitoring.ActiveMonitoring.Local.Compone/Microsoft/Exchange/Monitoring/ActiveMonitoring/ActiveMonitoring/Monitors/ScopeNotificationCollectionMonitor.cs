using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Monitors
{
	public class ScopeNotificationCollectionMonitor : MonitorWorkItem
	{
		public static MonitorDefinition CreateDefinition(string name)
		{
			return new MonitorDefinition
			{
				AssemblyPath = ScopeNotificationCollectionMonitor.AssemblyPath,
				TypeName = ScopeNotificationCollectionMonitor.TypeName,
				Name = name,
				Component = ExchangeComponent.Monitoring,
				ServiceName = ExchangeComponent.Monitoring.Name,
				ServicePriority = 0,
				TargetResource = Environment.MachineName,
				RecurrenceIntervalSeconds = 300,
				MonitoringIntervalSeconds = 900,
				TimeoutSeconds = 240,
				MaxRetryAttempts = 3,
				Enabled = false,
				SampleMask = "*"
			};
		}

		protected override void DoMonitorWork(CancellationToken cancellationToken)
		{
			ScopeMappingEndpoint endpoint = ScopeMappingEndpointManager.Instance.GetEndpoint();
			if (endpoint.ScopeMappings == null || endpoint.ScopeMappings.Count == 0)
			{
				WTFDiagnostics.TraceWarning<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "No scope mappings found for: {0}", (!string.IsNullOrWhiteSpace(Settings.InstanceName)) ? Settings.InstanceName : Environment.MachineName, null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Monitors\\ScopeNotificationCollectionMonitor.cs", 84);
				return;
			}
			foreach (KeyValuePair<string, ScopeNotificationUploadState> keyValuePair in ScopeNotificationCache.Instance.ScopeNotificationUploadStates)
			{
				ScopeNotificationUploadState value = keyValuePair.Value;
				if (DateTime.UtcNow.Subtract(value.LastSucceededUpload) > TimeSpan.FromSeconds((double)base.Definition.MonitoringIntervalSeconds) || value.LastSucceededUpload < value.LastFailedUpload || value.LastUploadedHealthState != value.Data.HealthState)
				{
					ScopeMapping scopeMapping = null;
					if (endpoint.ScopeMappings.TryGetValue(value.Data.ScopeName, out scopeMapping))
					{
						bool flag = false;
						value.Data.ScopeType = scopeMapping.ScopeType;
						foreach (SystemMonitoringMapping systemMonitoringMapping in scopeMapping.SystemMonitoringInstances)
						{
							if (!systemMonitoringMapping.Uploader.TryEnqueueItem(value.Data))
							{
								flag = true;
								WTFDiagnostics.TraceError<string, string, string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "Failed to upload '{0}' for WorkItemId {1} to scope: {2}", value.Data.NotificationName, keyValuePair.Key, value.Data.ScopeName, null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Monitors\\ScopeNotificationCollectionMonitor.cs", 124);
							}
						}
						if (!flag)
						{
							value.LastSucceededUpload = DateTime.UtcNow;
							value.LastUploadedHealthState = value.Data.HealthState;
						}
						else
						{
							value.LastFailedUpload = DateTime.UtcNow;
						}
					}
				}
			}
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(ScopeNotificationCollectionMonitor).FullName;
	}
}
