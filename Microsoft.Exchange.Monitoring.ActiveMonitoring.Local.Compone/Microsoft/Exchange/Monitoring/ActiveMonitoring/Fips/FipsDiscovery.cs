using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Fips.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Fips
{
	public sealed class FipsDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			try
			{
				LocalEndpointManager instance = LocalEndpointManager.Instance;
				if ((instance.ExchangeServerRoleEndpoint != null || !FfoLocalEndpointManager.IsHubTransportRoleInstalled) && (instance.ExchangeServerRoleEndpoint == null || !instance.ExchangeServerRoleEndpoint.IsBridgeheadRoleInstalled))
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.FIPSTracer, base.TraceContext, "[FipsDiscovery.DoWork]: Bridgehead role is not installed on this server.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\FIPS\\FipsDiscovery.cs", 85);
					base.Result.StateAttribute1 = "FipsDiscovery: Bridgehead role is not installed on this server.";
				}
				else
				{
					GenericWorkItemHelper.CreateAllDefinitions(new List<string>
					{
						"Fips.xml"
					}, base.Broker, base.TraceContext, base.Result);
					this.CreateFipsWorkItems();
					if (Datacenter.IsForefrontForOfficeDatacenter() || Datacenter.IsRunningInExchangeDatacenter(false))
					{
						this.CreateCheckAVEngineEnabledWorkItem();
					}
				}
			}
			catch (EndpointManagerEndpointUninitializedException)
			{
				WTFDiagnostics.TraceDebug(ExTraceGlobals.AntimalwareTracer, base.TraceContext, "[FipsDiscovery.DoWork]: Exception occurred due to EndpointManagerEndpointUninitializedException, ignoring exception and treating as transient.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\FIPS\\FipsDiscovery.cs", 112);
			}
		}

		private void CreateCheckAVEngineEnabledWorkItem()
		{
			WTFDiagnostics.TraceDebug(ExTraceGlobals.FIPSTracer, base.TraceContext, "Creating Check AV Engine Enabled work items.", null, "CreateCheckAVEngineEnabledWorkItem", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\FIPS\\FipsDiscovery.cs", 124);
			base.Broker.AddWorkDefinition<ProbeDefinition>(this.CreateCheckAVEngineEnabledProbeDefinition(), base.TraceContext).Wait();
			MonitorDefinition monitorDefinition = this.CreateCheckAVEngineEnabledMonitorDefinition();
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext).Wait();
			base.Broker.AddWorkDefinition<ResponderDefinition>(this.CreateCheckAVEngineEnabledResponderDefinition(monitorDefinition), base.TraceContext).Wait();
		}

		private ProbeDefinition CreateCheckAVEngineEnabledProbeDefinition()
		{
			return new ProbeDefinition
			{
				AssemblyPath = Assembly.GetExecutingAssembly().Location,
				TypeName = typeof(CheckAVEngineEnabled).FullName,
				Name = "CheckAVEngineEnabled",
				TargetResource = "fms",
				ServiceName = "AMScanners",
				RecurrenceIntervalSeconds = 3600,
				TimeoutSeconds = 300,
				MaxRetryAttempts = 3,
				Enabled = true
			};
		}

		private MonitorDefinition CreateCheckAVEngineEnabledMonitorDefinition()
		{
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("CheckAVEngineEnabledMonitor", "CheckAVEngineEnabled", ExchangeComponent.AMScanners.Name, ExchangeComponent.AMScanners, 3, true, 300);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0)
			};
			monitorDefinition.ServicePriority = 2;
			monitorDefinition.ScenarioDescription = "Validate FIPS health is not impacted by AV issues";
			return monitorDefinition;
		}

		private ResponderDefinition CreateCheckAVEngineEnabledResponderDefinition(MonitorDefinition monitor)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<b>{{Probe.ResultName}}<hr/>");
			stringBuilder.Append("Error: <pre>{{Probe.Error}}</pre><br/>");
			stringBuilder.Append("Exception: <pre>{{Probe.Exception}}</pre><br/>");
			stringBuilder.Append("Details: <pre>{{Probe.FailureContext}}</pre><br/>");
			stringBuilder.Append("Log: <pre>{{Probe.ExecutionContext}}</pre>");
			return EscalateResponder.CreateDefinition("CheckAVEngineEnabledResponder", ExchangeComponent.AMScanners.Name, monitor.Name, monitor.ConstructWorkItemResultName(), Environment.MachineName, ServiceHealthStatus.Unhealthy, ExchangeComponent.AMScanners.EscalationTeam, "Not all AV scan engines are enabled", string.Format(stringBuilder.ToString(), new object[0]), true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
		}

		private void CreateFipsWorkItems()
		{
			WTFDiagnostics.TraceDebug(ExTraceGlobals.FIPSTracer, base.TraceContext, "Creating FIPS work items.", null, "CreateFipsWorkItems", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\FIPS\\FipsDiscovery.cs", 214);
			this.InitializeValidEngines();
			WTFDiagnostics.TraceDebug<int>(ExTraceGlobals.FIPSTracer, base.TraceContext, "Valid Engines have been obtained. Number of valid Engines = {0}", this.engines.Count, null, "CreateFipsWorkItems", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\FIPS\\FipsDiscovery.cs", 221);
			foreach (string text in this.GetEnginesByCategory("Antivirus"))
			{
				MonitorStateTransition[] monitorStateTransitions = new MonitorStateTransition[]
				{
					new MonitorStateTransition(ServiceHealthStatus.Degraded, 0)
				};
				double threshold = 5.0;
				int num = 3;
				MonitorDefinition monitor = this.CreateMonitor("MSExchange Hygiene Antimalware", "Engine Errors", ExchangeComponent.AMScanError, text, "EngineErrors", monitorStateTransitions, threshold, num);
				this.CreateEscalateResponder(monitor, text, "EngineErrors", ExchangeComponent.AMScanError, ServiceHealthStatus.Degraded, Strings.EscalationSubjectUnhealthy, Strings.EscalationMessageFailuresUnhealthy(Strings.AntimalwareEngineErrorsEscalationMessage(text, threshold, num * 5)), NotificationServiceClass.UrgentInTraining);
			}
			foreach (string text2 in this.GetEnginesByCategory("Classification"))
			{
				MonitorStateTransition[] monitorStateTransitions2 = new MonitorStateTransition[]
				{
					new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0)
				};
				double threshold = 5.0;
				int num = 2;
				MonitorDefinition monitor2 = this.CreateMonitor("MSExchange Hygiene Classification", "Engine Errors", ExchangeComponent.AMScanError, text2, "EngineErrors", monitorStateTransitions2, threshold, num);
				this.CreateEscalateResponder(monitor2, text2, "EngineErrors", ExchangeComponent.AMScanError, ServiceHealthStatus.Unhealthy, Strings.EscalationSubjectUnhealthy, Strings.EscalationMessageFailuresUnhealthy(Strings.ClassficationEngineErrorsEscalationMessage(text2, threshold, num * 5)), NotificationServiceClass.UrgentInTraining);
			}
		}

		private void InitializeValidEngines()
		{
			if (this.engines == null)
			{
				this.engines = new List<FipsDiscovery.Engine>();
				try
				{
					Collection<PSObject> collection = FipsUtils.RunFipsCmdlet<object>("Get-ValidEngines", null);
					foreach (PSObject psobject in collection)
					{
						string[] array = psobject.Properties["Categories"].Value as string[];
						this.engines.Add(new FipsDiscovery.Engine(psobject.Properties["Engine"].Value.ToString(), array[0]));
					}
				}
				catch (Exception ex)
				{
					WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.FIPSTracer, base.TraceContext, "Exception occured while creating the list of Valid Engines. {0}", ex.Message, null, "InitializeValidEngines", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\FIPS\\FipsDiscovery.cs", 320);
					throw;
				}
			}
		}

		private IEnumerable<string> GetEnginesByCategory(string category)
		{
			return from engine in this.engines
			where engine.Category.Equals(category, StringComparison.OrdinalIgnoreCase)
			select engine.Name;
		}

		private MonitorDefinition CreateMonitor(string perfCounterCategory, string perfCounterName, Component component, string perfCounterInstance, string workItemNamePrefix, MonitorStateTransition[] monitorStateTransitions, double threshold, int numberOfSamples)
		{
			perfCounterName = string.Format("{0}\\{1}\\{2}", perfCounterCategory, perfCounterName, perfCounterInstance);
			MonitorDefinition monitorDefinition = OverallConsecutiveSampleValueAboveThresholdMonitor.CreateDefinition(FipsDiscovery.GetMonitorName(perfCounterInstance, workItemNamePrefix), PerformanceCounterNotificationItem.GenerateResultName(perfCounterName), component.Name, component, threshold, numberOfSamples, true);
			monitorDefinition.MonitorStateTransitions = monitorStateTransitions;
			monitorDefinition.ServicePriority = 2;
			monitorDefinition.ScenarioDescription = string.Format("Validate FIPS health is not impacted by {0} AV issues", perfCounterInstance);
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			return monitorDefinition;
		}

		private void CreateEscalateResponder(MonitorDefinition monitor, string perfCounterInstance, string workItemNamePrefix, Component component, ServiceHealthStatus serviceHealthStatus, string escalationSubject, string escalationMessage, NotificationServiceClass escalationLevel)
		{
			ResponderDefinition definition = EscalateResponder.CreateDefinition(FipsDiscovery.GetEscalateResponderName(perfCounterInstance, workItemNamePrefix), component.Name, monitor.Name, monitor.ConstructWorkItemResultName(), Environment.MachineName, serviceHealthStatus, component.EscalationTeam, escalationSubject, escalationMessage, true, escalationLevel, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
		}

		private void CreateCollectFipsLogsResponder(MonitorDefinition monitor, string perfCounterInstance, string workItemNamePrefix, Component component, ServiceHealthStatus serviceHealthStatus)
		{
			ResponderDefinition definition = CollectFIPSLogsResponder.CreateDefinition(FipsDiscovery.GetCollectFipsLogsResponderName(perfCounterInstance, workItemNamePrefix), component.Name, monitor.Name, monitor.ConstructWorkItemResultName(), Environment.MachineName, serviceHealthStatus, 900, 28800, 600, 1, null, 432000, true);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
		}

		private static string GetMonitorName(string perfCounterInstance, string workItemNamePrefix)
		{
			return string.Format("{0}{1}{2}", perfCounterInstance, workItemNamePrefix, "Monitor");
		}

		private static string GetEscalateResponderName(string perfCounterInstance, string workItemNamePrefix)
		{
			return string.Format("{0}{1}{2}", perfCounterInstance, workItemNamePrefix, "EscalateResponder");
		}

		private static string GetCollectFipsLogsResponderName(string perfCounterInstance, string workItemNamePrefix)
		{
			return string.Format("{0}{1}{2}", perfCounterInstance, workItemNamePrefix, "CollectFIPSLogsResponder");
		}

		private const string CmdletGetValidEngines = "Get-ValidEngines";

		private List<FipsDiscovery.Engine> engines;

		private class Engine
		{
			public Engine(string name, string category)
			{
				this.Name = name;
				this.Category = category;
			}

			public string Name { get; private set; }

			public string Category { get; private set; }
		}
	}
}
