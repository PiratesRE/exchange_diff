using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	internal static class VipProbeUtils
	{
		public static ProbeDefinition CreateVipProbeDefinition(MaintenanceDefinition maintenanceDefinition, Type type, string probeName)
		{
			return new ProbeDefinition
			{
				TypeName = type.FullName,
				Name = probeName,
				RecurrenceIntervalSeconds = 0,
				AssemblyPath = type.Assembly.Location,
				TimeoutSeconds = 500,
				WorkItemVersion = maintenanceDefinition.WorkItemVersion,
				ServiceName = maintenanceDefinition.ServiceName,
				DeploymentId = maintenanceDefinition.DeploymentId,
				ExecutionLocation = maintenanceDefinition.ExecutionLocation,
				Account = "",
				AccountPassword = "",
				AccountDisplayName = "",
				Endpoint = "",
				CreatedById = maintenanceDefinition.Id,
				Enabled = false
			};
		}

		public static int AddVipProbeWorkDefinition(ProbeDefinition probeDefinition, IMaintenanceWorkBroker broker, TracingContext traceContext, Trace tracer)
		{
			VipProbeUtils.<>c__DisplayClass5 CS$<>8__locals1 = new VipProbeUtils.<>c__DisplayClass5();
			CS$<>8__locals1.probeDefinition = probeDefinition;
			CS$<>8__locals1.broker = broker;
			CS$<>8__locals1.traceContext = traceContext;
			CS$<>8__locals1.tracer = tracer;
			CS$<>8__locals1.vipWorkItemId = -1;
			using (EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset))
			{
				Task task = CS$<>8__locals1.broker.AddWorkDefinition<ProbeDefinition>(CS$<>8__locals1.probeDefinition, CS$<>8__locals1.traceContext);
				task.ContinueWith(delegate(Task t)
				{
					Task task2 = CS$<>8__locals1.broker.GetProbeDefinition(CS$<>8__locals1.probeDefinition.TypeName).ExecuteAsync(delegate(ProbeDefinition pd)
					{
						WTFDiagnostics.TraceInformation<int>(CS$<>8__locals1.tracer, CS$<>8__locals1.traceContext, "VIP definition queried succeesfully. Definition ID: {0}", pd.Id, null, "AddVipProbeWorkDefinition", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Utils\\VipProbeUtils.cs", 80);
						CS$<>8__locals1.vipWorkItemId = pd.Id;
					}, CancellationToken.None, CS$<>8__locals1.traceContext);
					task2.ContinueWith(delegate(Task t2)
					{
						waitHandle.Set();
					}, TaskContinuationOptions.OnlyOnRanToCompletion);
					task2.ContinueWith(delegate(Task t2)
					{
						WTFDiagnostics.TraceInformation<AggregateException>(CS$<>8__locals1.tracer, CS$<>8__locals1.traceContext, "Query for VIP definition failed with the following exception: {0}", t2.Exception, null, "AddVipProbeWorkDefinition", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Utils\\VipProbeUtils.cs", 89);
						CS$<>8__locals1.vipWorkItemId = -1;
						waitHandle.Set();
					}, TaskContinuationOptions.NotOnRanToCompletion);
				}, TaskContinuationOptions.OnlyOnRanToCompletion);
				task.ContinueWith(delegate(Task t)
				{
					WTFDiagnostics.TraceInformation<AggregateException>(CS$<>8__locals1.tracer, CS$<>8__locals1.traceContext, "Insert of the VIP definition failed with the following exception: {0}", t.Exception, null, "AddVipProbeWorkDefinition", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Utils\\VipProbeUtils.cs", 98);
					CS$<>8__locals1.vipWorkItemId = -1;
					waitHandle.Set();
				}, TaskContinuationOptions.NotOnRanToCompletion);
				waitHandle.WaitOne();
			}
			return CS$<>8__locals1.vipWorkItemId;
		}

		public static void PublishResult(ProbeResult result, HashSet<string> publishedVips, IProbeWorkBroker broker)
		{
			if (result == null)
			{
				return;
			}
			if (!publishedVips.Contains(result.ExecutionContext))
			{
				broker.PublishResult(result);
				publishedVips.Add(result.ExecutionContext);
			}
		}
	}
}
