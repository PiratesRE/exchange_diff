using System;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ClientAccess
{
	internal class EscalateComponentStateResponder : ExtraDetailsEscalateResponder
	{
		internal static ResponderDefinition CreateDefinition(string name, string serviceName, string alertTypeId, string alertMask, string targetResource, ServiceHealthStatus targetHealthState, string escalationService, string escalationTeam, string escalationSubjectUnhealthy, string escalationMessageUnhealthy, ServerComponentEnum serverComponentToVerifyState, CafeUtils.TriggerConfig triggerConfig, int probeIntervalSeconds, string logFolderRelativePath, string appPoolName, Type probeMonitorResultParserType, bool enabled = true, NotificationServiceClass notificationServiceClass = NotificationServiceClass.Urgent, int minimumSecondsBetweenEscalates = 14400)
		{
			ResponderDefinition responderDefinition = ExtraDetailsEscalateResponder.CreateDefinition(name, serviceName, alertTypeId, alertMask, targetResource, targetHealthState, escalationService, escalationTeam, escalationSubjectUnhealthy, escalationMessageUnhealthy, probeIntervalSeconds, logFolderRelativePath, appPoolName, probeMonitorResultParserType, enabled, notificationServiceClass, minimumSecondsBetweenEscalates, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59");
			responderDefinition.AssemblyPath = EscalateComponentStateResponder.AssemblyPath;
			responderDefinition.TypeName = EscalateComponentStateResponder.TypeName;
			responderDefinition.Attributes["ComponentStateServerComponentName"] = serverComponentToVerifyState.ToString();
			responderDefinition.Attributes["ComponentStateTriggerConfig"] = triggerConfig.ToString();
			return responderDefinition;
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			CafeUtils.InvokeResponderGivenComponentState(this, delegate(CancellationToken cancelToken)
			{
				this.<>n__FabricatedMethod1(cancelToken);
			}, base.TraceContext, cancellationToken);
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(EscalateComponentStateResponder).FullName;
	}
}
