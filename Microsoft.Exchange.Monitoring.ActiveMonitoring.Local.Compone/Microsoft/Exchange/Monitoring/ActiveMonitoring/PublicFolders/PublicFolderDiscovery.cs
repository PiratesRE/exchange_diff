using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Local;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.PublicFolders
{
	public sealed class PublicFolderDiscovery : MaintenanceWorkItem
	{
		protected override void DoWork(CancellationToken cancellationToken)
		{
			LocalEndpointManager instance;
			try
			{
				instance = LocalEndpointManager.Instance;
				if (instance.ExchangeServerRoleEndpoint == null || !instance.ExchangeServerRoleEndpoint.IsMailboxRoleInstalled)
				{
					this.WriteTrace("PublicFolderDiscovery.DoWork: Skipping workitem generation since not on a mailbox server.");
					return;
				}
			}
			catch (EndpointManagerEndpointUninitializedException)
			{
				this.WriteTrace("PublicFolderDiscovery.DoWork(): Skipping due to EndpointManagerEndpointUninitializedException. LAM will retry.");
				return;
			}
			this.CreatePFMailboxConnectionCountWorkItems();
			this.CreatePublicFolderSyncSuccessWorkItems();
			this.CreatePublicFolderQuotaWorkItems();
			this.CreatePublicFolderMoveJobWorkItems();
			if (LocalEndpointManager.IsDataCenter)
			{
				this.CreatePFEWSLogonWorkItems(instance);
			}
		}

		private void CreatePublicFolderSyncSuccessWorkItems()
		{
			foreach (MonitorDefinition definition in PublicFolderSyncSuccessWorkItem.GenerateMonitorDefinitions(base.Definition))
			{
				this.AddWorkDefinition<MonitorDefinition>(definition);
			}
			foreach (ResponderDefinition definition2 in PublicFolderSyncSuccessWorkItem.GenerateResponderDefinitions(base.Definition))
			{
				this.AddWorkDefinition<ResponderDefinition>(definition2);
			}
		}

		private void CreatePFMailboxConnectionCountWorkItems()
		{
			foreach (MonitorDefinition definition in PFMailboxConnectionWorkItem.GenerateMonitorDefinitions(base.Definition))
			{
				this.AddWorkDefinition<MonitorDefinition>(definition);
			}
			foreach (ResponderDefinition definition2 in PFMailboxConnectionWorkItem.GenerateResponderDefinitions(base.Definition))
			{
				this.AddWorkDefinition<ResponderDefinition>(definition2);
			}
		}

		private void CreatePublicFolderQuotaWorkItems()
		{
			foreach (MonitorDefinition definition in PFMailboxQuotaWorkItem.GenerateMonitorDefinitions(base.Definition))
			{
				this.AddWorkDefinition<MonitorDefinition>(definition);
			}
			foreach (ResponderDefinition definition2 in PFMailboxQuotaWorkItem.GenerateResponderDefinitions(base.Definition))
			{
				this.AddWorkDefinition<ResponderDefinition>(definition2);
			}
		}

		private void CreatePublicFolderMoveJobWorkItems()
		{
			foreach (MonitorDefinition definition in PublicFolderMoveJobStuckWorkItem.GenerateMonitorDefinitions(base.Definition))
			{
				this.AddWorkDefinition<MonitorDefinition>(definition);
			}
			foreach (ResponderDefinition definition2 in PublicFolderMoveJobStuckWorkItem.GenerateResponderDefinitions(base.Definition))
			{
				this.AddWorkDefinition<ResponderDefinition>(definition2);
			}
		}

		private void CreatePFEWSLogonWorkItems(LocalEndpointManager endpointManager)
		{
			try
			{
				if (endpointManager.MailboxDatabaseEndpoint == null)
				{
					this.WriteTrace("PublicFolderDiscovery.CreatePFEWSLogonWorkItems(): Skipping due to MailboxDatabaseEndpoint uninitialized. LAM will retry.");
					return;
				}
			}
			catch (EndpointManagerEndpointUninitializedException)
			{
				this.WriteTrace("PublicFolderDiscovery.CreatePFEWSLogonWorkItems(): Skipping due to EndpointManagerEndpointUninitializedException. LAM will retry.");
				return;
			}
			ICollection<MailboxDatabaseInfo> mailboxDatabaseInfoCollectionForCafe = endpointManager.MailboxDatabaseEndpoint.MailboxDatabaseInfoCollectionForCafe;
			HashSet<string> hashSet = new HashSet<string>();
			foreach (MailboxDatabaseInfo mailboxDatabaseInfo in mailboxDatabaseInfoCollectionForCafe)
			{
				string text = DirectoryAccessor.Instance.GetServerFqdnForDatabase(mailboxDatabaseInfo.MailboxDatabaseGuid);
				text = ((text != null) ? text : Environment.MachineName);
				if (!hashSet.Contains(text))
				{
					WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.PublicFoldersTracer, base.TraceContext, "Creating {0} probe for local server {1} ...", PublicFolderLocalEWSLogon.Name, text, null, "CreatePFEWSLogonWorkItems", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderDiscovery.cs", 161);
					hashSet.Add(text);
					string account = mailboxDatabaseInfo.MonitoringAccount + "@" + mailboxDatabaseInfo.MonitoringAccountDomain;
					ProbeDefinition probeDefinition = PublicFolderLocalEWSLogon.CreateProbeDefinition(base.Definition, text, account, mailboxDatabaseInfo.MonitoringAccountPassword);
					this.AddWorkDefinition<ProbeDefinition>(probeDefinition);
					WTFDiagnostics.TraceInformation(ExTraceGlobals.PublicFoldersTracer, base.TraceContext, "configuring probe " + probeDefinition.Name, null, "CreatePFEWSLogonWorkItems", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderDiscovery.cs", 171);
					MonitorDefinition monitorDefinition = PublicFolderLocalEWSLogon.CreateMonitorDefinition(base.Definition, text, probeDefinition.ConstructWorkItemResultName());
					this.AddWorkDefinition<MonitorDefinition>(monitorDefinition);
					WTFDiagnostics.TraceInformation(ExTraceGlobals.PublicFoldersTracer, base.TraceContext, "configuring monitor " + monitorDefinition.Name, null, "CreatePFEWSLogonWorkItems", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderDiscovery.cs", 175);
					ResponderDefinition responderDefinition = PublicFolderLocalEWSLogon.CreateEscalateResponderDefinition(base.Definition, text, monitorDefinition.ConstructWorkItemResultName());
					this.AddWorkDefinition<ResponderDefinition>(responderDefinition);
					WTFDiagnostics.TraceInformation(ExTraceGlobals.PublicFoldersTracer, base.TraceContext, "configuring escalate responder " + responderDefinition.Name, null, "CreatePFEWSLogonWorkItems", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderDiscovery.cs", 179);
				}
				else
				{
					WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.PublicFoldersTracer, base.TraceContext, "Probe {0} already exists on server {1}. Skipping ...", PublicFolderLocalEWSLogon.Name, text, null, "CreatePFEWSLogonWorkItems", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderDiscovery.cs", 183);
				}
			}
		}

		private void AddWorkDefinition<T>(T definition) where T : WorkDefinition
		{
			base.Broker.AddWorkDefinition<T>(definition, base.TraceContext);
		}

		private void WriteTrace(string message)
		{
			WTFDiagnostics.TraceInformation(ExTraceGlobals.PublicFoldersTracer, base.TraceContext, message, null, "WriteTrace", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\PublicFolders\\PublicFolderDiscovery.cs", 207);
		}
	}
}
