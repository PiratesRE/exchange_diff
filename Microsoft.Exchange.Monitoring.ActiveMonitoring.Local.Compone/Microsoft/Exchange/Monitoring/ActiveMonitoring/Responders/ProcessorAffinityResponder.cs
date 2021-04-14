using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.Search;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Web.Administration;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders
{
	public class ProcessorAffinityResponder : ResponderWorkItem
	{
		public static ResponderDefinition CreateDefinition(string name, string serviceName, string alertMask, string targetResource, ServiceHealthStatus targetHealthState, int processorAffinityCount, int avoidProcessorCount = 0)
		{
			if (targetHealthState == ServiceHealthStatus.None)
			{
				throw new ArgumentException("The responder does not support ServiceHealthStatus.None as target health state.", "targetHealthState");
			}
			if (string.IsNullOrWhiteSpace(targetResource))
			{
				throw new ArgumentException("Invalid target resource.", "targetResource");
			}
			if (processorAffinityCount < 1)
			{
				throw new ArgumentException("processorAffinityCount must be greater than 0.", "processorAffinityCount");
			}
			if (avoidProcessorCount < 0)
			{
				throw new ArgumentException("avoidProcessorCount must be greater than or equal to 0.", "avoidProcessorCount");
			}
			ResponderDefinition responderDefinition = new ResponderDefinition();
			responderDefinition.AssemblyPath = ProcessorAffinityResponder.AssemblyPath;
			responderDefinition.TypeName = ProcessorAffinityResponder.TypeName;
			responderDefinition.Name = name;
			responderDefinition.ServiceName = serviceName;
			responderDefinition.AlertMask = alertMask;
			responderDefinition.TargetResource = targetResource;
			responderDefinition.TargetHealthState = targetHealthState;
			responderDefinition.Attributes["ProcessorAffinityCount"] = processorAffinityCount.ToString();
			responderDefinition.Attributes["AvoidProcessorCount"] = avoidProcessorCount.ToString();
			responderDefinition.AlertTypeId = "*";
			responderDefinition.RecurrenceIntervalSeconds = 300;
			responderDefinition.WaitIntervalSeconds = 300;
			responderDefinition.TimeoutSeconds = 300;
			responderDefinition.MaxRetryAttempts = 3;
			return responderDefinition;
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			string targetResource = base.Definition.TargetResource;
			if (string.IsNullOrWhiteSpace(targetResource))
			{
				throw new InvalidOperationException("Target resource cannot be empty.");
			}
			AttributeHelper attributeHelper = new AttributeHelper(base.Definition);
			int @int = attributeHelper.GetInt("ProcessorAffinityCount", true, 0, new int?(1), null);
			int int2 = attributeHelper.GetInt("AvoidProcessorCount", false, 0, new int?(0), null);
			int processorCount = Environment.ProcessorCount;
			if (@int >= processorCount)
			{
				throw new InvalidOperationException(string.Format("ProcessorAffinityResponder cannot set processor affinity count to {0} on server with {1} processors.", @int, processorCount));
			}
			Process[] processesFromName = this.GetProcessesFromName(targetResource);
			if (processesFromName == null || processesFromName.Length == 0)
			{
				throw new InvalidOperationException(string.Format("ProcessorAffinityResponder is not able to get process with name '{0}'.", targetResource));
			}
			foreach (Process process in processesFromName)
			{
				using (process)
				{
					this.SetProcessorAffinity(process, @int, processorCount, int2);
				}
			}
		}

		private Process[] GetProcessesFromName(string processName)
		{
			if (processName.StartsWith("noderunner#", StringComparison.OrdinalIgnoreCase))
			{
				return this.GetProcessesForNodeRunner(processName);
			}
			Process[] processesByName = Process.GetProcessesByName(processName);
			if (processesByName != null && processesByName.Length > 0)
			{
				return processesByName;
			}
			if (processName.EndsWith("apppool", StringComparison.OrdinalIgnoreCase))
			{
				return this.GetProcessesForAppPool(processName);
			}
			return null;
		}

		private Process[] GetProcessesForNodeRunner(string nodeRunnerInstanceName)
		{
			Dictionary<string, int> nodeProcessIds = SearchMonitoringHelper.GetNodeProcessIds();
			foreach (string text in nodeProcessIds.Keys)
			{
				if (nodeRunnerInstanceName.EndsWith(text, StringComparison.OrdinalIgnoreCase))
				{
					return new Process[]
					{
						Process.GetProcessById(nodeProcessIds[text])
					};
				}
			}
			return null;
		}

		private Process[] GetProcessesForAppPool(string appPoolName)
		{
			Process[] result;
			using (ServerManager serverManager = new ServerManager())
			{
				ApplicationPoolCollection applicationPools = serverManager.ApplicationPools;
				foreach (ApplicationPool applicationPool in applicationPools)
				{
					if (appPoolName.Equals(applicationPool.Name, StringComparison.OrdinalIgnoreCase))
					{
						WorkerProcessCollection workerProcesses = applicationPool.WorkerProcesses;
						List<Process> list = new List<Process>();
						foreach (WorkerProcess workerProcess in workerProcesses)
						{
							if (workerProcess.State == 1)
							{
								Process processById = Process.GetProcessById(workerProcess.ProcessId);
								if (processById != null)
								{
									list.Add(processById);
								}
							}
						}
						return list.ToArray();
					}
				}
				result = null;
			}
			return result;
		}

		private void SetProcessorAffinity(Process process, int affinityCount, int totalProcessorCount, int avoidProcessorCount)
		{
			ulong num = 0UL;
			Random random = new Random();
			int num2 = 0;
			if (avoidProcessorCount > totalProcessorCount - affinityCount)
			{
				avoidProcessorCount = totalProcessorCount - affinityCount;
			}
			for (int i = 0; i < totalProcessorCount; i++)
			{
				if (i >= avoidProcessorCount)
				{
					if (random.Next(totalProcessorCount - i) < affinityCount - num2)
					{
						num |= 1UL << i;
						num2++;
					}
					if (num2 == affinityCount)
					{
						break;
					}
				}
			}
			base.Result.StateAttribute1 = num.ToString();
			process.ProcessorAffinity = (IntPtr)((long)num);
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(ProcessorAffinityResponder).FullName;

		internal static class AttributeNames
		{
			internal const string ProcessorAffinityCount = "ProcessorAffinityCount";

			internal const string AvoidProcessorCount = "AvoidProcessorCount";
		}
	}
}
