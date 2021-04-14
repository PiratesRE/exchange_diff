using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.MailboxAssistants.Assistants.InferenceDataCollection;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Monitors;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring.Responders;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local.Components.Common.Probes;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Inference
{
	public sealed class InferenceDiscovery : MaintenanceWorkItem
	{
		public static ResponderDefinition CreateEscalateResponder(MonitorDefinition monitor, bool enabled, string escalationMessage, string escalationSubject = null, ServiceHealthStatus targetHealthStatus = ServiceHealthStatus.Unhealthy)
		{
			string alertMask = monitor.ConstructWorkItemResultName();
			string name = InferenceStrings.InferenceEscalateResponderName(monitor.Name);
			if (string.IsNullOrEmpty(escalationSubject))
			{
				escalationSubject = escalationMessage.Split(new char[]
				{
					'.'
				})[0];
			}
			ResponderDefinition responderDefinition = EscalateResponder.CreateDefinition(name, ExchangeComponent.Inference.Name, monitor.Name, alertMask, monitor.TargetResource, targetHealthStatus, ExchangeComponent.Inference.EscalationTeam, escalationSubject, escalationMessage, true, NotificationServiceClass.Urgent, 14400, "Pacific Standard Time/Monday,Tuesday,Wednesday,Thursday,Friday,Saturday,Sunday/00:00/23:59", false);
			responderDefinition.NotificationServiceClass = NotificationServiceClass.Scheduled;
			responderDefinition.Enabled = enabled;
			responderDefinition.RecurrenceIntervalSeconds = 0;
			return responderDefinition;
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			try
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.InferenceTracer, base.TraceContext, "InferenceDiscovery: DoWork: Starting discovery...", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Inference\\InferenceDiscovery.cs", 99);
				if (!LocalEndpointManager.IsDataCenter)
				{
					WTFDiagnostics.TraceInformation(ExTraceGlobals.InferenceTracer, base.TraceContext, "InferenceDiscovery: DoWork: Exchange datacenter is not found. Returning..", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Inference\\InferenceDiscovery.cs", 103);
				}
				else
				{
					this.attributeHelper = new AttributeHelper(base.Definition);
					LocalEndpointManager instance = LocalEndpointManager.Instance;
					if (instance.ExchangeServerRoleEndpoint == null || !instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
					{
						WTFDiagnostics.TraceInformation(ExTraceGlobals.SearchTracer, base.TraceContext, "InferenceDiscovery.DoWork: Mailbox role is not installed on this server, no need to create inference related work items", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Inference\\InferenceDiscovery.cs", 113);
					}
					else
					{
						this.TryEnableDataCollectionContext();
						this.CreateClassificationSLAMonitoringContext();
						this.CreateTrainingFailurePercentageMonitoringContext();
						this.CreateMailboxAssistantsCrashContext();
						this.CreateDeliveryCrashContext();
						this.CreateInferenceComponentsDisabledContext();
						this.CreateInferenceActivityLogSyntheticContext();
						WTFDiagnostics.TraceInformation(ExTraceGlobals.InferenceTracer, base.TraceContext, "InferenceDiscovery.DoWork: Finishing discovery...", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Inference\\InferenceDiscovery.cs", 125);
					}
				}
			}
			catch (EndpointManagerEndpointUninitializedException)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.InferenceTracer, base.TraceContext, "InferenceDiscovery.DoWork: EndpointManagerEndpointUninitializedException is caught", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Inference\\InferenceDiscovery.cs", 129);
			}
		}

		private bool TryEnableDataCollectionContext()
		{
			if (InferenceDataCollectionAssistantType.IsAssistantEnabled())
			{
				this.CreateHeartbeatMonitoringContext("InferenceDataCollectionSuccessMonitor", "InferenceDataCollectionSuccessNotification", 1800, 86400);
				this.CreateHeartbeatMonitoringContext("InferenceDataCollectionProgressMonitor", "InferenceDataCollectionProgressNotification", 1800, 21600);
				WTFDiagnostics.TraceInformation(ExTraceGlobals.InferenceTracer, base.TraceContext, "InferenceDiscovery.DoWork: Created data collection monitors and responders...", null, "TryEnableDataCollectionContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Inference\\InferenceDiscovery.cs", 158);
				return true;
			}
			WTFDiagnostics.TraceInformation(ExTraceGlobals.InferenceTracer, base.TraceContext, "InferenceDiscovery.DoWork: Skipped creation of data collection monitors as data collection is not enabled on this machine.", null, "TryEnableDataCollectionContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Inference\\InferenceDiscovery.cs", 163);
			return false;
		}

		private void CreateHeartbeatMonitoringContext(string monitorName, string notificationNameMask, int defaultRecurranceIntervalInSeconds, int defaultHeartbeatSLAInSeconds)
		{
			bool @bool = this.attributeHelper.GetBool(string.Format("{0}Enabled", monitorName), true, true);
			if (!@bool)
			{
				WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.InferenceTracer, base.TraceContext, "InferenceDiscovery.CreateHeartbeatMonitoringContext: Skipping monitor: {0} as it is disabled in the config", monitorName, null, "CreateHeartbeatMonitoringContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Inference\\InferenceDiscovery.cs", 181);
				return;
			}
			int @int = this.attributeHelper.GetInt(string.Format("{0}RecurranceIntervalSeconds", monitorName), true, defaultRecurranceIntervalInSeconds, null, null);
			int int2 = this.attributeHelper.GetInt(string.Format("{0}SLASeconds", monitorName), true, defaultHeartbeatSLAInSeconds, null, null);
			WTFDiagnostics.TraceInformation<string, int>(ExTraceGlobals.InferenceTracer, base.TraceContext, "InferenceDiscovery.CreateHeartbeatMonitoringContext: Adding monitor: {0} with interval", monitorName, @int, null, "CreateHeartbeatMonitoringContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Inference\\InferenceDiscovery.cs", 188);
			string notificationMask = NotificationItem.GenerateResultName(ExchangeComponent.Inference.Name, ExchangeComponent.Inference.Name, notificationNameMask);
			MonitorDefinition monitorDefinition = NotificationHeartbeatMonitor.CreateDefinition(monitorName, ExchangeComponent.Inference.Name, ExchangeComponent.Inference, notificationMask, @int, int2, @bool);
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate Inference health is not impacted by any issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			WTFDiagnostics.TraceInformation(ExTraceGlobals.InferenceTracer, base.TraceContext, "InferenceDiscovery.CreateHeartbeatMonitoringContext: Adding responder.", null, "CreateHeartbeatMonitoringContext", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Inference\\InferenceDiscovery.cs", 209);
			ResponderDefinition responderDefinition = InferenceDiscovery.CreateEscalateResponder(monitorDefinition, @bool, string.Format("Monitor {0} is unhealthy on machine {1}", monitorName, Environment.MachineName), string.Format("Monitor {0} is unhealthy", monitorName), ServiceHealthStatus.Unhealthy);
			responderDefinition.MinimumSecondsBetweenEscalates = int2;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
		}

		private void CreateClassificationSLAMonitoringContext()
		{
			bool @bool = this.attributeHelper.GetBool("InferenceClassificationSLAMonitorEnabled", true, true);
			int @int = this.attributeHelper.GetInt("InferenceClassificationSLAMonitorMonitoringThreshold", true, 0, null, null);
			int int2 = this.attributeHelper.GetInt("InferenceClassificationSLAMonitorMonitoringInterval", true, 0, null, null);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("InferenceClassificationSLAMonitor", NotificationItem.GenerateResultName(ExchangeComponent.Eds.Name, "ClassificationSLATrigger_Warning", "classificationpipeline"), ExchangeComponent.Inference.Name, ExchangeComponent.Inference, @int, true, 300);
			monitorDefinition.TargetResource = string.Empty;
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			monitorDefinition.MonitoringIntervalSeconds = int2;
			monitorDefinition.Enabled = @bool;
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate Inference health is not impacted by classification SLA performance issues";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderDefinition responderDefinition = InferenceDiscovery.CreateEscalateResponder(monitorDefinition, @bool, Strings.InferenceClassifcationSLAEscalationMessage, null, ServiceHealthStatus.Unhealthy);
			responderDefinition.NotificationServiceClass = NotificationServiceClass.Scheduled;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
		}

		private void CreateTrainingFailurePercentageMonitoringContext()
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (instance.MailboxDatabaseEndpoint == null || instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend.Count == 0)
			{
				return;
			}
			bool @bool = this.attributeHelper.GetBool("InferenceTrainingFailurePercentageMonitorEnabled", true, true);
			int @int = this.attributeHelper.GetInt("InferenceTrainingFailurePercentageMonitorMonitoringThreshold", true, 0, null, null);
			int int2 = this.attributeHelper.GetInt("InferenceTrainingFailurePercentageMonitorMonitoringInterval", true, 0, null, null);
			foreach (MailboxDatabaseInfo mailboxDatabaseInfo in instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend)
			{
				string mailboxDatabaseName = mailboxDatabaseInfo.MailboxDatabaseName;
				string str = mailboxDatabaseInfo.MailboxDatabaseGuid.ToString().ToLower();
				MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("InferenceTrainingFailurePercentageMonitor", NotificationItem.GenerateResultName(ExchangeComponent.Eds.Name, "TrainingFailurePercentageTrigger_Warning", "training-" + str), ExchangeComponent.Inference.Name, ExchangeComponent.Inference, @int, true, 300);
				monitorDefinition.TargetResource = mailboxDatabaseName;
				monitorDefinition.RecurrenceIntervalSeconds = 0;
				monitorDefinition.MonitoringIntervalSeconds = int2;
				monitorDefinition.Enabled = @bool;
				monitorDefinition.ServicePriority = 1;
				monitorDefinition.ScenarioDescription = "Validate Inference health is not impacted by inference training failure issues";
				base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
				ResponderDefinition definition = InferenceDiscovery.CreateEscalateResponder(monitorDefinition, @bool, Strings.InferenceTrainingSLAEscalationMessage, null, ServiceHealthStatus.Unhealthy);
				base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
			}
		}

		private void CreateMailboxAssistantsCrashContext()
		{
			string text = "MSExchangeMailboxAssistants";
			string moduleName = "Inference";
			bool @bool = this.attributeHelper.GetBool("InferenceMailboxAssistantsCrashMonitoringEnabled", true, true);
			int @int = this.attributeHelper.GetInt("InferenceMailboxAssistantsCrashProbeRecurrenceIntervalSeconds", true, 0, null, null);
			int int2 = this.attributeHelper.GetInt("InferenceMailboxAssistantsCrashMonitorMonitoringIntervalSeconds", true, 0, null, null);
			int int3 = this.attributeHelper.GetInt("InferenceMailboxAssistantsCrashMonitorMonitoringThreshold", true, 0, null, null);
			bool bool2 = this.attributeHelper.GetBool("InferenceMailboxAssistantsCrashDisableInferenceComponentResponderEnabled", true, true);
			ProbeDefinition probeDefinition = GenericProcessCrashDetectionProbe.CreateDefinition("InferenceMailboxAssistantsCrashProbe", text, @int, moduleName, true);
			probeDefinition.ServiceName = ExchangeComponent.Inference.Name;
			probeDefinition.Enabled = @bool;
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition("InferenceMailboxAssistantsCrashMonitor", probeDefinition.ConstructWorkItemResultName(), ExchangeComponent.Inference.Name, ExchangeComponent.Inference, int2, 0, int3, @bool);
			monitorDefinition.TargetResource = text;
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, 60)
			};
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate Inference health is not impacted by issues with mailbox assistances";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderDefinition definition = DisableInferenceComponentResponder.CreateDefinition(InferenceStrings.DisableInferenceComponentResponderName(monitorDefinition.Name), monitorDefinition.ConstructWorkItemResultName(), ServiceHealthStatus.Unhealthy, "Training, DataCollection", bool2 && @bool);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
			string escalationMessage = Strings.InferenceTrainingDataCollectionRepeatedCrashEscalationMessage(text, int3, int2);
			ResponderDefinition definition2 = InferenceDiscovery.CreateEscalateResponder(monitorDefinition, @bool, escalationMessage, null, ServiceHealthStatus.Unrecoverable);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition2, base.TraceContext);
		}

		private void CreateDeliveryCrashContext()
		{
			string text = "MSExchangeDelivery";
			string moduleName = "Inference";
			bool @bool = this.attributeHelper.GetBool("InferenceDeliveryCrashMonitoringEnabled", true, true);
			int @int = this.attributeHelper.GetInt("InferenceDeliveryCrashProbeRecurrenceIntervalSeconds", true, 0, null, null);
			int int2 = this.attributeHelper.GetInt("InferenceDeliveryCrashMonitorMonitoringIntervalSeconds", true, 0, null, null);
			int int3 = this.attributeHelper.GetInt("InferenceDeliveryCrashMonitorMonitoringThreshold", true, 0, null, null);
			bool bool2 = this.attributeHelper.GetBool("InferenceDeliveryCrashDisableInferenceComponentResponderEnabled", true, true);
			ProbeDefinition probeDefinition = GenericProcessCrashDetectionProbe.CreateDefinition("InferenceDeliveryCrashProbe", text, @int, moduleName, true);
			probeDefinition.ServiceName = ExchangeComponent.Inference.Name;
			probeDefinition.Enabled = @bool;
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition = OverallXFailuresMonitor.CreateDefinition("InferenceDeliveryCrashMonitor", probeDefinition.ConstructWorkItemResultName(), ExchangeComponent.Inference.Name, ExchangeComponent.Inference, int2, 0, int3, @bool);
			monitorDefinition.TargetResource = text;
			monitorDefinition.MonitorStateTransitions = new MonitorStateTransition[]
			{
				new MonitorStateTransition(ServiceHealthStatus.Unhealthy, 0),
				new MonitorStateTransition(ServiceHealthStatus.Unrecoverable, 60)
			};
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate Inference health is not impacted by issues with delivery assistances";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderDefinition definition = DisableInferenceComponentResponder.CreateDefinition(InferenceStrings.DisableInferenceComponentResponderName(monitorDefinition.Name), monitorDefinition.ConstructWorkItemResultName(), ServiceHealthStatus.Unhealthy, "Classification", bool2 && @bool);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
			string escalationMessage = Strings.InferenceClassificationRepeatedCrashEscalationMessage(text, int3, int2);
			ResponderDefinition definition2 = InferenceDiscovery.CreateEscalateResponder(monitorDefinition, @bool, escalationMessage, null, ServiceHealthStatus.Unrecoverable);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition2, base.TraceContext);
		}

		private void CreateInferenceComponentsDisabledContext()
		{
			bool @bool = this.attributeHelper.GetBool("InferenceComponentDisabledMonitoringEnabled", true, true);
			int @int = this.attributeHelper.GetInt("InferenceComponentDisabledProbeRecurrenceIntervalSeconds", true, 0, null, null);
			int int2 = this.attributeHelper.GetInt("InferenceComponentDisabledMonitorMonitoringThreshold", true, 0, null, null);
			ProbeDefinition probeDefinition = InferenceComponentDisabledProbe.CreateDefinition("InferenceComponentDisabledProbe", @int, @bool);
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition = OverallConsecutiveProbeFailuresMonitor.CreateDefinition("InferenceComponentDisabledMonitor", probeDefinition.ConstructWorkItemResultName(), ExchangeComponent.Inference.Name, ExchangeComponent.Inference, int2, @bool, int2 * @int);
			monitorDefinition.RecurrenceIntervalSeconds = 0;
			monitorDefinition.ServicePriority = 1;
			monitorDefinition.ScenarioDescription = "Validate Inference health is not impacted by issues due to disabled component";
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			string escalationMessage = Strings.InferenceComponentDisabledEscalationMessage;
			ResponderDefinition responderDefinition = InferenceDiscovery.CreateEscalateResponder(monitorDefinition, @bool, escalationMessage, null, ServiceHealthStatus.Unhealthy);
			responderDefinition.MinimumSecondsBetweenEscalates = 86400;
			responderDefinition.WaitIntervalSeconds = 86400;
			base.Broker.AddWorkDefinition<ResponderDefinition>(responderDefinition, base.TraceContext);
		}

		private void CreateInferenceActivityLogSyntheticContext()
		{
			bool @bool = this.attributeHelper.GetBool("InferenceActivityLogSyntheticProbeEnabled", true, true);
			int @int = this.attributeHelper.GetInt("InferenceActivityLogSyntheticProbeRecurrenceIntervalSeconds", true, 0, null, null);
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			foreach (MailboxDatabaseInfo mailboxDatabaseInfo in instance.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForBackend)
			{
				ProbeDefinition probeDefinition = InferenceActivityLogSyntheticProbe.CreateDefinition("InferenceActivityLogSyntheticProbe", @int, @bool);
				probeDefinition.TargetResource = mailboxDatabaseInfo.MailboxDatabaseName;
				string value = mailboxDatabaseInfo.MonitoringAccount + '@' + mailboxDatabaseInfo.MonitoringAccountDomain;
				probeDefinition.Attributes["MonitoringMailboxSmtpAddress"] = value;
				base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			}
		}

		public const string InferenceDataCollectionSuccessMonitorName = "InferenceDataCollectionSuccessMonitor";

		public const string InferenceDataCollectionProgressMonitorName = "InferenceDataCollectionProgressMonitor";

		private AttributeHelper attributeHelper;
	}
}
