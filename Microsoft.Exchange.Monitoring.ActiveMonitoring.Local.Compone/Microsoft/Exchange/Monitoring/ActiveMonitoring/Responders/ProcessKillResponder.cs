using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Audit;
using Microsoft.Exchange.Monitoring.ActiveMonitoring.ProcessIsolation;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Responders
{
	public class ProcessKillResponder : ResponderWorkItem
	{
		internal string ProcessName { get; set; }

		internal HashSet<int> ProcessIds { get; set; }

		internal bool IsMasterAndWorker { get; set; }

		internal ProcessKillMode MasterAndWorkerKillMode { get; set; }

		internal static ResponderDefinition CreateDefinition(string responderName, string monitorName, string processName, ServiceHealthStatus responderTargetState, bool isMasterWorker = false, string serviceName = "Exchange", bool enabled = true, int timeoutSeconds = 60, string throttleGroupName = null, string sampleMask = null)
		{
			ResponderDefinition responderDefinition = new ResponderDefinition();
			responderDefinition.AssemblyPath = ProcessKillResponder.AssemblyPath;
			responderDefinition.TypeName = ProcessKillResponder.TypeName;
			responderDefinition.Name = responderName;
			responderDefinition.ServiceName = processName;
			responderDefinition.AlertTypeId = "*";
			responderDefinition.AlertMask = monitorName;
			responderDefinition.RecurrenceIntervalSeconds = 300;
			responderDefinition.MaxRetryAttempts = 10;
			responderDefinition.TargetHealthState = responderTargetState;
			responderDefinition.WaitIntervalSeconds = 10;
			responderDefinition.Enabled = enabled;
			responderDefinition.TimeoutSeconds = timeoutSeconds;
			responderDefinition.Attributes["ProcessName"] = processName;
			responderDefinition.Attributes["IsMasterAndWorker"] = isMasterWorker.ToString();
			responderDefinition.Attributes["SampleMask"] = sampleMask.ToString();
			RecoveryActionRunner.SetThrottleProperties(responderDefinition, throttleGroupName, RecoveryActionId.KillProcess, processName, null);
			return responderDefinition;
		}

		protected void InitializeServiceAttributes(AttributeHelper attributeHelper)
		{
			this.ProcessName = attributeHelper.GetString("ProcessName", true, null);
			string @string = attributeHelper.GetString("SampleMask", false, null);
			ProbeResult probeResult = (from r in base.Broker.GetProbeResults(@string, base.Result.ExecutionStartTime.AddHours(-1.0), base.Result.ExecutionStartTime)
			where r.ResultType == ResultType.Failed
			orderby r.ExecutionEndTime
			select r).FirstOrDefault<ProbeResult>();
			this.ProcessIds = new HashSet<int>();
			string pidsFromErrorText = ProcessIsolationDiscovery.GetPidsFromErrorText(probeResult.Error);
			if (!string.IsNullOrEmpty(pidsFromErrorText))
			{
				string[] array = pidsFromErrorText.Split(new char[]
				{
					','
				}, StringSplitOptions.RemoveEmptyEntries);
				foreach (string s in array)
				{
					int item;
					if (int.TryParse(s, out item))
					{
						this.ProcessIds.Add(item);
					}
				}
			}
			this.IsMasterAndWorker = attributeHelper.GetBool("IsMasterAndWorker", false, false);
			this.MasterAndWorkerKillMode = attributeHelper.GetEnum<ProcessKillMode>("MasterAndWorkerKillMode", false, this.IsMasterAndWorker ? ProcessKillMode.SelfAndChildren : ProcessKillMode.SelfOnly);
		}

		protected virtual void InitializeAttributes()
		{
			AttributeHelper attributeHelper = new AttributeHelper(base.Definition);
			this.InitializeServiceAttributes(attributeHelper);
		}

		protected void InternalKillProcess(RecoveryActionEntry startEntry, CancellationToken cancellationToken)
		{
			Privilege.RunWithPrivilege("SeDebugPrivilege", true, delegate
			{
				if (this.ProcessIds.Count != 0)
				{
					using (HashSet<int>.Enumerator enumerator = this.ProcessIds.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							int num = enumerator.Current;
							try
							{
								using (Process processById = Process.GetProcessById(num))
								{
									this.SafeKillProcess(processById, startEntry.InstanceId);
								}
							}
							catch (Exception ex)
							{
								ManagedAvailabilityCrimsonEvents.ActiveMonitoringUnexpectedError.Log<int, string>(num, ex.Message);
							}
						}
						return;
					}
				}
				Process[] processesByName = Process.GetProcessesByName(this.ProcessName);
				foreach (Process process in processesByName)
				{
					using (process)
					{
						this.SafeKillProcess(process, startEntry.InstanceId);
					}
				}
			});
		}

		protected void SafeKillProcess(Process process, string instanceId)
		{
			try
			{
				if (process != null)
				{
					try
					{
						try
						{
							ProcessHelper.Kill(process, this.IsMasterAndWorker ? this.MasterAndWorkerKillMode : ProcessKillMode.SelfOnly, instanceId);
						}
						finally
						{
							if (process != null)
							{
								((IDisposable)process).Dispose();
							}
						}
					}
					catch (Win32Exception)
					{
					}
					catch (InvalidOperationException)
					{
					}
				}
			}
			finally
			{
				if (process != null)
				{
					((IDisposable)process).Dispose();
				}
			}
		}

		protected override void DoResponderWork(CancellationToken cancellationToken)
		{
			this.InitializeAttributes();
			RecoveryActionRunner recoveryActionRunner = new RecoveryActionRunner(RecoveryActionId.KillProcess, this.ProcessName, this, true, cancellationToken, null);
			recoveryActionRunner.Execute(delegate(RecoveryActionEntry startEntry)
			{
				this.InternalKillProcess(startEntry, cancellationToken);
			});
		}

		private static readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;

		private static readonly string TypeName = typeof(ProcessKillResponder).FullName;

		internal static class AttributeNames
		{
			internal const string ProcessName = "ProcessName";

			internal const string IsMasterAndWorker = "IsMasterAndWorker";

			internal const string MasterAndWorkerKillMode = "MasterAndWorkerKillMode";

			internal const string throttleGroupName = "throttleGroupName";

			internal const string SampleMask = "SampleMask";
		}

		internal class DefaultValues
		{
			internal const bool IsMasterWorker = false;

			internal const int TimeoutInSeconds = 60;

			internal const string ServiceName = "Exchange";

			internal const bool Enabled = true;
		}
	}
}
