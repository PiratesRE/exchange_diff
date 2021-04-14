using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.SiteMailbox
{
	public sealed class SiteMailboxDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			try
			{
				LocalEndpointManager instance = LocalEndpointManager.Instance;
				if (instance.ExchangeServerRoleEndpoint == null || !instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
				{
					this.WriteTrace("SiteMailboxDiscovery.DoWork: Skipping workitem generation since not on a mailbox server.");
					return;
				}
			}
			catch (EndpointManagerEndpointUninitializedException)
			{
				this.WriteTrace("SiteMailboxDiscovery.DoWork(): Skipping due to EndpointManagerEndpointUninitializedException. LAM will retry.");
				return;
			}
			this.CreateServiceHostServiceAvailabilityWorkItems();
			this.CreateServiceHostProcessCrashDetectionContext();
			this.CreateDocumentSyncSuccessWorkItems();
		}

		private void CreateServiceHostServiceAvailabilityWorkItems()
		{
			this.AddWorkDefinition<ProbeDefinition>(ServiceHostAvailabilityWorkItem.CreateProbeDefinition(base.Definition));
			this.AddWorkDefinition<MonitorDefinition>(ServiceHostAvailabilityWorkItem.CreateMonitorDefinition(base.Definition));
			this.AddWorkDefinition<ResponderDefinition>(ServiceHostAvailabilityWorkItem.CreateRecoveryResponderDefinition());
			this.AddWorkDefinition<ResponderDefinition>(ServiceHostAvailabilityWorkItem.CreateRecovery2ResponderDefinition());
			this.AddWorkDefinition<ResponderDefinition>(ServiceHostAvailabilityWorkItem.CreateEscalateResponderDefinition());
		}

		private void CreateServiceHostProcessCrashDetectionContext()
		{
			this.AddWorkDefinition<ProbeDefinition>(ServiceHostProcessCrashDetectionWorkItem.CreateProbeDefinition(base.Definition));
			this.AddWorkDefinition<MonitorDefinition>(ServiceHostProcessCrashDetectionWorkItem.CreateMonitorDefinition(base.Definition));
			this.AddWorkDefinition<ResponderDefinition>(ServiceHostProcessCrashDetectionWorkItem.CreateEscalateResponderDefinition(base.Definition));
		}

		private void CreateDocumentSyncSuccessWorkItems()
		{
			this.AddWorkDefinition<MonitorDefinition>(SiteMailboxSyncSuccessWorkItem.CreateMonitorDefinition(base.Definition));
			this.AddWorkDefinition<ResponderDefinition>(SiteMailboxSyncSuccessWorkItem.CreateEscalateResponderDefinition(base.Definition));
		}

		private void AddWorkDefinition<T>(T definition) where T : WorkDefinition
		{
			base.Broker.AddWorkDefinition<T>(definition, base.TraceContext);
		}

		private void WriteTrace(string message)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.SiteMailboxTracer, base.TraceContext, message, null, "WriteTrace", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\SiteMailbox\\SiteMailboxDiscovery.cs", 96);
		}
	}
}
