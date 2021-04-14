using System;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM
{
	internal class UMProtectedVoiceMessageEncryptDecryptFailedMonitorAndResponder : IUMLocalMonitoringMonitorAndResponder
	{
		public void InitializeMonitorAndResponder(IMaintenanceWorkBroker broker, TracingContext traceContext)
		{
			MonitorDefinition monitorDefinition = OverallPercentSuccessMonitor.CreateDefinition(UMProtectedVoiceMessageEncryptDecryptFailedMonitorAndResponder.UMProtectedVoiceMessageEncryptDecryptFailedMonitorName, NotificationItem.GenerateResultName(ExchangeComponent.UMProtocol.Name, UMNotificationEvent.ProtectedVoiceMessageEncryptDecrypt.ToString(), null), UMMonitoringConstants.UMProtocolHealthSet, ExchangeComponent.UMProtocol, 50.0, TimeSpan.FromSeconds(50.0), true);
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0)
			};
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate UM health is not impacted by voice message encryption and decryption issues";
			broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, traceContext);
			ResponderDefinition definition = EscalateResponder.CreateDefinition(UMProtectedVoiceMessageEncryptDecryptFailedMonitorAndResponder.UMProtectedVoiceMessageEncryptDecryptFailedResponderName, UMMonitoringConstants.UMProtocolHealthSet, UMProtectedVoiceMessageEncryptDecryptFailedMonitorAndResponder.UMProtectedVoiceMessageEncryptDecryptFailedMonitorName, UMProtectedVoiceMessageEncryptDecryptFailedMonitorAndResponder.UMProtectedVoiceMessageEncryptDecryptFailedMonitorName, string.Empty, ServiceHealthStatus.Unhealthy, UMMonitoringConstants.UmEscalationTeam, Strings.EscalationSubjectUnhealthy, Strings.UMProtectedVoiceMessageEncryptDecryptFailedEscalationMessage, true, NotificationServiceClass.Scheduled, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			broker.AddWorkDefinition<ResponderDefinition>(definition, traceContext);
		}

		private const int UMProtectedVoiceMessageEncryptDecryptFailedMonitorRecurrenceIntervalInSecs = 0;

		private const int UMProtectedVoiceMessageEncryptDecryptFailedMonitorMonitoringIntervalInSecs = 3600;

		private const int UMProtectedVoiceMessageEncryptDecryptFailedMonitorMonitoringThresholdInSecs = 50;

		private const int UMProtectedVoiceMessageEncryptDecryptFailedMonitorTransitionToUnhealthySecs = 0;

		private static readonly string UMProtectedVoiceMessageEncryptDecryptFailedMonitorName = "UMProtectedVoiceMessageEncryptDecryptFailedMonitor";

		private static readonly string UMProtectedVoiceMessageEncryptDecryptFailedResponderName = "UMProtectedVoiceMessageEncryptDecryptFailedEscalate";
	}
}
