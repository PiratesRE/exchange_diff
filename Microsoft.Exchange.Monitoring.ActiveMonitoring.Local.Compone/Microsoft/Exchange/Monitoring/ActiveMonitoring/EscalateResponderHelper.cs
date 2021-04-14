using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Diagnostics.Components.ForefrontActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring
{
	internal class EscalateResponderHelper : ResponderDefinitionHelper
	{
		public static Dictionary<string, EscalateResponderHelper.AdditionalMessageContainer> AlertMaskToCustomMessageMap
		{
			get
			{
				return EscalateResponderHelper.alertMaskToCustomMessageMap;
			}
		}

		internal override ResponderDefinition CreateDefinition()
		{
			MonitorDefinition monitorDefinition = base.DiscoveryContext.Monitors.FirstOrDefault((MonitorDefinition m) => string.Equals(m.Name, base.AlertMask, StringComparison.OrdinalIgnoreCase));
			if (monitorDefinition == null)
			{
				string message = string.Format("Could not find the monitor with this name {0}.", base.AlertMask);
				WTFDiagnostics.TraceError(ExTraceGlobals.GenericHelperTracer, base.TraceContext, message, null, "CreateDefinition", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\GenericWorkItemHelper\\EscalateResponderHelper.cs", 72);
				throw new XmlException(message);
			}
			base.AlertMask = monitorDefinition.ConstructWorkItemResultName();
			if (string.IsNullOrWhiteSpace(base.TargetResource))
			{
				base.TargetResource = Environment.MachineName;
			}
			this.UpdateEscalationMessage();
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition(base.Name, base.ServiceName, base.AlertTypeId, base.AlertMask, base.TargetResource, base.TargetHealthState, base.EscalationTeam, base.EscalationSubject, base.EscalationMessage, base.Enabled, base.NotificationServiceClass, base.MinimumSecondsBetweenEscalates, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			if (base.AlwaysEscalateOnMonitorChanges != null)
			{
				responderDefinition.AlwaysEscalateOnMonitorChanges = base.AlwaysEscalateOnMonitorChanges.Value;
			}
			responderDefinition.ExtensionAttributes = base.ExtensionAttributes;
			responderDefinition.ParseExtensionAttributes(false);
			return responderDefinition;
		}

		private void UpdateEscalationMessage()
		{
			if (!string.IsNullOrWhiteSpace(base.EscalationMessage))
			{
				base.EscalationMessage += Environment.NewLine;
			}
			base.EscalationMessage += EscalateResponderHelper.UpdateEscalationMessage(base.DiscoveryContext, base.EscalationSubject, base.EscalationMessage);
		}

		private static string UpdateEscalationMessage(DiscoveryContext discoveryContext, string escalationSubject, string escalationMessage)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (!EscalateResponderHelper.ContainsResponderVariable("{Probe.Error}", escalationSubject) && !EscalateResponderHelper.ContainsResponderVariable("{Probe.Error}", escalationMessage))
			{
				stringBuilder.AppendLine("{Probe.Error}");
			}
			if (!(discoveryContext is PerfCounter))
			{
				foreach (KeyValuePair<string, string> keyValuePair in EscalateResponderHelper.escalateResponderVariables)
				{
					if (!EscalateResponderHelper.ContainsResponderVariable(keyValuePair.Key, escalationSubject) && !EscalateResponderHelper.ContainsResponderVariable(keyValuePair.Key, escalationMessage))
					{
						stringBuilder.AppendLine(keyValuePair.Value);
					}
				}
			}
			return stringBuilder.ToString();
		}

		private static bool ContainsResponderVariable(string contextVariableName, string message)
		{
			return message != null && message.Contains(contextVariableName);
		}

		private const string ProbeError = "{Probe.Error}";

		public static readonly object CustomMessageDictionaryLock = new object();

		private static readonly Dictionary<string, string> escalateResponderVariables = new Dictionary<string, string>
		{
			{
				"{Probe.Exception}",
				"Probe Exception: '{Probe.Exception}'"
			},
			{
				"{Probe.FailureContext}",
				"Failure Context: '{Probe.FailureContext}'"
			},
			{
				"{Probe.ExecutionContext}",
				"Execution Context: '{Probe.ExecutionContext}'"
			},
			{
				"{Probe.ResultName}",
				"Probe Result Name: '{Probe.ResultName}'"
			},
			{
				"{Probe.ResultType}",
				"Probe Result Type: '{Probe.ResultType}'"
			},
			{
				"{Monitor.TotalValue}",
				"Monitor Total Value: '{Monitor.TotalValue}'"
			},
			{
				"{Monitor.TotalSampleCount}",
				"Monitor Total Sample Count: '{Monitor.TotalSampleCount}'"
			},
			{
				"{Monitor.TotalFailedCount}",
				"Monitor Total Failed Count: '{Monitor.TotalFailedCount}'"
			},
			{
				"{Monitor.PoisonedCount}",
				"Monitor Poisoned Count: '{Monitor.PoisonedCount}'"
			},
			{
				"{Monitor.FirstAlertObservedTime}",
				"Monitor First Alert Observed Time: '{Monitor.FirstAlertObservedTime}'"
			}
		};

		private static Dictionary<string, EscalateResponderHelper.AdditionalMessageContainer> alertMaskToCustomMessageMap = new Dictionary<string, EscalateResponderHelper.AdditionalMessageContainer>();

		internal class AdditionalMessageContainer
		{
			public AdditionalMessageContainer(DateTime updateTime, string additionalMessage)
			{
				this.updateTime = updateTime;
				this.additionalMessage = additionalMessage;
			}

			internal readonly DateTime updateTime;

			internal readonly string additionalMessage;
		}
	}
}
