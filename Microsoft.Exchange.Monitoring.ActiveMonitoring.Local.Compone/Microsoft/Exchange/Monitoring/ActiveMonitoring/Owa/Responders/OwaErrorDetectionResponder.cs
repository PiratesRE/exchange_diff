using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Owa.Responders
{
	public class OwaErrorDetectionResponder : EscalateResponder
	{
		public static ResponderDefinition CreateResponderDefinition(string name, string serviceName, string alertTypeId, string alertMask, string targetResource, ServiceHealthStatus targetHealthState, string escalationTeam, string escalationSubjectUnhealthy, string escalationMessageUnhealthy, string whiteListedExceptions, bool enabled = true, NotificationServiceClass notificationServiceClass = NotificationServiceClass.UrgentInTraining, int minimumSecondsBetweenEscalates = 14400, string dailySchedulePattern = "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", bool loadEscalationMessageUnhealthyFromResource = false)
		{
			if (string.IsNullOrWhiteSpace(escalationSubjectUnhealthy))
			{
				throw new ArgumentException("escalationSubjectUnhealthy");
			}
			ResponderDefinition responderDefinition = new ResponderDefinition();
			responderDefinition.AssemblyPath = OwaErrorDetectionResponder.AssemblyPath;
			responderDefinition.TypeName = OwaErrorDetectionResponder.TypeName;
			responderDefinition.Name = name;
			responderDefinition.ServiceName = serviceName;
			responderDefinition.AlertTypeId = alertTypeId;
			responderDefinition.AlertMask = alertMask;
			responderDefinition.TargetResource = targetResource;
			responderDefinition.TargetHealthState = targetHealthState;
			responderDefinition.EscalationTeam = escalationTeam;
			responderDefinition.EscalationSubject = escalationSubjectUnhealthy;
			responderDefinition.EscalationMessage = escalationMessageUnhealthy;
			responderDefinition.NotificationServiceClass = notificationServiceClass;
			responderDefinition.MinimumSecondsBetweenEscalates = minimumSecondsBetweenEscalates;
			responderDefinition.DailySchedulePattern = dailySchedulePattern;
			responderDefinition.RecurrenceIntervalSeconds = 300;
			responderDefinition.WaitIntervalSeconds = minimumSecondsBetweenEscalates;
			responderDefinition.TimeoutSeconds = 300;
			responderDefinition.MaxRetryAttempts = 3;
			responderDefinition.Enabled = enabled;
			responderDefinition.Attributes[EscalateResponderBase.LoadFromResourceAttributeValue] = loadEscalationMessageUnhealthyFromResource.ToString();
			if (!string.IsNullOrEmpty(whiteListedExceptions))
			{
				responderDefinition.Attributes[OwaErrorDetectionResponder.WhiteListedExceptions] = whiteListedExceptions;
			}
			EscalateResponderBase.SetActiveMonitoringCertificateSettings(responderDefinition);
			return responderDefinition;
		}

		protected override void InvokeNewServiceAlert(Guid alertGuid, string alertTypeId, string alertName, string alertDescription, DateTime raisedTime, string escalationTeam, string service, string alertSource, bool isDatacenter, bool urgent, string environment, string location, string forest, string dag, string site, string region, string capacityUnit, string rack, string alertCategory)
		{
			IDataAccessQuery<ResponderResult> lastSuccessfulRecoveryAttemptedResponderResult = base.Broker.GetLastSuccessfulRecoveryAttemptedResponderResult(base.Definition, DateTime.UtcNow - SqlDateTime.MinValue.Value);
			Task<ResponderResult> task = lastSuccessfulRecoveryAttemptedResponderResult.ExecuteAsync(base.LocalCancellationToken, base.TraceContext);
			task.Continue(delegate(ResponderResult lastResponderResult)
			{
				DateTime startTime = SqlDateTime.MinValue.Value;
				if (lastResponderResult != null)
				{
					startTime = lastResponderResult.ExecutionStartTime;
				}
				string sampleMask = string.Format("{0}/{1}", ExchangeComponent.Eds.Name, this.Definition.Name.Substring(0, this.Definition.Name.LastIndexOf("Escalate")));
				IDataAccessQuery<ProbeResult> probeResults = this.Broker.GetProbeResults(sampleMask, startTime, DateTime.UtcNow);
				probeResults.ExecuteAsync(delegate(ProbeResult probeResult)
				{
					bool flag = true;
					string text = string.IsNullOrEmpty(probeResult.Error) ? string.Empty : probeResult.Error.Replace("%n%n", Environment.NewLine + Environment.NewLine);
					if (this.Definition.Attributes.ContainsKey(OwaErrorDetectionResponder.WhiteListedExceptions) && !string.IsNullOrEmpty(text))
					{
						IEnumerable<string> source = from x in this.Definition.Attributes[OwaErrorDetectionResponder.WhiteListedExceptions].Split(new char[]
						{
							OwaErrorDetectionResponder.WhiteListedExceptionsSeparator
						})
						select x.Trim();
						string text2 = text;
						string text3 = "Exception Type:";
						string value = "\r\n\r\nException Summary";
						int num = text.IndexOf(text3);
						int num2 = text.IndexOf(value);
						if (num == -1 || num2 == -1 || num2 < num + text3.Length)
						{
							WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, this.TraceContext, string.Format("OwaErrorDetectionResponder:: InvokeNewServiceAlert(): Probe Error is not in expected format, Skipping whilelisted exception check and alerting for error: {0}", text), null, "InvokeNewServiceAlert", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\Eds\\OwaErrorDetectionResponder.cs", 227);
						}
						else
						{
							num += text3.Length;
							text2 = text.Substring(num, num2 - num).Trim();
							if (source.Contains(text2, StringComparer.OrdinalIgnoreCase))
							{
								flag = false;
								WTFDiagnostics.TraceInformation(ExTraceGlobals.OWATracer, this.TraceContext, string.Format("OwaErrorDetectionResponder:: InvokeNewServiceAlert(): Skipping sending service alert form known exception.  Exception:{0}", text2), null, "InvokeNewServiceAlert", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Owa\\Eds\\OwaErrorDetectionResponder.cs", 240);
								this.Result.TargetHealthState = ServiceHealthStatus.None;
							}
						}
						this.Result.Exception = text2;
					}
					if (flag)
					{
						string alertDescription2 = string.Format("{0}{2}{1}", text, alertDescription, Environment.NewLine);
						this.InvokeBaseNewServiceAlert(alertGuid, alertTypeId, alertName, alertDescription2, raisedTime, escalationTeam, service, alertSource, isDatacenter, urgent, environment, location, forest, dag, site, region, capacityUnit);
					}
				}, this.LocalCancellationToken, this.TraceContext);
			}, base.LocalCancellationToken, TaskContinuationOptions.AttachedToParent);
		}

		protected virtual void InvokeBaseNewServiceAlert(Guid alertGuid, string alertTypeId, string alertName, string alertDescription, DateTime raisedTime, string escalationTeam, string service, string alertSource, bool isDatacenter, bool urgent, string environment, string location, string forest, string dag, string site, string region, string capacityUnit)
		{
			base.InvokeNewServiceAlert(alertGuid, alertTypeId, alertName, alertDescription, raisedTime, escalationTeam, service, alertSource, isDatacenter, urgent, environment, location, forest, dag, site, region, capacityUnit, null, null);
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(OwaErrorDetectionResponder).FullName;

		public static readonly string WhiteListedExceptions = "WhiteListedExceptions";

		private static readonly char WhiteListedExceptionsSeparator = '|';
	}
}
