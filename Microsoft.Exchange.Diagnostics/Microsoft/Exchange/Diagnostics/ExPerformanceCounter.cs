using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Xml;
using Microsoft.Win32;

namespace Microsoft.Exchange.Diagnostics
{
	public class ExPerformanceCounter : IExPerformanceCounter
	{
		public ExPerformanceCounter(string categoryName, string counterName, string instanceName, bool processLifeTime, ExPerformanceCounter totalInstanceCounter, params ExPerformanceCounter[] autoUpdateCounters)
		{
			this.counter = PerformanceCounterFactory.CreatePerformanceCounter();
			this.counter.CategoryName = categoryName;
			this.counter.CounterName = counterName;
			this.counter.InstanceName = instanceName;
			this.counter.ReadOnly = false;
			this.counterIsUsable = true;
			if (processLifeTime)
			{
				this.counter.InstanceLifetime = PerformanceCounterInstanceLifetime.Process;
			}
			this.totalInstanceCounter = totalInstanceCounter;
			this.autoUpdateCounters = autoUpdateCounters;
		}

		public ExPerformanceCounter(string categoryName, string counterName, string instanceName, ExPerformanceCounter totalInstanceCounter, params ExPerformanceCounter[] autoUpdateCounters) : this(categoryName, counterName, instanceName, false, totalInstanceCounter, autoUpdateCounters)
		{
		}

		public string CounterName
		{
			get
			{
				return this.counter.CounterName;
			}
		}

		public string CategoryName
		{
			get
			{
				return this.counter.CategoryName;
			}
		}

		public string CounterHelp
		{
			get
			{
				return this.counter.CounterHelp;
			}
		}

		public PerformanceCounterType CounterType
		{
			get
			{
				return this.counter.CounterType;
			}
		}

		public string InstanceName
		{
			get
			{
				return this.counter.InstanceName;
			}
		}

		public virtual long RawValue
		{
			get
			{
				long result = 0L;
				if (this.counterIsUsable)
				{
					try
					{
						result = this.counter.RawValue;
					}
					catch (InvalidOperationException ex)
					{
						this.counterIsUsable = false;
						this.LogUpdateFailureEvent(ExPerformanceCounter.LogReason.Get, ex);
					}
				}
				return result;
			}
			set
			{
				if (this.counterIsUsable)
				{
					try
					{
						if (this.totalInstanceCounter != null)
						{
							this.totalInstanceCounter.IncrementBy(value - this.counter.RawValue);
						}
						this.counter.RawValue = value;
						for (int i = 0; i < this.autoUpdateCounters.Length; i++)
						{
							this.autoUpdateCounters[i].RawValue = value;
						}
					}
					catch (InvalidOperationException ex)
					{
						this.counterIsUsable = false;
						this.LogUpdateFailureEvent(ExPerformanceCounter.LogReason.Set, ex);
					}
				}
			}
		}

		public static string GetEncodedName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return name;
			}
			return XmlConvert.EncodeName(name.Replace(" ", string.Empty).Replace("-", "_").Replace(":", "__"));
		}

		public long Increment()
		{
			return this.IncrementBy(1L);
		}

		public long Decrement()
		{
			return this.IncrementBy(-1L);
		}

		public virtual void Reset()
		{
			this.RawValue = 0L;
		}

		public virtual long IncrementBy(long incrementValue)
		{
			long result = 0L;
			if (this.counterIsUsable)
			{
				try
				{
					if (this.totalInstanceCounter != null)
					{
						this.totalInstanceCounter.IncrementBy(incrementValue);
					}
					for (int i = 0; i < this.autoUpdateCounters.Length; i++)
					{
						this.autoUpdateCounters[i].IncrementBy(incrementValue);
					}
					result = this.counter.IncrementBy(incrementValue);
				}
				catch (InvalidOperationException ex)
				{
					this.counterIsUsable = false;
					this.LogUpdateFailureEvent(ExPerformanceCounter.LogReason.Inc, ex);
				}
			}
			return result;
		}

		public float NextValue()
		{
			float result = 0f;
			if (this.counterIsUsable)
			{
				try
				{
					result = this.counter.NextValue();
				}
				catch (InvalidOperationException ex)
				{
					this.counterIsUsable = false;
					this.LogUpdateFailureEvent(ExPerformanceCounter.LogReason.NextValue, ex);
				}
			}
			return result;
		}

		public virtual void Close()
		{
			this.counterIsUsable = false;
			this.counter.Close();
			this.counter.Dispose();
		}

		public void RemoveInstance()
		{
			if (this.counterIsUsable)
			{
				try
				{
					this.counter.RemoveInstance();
				}
				catch (InvalidOperationException ex)
				{
					this.counterIsUsable = false;
					this.LogUpdateFailureEvent(ExPerformanceCounter.LogReason.Remove, ex);
				}
			}
		}

		public void ResetCounterIsUsable()
		{
			this.counterIsUsable = true;
		}

		private void LogUpdateFailureEvent(ExPerformanceCounter.LogReason action, Exception ex)
		{
			try
			{
				if (ExPerformanceCounter.eventLogger == null)
				{
					ExPerformanceCounter.eventLogger = new ExEventLog(new Guid("{1f3d39b3-c7ba-4494-94ad-c64cbd33e061}"), "MSExchange Common");
				}
				ExPerformanceCounter.eventLogger.LogEvent(CommonEventLogConstants.Tuple_PerfCounterProblem, this.CategoryName + this.CounterName, new object[]
				{
					(int)action,
					this.CounterName,
					this.CategoryName,
					this.GetProcessAndExceptionInfo(ex.ToString())
				});
			}
			catch
			{
			}
		}

		private string GetProcessAndExceptionInfo(string exception)
		{
			string lastWorkerProcessInfo = this.GetLastWorkerProcessInfo();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("The exception thrown is : " + exception);
			stringBuilder.AppendLine("Last worker process info : " + lastWorkerProcessInfo);
			stringBuilder.AppendLine("Processes running while Performance counter failed to update: ");
			try
			{
				Process[] processes = Process.GetProcesses();
				if (processes != null)
				{
					foreach (Process process in processes)
					{
						stringBuilder.AppendLine(process.Id + " " + process.ProcessName);
					}
				}
			}
			catch (Exception ex)
			{
				stringBuilder.AppendLine("An Exception occured when getting running processes: " + ex.ToString());
			}
			string allInstancesLayout = this.GetAllInstancesLayout(this.CategoryName);
			stringBuilder.AppendLine("Performance Counters Layout information: " + allInstancesLayout);
			return stringBuilder.ToString();
		}

		private string GetAllInstancesLayout(string categoryName)
		{
			StringBuilder stringBuilder = new StringBuilder();
			try
			{
				using (PerformanceCounterMemoryMappedFile performanceCounterMemoryMappedFile = new PerformanceCounterMemoryMappedFile(categoryName, true))
				{
					CategoryEntry categoryEntry = performanceCounterMemoryMappedFile.FindCategory(categoryName);
					if (categoryEntry != null)
					{
						for (InstanceEntry instanceEntry = categoryEntry.FirstInstance; instanceEntry != null; instanceEntry = instanceEntry.Next)
						{
							CounterEntry firstCounter = instanceEntry.FirstCounter;
							if (firstCounter != null)
							{
								LifetimeEntry lifetime = firstCounter.Lifetime;
								if (lifetime != null && lifetime.Type == 1)
								{
									stringBuilder.AppendLine(string.Format("A process is holding onto a transport performance counter. processId : {0}, counter : {1}, currentInstance : {2}, categoryName: {3} ", new object[]
									{
										lifetime.ProcessId,
										firstCounter,
										instanceEntry,
										categoryName
									}));
								}
							}
						}
					}
				}
			}
			catch (Win32Exception ex)
			{
				stringBuilder.AppendLine(string.Concat(new object[]
				{
					"Win32Exception for category ",
					categoryName,
					" : ",
					ex
				}));
			}
			catch (FileMappingNotFoundException ex2)
			{
				stringBuilder.AppendLine(string.Concat(new object[]
				{
					"FileMappingNotFoundException for category ",
					categoryName,
					" : ",
					ex2
				}));
			}
			catch (Exception arg)
			{
				stringBuilder.AppendLine("Exception : " + arg);
			}
			return stringBuilder.ToString();
		}

		private string GetLastWorkerProcessInfo()
		{
			string result = "Last worker process info not available!";
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport"))
				{
					if (registryKey.GetValue("LastWorkerProcessId") != null)
					{
						int processId = (int)registryKey.GetValue("LastWorkerProcessId");
						using (Process processById = Process.GetProcessById(processId))
						{
							result = processId.ToString() + processById.HasExited;
						}
					}
				}
			}
			catch (Exception ex)
			{
				result = ex.ToString();
			}
			return result;
		}

		public const string LastWorkerProcessIdLocation = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport";

		public const string LastWorkerProcessIdKeyName = "LastWorkerProcessId";

		private static ExEventLog eventLogger;

		private IPerformanceCounter counter;

		private bool counterIsUsable;

		private ExPerformanceCounter totalInstanceCounter;

		private ExPerformanceCounter[] autoUpdateCounters;

		private enum LogReason
		{
			Inc = 1,
			Get,
			Set,
			Remove,
			NextValue
		}
	}
}
