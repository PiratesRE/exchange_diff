using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Monitors
{
	public abstract class MaintenanceMonitor : MonitorWorkItem
	{
		protected override void DoMonitorWork(CancellationToken cancellationToken)
		{
			if (DateTime.UtcNow < LocalDataAccess.StartTime.AddMinutes(base.Definition.SecondaryMonitoringThreshold))
			{
				WTFDiagnostics.TraceDebug<double>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "MaintenanceMonitor.DoWork: Skip checking maintenance failure because worker process has not been running for {0} minutes", base.Definition.SecondaryMonitoringThreshold, null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\MaintenanceMonitor.cs", 45);
				base.Result.StateAttribute3 = "Skipped";
				return;
			}
			WTFDiagnostics.TraceDebug(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "MaintenanceMonitor.DoWork: Getting maintenance failure items from last monitor result...", null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\MaintenanceMonitor.cs", 50);
			Dictionary<int, string> failedItems = new Dictionary<int, string>();
			Dictionary<int, string> escalatedItems = new Dictionary<int, string>();
			if (base.LastSuccessfulResult != null && base.LastSuccessfulResult.ExecutionStartTime > LocalDataAccess.StartTime)
			{
				this.Deserialize(base.LastSuccessfulResult.StateAttribute1, failedItems);
				this.Deserialize(base.LastSuccessfulResult.StateAttribute3, escalatedItems);
			}
			WTFDiagnostics.TraceDebug(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "MaintenanceMonitor.DoWork: Getting maintenance failure notification...", null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\MaintenanceMonitor.cs", 62);
			DateTime dateTime = base.MonitoringWindowStartTime;
			if (dateTime < LocalDataAccess.StartTime)
			{
				dateTime = LocalDataAccess.StartTime;
			}
			IDataAccessQuery<ProbeResult> probeResults = base.Broker.GetProbeResults(base.Definition.SampleMask, dateTime, base.Result.ExecutionStartTime);
			Task<int> task = probeResults.ExecuteAsync(delegate(ProbeResult r)
			{
				WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.CommonComponentsTracer, this.TraceContext, "MaintenanceMonitor.DoWork: processing notification with ExtensionXml {0}", r.ExtensionXml, null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\MaintenanceMonitor.cs", 77);
				this.Result.LastFailedProbeId = r.WorkItemId;
				Dictionary<string, string> dictionary = CrimsonHelper.ConvertXmlToDictionary(r.ExtensionXml);
				string text;
				string text2;
				string text3;
				if (dictionary.TryGetValue("Component", out text) && dictionary.TryGetValue("Name", out text2) && dictionary.TryGetValue("Id", out text3))
				{
					WTFDiagnostics.TraceDebug<string, string, string>(ExTraceGlobals.CommonComponentsTracer, this.TraceContext, "MaintenanceMonitor.DoWork: processing: {0} {1} {2}", text, text2, text3, null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\MaintenanceMonitor.cs", 88);
					int key;
					if (int.TryParse(text3, out key))
					{
						if (r.ResultType == ResultType.Failed && !failedItems.ContainsKey(key))
						{
							if (!string.IsNullOrWhiteSpace(r.Error) && r.Error.IndexOf(MaintenanceMonitor.InvalidMailboxDatabaseEndpointExceptionTypeName) == 0)
							{
								WTFDiagnostics.TraceDebug<string, string, string>(ExTraceGlobals.CommonComponentsTracer, this.TraceContext, "MaintenanceMonitor.DoWork: detected InvalidMailboxDatabaseEndpointException in: {0} {1} {2}", text, text2, text3, null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\MaintenanceMonitor.cs", 100);
								failedItems.Add(key, string.Join("\\", new string[]
								{
									text,
									text2,
									r.ResultId.ToString(),
									MaintenanceMonitor.InvalidMailboxDatabaseEndpointExceptionTypeName
								}));
							}
							else
							{
								failedItems.Add(key, string.Join("\\", new string[]
								{
									text,
									text2,
									r.ResultId.ToString()
								}));
							}
						}
						if (r.ResultType == ResultType.Succeeded)
						{
							failedItems.Remove(key);
							escalatedItems.Remove(key);
						}
					}
				}
			}, cancellationToken, base.TraceContext);
			task.Continue(delegate(int count)
			{
				if (failedItems.Count > 0)
				{
					foreach (KeyValuePair<int, string> keyValuePair in failedItems)
					{
						if (!escalatedItems.ContainsKey(keyValuePair.Key))
						{
							escalatedItems[keyValuePair.Key] = keyValuePair.Value;
							this.Result.StateAttribute6 = (double)keyValuePair.Key;
							string[] array = keyValuePair.Value.Split(new char[]
							{
								'\\'
							});
							Component component = ExchangeComponent.Monitoring;
							if (ExchangeComponent.WellKnownComponents.ContainsKey(array[0]))
							{
								component = ExchangeComponent.WellKnownComponents[array[0]];
							}
							if (array.Length >= 4 && !string.IsNullOrWhiteSpace(array[3]) && array[3].IndexOf(MaintenanceMonitor.InvalidMailboxDatabaseEndpointExceptionTypeName) == 0)
							{
								WTFDiagnostics.TraceDebug<string, string>(ExTraceGlobals.CommonComponentsTracer, this.TraceContext, "MaintenanceMonitor.DoWork: redirecting escalation from '{0}' to '{1}' due to InvalidMailboxDatabaseEndpointException", component.Name, ExchangeComponent.Monitoring.Name, null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\MaintenanceMonitor.cs", 155);
								component = ExchangeComponent.Monitoring;
							}
							this.Result.Component = component;
							if (array.Length >= 2)
							{
								this.Result.StateAttribute2 = array[1];
							}
							int lastFailedProbeResultId;
							if (array.Length >= 3 && int.TryParse(array[2], out lastFailedProbeResultId))
							{
								this.Result.LastFailedProbeResultId = lastFailedProbeResultId;
							}
							if (this.Result.LastFailedProbeId == -1 && this.LastSuccessfulResult != null)
							{
								this.Result.LastFailedProbeId = this.LastSuccessfulResult.LastFailedProbeId;
								break;
							}
							break;
						}
					}
					if (this.Result.StateAttribute6 == 0.0 && this.LastSuccessfulResult != null && this.LastSuccessfulResult.ExecutionStartTime > LocalDataAccess.StartTime)
					{
						this.Result.Component = this.LastSuccessfulResult.Component;
						this.Result.StateAttribute2 = this.LastSuccessfulResult.StateAttribute2;
						this.Result.StateAttribute6 = this.LastSuccessfulResult.StateAttribute6;
						this.Result.LastFailedProbeResultId = this.LastSuccessfulResult.LastFailedProbeResultId;
						this.Result.LastFailedProbeId = this.LastSuccessfulResult.LastFailedProbeId;
					}
					this.Result.IsAlert = true;
				}
				this.Result.StateAttribute1 = this.Serialize(failedItems);
				this.Result.StateAttribute3 = this.Serialize(escalatedItems);
			}, cancellationToken, TaskContinuationOptions.AttachedToParent);
		}

		private string Serialize(Dictionary<int, string> items)
		{
			return string.Join(",", from p in items
			select p.Key.ToString() + "|" + p.Value);
		}

		private void Deserialize(string data, Dictionary<int, string> items)
		{
			if (!string.IsNullOrWhiteSpace(data))
			{
				string[] array = data.Split(new char[]
				{
					','
				});
				foreach (string text in array)
				{
					string[] array3 = text.Split(new char[]
					{
						'|'
					});
					int key;
					if (array3.Length == 2 && int.TryParse(array3[0], out key))
					{
						items.Add(key, array3[1]);
					}
				}
			}
		}

		private static readonly string InvalidMailboxDatabaseEndpointExceptionTypeName = typeof(InvalidMailboxDatabaseEndpointException).FullName;
	}
}
