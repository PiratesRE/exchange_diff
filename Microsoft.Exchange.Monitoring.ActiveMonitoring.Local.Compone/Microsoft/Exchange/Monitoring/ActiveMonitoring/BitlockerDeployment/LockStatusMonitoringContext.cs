using System;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.BitlockerDeployment.Probes;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.BitlockerDeployment.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.BitlockerDeployment
{
	internal sealed class LockStatusMonitoringContext : MonitoringContextBase
	{
		public LockStatusMonitoringContext(IMaintenanceWorkBroker broker, TracingContext traceContext) : base(broker, traceContext)
		{
		}

		public override void CreateContext()
		{
			if (!LocalEndpointManager.IsDataCenter)
			{
				return;
			}
			ProbeDefinition probeDefinition = LockStatusProbe.CreateDefinition("LockStatusProbe", 600);
			base.EnrollWorkItem<ProbeDefinition>(probeDefinition);
			string name = "BitlockerDeploymentLockMonitor";
			string sampleMask = probeDefinition.ConstructWorkItemResultName();
			string name2 = ExchangeComponent.BitlockerDeployment.Name;
			Component bitlockerDeployment = ExchangeComponent.BitlockerDeployment;
			int numberOfFailures = 1;
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition(name, sampleMask, name2, bitlockerDeployment, 1200, 600, numberOfFailures, true);
			monitorDefinition.TargetResource = Environment.MachineName;
			monitorDefinition.ServicePriority = 2;
			monitorDefinition.ScenarioDescription = "Validate Bitlocker encryption deployment health is not impacted by any issues";
			MonitorStateResponderTuple[] array = new MonitorStateResponderTuple[2];
			array[0] = new MonitorStateResponderTuple
			{
				MonitorState = new MonitorStateTransition(ServiceHealthStatus.Degraded, 1200),
				Responder = BitlockerDeploymentLockResponder.CreateDefinition("BitlockerDeploymentLockResponder", ExchangeComponent.BitlockerDeployment.Name, monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), monitorDefinition.TargetResource, ServiceHealthStatus.Unhealthy)
			};
			MonitorStateResponderTuple[] array2 = array;
			int num = 1;
			MonitorStateResponderTuple monitorStateResponderTuple = default(MonitorStateResponderTuple);
			monitorStateResponderTuple.MonitorState = new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 1260);
			string name3 = "BitlockerLockEscalate";
			string name4 = ExchangeComponent.BitlockerDeployment.Name;
			string name5 = monitorDefinition.Name;
			string alertMask = monitorDefinition.ConstructWorkItemResultName();
			string targetResource = monitorDefinition.TargetResource;
			ServiceHealthStatus targetHealthState = ServiceHealthStatus.Unhealthy;
			string escalationTeam = "High Availability";
			string escalationSubjectUnhealthy = Strings.LocalMachineDriveEncryptionLockEscalationSubject(Environment.MachineName);
			string escalationMessageUnhealthy = Strings.LocalMachineDriveEncryptionLockEscalationMessage(BitlockerDeploymentUtility.GetLockedVolumes(), Environment.MachineName);
			int minimumSecondsBetweenEscalates = 3600;
			monitorStateResponderTuple.Responder = EscalateResponder.CreateDefinition(name3, name4, name5, alertMask, targetResource, targetHealthState, escalationTeam, escalationSubjectUnhealthy, escalationMessageUnhealthy, true, NotificationServiceClass.Scheduled, minimumSecondsBetweenEscalates, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			array2[num] = monitorStateResponderTuple;
			base.AddChainedResponders(ref monitorDefinition, array);
		}
	}
}
