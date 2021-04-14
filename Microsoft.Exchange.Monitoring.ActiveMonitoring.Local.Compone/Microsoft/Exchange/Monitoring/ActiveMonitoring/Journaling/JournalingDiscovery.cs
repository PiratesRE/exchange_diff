using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Journaling
{
	public sealed class JournalingDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			LocalEndpointManager instance = LocalEndpointManager.Instance;
			if (instance.ExchangeServerRoleEndpoint == null || !instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
			{
				this.WriteTrace("JournalingDiscovery.DoWork: Skipping workitem generation since not on a mailbox server.");
				return;
			}
			this.CreateJournalingWorkItems();
		}

		private void CreateJournalingWorkItems()
		{
			this.AddWorkDefinition<MonitorDefinition>(JournalingWorkItem.CreateMonitorDefinition(base.Definition));
			this.AddWorkDefinition<ResponderDefinition>(JournalingWorkItem.CreateEscalateResponderDefinition(base.Definition));
			this.AddWorkDefinition<MonitorDefinition>(JournalFilterAgentWorkItem.CreateMonitorDefinition(base.Definition));
			this.AddWorkDefinition<ResponderDefinition>(JournalFilterAgentWorkItem.CreateEscalateResponderDefinition(base.Definition));
		}

		private void AddWorkDefinition<T>(T definition) where T : WorkDefinition
		{
			base.Broker.AddWorkDefinition<T>(definition, base.TraceContext);
		}

		private void WriteTrace(string message)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.JournalingTracer, base.TraceContext, message, null, "WriteTrace", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Journaling\\JournalingDiscovery.cs", 69);
		}
	}
}
