using System;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Directory
{
	public class CheckMultipleDCInMMEscalateResponder : EscalateResponder
	{
		public static ResponderDefinition CreateDefinition(string responderName, string componentName, string alertTypeId, string alertMask, string DCFqdn, ServiceHealthStatus targetHealthState, string escalationTeam, string escalationSubject, string escalationMessage, NotificationServiceClass notificationServiceClass)
		{
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition(responderName, componentName, alertTypeId, alertMask, DCFqdn, targetHealthState, escalationTeam, escalationSubject, escalationMessage, true, notificationServiceClass, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition.AssemblyPath = CheckMultipleDCInMMEscalateResponder.AssemblyPath;
			responderDefinition.TypeName = CheckMultipleDCInMMEscalateResponder.TypeName;
			responderDefinition.RecurrenceIntervalSeconds = 1200;
			responderDefinition.WaitIntervalSeconds = 60;
			responderDefinition.TimeoutSeconds = 600;
			responderDefinition.MaxRetryAttempts = 3;
			responderDefinition.Enabled = true;
			responderDefinition.StartTime = DateTime.UtcNow;
			return responderDefinition;
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			string text = string.Empty;
			MonitorResult lastFailedMonitorResult = WorkItemResultHelper.GetLastFailedMonitorResult(this, base.Broker, cancellationToken);
			if (lastFailedMonitorResult != null)
			{
				text = lastFailedMonitorResult.StateAttribute2;
				if (string.IsNullOrEmpty(text))
				{
					stringBuilder.Append("DCs to recover (ProbeResult.StateAttribute2) was empty in the last failed probe result.");
				}
				else
				{
					string[] array = text.Split(new char[]
					{
						','
					});
					StringBuilder stringBuilder2 = new StringBuilder();
					StringBuilder stringBuilder3 = new StringBuilder();
					foreach (string text2 in array)
					{
						if (DirectoryGeneralUtils.CheckIfDCProvisioned(text2))
						{
							stringBuilder3.AppendFormat("{0} ", text2);
						}
						else
						{
							stringBuilder2.AppendFormat("{0} ", text2);
						}
					}
					if (!string.IsNullOrEmpty(stringBuilder3.ToString()))
					{
						stringBuilder.Append(Strings.PutMultipleDCIntoMMFailureEscalateMessage(stringBuilder3.ToString()));
						stringBuilder.AppendLine();
						flag = true;
					}
					if (!string.IsNullOrEmpty(stringBuilder2.ToString()))
					{
						stringBuilder.Append(Strings.PutMultipleDCIntoMMSuccessNotificationMessage(stringBuilder2.ToString()));
					}
				}
			}
			else
			{
				stringBuilder.Append("Unable to get Target DCs for recovery from last failed probe result.");
			}
			base.Result.StateAttribute1 = stringBuilder.ToString();
			if (flag)
			{
				base.DoResponderWork(cancellationToken);
			}
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(CheckMultipleDCInMMEscalateResponder).FullName;
	}
}
