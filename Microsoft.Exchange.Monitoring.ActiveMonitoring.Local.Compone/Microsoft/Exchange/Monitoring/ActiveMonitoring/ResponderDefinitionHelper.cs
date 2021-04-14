using System;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	internal abstract class ResponderDefinitionHelper : DefinitionHelperBase
	{
		internal string AlertMask { get; set; }

		internal int WaitIntervalSeconds { get; set; }

		internal int MinimumSecondsBetweenEscalates { get; set; }

		internal string EscalationSubject { get; set; }

		internal string EscalationMessage { get; set; }

		internal string EscalationTeam { get; set; }

		internal NotificationServiceClass NotificationServiceClass { get; set; }

		internal string Endpoint { get; set; }

		internal string Account { get; set; }

		internal string AccountPassword { get; set; }

		internal string AlertTypeId { get; set; }

		internal ServiceHealthStatus TargetHealthState { get; set; }

		internal int Version { get; set; }

		internal string CorrelatedMonitorsXml { get; set; }

		public bool? AlwaysEscalateOnMonitorChanges { get; set; }

		internal CorrelatedMonitorAction ActionOnCorrelatedMonitors { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(base.ToString());
			stringBuilder.AppendLine("AlertTypeId: " + this.AlertTypeId);
			stringBuilder.AppendLine("AlertMask: " + this.AlertMask);
			stringBuilder.AppendLine("WaitIntervalSeconds: " + this.WaitIntervalSeconds);
			stringBuilder.AppendLine("MinimumSecondsBetweenEscalates: " + this.MinimumSecondsBetweenEscalates);
			stringBuilder.AppendLine("Account: " + this.Account);
			stringBuilder.AppendLine("AccountPassword: " + this.AccountPassword);
			stringBuilder.AppendLine("EscalationSubject: " + this.EscalationSubject);
			stringBuilder.AppendLine("EscalationMessage: " + this.EscalationMessage);
			stringBuilder.AppendLine("EscalationTeam: " + this.EscalationTeam);
			stringBuilder.AppendLine("Endpoint: " + this.Endpoint);
			stringBuilder.AppendLine("TargetHealthState: " + this.TargetHealthState);
			stringBuilder.AppendLine("NotificationServiceClass: " + this.NotificationServiceClass);
			stringBuilder.AppendLine("CorrelatedMonitorsXml: " + this.CorrelatedMonitorsXml);
			stringBuilder.AppendLine("AlwaysEscalateOnMonitorChanges: " + ((this.AlwaysEscalateOnMonitorChanges != null) ? this.AlwaysEscalateOnMonitorChanges.Value.ToString() : "<NULL>"));
			stringBuilder.AppendLine("ActionOnCorrelatedMonitors: " + this.ActionOnCorrelatedMonitors);
			return stringBuilder.ToString();
		}

		internal abstract ResponderDefinition CreateDefinition();

		internal override void ReadDiscoveryXml()
		{
			base.ReadDiscoveryXml();
			if (base.TypeName.EndsWith(typeof(EscalateResponder).Name))
			{
				this.EscalationSubject = base.GetMandatoryXmlAttribute<string>("EscalationSubjectUnhealthy");
			}
			else
			{
				this.EscalationSubject = base.GetOptionalXmlAttribute<string>("EscalationSubjectUnhealthy", string.Empty);
			}
			this.AlertTypeId = base.GetOptionalXmlAttribute<string>("AlertTypeId", string.Empty);
			this.AlertMask = base.GetOptionalXmlAttribute<string>("AlertMask", string.Empty);
			this.WaitIntervalSeconds = base.GetOptionalXmlAttribute<int>("WaitIntervalSeconds", 0);
			this.MinimumSecondsBetweenEscalates = base.GetOptionalXmlAttribute<int>("MinimumSecondsBetweenEscalates", 14400);
			this.Account = base.GetOptionalXmlAttribute<string>("Account", string.Empty);
			this.AccountPassword = base.GetOptionalXmlAttribute<string>("AccountPassword", string.Empty);
			this.EscalationTeam = base.GetOptionalXmlAttribute<string>("EscalationTeam", base.Component.EscalationTeam);
			this.EscalationMessage = base.GetOptionalXmlAttribute<string>("EscalationMessageUnhealthy", string.Empty);
			this.Endpoint = base.GetOptionalXmlAttribute<string>("Endpoint", string.Empty);
			this.TargetHealthState = base.GetOptionalXmlEnumAttribute<ServiceHealthStatus>("TargetHealthState", ServiceHealthStatus.None);
			this.NotificationServiceClass = base.GetOptionalXmlEnumAttribute<NotificationServiceClass>("EscalationLevel", NotificationServiceClass.Scheduled);
			this.AlwaysEscalateOnMonitorChanges = base.GetOptionalXmlAttribute<bool?>("AlwaysEscalateOnMonitorChanges", null);
			this.CorrelatedMonitorsXml = string.Empty;
			XmlNode xmlNode = base.DefinitionNode.SelectSingleNode("Correlation");
			if (xmlNode != null)
			{
				this.CorrelatedMonitorsXml = xmlNode.OuterXml;
				this.ActionOnCorrelatedMonitors = DefinitionHelperBase.GetOptionalXmlAttribute<CorrelatedMonitorAction>(xmlNode, "ActionOnCorrelatedMonitors", CorrelatedMonitorAction.LogAndContinue, base.TraceContext);
			}
		}

		internal override string ToString(WorkDefinition workItem)
		{
			ResponderDefinition responderDefinition = (ResponderDefinition)workItem;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(base.ToString(workItem));
			stringBuilder.AppendLine("AlertTypeId: " + responderDefinition.AlertTypeId);
			stringBuilder.AppendLine("AlertMask: " + responderDefinition.AlertMask);
			stringBuilder.AppendLine("WaitIntervalSeconds: " + responderDefinition.WaitIntervalSeconds);
			stringBuilder.AppendLine("Account: " + responderDefinition.Account);
			stringBuilder.AppendLine("AccountPassword: " + responderDefinition.AccountPassword);
			stringBuilder.AppendLine("EscalationSubject: " + responderDefinition.EscalationSubject);
			stringBuilder.AppendLine("EscalationMessage: " + responderDefinition.EscalationMessage);
			stringBuilder.AppendLine("EscalationTeam: " + responderDefinition.EscalationTeam);
			stringBuilder.AppendLine("Endpoint: " + responderDefinition.Endpoint);
			stringBuilder.AppendLine("TargetHealthState: " + responderDefinition.TargetHealthState);
			stringBuilder.AppendLine("NotificationServiceClass: " + responderDefinition.NotificationServiceClass);
			stringBuilder.AppendLine("MinimumSecondsBetweenEscalates: " + responderDefinition.MinimumSecondsBetweenEscalates);
			return stringBuilder.ToString();
		}

		protected ResponderDefinition CreateResponderDefinition()
		{
			ResponderDefinition responderDefinition = new ResponderDefinition();
			base.CreateBaseWorkDefinition(responderDefinition);
			responderDefinition.AlertTypeId = this.AlertTypeId;
			responderDefinition.AlertMask = this.AlertMask;
			responderDefinition.EscalationTeam = this.EscalationTeam;
			responderDefinition.EscalationSubject = this.EscalationSubject;
			responderDefinition.EscalationMessage = this.EscalationMessage;
			responderDefinition.NotificationServiceClass = this.NotificationServiceClass;
			responderDefinition.WaitIntervalSeconds = this.WaitIntervalSeconds;
			responderDefinition.TargetHealthState = this.TargetHealthState;
			responderDefinition.Account = this.Account;
			responderDefinition.AccountPassword = this.AccountPassword;
			responderDefinition.Endpoint = this.Endpoint;
			if (this.AlwaysEscalateOnMonitorChanges != null)
			{
				responderDefinition.AlwaysEscalateOnMonitorChanges = this.AlwaysEscalateOnMonitorChanges.Value;
			}
			responderDefinition.CorrelatedMonitorsXml = this.CorrelatedMonitorsXml;
			responderDefinition.ActionOnCorrelatedMonitors = this.ActionOnCorrelatedMonitors;
			return responderDefinition;
		}
	}
}
