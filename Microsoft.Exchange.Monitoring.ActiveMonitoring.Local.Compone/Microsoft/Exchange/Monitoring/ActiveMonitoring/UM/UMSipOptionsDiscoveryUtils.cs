using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.UM.Monitors;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.UM.Probes;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.UM.Responders;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.UM
{
	internal class UMSipOptionsDiscoveryUtils
	{
		public static void InstantiateSipOptionsMonitoringForUMCallRouterService(IMaintenanceWorkBroker broker, TracingContext traceContext)
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			try
			{
				ExAssert.RetailAssert(instance.UnifiedMessagingCallRouterEndpoint != null, "On Cafe Server, UnifiedMessagingCallRouterEndpoint should not be null");
				UMStartupMode startupMode = instance.UnifiedMessagingCallRouterEndpoint.StartupMode;
				if (startupMode == UMStartupMode.TCP)
				{
					UMSipOptionsDiscoveryUtils.InstantiateSipOptionsProbeMonitorResponderForUMCallRouterService(broker, traceContext, SipTransportType.TCP, instance.UnifiedMessagingCallRouterEndpoint.SipTcpListeningPort);
				}
				if (startupMode == UMStartupMode.TLS || startupMode == UMStartupMode.Dual)
				{
					UMSipOptionsDiscoveryUtils.InstantiateSipOptionsProbeMonitorResponderForUMCallRouterService(broker, traceContext, SipTransportType.TLS, instance.UnifiedMessagingCallRouterEndpoint.SipTlsListeningPort);
				}
			}
			catch (EndpointManagerEndpointUninitializedException)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, traceContext, "InstantiateSipOptionsMonitoringForUMCallRouterService: EndpointManagerEndpointUninitializedException is caught.", null, "InstantiateSipOptionsMonitoringForUMCallRouterService", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\UM\\Discovery\\UMSipOptionsDiscoveryUtils.cs", 275);
			}
		}

		public static void InstantiateSipOptionsMonitoringForUMService(IMaintenanceWorkBroker broker, TracingContext traceContext)
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			try
			{
				ExAssert.RetailAssert(instance.UnifiedMessagingServiceEndpoint != null, "On Mailbox Server, UnifiedMessagingServiceEndpoint should not be null");
				UMStartupMode startupMode = instance.UnifiedMessagingServiceEndpoint.StartupMode;
				int sipTcpListeningPort = instance.UnifiedMessagingServiceEndpoint.SipTcpListeningPort;
				int sipTlsListeningPort = instance.UnifiedMessagingServiceEndpoint.SipTlsListeningPort;
				if (startupMode == UMStartupMode.TCP)
				{
					UMSipOptionsDiscoveryUtils.InstantiateSipOptionsProbeMonitorResponderForUMService(broker, traceContext, SipTransportType.TCP, sipTcpListeningPort);
				}
				if (startupMode == UMStartupMode.TLS || startupMode == UMStartupMode.Dual)
				{
					UMSipOptionsDiscoveryUtils.InstantiateSipOptionsProbeMonitorResponderForUMService(broker, traceContext, SipTransportType.TLS, sipTlsListeningPort);
				}
			}
			catch (EndpointManagerEndpointUninitializedException)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.DirectoryTracer, traceContext, "InstantiateSipOptionsMonitoringForUMService: EndpointManagerEndpointUninitializedException is caught.", null, "InstantiateSipOptionsMonitoringForUMService", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\UM\\Discovery\\UMSipOptionsDiscoveryUtils.cs", 319);
			}
		}

		private static void InstantiateSipOptionsProbeMonitorResponderForUMCallRouterService(IMaintenanceWorkBroker broker, TracingContext traceContext, SipTransportType sipTransportType, int sipListeningPort)
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			UnifiedMessagingCallRouterEndpoint unifiedMessagingCallRouterEndpoint = instance.UnifiedMessagingCallRouterEndpoint;
			ProbeDefinition definition = UMSipOptionsProbe.CreateUMOptionsProbe(UMServiceType.MSExchangeUMCR, string.Empty, UMMonitoringConstants.UMCallRouterHealthSet, "UMCallRouterTestProbe", sipTransportType, unifiedMessagingCallRouterEndpoint.CertificateThumbprint, unifiedMessagingCallRouterEndpoint.CertificateSubjectName, sipListeningPort, "localhost", 120, 5, 0, traceContext);
			StartupNotification.SetStartupNotificationDefinition(definition, UMServiceType.MSExchangeUMCR.ToString(), 120);
			broker.AddWorkDefinition<ProbeDefinition>(definition, traceContext);
			MonitorDefinition monitorDefinition = UMSipOptionsMonitor.CreateUMSipOptionMonitor(string.Empty, ExchangeComponent.UMCallRouter, "UMCallRouterTestMonitor", "UMCallRouterTestProbe", 0, 5, 0, 480, 4, traceContext);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 1800),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, 2700)
			};
			StartupNotification.SetStartupNotificationDefinition(monitorDefinition, UMServiceType.MSExchangeUMCR.ToString(), 120);
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate UM Health is not impacted by call router issues";
			broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, traceContext);
			ResponderDefinition definition2 = UMSipOptionsRestartServiceResponder.CreateUMSipOptionRestartServiceResponder("UMCallRouterTestRestart", "UMCallRouterTestMonitor", string.Empty, UMMonitoringConstants.UMCallRouterHealthSet, UMServiceType.MSExchangeUMCR.ToString(), 60, 30, 120, 0, 300, 900, ServiceHealthStatus.Degraded, traceContext, "Cafe");
			StartupNotification.SetStartupNotificationDefinition(definition2, UMServiceType.MSExchangeUMCR.ToString(), 120);
			broker.AddWorkDefinition<ResponderDefinition>(definition2, traceContext);
			ResponderDefinition responderDefinition = CafeOfflineResponder.CreateDefinition("UMCallRouterTestOffline", "UMCallRouterTestMonitor", ServerComponentEnum.UMCallRouter, ServiceHealthStatus.Unhealthy, UMMonitoringConstants.UMCallRouterHealthSet, -1.0, "", "Datacenter", "F5AvailabilityData", "MachineOut");
			responderDefinition.ServiceName = UMMonitoringConstants.UMCallRouterHealthSet;
			StartupNotification.SetStartupNotificationDefinition(responderDefinition, UMServiceType.MSExchangeUMCR.ToString(), 120);
			broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, traceContext);
			ResponderDefinition definition3 = EscalateResponder.CreateDefinition("UMCallRouterTestEscalate", UMMonitoringConstants.UMCallRouterHealthSet, "UMCallRouterTestMonitor", "UMCallRouterTestMonitor", string.Empty, ServiceHealthStatus.Unrecoverable, UMSipOptionsDiscoveryUtils.SipOptionsEscalationTeam, Strings.UMSipOptionsToUMCallRouterServiceFailedEscalationSubject(Environment.MachineName), Strings.UMSipOptionsToUMCallRouterServiceFailedEscalationBody(Environment.MachineName), true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			StartupNotification.SetStartupNotificationDefinition(definition3, UMServiceType.MSExchangeUMCR.ToString(), 120);
			broker.AddWorkDefinition<ResponderDefinition>(definition3, traceContext);
		}

		private static void InstantiateSipOptionsProbeMonitorResponderForUMService(IMaintenanceWorkBroker broker, TracingContext traceContext, SipTransportType sipTransportType, int sipListeningPort)
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			UnifiedMessagingServiceEndpoint unifiedMessagingServiceEndpoint = instance.UnifiedMessagingServiceEndpoint;
			ProbeDefinition definition = UMSipOptionsProbe.CreateUMOptionsProbe(UMServiceType.MSExchangeUM, string.Empty, UMMonitoringConstants.UMProtocolHealthSet, "UMSelfTestProbe", sipTransportType, unifiedMessagingServiceEndpoint.CertificateThumbprint, unifiedMessagingServiceEndpoint.CertificateSubjectName, sipListeningPort, "localhost", 120, 5, 0, traceContext);
			StartupNotification.SetStartupNotificationDefinition(definition, UMServiceType.MSExchangeUM.ToString(), 300);
			broker.AddWorkDefinition<ProbeDefinition>(definition, traceContext);
			MonitorDefinition monitorDefinition = UMSipOptionsMonitor.CreateUMSipOptionMonitor(string.Empty, ExchangeComponent.UMProtocol, "UMSelfTestMonitor", "UMSelfTestProbe", 0, 5, 0, 480, 4, traceContext);
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Degraded, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 1800),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, 2700)
			};
			StartupNotification.SetStartupNotificationDefinition(monitorDefinition, UMServiceType.MSExchangeUM.ToString(), 300);
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate UM health is not impacted by UM backend service issues";
			broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, traceContext);
			ResponderDefinition definition2 = UMSipOptionsRestartServiceResponder.CreateUMSipOptionRestartServiceResponder("UMSelfTestRestart", "UMSelfTestMonitor", string.Empty, UMMonitoringConstants.UMProtocolHealthSet, UMServiceType.MSExchangeUM.ToString(), 60, 30, 120, 0, 300, 1200, ServiceHealthStatus.Degraded, traceContext, "Dag");
			StartupNotification.SetStartupNotificationDefinition(definition2, UMServiceType.MSExchangeUM.ToString(), 300);
			broker.AddWorkDefinition<ResponderDefinition>(definition2, traceContext);
			ResponderDefinition definition3 = UMSipOptionsEscalateResponder.CreateUMSipOptionEscalateResponder("UMSelfTestWithoutRecoveryEscalate", typeof(UMSipOptionsEscalateResponder).FullName, "UMSelfTestMonitor", "UMSelfTestMonitor", string.Empty, UMMonitoringConstants.UMProtocolHealthSet, ServiceHealthStatus.Unhealthy, UMSipOptionsDiscoveryUtils.SipOptionsEscalationTeam, Strings.UMSipOptionsToUMServiceFailedEscalationSubject(Environment.MachineName), Strings.UMSipOptionsToUMServiceFailedEscalationBody(Environment.MachineName), traceContext);
			StartupNotification.SetStartupNotificationDefinition(definition3, UMServiceType.MSExchangeUM.ToString(), 300);
			broker.AddWorkDefinition<ResponderDefinition>(definition3, traceContext);
			ResponderDefinition definition4 = EscalateResponder.CreateDefinition("UMSelfTestEscalate", UMMonitoringConstants.UMProtocolHealthSet, "UMSelfTestMonitor", "UMSelfTestMonitor", string.Empty, ServiceHealthStatus.Unrecoverable, UMSipOptionsDiscoveryUtils.SipOptionsEscalationTeam, Strings.UMSipOptionsToUMServiceFailedEscalationSubject(Environment.MachineName), Strings.UMSipOptionsToUMServiceFailedEscalationBody(Environment.MachineName), true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			StartupNotification.SetStartupNotificationDefinition(definition4, UMServiceType.MSExchangeUM.ToString(), 300);
			broker.AddWorkDefinition<ResponderDefinition>(definition4, traceContext);
		}

		private const int CallRouterStartupNotificationMaxStartWaitInSeconds = 120;

		private const int CallRouterServiceProbeTimeoutSecs = 5;

		private const int CallRouterServiceProbeRecurrenceIntervalSecs = 120;

		private const int CallRouterServiceProbeMaxRetryAttempt = 0;

		private const int CallRouterServiceMonitorTimeoutSecs = 5;

		private const int CallRouterServiceMonitorRecurrenceIntervalSecs = 0;

		private const int CallRouterServiceMonitorMonitoringIntervalSecs = 480;

		private const int CallRouterServiceMonitorMonitoringThreshold = 4;

		private const int CallRouterServiceMonitorMaxRetryAttempts = 0;

		private const int CallRouterServiceMonitorTransitionToDegradedSecs = 0;

		private const int CallRouterServiceMonitorTransitionToUnhealthySecs = 1800;

		private const int CallRouterServiceMonitorTransitionToUnrecoverableSecs = 2700;

		private const int CallRouterServiceRestartServiceResponderRecurrenceIntervalInSecs = 60;

		private const int CallRouterServiceRestartServiceResponderTimeoutInSecs = 30;

		private const int CallRouterServiceRestartServiceResponderWaitIntervalInSecs = 120;

		private const int CallRouterServiceRestartServiceResponderMaxRetryAttempt = 0;

		private const int CallRouterServiceRestartServiceResponderStopTimeoutInSecs = 300;

		private const int CallRouterServiceRestartServiceResponderStartTimeoutInSecs = 900;

		private const int UMServiceStartupNotificationMaxStartWaitInSeconds = 300;

		private const int UMServiceProbeTimeout = 5;

		private const int UMServiceProbeRecurrenceIntervalSeconds = 120;

		private const int UMServiceProbeMaxRetryAttempt = 0;

		private const int UMServiceMonitorTimeout = 5;

		private const int UMServiceMonitorRecurrenceIntervalSecs = 0;

		private const int UMServiceMonitorMonitoringIntervalSecs = 480;

		private const int UMServiceMonitorMonitoringThreshold = 4;

		private const int UMServiceMonitorMaxRetryAttempts = 0;

		private const int UMServiceMonitorTransitionToDegradedSecs = 0;

		private const int UMServiceMonitorTransitionToUnhealthySecs = 1800;

		private const int UMServiceMonitorTransitionToUnrecoverableSecs = 2700;

		private const int UMServiceRestartServiceResponderRecurrenceIntervalInSecs = 60;

		private const int UMServiceRestartServiceResponderTimeoutInSecs = 30;

		private const int UMServiceRestartServiceResponderWaitIntervalInSecs = 120;

		private const int UMServiceRestartServiceResponderMaxRetryAttempt = 0;

		private const int UMServiceRestartServiceResponderStopTimeoutInSecs = 300;

		private const int UMServiceRestartServiceResponderStartTimeoutInSecs = 1200;

		private static readonly string SipOptionsEscalationTeam = UMMonitoringConstants.UmEscalationTeam;
	}
}
