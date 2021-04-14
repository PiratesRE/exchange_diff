using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Monitoring.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Monitoring.Discovery
{
	public class KeynoteLocalDiscovery : CentralMaintenanceWorkItem
	{
		public override Task GenerateWorkItems(CancellationToken cancellationToken)
		{
			string rulesFile = base.Definition.Attributes["RulesFile"];
			if (string.IsNullOrEmpty(rulesFile))
			{
				WTFDiagnostics.TraceInformation<TracingContext>(ExTraceGlobals.MonitoringTracer, base.TraceContext, string.Format("Rules file path is not set.  No more operations for this maintenance workitem.", new object[0]), base.TraceContext, null, "GenerateWorkItems", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Monitoring\\Keynote\\KeynoteLocalDiscovery.cs", 99);
				throw new Exception("keynote measurement alerting configuration rules file is not found");
			}
			Task<XDocument> task = Task.Factory.StartNew<XDocument>(delegate()
			{
				XDocument result;
				try
				{
					string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
					result = XDocument.Load(Path.Combine(directoryName, rulesFile));
				}
				catch (XmlException ex)
				{
					WTFDiagnostics.TraceError<TracingContext>(ExTraceGlobals.MonitoringTracer, this.TraceContext, string.Format("Rules file load failed from path: {0}.  Failure detail: {1}", rulesFile, ex), this.TraceContext, null, "GenerateWorkItems", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Monitoring\\Keynote\\KeynoteLocalDiscovery.cs", 114);
					throw new XmlException(string.Format("XML load failed for rules file path {0}", rulesFile), ex);
				}
				catch (IOException ex2)
				{
					WTFDiagnostics.TraceError<TracingContext>(ExTraceGlobals.MonitoringTracer, this.TraceContext, string.Format("Rules file load failed from path: {0}.  Failure detail: {1}", rulesFile, ex2), this.TraceContext, null, "GenerateWorkItems", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Monitoring\\Keynote\\KeynoteLocalDiscovery.cs", 119);
					throw new XmlException(string.Format("XML load failed for rules file path {0} with IO exception", rulesFile), ex2);
				}
				return result;
			}, cancellationToken, TaskCreationOptions.AttachedToParent, TaskScheduler.Default);
			task.ContinueWith(delegate(Task<XDocument> rulesResult)
			{
				if (rulesResult.Result != null)
				{
					using (IEnumerator<XElement> enumerator = rulesResult.Result.Element("KeynoteRules").Elements().GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							XElement xelement = enumerator.Current;
							double minAvailabilityThreshold;
							double.TryParse(xelement.Attribute("minAvailabilityThreshold").Value, out minAvailabilityThreshold);
							string targetDatabase = base.Definition.Attributes["TargetDatabase"];
							string targetServer = base.Definition.Attributes["TargetServer"];
							int lookbackMinutes;
							int.TryParse(base.Definition.Attributes["LookBackMinutes"], out lookbackMinutes);
							int num;
							int.TryParse(xelement.Attribute("minISPCountThreshold").Value, out num);
							int aggregationLevel;
							int.TryParse(xelement.Attribute("aggregationLevel").Value, out aggregationLevel);
							string value = xelement.Attribute("target").Value;
							bool flag;
							bool.TryParse(xelement.Attribute("Urgent").Value, out flag);
							MonitorDefinition monitorDefinition = KeynoteLocalDiscovery.CreateMonitorDefinition("KeynoteMeasurements", value, aggregationLevel, num, minAvailabilityThreshold, lookbackMinutes, targetServer, targetDatabase);
							base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
							ResponderDefinition definition = KeynoteEscalateResponder.CreateResponderDefinition(monitorDefinition.ConstructWorkItemResultName(), value, minAvailabilityThreshold, num, flag ? NotificationServiceClass.Urgent : NotificationServiceClass.Scheduled);
							base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
						}
						return;
					}
				}
				WTFDiagnostics.TraceInformation<TracingContext>(ExTraceGlobals.MonitoringTracer, base.TraceContext, string.Format("Rules file load failed.  Hence no maintenance workitems were created.", new object[0]), base.TraceContext, null, "GenerateWorkItems", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Monitoring\\Keynote\\KeynoteLocalDiscovery.cs", 177);
			}, cancellationToken, TaskContinuationOptions.AttachedToParent, TaskScheduler.Default);
			return task;
		}

		private static MonitorDefinition CreateMonitorDefinition(string sampleMask, string target, int aggregationLevel, int minIspCountThreshold, double minAvailabilityThreshold, int lookbackMinutes, string targetServer, string targetDatabase)
		{
			string value = string.Format("Data Source={0};Initial Catalog={1};Integrated Security=True", targetServer, targetDatabase);
			MonitorDefinition monitorDefinition = KeynoteMeasurementsMonitor.CreateDefinition("Keynote Measurements Monitor", sampleMask, ExchangeComponent.Monitoring.Name, ExchangeComponent.Monitoring, 1, true);
			monitorDefinition.RecurrenceIntervalSeconds = 600;
			monitorDefinition.TimeoutSeconds = 60;
			monitorDefinition.MonitoringIntervalSeconds = 1200;
			monitorDefinition.MaxRetryAttempts = 0;
			monitorDefinition.WorkItemVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			monitorDefinition.TypeName = typeof(KeynoteMeasurementsMonitor).FullName;
			monitorDefinition.ExecutionLocation = "EXO";
			monitorDefinition.TargetResource = target;
			monitorDefinition.InsufficientSamplesIntervalSeconds = 0;
			monitorDefinition.Attributes["LookBackMinutes"] = lookbackMinutes.ToString();
			monitorDefinition.Attributes["aggregationLevel"] = aggregationLevel.ToString();
			monitorDefinition.Attributes["minISPCountThreshold"] = minIspCountThreshold.ToString();
			monitorDefinition.Attributes["minAvailabilityThreshold"] = minAvailabilityThreshold.ToString();
			monitorDefinition.Attributes["Endpoint"] = value;
			return monitorDefinition;
		}

		public const string TargetDatabaseAttributeName = "TargetDatabase";

		public const string TargetServerAttributeName = "TargetServer";

		public const string MinimumIspCountThresholdAttributeName = "minISPCountThreshold";

		public const string MinimumAvailabilityThresholdAttributeName = "minAvailabilityThreshold";

		public const string AggregationLevelAttributeName = "aggregationLevel";

		public const string LookBackMinutesAttributeName = "LookBackMinutes";

		private const string UrgentAttribute = "Urgent";

		public const string TargetAttributename = "target";

		public const string EndpointAttributeName = "Endpoint";

		public const int MonitorRecurrenceSeconds = 600;

		public const int ResponderRecurrenceSeconds = 300;

		private const string KeynoteMonitorName = "Keynote Measurements Monitor";
	}
}
