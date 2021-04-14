using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ActiveMonitoring
{
	public sealed class KeepAliveMaintenance : MaintenanceWorkItem
	{
		public static MaintenanceDefinition CreateDefinition()
		{
			return new MaintenanceDefinition
			{
				AssemblyPath = Assembly.GetExecutingAssembly().Location,
				TypeName = typeof(KeepAliveMaintenance).FullName,
				Name = typeof(KeepAliveMaintenance).Name,
				ServiceName = ExchangeComponent.Monitoring.Name,
				RecurrenceIntervalSeconds = 300,
				TimeoutSeconds = 150,
				Enabled = true
			};
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			if (LocalEndpointManager.Instance.MonitoringEndpoint == null || !LocalEndpointManager.Instance.MonitoringEndpoint.IsOnline)
			{
				base.Result.StateAttribute4 = "Skip heartbeat because server monitoring state is off";
				return;
			}
			string[] allDatabaseGuids = MonitoringServerManager.GetAllDatabaseGuids();
			if (allDatabaseGuids == null || allDatabaseGuids.Length == 0)
			{
				return;
			}
			List<Task> list = new List<Task>();
			ConcurrentQueue<string> successList = new ConcurrentQueue<string>();
			ConcurrentQueue<Guid> invalidList = new ConcurrentQueue<Guid>();
			ConcurrentQueue<string> failedList = new ConcurrentQueue<string>();
			ConcurrentQueue<string> removedList = new ConcurrentQueue<string>();
			foreach (string state in allDatabaseGuids)
			{
				Task item = Task.Factory.StartNew(delegate(object guid)
				{
					Guid guid2;
					if (!Guid.TryParse((string)guid, out guid2))
					{
						WTFDiagnostics.TraceDebug<object>(ExTraceGlobals.CommonComponentsTracer, this.TraceContext, "Ignore invalid database guid: {0}", guid, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\KeepAliveMaintenance.cs", 101);
						invalidList.Enqueue(guid2);
						return;
					}
					string databaseActiveHost = DirectoryAccessor.Instance.GetDatabaseActiveHost(guid2);
					if (string.IsNullOrWhiteSpace(databaseActiveHost))
					{
						WTFDiagnostics.TraceDebug<object>(ExTraceGlobals.CommonComponentsTracer, this.TraceContext, "Ignore invalid database entry as host server is not found: {0}", guid, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\KeepAliveMaintenance.cs", 109);
						invalidList.Enqueue(guid2);
						return;
					}
					string item2 = string.Format("{0}:{1}", guid2.ToString().ToLower(), databaseActiveHost);
					try
					{
						WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.CommonComponentsTracer, this.TraceContext, "Sending Heartbeat to {0}", databaseActiveHost, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\KeepAliveMaintenance.cs", 117);
						using (ActiveMonitoringRpcClient activeMonitoringRpcClient = new ActiveMonitoringRpcClient(databaseActiveHost))
						{
							activeMonitoringRpcClient.Heartbeat(guid2);
						}
						successList.Enqueue(item2);
					}
					catch (RpcException ex)
					{
						WTFDiagnostics.TraceDebug<string, RpcException>(ExTraceGlobals.CommonComponentsTracer, this.TraceContext, "Heartbeat to {0} failed with RPC error:\n{1}", databaseActiveHost, ex, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\KeepAliveMaintenance.cs", 129);
						failedList.Enqueue(item2);
						if (ex.ErrorCode == -2147417342)
						{
							WTFDiagnostics.TraceDebug<object>(ExTraceGlobals.CommonComponentsTracer, this.TraceContext, "This server is no longer the owner of database {0}, removing", guid, null, "DoWork", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\HM\\KeepAliveMaintenance.cs", 135);
							MonitoringServerManager.RemoveDatabase(guid2);
							removedList.Enqueue(item2);
						}
					}
				}, state);
				list.Add(item);
			}
			try
			{
				Task.WaitAll(list.ToArray());
			}
			finally
			{
				base.Result.StateAttribute1 = "Heartbeat success for " + string.Join(",", successList);
				base.Result.StateAttribute2 = "Heartbeat skipped for non-existant databases: " + string.Join<Guid>(",", invalidList);
				base.Result.StateAttribute3 = "Heartbeat failed for " + string.Join(",", failedList);
				base.Result.StateAttribute4 = "MDB removed because not owner " + string.Join(",", removedList);
			}
		}
	}
}
