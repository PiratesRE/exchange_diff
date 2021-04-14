using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.MailboxAssistants.Assistants.RecipientDLExpansion;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.DLExpansion
{
	public sealed class DLExpansionDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (instance.ExchangeServerRoleEndpoint == null || !instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
			{
				this.WriteTrace("DLExpansionDiscovery.DoWork: Skipping workitem generation since not on a mailbox server.");
				return;
			}
			this.CreateDLExpansionSuccessWorkItems();
		}

		private void CreateDLExpansionSuccessWorkItems()
		{
			this.AddWorkDefinition<MonitorDefinition>(DLExpansionSuccessWorkItem.CreateMonitorDefinition(base.Definition));
			this.AddWorkDefinition<ResponderDefinition>(DLExpansionSuccessWorkItem.CreateEscalateResponderDefinition(base.Definition));
		}

		private void AddWorkDefinition<T>(T definition) where T : WorkDefinition
		{
			base.Broker.AddWorkDefinition<T>(definition, base.TraceContext);
		}

		private void WriteTrace(string message)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.RecipientDLExpansionEventBasedAssistantTracer, base.TraceContext, message, null, "WriteTrace", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\DLExpansion\\DLExpansionDiscovery.cs", 64);
		}
	}
}
