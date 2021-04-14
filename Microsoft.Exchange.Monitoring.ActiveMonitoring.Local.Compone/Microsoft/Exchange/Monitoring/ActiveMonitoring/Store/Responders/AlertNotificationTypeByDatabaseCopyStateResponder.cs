using System;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Store.Responders
{
	public class AlertNotificationTypeByDatabaseCopyStateResponder : EscalateResponder
	{
		public static ResponderDefinition CreateDefinition(string name, string serviceName, string alertTypeId, string alertMask, string targetResource, string targetExtension, ServiceHealthStatus targetHealthState, string escalationTeam, string escalationSubject, string escalationMessage, NotificationServiceClass notificationServiceClass, TimeSpan recurrenceInterval, bool enabled = true)
		{
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition(name, serviceName, alertTypeId, alertMask, targetResource, targetHealthState, escalationTeam, escalationSubject, escalationMessage, enabled, notificationServiceClass, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition.RecurrenceIntervalSeconds = (int)recurrenceInterval.TotalSeconds;
			responderDefinition.TargetExtension = targetExtension;
			responderDefinition.AssemblyPath = AlertNotificationTypeByDatabaseCopyStateResponder.AssemblyPath;
			responderDefinition.TypeName = AlertNotificationTypeByDatabaseCopyStateResponder.TypeName;
			return responderDefinition;
		}

		protected override void BeforeEscalate(CancellationToken cancellationToken)
		{
			if (base.Definition.TargetHealthState == ServiceHealthStatus.None)
			{
				throw new InvalidOperationException(Strings.UnableToRunAlertNotificationTypeByDatabaseCopyStateResponder);
			}
			Guid guid;
			if (Guid.TryParse(base.Definition.TargetExtension, out guid))
			{
				Exception ex;
				CopyStatusClientCachedEntry[] copyStatus = CopyStatusHelper.GetCopyStatus(AmServerName.LocalComputerName, RpcGetDatabaseCopyStatusFlags2.None, new Guid[]
				{
					guid
				}, (int)AlertNotificationTypeByDatabaseCopyStateResponder.copyStatusTimeoutMSec, null, out ex);
				if (ex != null || copyStatus == null || copyStatus.Length != 1)
				{
					WTFDiagnostics.TraceError(ExTraceGlobals.StoreTracer, base.TraceContext, "Unable to get database state from AM; escalating with default notification type", null, "BeforeEscalate", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\AlertNotificationTypeByDatabaseCopyStateResponder.cs", 130);
					base.Result.StateAttribute4 = "UnableToGetDatabaseStateDefaultNotificationType";
					base.Result.StateAttribute5 = base.Definition.NotificationServiceClass.ToString();
					return;
				}
				if (!copyStatus[0].IsActive)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.StoreTracer, base.TraceContext, "Database state matches the state for downgrading the escalation; setting custom notification type", null, "BeforeEscalate", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\AlertNotificationTypeByDatabaseCopyStateResponder.cs", 145);
					base.Result.StateAttribute4 = "ChangingNotificationTypeBasedOnDatabaseState";
					this.SetEscalationNotificationType("EscalationNotificationTypeForPassive");
				}
			}
			else
			{
				WTFDiagnostics.TraceError(ExTraceGlobals.StoreTracer, base.TraceContext, "Unable to get database information based on the Guid supplied; escalating with default notification type", null, "BeforeEscalate", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Store\\AlertNotificationTypeByDatabaseCopyStateResponder.cs", 160);
				base.Result.StateAttribute4 = "UnableToGetDatabaseInfoDefaultNotificationType";
			}
			base.Result.StateAttribute5 = ((base.EscalationNotificationType != null) ? base.EscalationNotificationType.ToString() : base.Definition.NotificationServiceClass.ToString());
		}

		internal void SetEscalationNotificationType(string extensionAttribute)
		{
			string value;
			NotificationServiceClass value2;
			if (base.Definition.Attributes.TryGetValue(extensionAttribute, out value) && Enum.TryParse<NotificationServiceClass>(value, true, out value2))
			{
				base.EscalationNotificationType = new NotificationServiceClass?(value2);
			}
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(AlertNotificationTypeByDatabaseCopyStateResponder).FullName;

		private static double copyStatusTimeoutMSec = TimeSpan.FromSeconds(20.0).TotalMilliseconds;
	}
}
