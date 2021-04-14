using System;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Store.Responders
{
	public class DatabaseAvailabilityEscalateResponder : EscalateResponder
	{
		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			ProbeResult probeResult = null;
			string text = null;
			ProbeResult lastFailedProbeResult = WorkItemResultHelper.GetLastFailedProbeResult(this, base.Broker, cancellationToken);
			if (lastFailedProbeResult != null)
			{
				if (!string.IsNullOrWhiteSpace(lastFailedProbeResult.StateAttribute3))
				{
					int probeId;
					int resultId;
					if (int.TryParse(lastFailedProbeResult.StateAttribute2, out probeId) && int.TryParse(lastFailedProbeResult.StateAttribute3, out resultId))
					{
						probeResult = WorkItemResultHelper.GetProbeResultById(probeId, resultId, base.Broker, cancellationToken);
					}
					else
					{
						WTFDiagnostics.TraceInformation(ExTraceGlobals.StoreTracer, base.TraceContext, "DatabaseAvailabilityEscalateResponder.BeforeEscalate: Unable to get probe id's from probe stateattributes.", null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\DatabaseAvailabilityEscalateResponder.cs", 51);
					}
				}
				this.UpdateEscalationMessage(lastFailedProbeResult);
				if (probeResult != null)
				{
					if (!string.IsNullOrWhiteSpace(probeResult.StateAttribute12))
					{
						string value;
						bool flag;
						if (base.Definition.Attributes.TryGetValue(string.Format("{0}_{1}", probeResult.StateAttribute12, "Suppress"), out value) && !string.IsNullOrWhiteSpace(value) && bool.TryParse(value, out flag) && flag)
						{
							base.Result.StateAttribute4 = string.Format("Suppressing escalation for exception type {0}", probeResult.StateAttribute12);
							return;
						}
						if (base.Definition.Attributes.TryGetValue(probeResult.StateAttribute12, out text))
						{
							base.SetServiceAndTeam(ExchangeComponent.Store.Service, text);
						}
						string value2;
						NotificationServiceClass notificationServiceClass;
						if (base.Definition.Attributes.TryGetValue(string.Format("{0}_{1}", probeResult.StateAttribute12, "EscalationType"), out value2) && !string.IsNullOrWhiteSpace(value2) && Enum.TryParse<NotificationServiceClass>(value2, true, out notificationServiceClass))
						{
							WTFDiagnostics.TraceDebug<NotificationServiceClass>(ExTraceGlobals.StoreTracer, base.TraceContext, "DatabaseAvailabilityEscalateResponder.BeforeEscalate: setting NotificationServiceClass to '{0}'", notificationServiceClass, null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\DatabaseAvailabilityEscalateResponder.cs", 109);
							base.EscalationNotificationType = new NotificationServiceClass?(notificationServiceClass);
						}
					}
					base.CustomEscalationMessage = StoreMonitoringHelpers.PopulateEscalationMessage(base.CustomEscalationMessage, probeResult);
					WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.StoreTracer, base.TraceContext, "DatabaseAvailabilityEscalateResponder.BeforeEscalate: Escalating to '{0}' team based on failed probe result", (!string.IsNullOrWhiteSpace(text)) ? text : base.Definition.EscalationTeam, null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\DatabaseAvailabilityEscalateResponder.cs", 124);
				}
				else
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.StoreTracer, base.TraceContext, "DatabaseAvailabilityEscalateResponder.BeforeEscalate: Failed to get database availability failed probe result based on id's, escalating to the default team.", null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\DatabaseAvailabilityEscalateResponder.cs", 132);
				}
			}
			else
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.StoreTracer, base.TraceContext, "DatabaseAvailabilityEscalateResponder.BeforeEscalate: Failed to get last failed probe result, escalating to the default team.", null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\DatabaseAvailabilityEscalateResponder.cs", 140);
			}
			base.Result.StateAttribute4 = ((!string.IsNullOrWhiteSpace(text)) ? text : base.Definition.EscalationTeam);
			base.DoResponderWork(cancellationToken);
		}

		private void UpdateEscalationMessage(ProbeResult lastFailedProbeResult)
		{
			TimeSpan t;
			if (lastFailedProbeResult.ResultName.Contains("ActiveDatabaseAvailabilityEscalationNotification"))
			{
				t = StoreDiscovery.activeDBAvailabilityMonitoringInterval + StoreDiscovery.activeDBAvailabilityEscalationNotificationInterval + StoreDiscovery.activeDBAvailabilityRecurrence;
			}
			else
			{
				t = StoreDiscovery.passiveDBAvailabilityMonitoringInterval + StoreDiscovery.passiveDBAvailabilityEscalationNotificationInterval;
			}
			base.CustomEscalationMessage = base.Definition.EscalationMessage;
			if (lastFailedProbeResult.ResultName.Contains("ActiveDatabaseAvailabilityBackup"))
			{
				base.Result.StateAttribute5 = "ActiveDatabaseAvailabilityBackup";
				t += StoreDiscovery.activeDBAvailabilityBackupEscalationNotificationInterval;
				base.CustomEscalationMessage = string.Format("{0} {1}", "DELAYED ALERT: Escalation from backup escalation notification responder due to dependency monitoring suppressing the escalation notification responder. \n\n", base.CustomEscalationMessage);
			}
			Regex regex = new Regex("'%Duration%'", RegexOptions.Compiled);
			if (!string.IsNullOrWhiteSpace(base.Definition.EscalationSubject))
			{
				base.CustomEscalationSubject = regex.Replace(base.Definition.EscalationSubject, t.ToString());
			}
			if (!string.IsNullOrWhiteSpace(base.CustomEscalationMessage))
			{
				base.CustomEscalationMessage = regex.Replace(base.CustomEscalationMessage, t.ToString());
			}
		}
	}
}
