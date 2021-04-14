using System;
using System.Threading;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.TimeBasedAssistants
{
	public sealed class TimeBasedAssistantsDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (LocalEndpointManager.Instance.ExchangeServerRoleEndpoint == null || !LocalEndpointManager.Instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.TimeBasedAssistantsTracer, base.TraceContext, "TimeBasedAssistantsDiscovery.DoWork: Mailbox role is not installed.No need to create TimeBasedAssistants related probes.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\TimeBasedAssistants\\TimeBasedAssistantsDiscovery.cs", 50);
				return;
			}
			if (DirectoryAccessor.Instance.IsRecoveryActionsEnabledOffline(TimeBasedAssistantsDiscovery.Server.Name))
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.TimeBasedAssistantsTracer, base.TraceContext, "TimeBasedAssistantsDiscovery.DoWork: Server is in maintenance mode.No need to create TimeBasedAssistants related probes.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\TimeBasedAssistants\\TimeBasedAssistantsDiscovery.cs", 62);
				return;
			}
			VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null);
			if (!snapshot.MailboxAssistants.TimeBasedAssistantsMonitoring.Enabled)
			{
				WTFDiagnostics.TraceInformation(ExTraceGlobals.TimeBasedAssistantsTracer, base.TraceContext, "TimeBasedAssistantsDiscovery.DoWork: Alerts are in training and should be running on SDF only (OM:1050471).No need to create TimeBasedAssistants related probes.", null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\TimeBasedAssistants\\TimeBasedAssistantsDiscovery.cs", 76);
				return;
			}
			this.CreateAssistantsOutOfSlaContext(TbaOutOfSlaAlertType.Urgent);
			this.CreateAssistantsOutOfSlaContext(TbaOutOfSlaAlertType.Scheduled);
			this.CreateAssistantsInfrastructureValidationContext();
			this.CreateAssistantsNotRunningToCompletionContext();
			this.CreateAssistantsActiveDatabaseContext();
		}

		private void CreateAssistantsOutOfSlaContext(TbaOutOfSlaAlertType alertType)
		{
			if (ExEnvironment.IsTest)
			{
				TimeBasedAssistantsOutOfSlaMonitor.ConfigureForTest();
			}
			this.CreateOutOfSlaContext(alertType);
		}

		private void CreateAssistantsInfrastructureValidationContext()
		{
			if (ExEnvironment.IsTest)
			{
				InfrastructureValidationDiscovery.ConfigureForTest();
			}
			this.CreateInfrastructureValidationContext();
		}

		private void CreateAssistantsNotRunningToCompletionContext()
		{
			if (ExEnvironment.IsTest)
			{
				TimeBasedAssistantsNotRunningToCompletionDiscovery.ConfigureForTest();
			}
			this.CreateNotRunningToCompletionContext();
		}

		private void CreateAssistantsActiveDatabaseContext()
		{
			if (ExEnvironment.IsTest)
			{
				TimeBasedAssistantsActiveDatabaseDiscovery.ConfigureForTest();
			}
			this.CreateActiveDatabaseContext();
		}

		private void CreateOutOfSlaContext(TbaOutOfSlaAlertType alertType)
		{
			ProbeDefinition probeDefinition = TimeBasedAssistantsOutOfSlaMonitor.CreateOutOfSlaProbeDefinition(base.TraceContext, alertType);
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition = TimeBasedAssistantsOutOfSlaMonitor.CreateOutOfSlaMonitorDefinition(probeDefinition.Name, base.TraceContext, alertType);
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderDefinition definition = TimeBasedAssistantsOutOfSlaMonitor.CreateOutOfSlaEscalateResponderDefinition(monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName(), alertType);
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
		}

		private void CreateInfrastructureValidationContext()
		{
			ProbeDefinition probeDefinition = InfrastructureValidationDiscovery.CreateInfrastructureValidationProbeDefinition(base.TraceContext);
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition = InfrastructureValidationDiscovery.CreateInfrastructureValidationMonitorDefinition(probeDefinition.Name, base.TraceContext);
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderDefinition definition = InfrastructureValidationDiscovery.CreateInfrastructureValidationEscalateResponderDefinition(monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName());
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
		}

		private void CreateNotRunningToCompletionContext()
		{
			ProbeDefinition probeDefinition = TimeBasedAssistantsNotRunningToCompletionDiscovery.CreateNotRunningToCompletionProbeDefinition(base.TraceContext);
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition = TimeBasedAssistantsNotRunningToCompletionDiscovery.CreateNotRunningToCompletionMonitorDefinition(probeDefinition.Name, base.TraceContext);
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderDefinition definition = TimeBasedAssistantsNotRunningToCompletionDiscovery.CreateNotRunningToCompletionEscalateResponderDefinition(monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName());
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
		}

		private void CreateActiveDatabaseContext()
		{
			ProbeDefinition probeDefinition = TimeBasedAssistantsActiveDatabaseDiscovery.CreateActiveDatabaseProbeDefinition(base.TraceContext);
			base.Broker.AddWorkDefinition<ProbeDefinition>(probeDefinition, base.TraceContext);
			MonitorDefinition monitorDefinition = TimeBasedAssistantsActiveDatabaseDiscovery.CreateActiveDatabaseMonitorDefinition(probeDefinition.Name, base.TraceContext);
			base.Broker.AddWorkDefinition<MonitorDefinition>(monitorDefinition, base.TraceContext);
			ResponderDefinition definition = TimeBasedAssistantsActiveDatabaseDiscovery.CreateActiveDatabaseEscalateResponderDefinition(monitorDefinition.Name, monitorDefinition.ConstructWorkItemResultName());
			base.Broker.AddWorkDefinition<ResponderDefinition>(definition, base.TraceContext);
		}

		private const string NoNeedToCreateProbesString = "No need to create TimeBasedAssistants related probes.";

		private static readonly Server Server = LocalServer.GetServer();
	}
}
