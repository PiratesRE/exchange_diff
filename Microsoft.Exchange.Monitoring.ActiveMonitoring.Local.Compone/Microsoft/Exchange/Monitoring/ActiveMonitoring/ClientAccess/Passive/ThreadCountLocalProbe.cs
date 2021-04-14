using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Common;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Web.Administration;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.ClientAccess.Passive
{
	public sealed class ThreadCountLocalProbe : ProbeWorkItem
	{
		private static void PopulateDefinition(ProbeDefinition probe, string assemblyPath, string typeName, string name, string serviceName, string targetResource, int recurrenceIntervalSeconds, int threshold)
		{
			probe.AssemblyPath = assemblyPath;
			probe.TypeName = typeName;
			probe.Name = name;
			probe.ServiceName = serviceName;
			probe.TargetResource = targetResource;
			probe.RecurrenceIntervalSeconds = recurrenceIntervalSeconds;
			probe.Attributes[ThreadCountLocalProbe.ThresholdPropertyKey] = threshold.ToString();
		}

		public override void PopulateDefinition<Definition>(Definition definition, Dictionary<string, string> propertyBag)
		{
			ProbeDefinition probe = definition as ProbeDefinition;
			int threshold = 0;
			int.TryParse(propertyBag[ThreadCountLocalProbe.ThresholdPropertyKey], out threshold);
			ThreadCountLocalProbe.PopulateDefinition(probe, ThreadCountLocalProbe.AssemblyPath, ThreadCountLocalProbe.ProbeTypeName, propertyBag[ThreadCountLocalProbe.NamePropertyKey], propertyBag[ThreadCountLocalProbe.ServiceNamePropertyKey], propertyBag[ThreadCountLocalProbe.TargetResourcePropertyKey], 0, threshold);
		}

		public static ProbeDefinition CreateDefinition(ProbeIdentity probeIdentity, int threshold)
		{
			ProbeDefinition probeDefinition = new ProbeDefinition();
			ThreadCountLocalProbe.PopulateDefinition(probeDefinition, ThreadCountLocalProbe.AssemblyPath, ThreadCountLocalProbe.ProbeTypeName, probeIdentity.Name, probeIdentity.Component.Name, probeIdentity.TargetResource, 60, threshold);
			return probeDefinition;
		}

		protected override void DoWork(CancellationToken cancellationToken)
		{
			Breadcrumbs breadcrumbs = new Breadcrumbs(1024, base.TraceContext);
			string targetResource = base.Definition.TargetResource;
			int num = this.ReadAttribute(ThreadCountLocalProbe.ThresholdPropertyKey, 0);
			breadcrumbs.Drop("Starting probe. Threshold for threads/requests is {0} each.", new object[]
			{
				num
			});
			try
			{
				int num2;
				int num3;
				if (ThreadCountLocalProbe.TryGetAppPoolThreadRequestCount(targetResource, breadcrumbs, out num2, out num3))
				{
					base.Result.SampleValue = (double)num2;
					base.Result.StateAttribute2 = num3.ToString();
					if (num2 > num)
					{
						throw new ApplicationException(string.Format("FAILED: Running/ready thread count ({0}) for the app pool is above the threshold of {1}. Currently {2} request(s) are live.", num2, num, num3));
					}
				}
			}
			finally
			{
				base.Result.StateAttribute1 = breadcrumbs.ToString();
			}
		}

		private static bool TryGetAppPoolThreadRequestCount(string appPoolName, Breadcrumbs breadcrumbs, out int threadCount, out int requestCount)
		{
			if (string.IsNullOrEmpty(appPoolName))
			{
				throw new ArgumentNullException("appPoolName");
			}
			threadCount = (requestCount = -1);
			breadcrumbs.Drop("Calling IIS server manager for app pool worker processes");
			WorkerProcessCollection workerProcessCollection;
			using (ServerManager serverManager = new ServerManager())
			{
				workerProcessCollection = (from appPool in serverManager.ApplicationPools
				where appPool.Name == appPoolName
				select appPool.WorkerProcesses).FirstOrDefault<WorkerProcessCollection>();
			}
			if (workerProcessCollection == null || workerProcessCollection.Count == 0)
			{
				breadcrumbs.Drop("Couldn't get worker processes for appPool {0}. Check if its w3wp.exe is running.", new object[]
				{
					appPoolName
				});
				return false;
			}
			if (workerProcessCollection.Count > 1)
			{
				breadcrumbs.Drop("Found {0} worker process for this app pool. Only 1 expected.", new object[]
				{
					workerProcessCollection.Count
				});
				return false;
			}
			int num = -1;
			string empty = string.Empty;
			bool result;
			try
			{
				WorkerProcess workerProcess = workerProcessCollection.FirstOrDefault<WorkerProcess>();
				num = workerProcess.ProcessId;
				Process processById = Process.GetProcessById(workerProcess.ProcessId);
				breadcrumbs.Drop("{0} ProcessId={1}", new object[]
				{
					processById.ProcessName,
					num
				});
				RequestCollection requests = workerProcess.GetRequests(0);
				Dictionary<PipelineState, int> collection = ThreadCountLocalProbe.CountDisinctStates<Request, PipelineState>(requests, (Request r) => r.PipelineState);
				requestCount = ThreadCountLocalProbe.CountValidStatesAndCreateSnapshot<PipelineState>(collection, (PipelineState o) => true, "RequestStates", out empty);
				breadcrumbs.Drop(empty);
				IEnumerable<ProcessThread> objects = processById.Threads.OfType<ProcessThread>();
				Dictionary<System.Diagnostics.ThreadState, int> collection2 = ThreadCountLocalProbe.CountDisinctStates<ProcessThread, System.Diagnostics.ThreadState>(objects, (ProcessThread t) => t.ThreadState);
				threadCount = ThreadCountLocalProbe.CountValidStatesAndCreateSnapshot<System.Diagnostics.ThreadState>(collection2, new Func<System.Diagnostics.ThreadState, bool>(ThreadCountLocalProbe.IsThreadStateRunning), "ThreadStates", out empty);
				breadcrumbs.Drop(empty);
				result = true;
			}
			catch (ArgumentException ex)
			{
				if (!ex.Message.Contains(ThreadCountLocalProbe.ProcessNotRunningKeywords))
				{
					throw;
				}
				breadcrumbs.Drop("Process {0} is not running.", new object[]
				{
					num
				});
				result = false;
			}
			return result;
		}

		private static int CountValidStatesAndCreateSnapshot<TKey>(Dictionary<TKey, int> collection, Func<TKey, bool> isValidDelegate, string title, out string snapshot)
		{
			StringBuilder stringBuilder = new StringBuilder(512);
			stringBuilder.Append(title + ":");
			int num = 0;
			foreach (TKey tkey in collection.Keys)
			{
				int num2 = collection[tkey];
				if (isValidDelegate(tkey))
				{
					num += num2;
				}
				stringBuilder.AppendFormat(" {0}={1}", tkey.ToString(), num2);
			}
			snapshot = stringBuilder.ToString();
			return num;
		}

		private static Dictionary<TGroup, int> CountDisinctStates<TObj, TGroup>(IEnumerable<TObj> objects, Func<TObj, TGroup> convertDelegate)
		{
			Dictionary<TGroup, int> dictionary = new Dictionary<TGroup, int>(10);
			foreach (TObj arg in objects)
			{
				TGroup tgroup = convertDelegate(arg);
				if (!dictionary.ContainsKey(tgroup))
				{
					dictionary[tgroup] = 0;
				}
				Dictionary<TGroup, int> dictionary2;
				TGroup key;
				(dictionary2 = dictionary)[key = tgroup] = dictionary2[key] + 1;
			}
			return dictionary;
		}

		private static bool IsThreadStateRunning(System.Diagnostics.ThreadState state)
		{
			switch (state)
			{
			case System.Diagnostics.ThreadState.Initialized:
			case System.Diagnostics.ThreadState.Terminated:
			case System.Diagnostics.ThreadState.Wait:
			case System.Diagnostics.ThreadState.Transition:
				return false;
			case System.Diagnostics.ThreadState.Ready:
			case System.Diagnostics.ThreadState.Running:
			case System.Diagnostics.ThreadState.Standby:
			case System.Diagnostics.ThreadState.Unknown:
				return true;
			default:
				return false;
			}
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string ProbeTypeName = typeof(ThreadCountLocalProbe).FullName;

		private static readonly string NamePropertyKey = "Name";

		private static readonly string ServiceNamePropertyKey = "ServiceName";

		private static readonly string ThresholdPropertyKey = "Threshold";

		private static readonly string TargetResourcePropertyKey = "TargetResource";

		private static readonly string ProcessNotRunningKeywords = "is not running";
	}
}
