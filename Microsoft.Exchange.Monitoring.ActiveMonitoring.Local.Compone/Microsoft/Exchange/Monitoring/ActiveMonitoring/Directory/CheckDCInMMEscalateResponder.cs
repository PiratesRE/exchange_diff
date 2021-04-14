using System;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Directory
{
	public class CheckDCInMMEscalateResponder : EscalateResponder
	{
		public static ResponderDefinition CreateDefinition(string responderName, string componentName, string alertTypeId, string alertMask, string DCFqdn, ServiceHealthStatus targetHealthState, string escalationTeam, string escalationSubject, string escalationMessage)
		{
			string targetResource = string.Empty;
			if (string.IsNullOrEmpty(DCFqdn))
			{
				targetResource = DirectoryGeneralUtils.GetLocalFQDN();
			}
			else
			{
				targetResource = DCFqdn;
			}
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition(responderName, componentName, alertTypeId, alertMask, targetResource, targetHealthState, escalationTeam, escalationSubject, escalationMessage, true, NotificationServiceClass.UrgentInTraining, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition.TypeName = CheckDCInMMEscalateResponder.TypeName;
			responderDefinition.RecurrenceIntervalSeconds = 300;
			responderDefinition.WaitIntervalSeconds = 600;
			responderDefinition.TimeoutSeconds = 300;
			responderDefinition.MaxRetryAttempts = 3;
			EscalateResponderBase.SetActiveMonitoringCertificateSettings(responderDefinition);
			return responderDefinition;
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Inside CheckDCInMMEscalateResponder::DoResponderWork.", null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\CheckDCInMMEscalateResponder.cs", 93);
			string text = base.Definition.TargetResource;
			if (string.IsNullOrEmpty(text))
			{
				text = DirectoryGeneralUtils.GetLocalFQDN();
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, base.TraceContext, string.Format("Checking the MM Flag for the DC {0} on RID Master.", text), null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\CheckDCInMMEscalateResponder.cs", 104);
			this.CheckDCInMMAndEscalate(text);
			base.DoResponderWork(cancellationToken);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, base.TraceContext, "Check DC In MM and Escalate Responder is Completed", null, "DoResponderWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Directory\\CheckDCInMMEscalateResponder.cs", 114);
		}

		private void CheckDCInMMAndEscalate(string targetSvrFQDN)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text = string.Empty;
			stringBuilder.AppendFormat("Checking if the DC {0} is in provisioned state on RID Master;  ", targetSvrFQDN);
			if (DirectoryGeneralUtils.CheckIfDCProvisioned(targetSvrFQDN))
			{
				stringBuilder.AppendFormat("DC {0} is still in provisioned state.  Expected that it should be in MM.  This will result in Escalation;  ", targetSvrFQDN);
				base.Definition.NotificationServiceClass = NotificationServiceClass.Urgent;
				text = Strings.PutDCIntoMMFailureEscalateMessage(base.Definition.EscalationMessage, targetSvrFQDN);
				stringBuilder.Append(text);
			}
			else
			{
				stringBuilder.AppendFormat("MM Flag for the DC {0} on RID Master shows that the DC is in MM.  There will be no escalation;  ", targetSvrFQDN);
				base.Definition.NotificationServiceClass = NotificationServiceClass.UrgentInTraining;
				text = Strings.PutDCIntoMMSuccessNotificationMessage(base.Definition.EscalationMessage, targetSvrFQDN);
				stringBuilder.Append(text);
			}
			base.Definition.EscalationMessage = text;
			base.Result.StateAttribute5 = stringBuilder.ToString();
		}

		private static readonly string TypeName = typeof(CheckDCInMMEscalateResponder).FullName;
	}
}
