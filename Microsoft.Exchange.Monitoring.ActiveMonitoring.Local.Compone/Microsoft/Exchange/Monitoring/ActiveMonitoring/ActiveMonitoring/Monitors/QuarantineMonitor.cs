using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Monitors
{
	public sealed class QuarantineMonitor : MonitorWorkItem
	{
		internal static MonitorDefinition CreateDefinition()
		{
			return new MonitorDefinition
			{
				AssemblyPath = QuarantineMonitor.AssemblyPath,
				TypeName = QuarantineMonitor.TypeName,
				Name = "HealthManagerWorkItemQuarantineMonitor",
				SampleMask = NotificationItem.GenerateResultName(ExchangeComponent.Monitoring.Name, MonitoringNotificationEvent.WorkitemQuarantine.ToString(), null),
				ServiceName = ExchangeComponent.Monitoring.Name,
				Component = ExchangeComponent.Monitoring,
				MaxRetryAttempts = 0,
				Enabled = true,
				TimeoutSeconds = 30,
				MonitoringIntervalSeconds = 3600,
				RecurrenceIntervalSeconds = 0
			};
		}

		protected override void DoMonitorWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceDebug(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "QuarantineMonitor.DoWork: Getting escalated quarantine from last monitor result", null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\QuarantineMonitor.cs", 87);
			Dictionary<int, int> quarantinedMaintenanceWorkItem = new Dictionary<int, int>();
			Dictionary<int, int> quarantinedProbes = new Dictionary<int, int>();
			Dictionary<int, int> quarantinedMonitors = new Dictionary<int, int>();
			Dictionary<int, int> quarantinedResponders = new Dictionary<int, int>();
			if (base.LastSuccessfulResult != null)
			{
				int offsetSeconds = (int)(DateTime.UtcNow - base.LastSuccessfulResult.ExecutionEndTime).TotalSeconds;
				this.Deserialize(base.LastSuccessfulResult.StateAttribute1, quarantinedMaintenanceWorkItem, offsetSeconds);
				this.Deserialize(base.LastSuccessfulResult.StateAttribute2, quarantinedProbes, offsetSeconds);
				this.Deserialize(base.LastSuccessfulResult.StateAttribute3, quarantinedMonitors, offsetSeconds);
				this.Deserialize(base.LastSuccessfulResult.StateAttribute4, quarantinedResponders, offsetSeconds);
			}
			WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.CommonComponentsTracer, base.TraceContext, "QuarantineMonitor.DoWork: Getting quarantine notifications in last monitoring interval using mask '{0}'...", base.Definition.SampleMask, null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\QuarantineMonitor.cs", 110);
			DateTime executionStartTime = base.Result.ExecutionStartTime;
			IDataAccessQuery<ProbeResult> probeResults = base.Broker.GetProbeResults(base.Definition.SampleMask, base.MonitoringWindowStartTime, executionStartTime);
			bool found = false;
			Task<int> task = probeResults.ExecuteAsync(delegate(ProbeResult r)
			{
				WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.CommonComponentsTracer, this.TraceContext, "QuarantineMonitor.DoWork: processing notification with ExtensionXml {0}", r.ExtensionXml, null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\QuarantineMonitor.cs", 122);
				Dictionary<string, string> dictionary = CrimsonHelper.ConvertXmlToDictionary(r.ExtensionXml);
				string text;
				string text2;
				string text3;
				string text4;
				if (dictionary.TryGetValue("Component", out text) && dictionary.TryGetValue("Type", out text2) && dictionary.TryGetValue("Name", out text3) && dictionary.TryGetValue("Id", out text4))
				{
					WTFDiagnostics.TraceDebug<string, string, string, string>(ExTraceGlobals.CommonComponentsTracer, this.TraceContext, "QuarantineMonitor.DoWork: processing: {0} {1} {2} {3}", text, text2, text3, text4, null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\QuarantineMonitor.cs", 134);
					Dictionary<int, int> dictionary2 = null;
					if (text2 == typeof(MaintenanceDefinition).Name)
					{
						dictionary2 = quarantinedMaintenanceWorkItem;
					}
					else if (text2 == typeof(ProbeDefinition).Name)
					{
						dictionary2 = quarantinedProbes;
					}
					else if (text2 == typeof(MonitorDefinition).Name)
					{
						dictionary2 = quarantinedMonitors;
					}
					else if (text2 == typeof(ResponderDefinition).Name)
					{
						dictionary2 = quarantinedResponders;
					}
					int num;
					if (dictionary2 != null && int.TryParse(text4, out num))
					{
						WTFDiagnostics.TraceDebug<int, ResultType>(ExTraceGlobals.CommonComponentsTracer, this.TraceContext, "QuarantineMonitor.DoWork: processing notification for Id {0} with tpe {1}", num, r.ResultType, null, "DoMonitorWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\QuarantineMonitor.cs", 161);
						if (r.ResultType == ResultType.Failed && !found)
						{
							Component component = ExchangeComponent.Monitoring;
							if (ExchangeComponent.WellKnownComponents.ContainsKey(text))
							{
								component = ExchangeComponent.WellKnownComponents[text];
							}
							this.Result.Component = component;
							this.Result.StateAttribute5 = text3;
							this.Result.StateAttribute6 = (double)num;
							if (!dictionary2.ContainsKey(num))
							{
								dictionary2.Add(num, 0);
								found = true;
							}
						}
						if (r.ResultType == ResultType.Succeeded && dictionary2.ContainsKey(num))
						{
							dictionary2.Remove(num);
						}
					}
				}
			}, cancellationToken, base.TraceContext);
			task.Continue(delegate(int count)
			{
				if (quarantinedMaintenanceWorkItem.Count > 0 || quarantinedProbes.Count > 0 || quarantinedMonitors.Count > 0 || quarantinedResponders.Count > 0)
				{
					this.Result.StateAttribute1 = this.Serialize(quarantinedMaintenanceWorkItem);
					this.Result.StateAttribute2 = this.Serialize(quarantinedProbes);
					this.Result.StateAttribute3 = this.Serialize(quarantinedMonitors);
					this.Result.StateAttribute4 = this.Serialize(quarantinedResponders);
					if (string.IsNullOrWhiteSpace(this.Result.StateAttribute5))
					{
						this.Result.Component = this.LastSuccessfulResult.Component;
						this.Result.StateAttribute5 = this.LastSuccessfulResult.StateAttribute5;
						this.Result.StateAttribute6 = this.LastSuccessfulResult.StateAttribute6;
					}
					this.Result.IsAlert = true;
					return;
				}
				this.Result.IsAlert = false;
			}, cancellationToken, TaskContinuationOptions.AttachedToParent);
		}

		private void Deserialize(string data, Dictionary<int, int> escalatedQuarantines, int offsetSeconds)
		{
			if (!string.IsNullOrWhiteSpace(data))
			{
				string[] array = data.Split(new char[]
				{
					'|'
				});
				foreach (string text in array)
				{
					string[] array3 = text.Split(new char[]
					{
						','
					});
					int key;
					int num;
					if (array3.Length == 2 && int.TryParse(array3[0], out key) && int.TryParse(array3[1], out num))
					{
						num += offsetSeconds;
						if (num < this.AgeThresholdSeconds)
						{
							escalatedQuarantines.Add(key, num);
						}
					}
				}
			}
		}

		private string Serialize(Dictionary<int, int> escalatedQuarantines)
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<int, int> keyValuePair in escalatedQuarantines)
			{
				list.Add(string.Format("{0},{1}", keyValuePair.Key, keyValuePair.Value));
			}
			return string.Join("|", list);
		}

		private const string QuarantineMonitorName = "HealthManagerWorkItemQuarantineMonitor";

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(QuarantineMonitor).FullName;

		private readonly int AgeThresholdSeconds = 2 * Settings.QuarantineHours * 3600;
	}
}
