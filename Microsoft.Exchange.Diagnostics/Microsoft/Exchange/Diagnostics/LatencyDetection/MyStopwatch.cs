using System;

namespace Microsoft.Exchange.Diagnostics.LatencyDetection
{
	public class MyStopwatch
	{
		public MyStopwatch()
		{
			this.Reset();
		}

		public static bool IsHighResolution
		{
			get
			{
				return MyStopwatch.isHighResolution;
			}
		}

		public static long Frequency
		{
			get
			{
				return MyStopwatch.frequency;
			}
		}

		public static bool CpuTimeIsAvailable
		{
			get
			{
				return MyStopwatch.isWindowsVersionAtLeastLonghorn && !MyStopwatch.cpuTimeIsSuppressed;
			}
		}

		public static long CpuCycles
		{
			get
			{
				ulong result = 0UL;
				if (MyStopwatch.CpuTimeIsAvailable)
				{
					using (SafeThreadHandle currentThread = NativeMethods.GetCurrentThread())
					{
						if (!NativeMethods.QueryThreadCycleTime(currentThread, out result))
						{
							throw new InvalidOperationException("NativeMethods.QueryThreadCycleTime() unexpectedly returned false.");
						}
					}
				}
				return (long)result;
			}
		}

		public static long Timestamp
		{
			get
			{
				long result = 0L;
				if (MyStopwatch.IsHighResolution)
				{
					if (!NativeMethods.QueryPerformanceCounter(out result))
					{
						result = DateTime.UtcNow.Ticks;
						MyStopwatch.isHighResolution = false;
					}
				}
				else
				{
					result = DateTime.UtcNow.Ticks;
				}
				return result;
			}
		}

		public TimeSpan Elapsed
		{
			get
			{
				return new TimeSpan(this.GetElapsedDateTimeTicks());
			}
		}

		public TimeSpan ElapsedCpu
		{
			get
			{
				if (MyStopwatch.CpuTimeIsAvailable && this.timerEndMaxMHz > 0)
				{
					if (this.isRunning)
					{
						this.InternalGetElapsedCpuStatus();
					}
					long ticks = 10L * this.elapsedCPU / (long)this.timerEndMaxMHz;
					return new TimeSpan(ticks);
				}
				return TimeSpan.Zero;
			}
		}

		public long ElapsedCpuTicks
		{
			get
			{
				return this.elapsedCPU;
			}
		}

		public long ElapsedMilliseconds
		{
			get
			{
				return this.GetElapsedDateTimeTicks() / 10000L;
			}
		}

		public long ElapsedTicks
		{
			get
			{
				return this.GetRawElapsedTicks();
			}
		}

		public bool ThreadContextSwitchOccurred
		{
			get
			{
				return this.threadSwitchOccurred;
			}
		}

		public bool FinishedOnDifferentProcessor
		{
			get
			{
				return this.cpuSwitchOccurred;
			}
		}

		public bool PowerManagementChangeOccurred
		{
			get
			{
				return this.cpuspeedChangeOccurred;
			}
		}

		public int CurrentCpuMhz
		{
			get
			{
				return this.timerEndCurrentMHz;
			}
		}

		public int MaxCpuMhz
		{
			get
			{
				return this.timerEndMaxMHz;
			}
		}

		public bool IsRunning
		{
			get
			{
				return this.isRunning;
			}
		}

		public static void SuppressCpuTimeMeasurement()
		{
			MyStopwatch.cpuTimeIsSuppressed = true;
		}

		public static MyStopwatch StartNew()
		{
			MyStopwatch myStopwatch = new MyStopwatch();
			myStopwatch.Start();
			return myStopwatch;
		}

		public void Start()
		{
			if (!this.isRunning)
			{
				this.startTimeStamp = MyStopwatch.Timestamp;
				if (MyStopwatch.CpuTimeIsAvailable)
				{
					using (SafeThreadHandle currentThread = NativeMethods.GetCurrentThread())
					{
						this.timerStartThreadHandle = currentThread.DangerousGetHandle().ToInt64();
					}
					this.timerStartCPUID = (int)NativeMethods.GetCurrentProcessorNumber();
					this.timerStartCurrentMHz = MyStopwatch.GetCurrentMHz(this.timerStartCPUID);
					this.timerEndCurrentMHz = this.timerStartCurrentMHz;
					this.startCPUcycles = MyStopwatch.CpuCycles;
				}
				this.isRunning = true;
			}
		}

		public void Stop()
		{
			if (this.isRunning)
			{
				long timestamp = MyStopwatch.Timestamp;
				if (MyStopwatch.CpuTimeIsAvailable)
				{
					this.InternalGetElapsedCpuStatus();
				}
				long num = timestamp - this.startTimeStamp;
				this.elapsed += num;
				this.isRunning = false;
				if (this.elapsed < 0L)
				{
					this.elapsed = 0L;
				}
				if (MyStopwatch.CpuTimeIsAvailable && this.elapsedCPU > 0L)
				{
					this.timerEndCPUID = (int)NativeMethods.GetCurrentProcessorNumber();
					if (this.timerEndCPUID != this.timerStartCPUID)
					{
						this.cpuSwitchOccurred = true;
					}
					MyStopwatch.GetCurrentAndMaxMHz(this.timerEndCPUID, out this.timerEndCurrentMHz, out this.timerEndMaxMHz);
					if (this.timerStartCurrentMHz != this.timerEndCurrentMHz)
					{
						this.cpuspeedChangeOccurred = true;
					}
				}
			}
		}

		public void Reset()
		{
			this.elapsed = 0L;
			this.isRunning = false;
			this.startTimeStamp = 0L;
			this.elapsedCPU = 0L;
			this.startCPUcycles = 0L;
			this.threadSwitchOccurred = false;
			this.cpuSwitchOccurred = false;
			this.cpuspeedChangeOccurred = false;
		}

		private static int GetCurrentMHz(int cpuId)
		{
			int result;
			int num;
			MyStopwatch.GetCurrentAndMaxMHz(cpuId, out result, out num);
			return result;
		}

		private static void GetCurrentAndMaxMHz(int cpuId, out int current, out int max)
		{
			if (cpuId < 0 || cpuId > Environment.ProcessorCount)
			{
				throw new ArgumentException("Needs to be between 0 and Environment.ProcessorCount-1.", "cpuId");
			}
			current = 0;
			max = 0;
			if (!MyStopwatch.isCpuThrottlingSupported)
			{
				current = MyStopwatch.nonThrottlingCpuMHz[cpuId];
				max = MyStopwatch.nonThrottlingCpuMHz[cpuId];
				return;
			}
			PROCESSOR_POWER_INFORMATION processor_POWER_INFORMATION;
			if (CPUPowerWrapper.GetCurrentPowerInfoForProcessor(cpuId, out processor_POWER_INFORMATION))
			{
				current = (int)processor_POWER_INFORMATION.CurrentMhz;
				max = (int)processor_POWER_INFORMATION.MaxMhz;
				return;
			}
			throw new InvalidOperationException("CPUPowerWrapper.GetCurrentPowerInfoForProcessor() unexpectedly returned false.");
		}

		private static long QueryOsForFrequency(out bool highFrequency)
		{
			long result;
			highFrequency = NativeMethods.QueryPerformanceFrequency(out result);
			if (!highFrequency)
			{
				result = 10000000L;
			}
			return result;
		}

		private static bool GetIsCpuThrottlingSupported()
		{
			bool isProcessorThrottlingEnabled = CPUPowerWrapper.IsProcessorThrottlingEnabled;
			if (!isProcessorThrottlingEnabled && !CPUPowerWrapper.GetMaxProcessorSpeeds(out MyStopwatch.nonThrottlingCpuMHz))
			{
				throw new InvalidOperationException("CPUPowerWrapper.GetMaxProcessorSpeeds() unexpectedly returned false.");
			}
			return isProcessorThrottlingEnabled;
		}

		private void InternalGetElapsedCpuStatus()
		{
			using (SafeThreadHandle currentThread = NativeMethods.GetCurrentThread())
			{
				long cpuCycles = MyStopwatch.CpuCycles;
				if (currentThread.DangerousGetHandle().ToInt64() == this.timerStartThreadHandle)
				{
					if (cpuCycles > this.startCPUcycles)
					{
						long num = cpuCycles - this.startCPUcycles;
						this.elapsedCPU += num;
					}
					else
					{
						this.elapsedCPU = 0L;
						this.startCPUcycles = 0L;
					}
				}
				else
				{
					this.threadSwitchOccurred = true;
					this.startCPUcycles = 0L;
				}
			}
		}

		private long GetRawElapsedTicks()
		{
			long num = this.elapsed;
			if (this.isRunning)
			{
				long num2 = MyStopwatch.Timestamp - this.startTimeStamp;
				num += num2;
			}
			return num;
		}

		private long GetElapsedDateTimeTicks()
		{
			long num = this.GetRawElapsedTicks();
			if (MyStopwatch.IsHighResolution)
			{
				double num2 = (double)num * MyStopwatch.elapsedTicksToDateTimeTicks;
				num = (long)num2;
			}
			return num;
		}

		private const long TicksPerMillisecond = 10000L;

		private const long TicksPerSecond = 10000000L;

		private const long CyclesPerMHz = 10L;

		private const int OSVista = 6;

		private static readonly long frequency = MyStopwatch.QueryOsForFrequency(out MyStopwatch.isHighResolution);

		private static readonly double elapsedTicksToDateTimeTicks = MyStopwatch.isHighResolution ? (10000000.0 / (double)MyStopwatch.frequency) : 1.0;

		private static readonly bool isCpuThrottlingSupported = MyStopwatch.GetIsCpuThrottlingSupported();

		private static readonly bool isWindowsVersionAtLeastLonghorn = Environment.OSVersion.Version.Major >= 6;

		private static int[] nonThrottlingCpuMHz;

		private static bool isHighResolution;

		private static bool cpuTimeIsSuppressed;

		private long elapsed;

		private long elapsedCPU;

		private long startTimeStamp;

		private long startCPUcycles;

		private int timerStartCPUID;

		private int timerEndCPUID;

		private int timerStartCurrentMHz;

		private int timerEndCurrentMHz;

		private int timerEndMaxMHz;

		private long timerStartThreadHandle;

		private bool threadSwitchOccurred;

		private bool cpuSwitchOccurred;

		private bool cpuspeedChangeOccurred;

		private bool isRunning;
	}
}
