using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Monitors;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	internal abstract class MonitorDefinitionHelper : DefinitionHelperBase
	{
		internal string SampleMask { get; set; }

		internal int MonitoringIntervalSeconds { get; set; }

		internal int MinimumErrorCount { get; set; }

		internal double MonitoringThreshold { get; set; }

		internal double SecondaryMonitoringThreshold { get; set; }

		internal int ServicePriority { get; set; }

		internal ServiceSeverity ServiceSeverity { get; set; }

		internal bool IsHaImpacting { get; set; }

		internal int InsufficientSamplesIntervalSeconds { get; set; }

		internal string StateAttribute1Mask { get; set; }

		internal int FailureCategoryMask { get; set; }

		internal int Version { get; set; }

		internal MonitorStateTransition[] MonitorStateTransitions { get; set; }

		internal bool AllowCorrelationToMonitor { get; set; }

		internal string TargetScopes { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(base.ToString());
			stringBuilder.AppendLine("SampleMask: " + this.SampleMask);
			stringBuilder.AppendLine("MonitoringIntervalSeconds: " + this.MonitoringIntervalSeconds);
			stringBuilder.AppendLine("MonitoringThreshold: " + this.MonitoringThreshold);
			stringBuilder.AppendLine("SecondaryMonitoringThreshold: " + this.SecondaryMonitoringThreshold);
			stringBuilder.AppendLine("MinimumErrorCount: " + this.MinimumErrorCount);
			stringBuilder.AppendLine("InsufficientSamplesIntervalSeconds: " + this.InsufficientSamplesIntervalSeconds);
			stringBuilder.AppendLine("StateAttribute1Mask: " + this.StateAttribute1Mask);
			stringBuilder.AppendLine("FailureCategoryMask: " + this.FailureCategoryMask);
			stringBuilder.AppendLine("ServicePriority: " + this.ServicePriority);
			stringBuilder.AppendLine("ServiceSeverity: " + this.ServiceSeverity);
			foreach (MonitorStateTransition arg in this.MonitorStateTransitions)
			{
				stringBuilder.AppendLine("MonitorStateTransition: " + arg);
			}
			stringBuilder.AppendLine("AllowCorrelationToMonitor: " + this.AllowCorrelationToMonitor);
			stringBuilder.AppendLine("TargetScopes: " + this.TargetScopes);
			return stringBuilder.ToString();
		}

		internal abstract MonitorDefinition CreateDefinition();

		internal override void ReadDiscoveryXml()
		{
			base.ReadDiscoveryXml();
			this.SampleMask = this.GetSampleMask();
			this.StateAttribute1Mask = base.GetOptionalXmlAttribute<string>("StateAttribute1Mask", null);
			this.FailureCategoryMask = base.GetOptionalXmlAttribute<int>("FailureCategoryMask", -1);
			this.MonitoringIntervalSeconds = base.GetOptionalXmlAttribute<int>("MonitoringIntervalSeconds", 0);
			this.MinimumErrorCount = base.GetOptionalXmlAttribute<int>("MinimumErrorCount", 0);
			this.MonitoringThreshold = base.GetOptionalXmlAttribute<double>("MonitoringThreshold", 0.0);
			this.SecondaryMonitoringThreshold = base.GetOptionalXmlAttribute<double>("SecondaryMonitoringThreshold", 0.0);
			this.InsufficientSamplesIntervalSeconds = base.GetOptionalXmlAttribute<int>("InsufficientSamplesIntervalSeconds", (int)TimeSpan.FromHours(8.0).TotalSeconds);
			string optionalXmlAttribute = base.GetOptionalXmlAttribute<string>("ServicePriority", string.Empty);
			if (!string.IsNullOrEmpty(optionalXmlAttribute))
			{
				this.ServicePriority = int.Parse(optionalXmlAttribute);
			}
			optionalXmlAttribute = base.GetOptionalXmlAttribute<string>("ServiceSeverity", string.Empty);
			if (!string.IsNullOrEmpty(optionalXmlAttribute))
			{
				this.ServiceSeverity = (ServiceSeverity)Enum.Parse(typeof(ServiceSeverity), optionalXmlAttribute);
			}
			this.MonitorStateTransitions = this.GetStateTransitions(base.DefinitionNode);
			this.AllowCorrelationToMonitor = base.GetOptionalXmlAttribute<bool>("AllowCorrelationToMonitor", false);
			this.TargetScopes = base.GetOptionalXmlAttribute<string>("TargetScopes", null);
		}

		internal override string ToString(WorkDefinition workItem)
		{
			MonitorDefinition monitorDefinition = (MonitorDefinition)workItem;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(base.ToString(workItem));
			stringBuilder.AppendLine("ComponentName: " + monitorDefinition.ComponentName);
			stringBuilder.AppendLine("SampleMask: " + monitorDefinition.SampleMask);
			stringBuilder.AppendLine("MonitoringIntervalSeconds: " + monitorDefinition.MonitoringIntervalSeconds);
			stringBuilder.AppendLine("MonitoringThreshold: " + monitorDefinition.MonitoringThreshold);
			stringBuilder.AppendLine("SecondaryMonitoringThreshold: " + monitorDefinition.SecondaryMonitoringThreshold);
			stringBuilder.AppendLine("MinimumErrorCount: " + monitorDefinition.MinimumErrorCount);
			stringBuilder.AppendLine("InsufficientSamplesIntervalSeconds: " + monitorDefinition.InsufficientSamplesIntervalSeconds);
			stringBuilder.AppendLine("StateAttribute1Mask: " + monitorDefinition.StateAttribute1Mask);
			stringBuilder.AppendLine("FailureCategoryMask: " + monitorDefinition.FailureCategoryMask);
			stringBuilder.AppendLine("ServicePriority: " + monitorDefinition.ServicePriority);
			stringBuilder.AppendLine("ServiceSeverity: " + monitorDefinition.ServiceSeverity);
			foreach (MonitorStateTransition arg in monitorDefinition.MonitorStateTransitions)
			{
				stringBuilder.AppendLine("MonitorStateTransition: " + arg);
			}
			stringBuilder.AppendLine("TargetScopes: " + monitorDefinition.TargetScopes);
			return stringBuilder.ToString();
		}

		protected MonitorDefinition CreateMonitorDefinition()
		{
			MonitorDefinition monitorDefinition = new MonitorDefinition();
			base.CreateBaseWorkDefinition(monitorDefinition);
			monitorDefinition.SampleMask = this.SampleMask;
			monitorDefinition.Component = base.Component;
			monitorDefinition.MonitoringThreshold = this.MonitoringThreshold;
			monitorDefinition.SecondaryMonitoringThreshold = this.SecondaryMonitoringThreshold;
			monitorDefinition.MonitoringIntervalSeconds = this.MonitoringIntervalSeconds;
			monitorDefinition.MinimumErrorCount = this.MinimumErrorCount;
			monitorDefinition.InsufficientSamplesIntervalSeconds = this.InsufficientSamplesIntervalSeconds;
			monitorDefinition.MonitorStateTransitions = this.MonitorStateTransitions;
			monitorDefinition.ServicePriority = this.ServicePriority;
			monitorDefinition.ServiceSeverity = this.ServiceSeverity;
			monitorDefinition.StateAttribute1Mask = this.StateAttribute1Mask;
			monitorDefinition.FailureCategoryMask = this.FailureCategoryMask;
			monitorDefinition.AllowCorrelationToMonitor = this.AllowCorrelationToMonitor;
			monitorDefinition.TargetScopes = this.TargetScopes;
			return monitorDefinition;
		}

		protected void GetAdditionalProperties(MonitorDefinition monitor)
		{
			monitor.MonitorStateTransitions = this.MonitorStateTransitions;
			monitor.ExtensionAttributes = base.ExtensionAttributes;
			monitor.ParseExtensionAttributes(false);
		}

		private string GetSampleMask()
		{
			if (base.DiscoveryContext is PerfCounter)
			{
				return PerformanceCounterNotificationItem.GenerateResultName(((PerfCounter)base.DiscoveryContext).PerfCounterName);
			}
			if (base.DiscoveryContext is NTEvent && ((NTEvent)base.DiscoveryContext).IsInstrumented)
			{
				string mandatoryXmlAttribute = base.GetMandatoryXmlAttribute<string>("EventNotificationServiceName");
				string mandatoryXmlAttribute2 = base.GetMandatoryXmlAttribute<string>("EventNotificationComponent");
				string optionalXmlAttribute = base.GetOptionalXmlAttribute<string>("EventNotificationTag", string.Empty);
				return NotificationItem.GenerateResultName(mandatoryXmlAttribute, mandatoryXmlAttribute2, optionalXmlAttribute);
			}
			if (base.WorkItemType == typeof(ComponentHealthHeartbeatMonitor) || base.WorkItemType == typeof(ComponentHealthPercentFailureMonitor))
			{
				return "*";
			}
			return base.GetMandatoryXmlAttribute<string>("SampleMask");
		}

		private MonitorStateTransition[] GetStateTransitions(XmlNode definition)
		{
			List<MonitorStateTransition> list = new List<MonitorStateTransition>();
			XmlNode xmlNode = definition.SelectSingleNode("StateTransitions");
			if (xmlNode != null)
			{
				using (XmlNodeList childNodes = xmlNode.ChildNodes)
				{
					if (childNodes != null)
					{
						foreach (object obj in childNodes)
						{
							XmlNode definition2 = (XmlNode)obj;
							ServiceHealthStatus mandatoryXmlEnumAttribute = DefinitionHelperBase.GetMandatoryXmlEnumAttribute<ServiceHealthStatus>(definition2, "ToState", base.TraceContext);
							int mandatoryXmlAttribute = DefinitionHelperBase.GetMandatoryXmlAttribute<int>(definition2, "TimeoutInSeconds", base.TraceContext);
							MonitorStateTransition monitorStateTransition = new MonitorStateTransition(mandatoryXmlEnumAttribute, mandatoryXmlAttribute);
							WTFDiagnostics.TraceDebug<ServiceHealthStatus, TimeSpan>(ExTraceGlobals.GenericHelperTracer, base.TraceContext, "[Transition] {0} Timeout:{1}", monitorStateTransition.ToState, monitorStateTransition.TransitionTimeout, null, "GetStateTransitions", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\MonitorDefinitionHelper.cs", 330);
							list.Add(monitorStateTransition);
						}
					}
				}
			}
			return list.ToArray();
		}
	}
}
