using System;
using System.Diagnostics;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ReplayServiceDiagnostics
	{
		private static Microsoft.Exchange.Diagnostics.Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ReplayServiceDiagnosticsTracer;
			}
		}

		internal long ProcessThreadCount
		{
			get
			{
				return this.processThreadCount;
			}
		}

		internal long ProcessHandleCount
		{
			get
			{
				return this.processHandleCount;
			}
		}

		internal long ProcessPrivateMemorySize
		{
			get
			{
				return this.processPrivateMemorySize;
			}
		}

		public bool LimitReached { get; private set; }

		public bool DisableCrash { get; set; }

		public ReplayServiceDiagnostics()
		{
			this.ObtainLocalCopyCount = new Func<int>(this.GetLocalCopyCount);
			this.ProcessName = "msexchangerepl";
		}

		public string ProcessName { get; set; }

		protected void SendReportAndCrash(Exception ex, string key)
		{
			this.LimitReached = true;
			ReplayEventLogConstants.Tuple_ProcessDiagnosticsTerminatingService.LogEvent(null, new object[]
			{
				ex.Message,
				key
			});
			if (!this.DisableCrash)
			{
				ExWatson.SendReportAndCrashOnAnotherThread(ex);
			}
		}

		protected void SendReportWithoutDumpAndCrash(Exception ex, string key)
		{
			this.LimitReached = true;
			ReplayEventLogConstants.Tuple_ProcessDiagnosticsTerminatingServiceNoDump.LogEvent(null, new object[]
			{
				ex.Message,
				key
			});
			if (!this.DisableCrash)
			{
				ExWatson.SendReport(ex, ReportOptions.ReportTerminateAfterSend | ReportOptions.DoNotCollectDumps, "No Watson dump being taken.");
				ExWatson.TerminateCurrentProcess();
			}
		}

		private int GetLocalCopyCount()
		{
			int result = 0;
			ReplicaInstanceManager replicaInstanceManager = Dependencies.ReplayCoreManager.ReplicaInstanceManager;
			if (replicaInstanceManager != null)
			{
				result = replicaInstanceManager.GetRICount();
			}
			return result;
		}

		public Func<int> ObtainLocalCopyCount { get; set; }

		private int MemoryLimitInMB()
		{
			int num = this.ObtainLocalCopyCount();
			return Math.Min(RegistryParameters.MaximumProcessPrivateMemoryMB, RegistryParameters.MemoryLimitBaseInMB + num * RegistryParameters.MemoryLimitPerDBInMB);
		}

		private string BuildMemoryLimitRegKeyString()
		{
			return string.Format("RegKeyBase={0};RegValues={1},{2},{3};EffectiveLimit=Min({1},{2}+nCopies*{3})", new object[]
			{
				"SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters",
				"MaximumProcessPrivateMemoryMB",
				"MemoryLimitBaseInMB",
				"MemoryLimitPerDBInMB"
			});
		}

		private long GetWarningLimit(long errorLimit)
		{
			return errorLimit * 80L / 100L;
		}

		public void RunProcessDiagnostics()
		{
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				this.processThreadCount = (long)currentProcess.Threads.Count;
				ReplayServiceDiagnostics.Tracer.TraceDebug<long>((long)this.GetHashCode(), "Replay service current number of threads: {0}", this.processThreadCount);
				long num = (long)RegistryParameters.MaximumProcessThreadCount;
				long warningLimit = this.GetWarningLimit(num);
				if (this.processThreadCount > warningLimit)
				{
					ReplayCrimsonEvents.ResourceWarningForThreadsReached.LogPeriodic<string, long, long, long, string>(Environment.MachineName, this.ResourceWarningPeriod, this.ProcessName, this.processThreadCount, warningLimit, num, "Threads");
					if (this.processThreadCount > num)
					{
						Exception ex = new ReplayServiceTooManyThreadsException(this.processThreadCount, (long)RegistryParameters.MaximumProcessThreadCount);
						this.SendReportAndCrash(ex, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters\\MaximumProcessThreadCount");
					}
				}
				this.processHandleCount = (long)currentProcess.HandleCount;
				ReplayServiceDiagnostics.Tracer.TraceDebug<long>((long)this.GetHashCode(), "Replay service current number of handles: {0}", this.processHandleCount);
				num = (long)RegistryParameters.MaximumProcessHandleCount;
				warningLimit = this.GetWarningLimit(num);
				if (this.processHandleCount > warningLimit)
				{
					ReplayCrimsonEvents.ResourceWarningForHandlesReached.LogPeriodic<string, long, long, long, string>(Environment.MachineName, this.ResourceWarningPeriod, this.ProcessName, this.processHandleCount, warningLimit, num, "OS Handles");
					if (this.processHandleCount > num)
					{
						Exception ex2 = new ReplayServiceTooManyHandlesException(this.processHandleCount, (long)RegistryParameters.MaximumProcessHandleCount);
						this.SendReportAndCrash(ex2, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters\\MaximumProcessHandleCount");
					}
				}
				this.processPrivateMemorySize = currentProcess.PrivateMemorySize64;
				double num2 = (double)this.processPrivateMemorySize / ReplayServiceDiagnostics.memorySizeMultiplier;
				ReplayServiceDiagnostics.Tracer.TraceDebug<double>((long)this.GetHashCode(), "Replay service current memory usage: {0} MiB", num2);
				long num3 = (long)num2;
				num = (long)this.MemoryLimitInMB();
				warningLimit = this.GetWarningLimit(num);
				if (num3 > warningLimit)
				{
					ReplayCrimsonEvents.ResourceWarningForMemoryReached.LogPeriodic<string, long, long, long, string>(Environment.MachineName, this.ResourceWarningPeriod, this.ProcessName, num3, warningLimit, num, "processPrivateMemorySize");
					if (num3 > num)
					{
						if (RegistryParameters.EnableWatsonDumpOnTooMuchMemory)
						{
							Exception ex3 = new ReplayServiceTooMuchMemoryException(num2, num);
							this.SendReportAndCrash(ex3, this.BuildMemoryLimitRegKeyString());
						}
						else
						{
							Exception ex4 = new ReplayServiceTooMuchMemoryNoDumpException(num2, num, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters\\EnableWatsonDumpOnTooMuchMemory");
							this.SendReportWithoutDumpAndCrash(ex4, this.BuildMemoryLimitRegKeyString());
						}
					}
				}
				if (RegistryParameters.MonitorGCHandleCount)
				{
					num = (long)RegistryParameters.MaximumGCHandleCount;
					warningLimit = this.GetWarningLimit(num);
					PerformanceCounter performanceCounter = null;
					try
					{
						performanceCounter = new PerformanceCounter(".NET CLR Memory", "# GC Handles", "msexchangerepl");
						long rawValue = performanceCounter.RawValue;
						if (rawValue > warningLimit)
						{
							ReplayCrimsonEvents.ResourceWarningForHandlesReached.LogPeriodic<string, long, long, long, string>(Environment.MachineName, this.ResourceWarningPeriod, this.ProcessName, rawValue, warningLimit, num, "GC Handles");
							if (rawValue > num)
							{
								Exception ex5 = new ReplayServiceTooManyHandlesException(rawValue, num);
								this.SendReportAndCrash(ex5, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Replay\\Parameters\\MaximumGCHandleCount");
							}
						}
					}
					catch (InvalidOperationException arg)
					{
						ReplayServiceDiagnostics.Tracer.TraceError<InvalidOperationException>((long)this.GetHashCode(), "Perf counters are broken, preventing the MaximumGCHandleCount check: {0}", arg);
					}
					finally
					{
						if (performanceCounter != null)
						{
							performanceCounter.Dispose();
							performanceCounter = null;
						}
					}
				}
			}
		}

		private const long warningLimitPercentage = 80L;

		private static double memorySizeMultiplier = 1048576.0;

		private long processThreadCount;

		private long processHandleCount;

		private long processPrivateMemorySize;

		private readonly TimeSpan ResourceWarningPeriod = TimeSpan.FromMinutes(60.0);
	}
}
