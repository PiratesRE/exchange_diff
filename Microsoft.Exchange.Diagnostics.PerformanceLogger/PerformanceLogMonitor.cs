using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Service.Common;

namespace Microsoft.Exchange.Diagnostics.PerformanceLogger
{
	public class PerformanceLogMonitor
	{
		public PerformanceLogMonitor(TimeSpan monitorInterval, Action<PerformanceLogMonitor> createLogSets)
		{
			this.threadDone = new ManualResetEvent(false);
			this.monitorInterval = monitorInterval;
			this.monitoredSets = new List<PerformanceLogMonitor.MonitoredLogSet>();
			this.createLogSets = createLogSets;
		}

		public TimeSpan MonitorInterval
		{
			get
			{
				return this.monitorInterval;
			}
		}

		public void AddPerflog(PerformanceLogSet performanceLog, TimeSpan? restartInterval)
		{
			lock (this.performanceLogListLock)
			{
				PerformanceLogMonitor.MonitoredLogSet item = new PerformanceLogMonitor.MonitoredLogSet(performanceLog, restartInterval);
				this.monitoredSets.Add(item);
			}
		}

		public void StartMonitor()
		{
			if (!this.isStarted)
			{
				lock (this.monitorControlLock)
				{
					if (!this.isStarted)
					{
						this.statusThread = new Thread(new ThreadStart(this.CheckStatusThread));
						this.statusThread.Name = "PerformanceLogMonitor";
						this.statusThread.Start();
						this.isStarted = true;
					}
				}
			}
		}

		public void StopMonitor()
		{
			if (this.isStarted)
			{
				lock (this.monitorControlLock)
				{
					if (this.isStarted)
					{
						this.threadDone.Set();
						this.statusThread.Join();
						this.threadDone.Reset();
						this.isStarted = false;
						this.statusThread = null;
					}
				}
			}
		}

		private void CheckStatusThread()
		{
			this.createLogSets(this);
			do
			{
				this.CheckPerflogStatus();
			}
			while (!this.threadDone.WaitOne(this.MonitorInterval));
		}

		private void CheckPerflogStatus()
		{
			lock (this.performanceLogListLock)
			{
				foreach (PerformanceLogMonitor.MonitoredLogSet monitoredLogSet in this.monitoredSets)
				{
					try
					{
						if (monitoredLogSet.LastRestart == DateTime.MinValue)
						{
							monitoredLogSet.CounterSet.StopLog(true);
							monitoredLogSet.CounterSet.CreateLogSettings();
						}
						PerformanceLogSet.PerformanceLogSetStatus status = monitoredLogSet.CounterSet.Status;
						bool flag2 = status != PerformanceLogSet.PerformanceLogSetStatus.Running;
						if (status == PerformanceLogSet.PerformanceLogSetStatus.DoesNotExist)
						{
							monitoredLogSet.CounterSet.CreateLogSettings();
						}
						if (!flag2 && monitoredLogSet.RestartInterval != null && DateTime.UtcNow - monitoredLogSet.LastRestart > monitoredLogSet.RestartInterval)
						{
							monitoredLogSet.CounterSet.StopLog(true);
							flag2 = true;
						}
						if (flag2)
						{
							monitoredLogSet.CounterSet.StartLog(false);
							monitoredLogSet.LastRestart = DateTime.UtcNow;
						}
						monitoredLogSet.FailureCount = 0;
					}
					catch (Exception ex)
					{
						Logger.LogWarningMessage("Check PerflogStatus hit exception {0} for performance log set {1}", new object[]
						{
							ex,
							monitoredLogSet.CounterSet.CounterSetName
						});
						if (5 <= monitoredLogSet.FailureCount)
						{
							Logger.LogEvent(MSExchangeDiagnosticsEventLogConstants.Tuple_PerformanceLogError, new object[]
							{
								ex,
								monitoredLogSet.CounterSet.CounterSetName
							});
							throw;
						}
						monitoredLogSet.FailureCount++;
						if (!(ex is ArgumentException) && !(ex is COMException))
						{
							throw;
						}
						string text = Path.Combine(Environment.GetEnvironmentVariable("windir"), "System32\\Tasks\\Microsoft\\Windows\\PLA", monitoredLogSet.CounterSet.CounterSetName);
						Logger.LogWarningMessage("PerflogStatus deleting logset info at '{0}'", new object[]
						{
							text
						});
						PerformanceLogSet.DeleteTask(monitoredLogSet.CounterSet.CounterSetName);
						if (File.Exists(text))
						{
							File.Delete(text);
						}
					}
				}
			}
		}

		private const int MaximumFailureCount = 5;

		private readonly List<PerformanceLogMonitor.MonitoredLogSet> monitoredSets;

		private readonly ManualResetEvent threadDone;

		private readonly TimeSpan monitorInterval;

		private readonly object monitorControlLock = new object();

		private readonly object performanceLogListLock = new object();

		private readonly Action<PerformanceLogMonitor> createLogSets;

		private Thread statusThread;

		private bool isStarted;

		internal class MonitoredLogSet
		{
			internal MonitoredLogSet(PerformanceLogSet counterSet, TimeSpan? restartInterval)
			{
				this.counterSet = counterSet;
				this.restartInterval = restartInterval;
				this.lastRestart = DateTime.MinValue;
				this.failureCount = 0;
			}

			internal PerformanceLogSet CounterSet
			{
				get
				{
					return this.counterSet;
				}
			}

			internal TimeSpan? RestartInterval
			{
				get
				{
					return this.restartInterval;
				}
			}

			internal DateTime LastRestart
			{
				get
				{
					return this.lastRestart;
				}
				set
				{
					this.lastRestart = value;
				}
			}

			internal int FailureCount
			{
				get
				{
					return this.failureCount;
				}
				set
				{
					this.failureCount = value;
				}
			}

			private readonly PerformanceLogSet counterSet;

			private readonly TimeSpan? restartInterval;

			private DateTime lastRestart;

			private int failureCount;
		}
	}
}
