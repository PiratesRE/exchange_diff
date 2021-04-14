using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Microsoft.Exchange.Diagnostics.LatencyDetection
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class LatencyDetectionContext : IFormattable
	{
		internal LatencyDetectionContext(LatencyDetectionLocation location, ContextOptions contextOptions, string version, object hash, params IPerformanceDataProvider[] providers)
		{
			this.latencyDetectionLocation = location;
			this.assemblyVersion = version;
			this.StackTraceContext = hash;
			this.contextOptions = contextOptions;
			if ((contextOptions & ContextOptions.DoNotMeasureTime) == ContextOptions.DoNotMeasureTime)
			{
				this.timer = null;
			}
			else
			{
				this.timer = MyStopwatch.StartNew();
			}
			this.SetDataProviders(providers);
		}

		internal LatencyDetectionContext(LatencyDetectionLocation location, string version, object hash, params IPerformanceDataProvider[] providers) : this(location, ContextOptions.Default, version, hash, providers)
		{
		}

		public string UserIdentity { get; set; }

		public TriggerOptions TriggerOptions
		{
			get
			{
				return this.triggerOptions;
			}
			set
			{
				this.triggerOptions = value;
			}
		}

		public TimeSpan Elapsed
		{
			get
			{
				this.CheckDisallowedOptions(ContextOptions.DoNotMeasureTime);
				return this.timer.Elapsed;
			}
		}

		public TimeSpan ElapsedCpu
		{
			get
			{
				this.CheckDisallowedOptions(ContextOptions.DoNotMeasureTime);
				return this.timer.ElapsedCpu;
			}
		}

		public string Version
		{
			get
			{
				return this.assemblyVersion;
			}
		}

		public DateTime TimeStarted
		{
			get
			{
				return this.timeStarted;
			}
		}

		public bool HasTrustworthyCpuTime
		{
			get
			{
				return this.timer != null && (MyStopwatch.CpuTimeIsAvailable && !this.timer.FinishedOnDifferentProcessor) && !this.timer.PowerManagementChangeOccurred;
			}
		}

		internal static int EstimatedStringCapacity
		{
			get
			{
				return LatencyDetectionContext.estimatedStringCapacity;
			}
		}

		internal object StackTraceContext { get; private set; }

		internal LatencyDetectionLocation Location
		{
			get
			{
				return this.latencyDetectionLocation;
			}
		}

		internal ICollection<LabeledTimeSpan> Latencies
		{
			get
			{
				if (this.latencies == null)
				{
					bool flag = MyStopwatch.CpuTimeIsAvailable && this.timer != null;
					int capacity = this.taskData.Length + (flag ? 1 : 0);
					this.latencies = new List<LabeledTimeSpan>(capacity);
					if (flag)
					{
						LabeledTimeSpan item = new LabeledTimeSpan("CPU", this.timer.ElapsedCpu);
						this.latencies.Add(item);
					}
					for (int i = 0; i < this.taskData.Length; i++)
					{
						int milliseconds = this.taskData[i].Difference.Milliseconds;
						LabeledTimeSpan item2 = new LabeledTimeSpan(this.providers[i].Name, TimeSpan.FromMilliseconds((double)milliseconds));
						this.latencies.Add(item2);
					}
				}
				return this.latencies;
			}
		}

		public override string ToString()
		{
			return this.ToString(null, null);
		}

		public string ToString(string format)
		{
			return this.ToString(format, null);
		}

		public string ToString(string format, IFormatProvider formatProvider)
		{
			StringBuilder stringBuilder = new StringBuilder(LatencyDetectionContext.EstimatedStringCapacity);
			stringBuilder.AppendLine("---");
			LatencyDetectionContext.AppendLine(stringBuilder, "Location: ", this.latencyDetectionLocation.Identity);
			LatencyDetectionContext.AppendLine(stringBuilder, "Version: ", this.assemblyVersion);
			LatencyDetectionContext.AppendLine(stringBuilder, "Stack Trace Context: ", this.StackTraceContext.ToString());
			if (!string.IsNullOrEmpty(this.UserIdentity))
			{
				LatencyDetectionContext.AppendLine(stringBuilder, "User Identity: ", this.UserIdentity);
			}
			LatencyDetectionContext.AppendLine(stringBuilder, "Started: ", this.timeStarted.ToString(CultureInfo.InvariantCulture));
			TimeSpan elapsed = this.Elapsed;
			LatencyDetectionContext.AppendLine(stringBuilder, "Total Time: ", elapsed.ToString());
			if (MyStopwatch.CpuTimeIsAvailable && this.timer != null)
			{
				TimeSpan elapsedCpu = this.timer.ElapsedCpu;
				LatencyDetectionContext.AppendLine(stringBuilder, "Elapsed in CPU: ", elapsedCpu.ToString());
				LatencyDetectionContext.AppendLine(stringBuilder, "Elapsed in CPU (% of Latency): ", (100.0 * elapsedCpu.TotalMilliseconds / elapsed.TotalMilliseconds).ToString(CultureInfo.InvariantCulture));
				if (this.timer.FinishedOnDifferentProcessor)
				{
					stringBuilder.AppendLine("Finished on different processor.");
				}
				if (this.timer.PowerManagementChangeOccurred)
				{
					stringBuilder.AppendLine("Power management change occured.");
				}
			}
			for (int i = 0; i < this.providers.Length; i++)
			{
				TaskPerformanceData taskPerformanceData = this.taskData[i];
				uint count = taskPerformanceData.Difference.Count;
				if (count > 0U)
				{
					string name = this.providers[i].Name;
					LatencyDetectionContext.AppendLine<uint>(stringBuilder, name, " Count: ", count);
					LatencyDetectionContext.AppendLine<int>(stringBuilder, name, " Latency: ", taskPerformanceData.Difference.Milliseconds, " ms");
					if (format != "s" && !string.IsNullOrEmpty(taskPerformanceData.Operations))
					{
						LatencyDetectionContext.AppendLine<string>(stringBuilder, name, " Operations: ", taskPerformanceData.Operations);
					}
				}
			}
			LatencyDetectionContext.estimatedStringCapacity = Math.Min(Math.Max(LatencyDetectionContext.estimatedStringCapacity, stringBuilder.Capacity), 42000);
			return stringBuilder.ToString();
		}

		public TaskPerformanceData[] StopAndFinalizeCollection()
		{
			if (this.timer != null)
			{
				this.timer.Stop();
			}
			for (int i = 0; i < this.providers.Length; i++)
			{
				IPerformanceDataProvider performanceDataProvider = this.providers[i];
				TaskPerformanceData taskPerformanceData = this.taskData[i];
				taskPerformanceData.End = performanceDataProvider.TakeSnapshot(false);
				taskPerformanceData.Operations = performanceDataProvider.Operations;
				performanceDataProvider.ResetOperations();
				if (performanceDataProvider.ThreadLocal)
				{
					taskPerformanceData.InvalidateIfAsynchronous();
				}
			}
			if ((this.contextOptions & ContextOptions.DoNotCreateReport) != ContextOptions.DoNotCreateReport && LatencyDetectionContext.options.LatencyDetectionEnabled)
			{
				LatencyDetectionContext.reporter.Log(this);
			}
			return this.taskData;
		}

		internal static void ValidateBinningParameters(LatencyDetectionLocation location, string version, object hash)
		{
			if (location == null)
			{
				throw new ArgumentNullException("location");
			}
			if (string.IsNullOrEmpty(version))
			{
				throw new ArgumentException("May not be null or empty.", "version");
			}
			if (hash == null)
			{
				throw new ArgumentNullException("hash");
			}
		}

		internal LatencyDetectionException CreateLatencyDetectionException()
		{
			LatencyDetectionException ex = null;
			int num = (int)(this.Elapsed.TotalMilliseconds / 2.0);
			if (this.HasTrustworthyCpuTime && this.timer.ElapsedCpu.TotalMilliseconds >= (double)num)
			{
				ex = new CpuLatencyDetectionException(this);
			}
			else if (this.providers.Length > 0)
			{
				for (int i = 0; i < this.providers.Length; i++)
				{
					int milliseconds = this.taskData[i].Difference.Milliseconds;
					if (milliseconds >= num)
					{
						ex = new DataProviderLatencyDetectionException(this, this.providers[i]);
						break;
					}
				}
			}
			if (ex == null)
			{
				ex = new LatencyDetectionException(this);
			}
			return ex;
		}

		private static void AppendLine(StringBuilder builder, string param1, string param2)
		{
			builder.Append(param1).AppendLine(param2);
		}

		private static void AppendLine<T>(StringBuilder builder, string param1, string param2, T param3)
		{
			builder.Append(param1).Append(param2).Append(param3.ToString()).AppendLine();
		}

		private static void AppendLine<T>(StringBuilder builder, string param1, string param2, T param3, string param4)
		{
			builder.Append(param1).Append(param2).Append(param3.ToString()).Append(param4).AppendLine();
		}

		private void SetDataProviders(IPerformanceDataProvider[] dataProviders)
		{
			int num = (dataProviders != null) ? dataProviders.Length : 0;
			this.providers = new IPerformanceDataProvider[num];
			this.taskData = new TaskPerformanceData[num];
			for (int i = 0; i < num; i++)
			{
				IPerformanceDataProvider performanceDataProvider = dataProviders[i];
				if (performanceDataProvider == null)
				{
					string message = string.Format(CultureInfo.InvariantCulture, "dataProviders[{0}] was null.", new object[]
					{
						i
					});
					throw new ArgumentNullException("dataProviders", message);
				}
				this.providers[i] = performanceDataProvider;
				TaskPerformanceData taskPerformanceData = new TaskPerformanceData();
				taskPerformanceData.Start = performanceDataProvider.TakeSnapshot(true);
				this.taskData[i] = taskPerformanceData;
			}
		}

		private void CheckDisallowedOptions(ContextOptions testOptions)
		{
			if ((this.contextOptions & testOptions) == testOptions)
			{
				throw new InvalidOperationException(string.Format("The operation is not allowed due to the mode selected at creation time. testOptions = {0}, contextOptions = {1}.", testOptions, this.contextOptions));
			}
		}

		public const string UserIdentityLabel = "User Identity: ";

		private const int MaxDepthToLog = 16;

		internal const int MaxStringBuilderCapacity = 42000;

		private const string NullOrEmptyError = "May not be null or empty.";

		private const string ContextSlot = "LatencyDetectionStack";

		private const string ContextDivider = "---";

		private const string LocationLabel = "Location: ";

		private const string VersionLabel = "Version: ";

		private const string StackTraceContextLabel = "Stack Trace Context: ";

		private const string StartTimeLabel = "Started: ";

		private const string LatencyLabel = "Total Time: ";

		private const string ProviderCountLabel = " Count: ";

		private const string ProviderLatencyLabel = " Latency: ";

		private const string ProviderOperationsLabel = " Operations: ";

		private const string MsecUnits = " ms";

		private const string ElapsedInCpuLabel = "Elapsed in CPU: ";

		private const string CpuPercentage = "Elapsed in CPU (% of Latency): ";

		private const string FinishedOnDifferentProcessor = "Finished on different processor.";

		private const string PowerManagementChangeOccured = "Power management change occured.";

		private static readonly PerformanceReportingOptions options = PerformanceReportingOptions.Instance;

		private static readonly PerformanceReporter reporter = PerformanceReporter.Instance;

		private static int estimatedStringCapacity = 360;

		private readonly DateTime timeStarted = DateTime.UtcNow;

		private readonly MyStopwatch timer;

		private readonly ContextOptions contextOptions;

		private TriggerOptions triggerOptions;

		private LatencyDetectionLocation latencyDetectionLocation;

		private string assemblyVersion;

		private IPerformanceDataProvider[] providers;

		private TaskPerformanceData[] taskData;

		private List<LabeledTimeSpan> latencies;
	}
}
