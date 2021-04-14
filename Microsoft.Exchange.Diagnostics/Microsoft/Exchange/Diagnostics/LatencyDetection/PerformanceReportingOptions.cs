using System;
using System.Threading;
using Microsoft.Win32;

namespace Microsoft.Exchange.Diagnostics.LatencyDetection
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PerformanceReportingOptions : IThresholdInitializer
	{
		private PerformanceReportingOptions()
		{
			this.Refresh();
			this.LoadTime = DateTime.UtcNow;
		}

		internal static PerformanceReportingOptions Instance
		{
			get
			{
				return PerformanceReportingOptions.singletonInstance;
			}
		}

		internal bool LatencyDetectionEnabled
		{
			get
			{
				this.RefreshIfExpired();
				return this.latencyDetectionEnabled && this.LoadTime + this.InitialWait < DateTime.UtcNow;
			}
		}

		internal TimeSpan WatsonThrottle { get; private set; }

		internal TimeSpan InitialWait { get; private set; }

		internal DateTime LoadTime { get; set; }

		internal TimeSpan RefreshOptionsInterval { get; private set; }

		internal TimeSpan BacklogRetirementAge { get; private set; }

		internal uint MaximumBacklogSize { get; private set; }

		public bool EnableLatencyEventLogging { get; private set; }

		void IThresholdInitializer.SetThresholdFromConfiguration(LatencyDetectionLocation location, LoggingType type)
		{
			TimeSpan threshold = location.DefaultThreshold;
			if (LoggingType.WindowsErrorReporting == type)
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\LatencyDetectionOptions"))
				{
					if (registryKey != null)
					{
						uint registryUInt = PerformanceReportingOptions.GetRegistryUInt32(registryKey, location.Identity + "ThresholdMilliseconds", PerformanceReportingOptions.GetTotalMilliseconds(location.DefaultThreshold), PerformanceReportingOptions.GetTotalMilliseconds(location.MinimumThreshold));
						threshold = TimeSpan.FromMilliseconds(registryUInt);
					}
				}
			}
			location.SetThreshold(type, threshold);
		}

		internal void Refresh()
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\LatencyDetectionOptions"))
			{
				if (registryKey == null)
				{
					this.latencyDetectionEnabled = false;
					this.WatsonThrottle = TimeSpan.FromMinutes(10.0);
					this.InitialWait = TimeSpan.FromMinutes(3.0);
					this.RefreshOptionsInterval = TimeSpan.FromSeconds(30.0);
					this.BacklogRetirementAge = TimeSpan.FromMinutes(10.0);
					this.MaximumBacklogSize = 32U;
					this.EnableLatencyEventLogging = PerformanceReportingOptions.IsLatencyEventLoggingEnabled();
				}
				else
				{
					this.latencyDetectionEnabled = PerformanceReportingOptions.GetRegistryBoolean(registryKey, "Enabled", false);
					this.WatsonThrottle = TimeSpan.FromMinutes(PerformanceReportingOptions.GetRegistryUInt32(registryKey, "WatsonThrottleMinutes", 10U, 10U));
					this.InitialWait = TimeSpan.FromMinutes(PerformanceReportingOptions.GetRegistryUInt32(registryKey, "InitialWaitMinutes", 3U, 0U, 120U));
					this.RefreshOptionsInterval = TimeSpan.FromSeconds(PerformanceReportingOptions.GetRegistryUInt32(registryKey, "RefreshOptionsIntervalSeconds", 30U, 30U, 3600U));
					this.BacklogRetirementAge = TimeSpan.FromMinutes(PerformanceReportingOptions.GetRegistryUInt32(registryKey, "BacklogRetirementAgeMinutes", 10U, 1U, 480U));
					this.MaximumBacklogSize = PerformanceReportingOptions.GetRegistryUInt32(registryKey, "BacklogSizeLimit", 32U, 0U, 256U);
					this.EnableLatencyEventLogging = PerformanceReportingOptions.GetRegistryBoolean(registryKey, "EnableLatencyEventLogging", false);
				}
				lock (this.refreshLockObject)
				{
					this.nextRefresh = DateTime.UtcNow + this.RefreshOptionsInterval;
				}
			}
		}

		private static uint GetTotalMilliseconds(TimeSpan duration)
		{
			double totalMilliseconds = duration.TotalMilliseconds;
			uint result = 2147483647U;
			if (totalMilliseconds < 2147483647.0)
			{
				result = (uint)totalMilliseconds;
			}
			return result;
		}

		private static uint GetRegistryUInt32(RegistryKey key, string name, uint defaultValue, uint minimum, uint maximum)
		{
			uint num = defaultValue;
			int? num2 = key.GetValue(name, defaultValue) as int?;
			if (num2 != null && num2 >= 0)
			{
				num = Math.Max(minimum, (uint)num2.Value);
				num = Math.Min(maximum, num);
			}
			return num;
		}

		private static uint GetRegistryUInt32(RegistryKey key, string name, uint defaultValue, uint minimum)
		{
			return PerformanceReportingOptions.GetRegistryUInt32(key, name, defaultValue, minimum, 2147483647U);
		}

		private static bool GetRegistryBoolean(RegistryKey key, string name, bool defaultValue)
		{
			int? num = key.GetValue(name, defaultValue) as int?;
			if (num == null)
			{
				return defaultValue;
			}
			return num != 0;
		}

		private static bool IsLatencyEventLoggingEnabled()
		{
			return DatacenterRegistry.IsMicrosoftHostedOnly() || DatacenterRegistry.IsDatacenterDedicated();
		}

		private void RefreshWorker(object ignored)
		{
			this.Refresh();
		}

		private void RefreshIfExpired()
		{
			if (this.nextRefresh <= DateTime.UtcNow)
			{
				lock (this.refreshLockObject)
				{
					DateTime utcNow = DateTime.UtcNow;
					if (this.nextRefresh <= utcNow)
					{
						this.nextRefresh = utcNow + this.RefreshOptionsInterval;
						ThreadPool.QueueUserWorkItem(new WaitCallback(this.RefreshWorker));
					}
				}
			}
		}

		internal const bool DefaultLatencyDetectionEnabled = false;

		internal const uint DefaultMinWatsonThrottleMinutes = 10U;

		internal const uint DefaultInitialWaitMinutes = 3U;

		internal const uint MaxInitialWaitMinutes = 120U;

		internal const uint DefaultMinRefreshOptionsIntervalSeconds = 30U;

		internal const uint MaximumRefreshOptionsIntervalSeconds = 3600U;

		internal const uint DefaultBacklogRetirementAgeMinutes = 10U;

		internal const uint MinBacklogRetirementAgeMinutes = 1U;

		internal const uint DefaultBacklogSizeLimit = 32U;

		internal const uint MinimumBacklogSizeLimit = 0U;

		internal const uint MaximumBacklogSizeLimit = 256U;

		internal const uint MaxBacklogRetirementAgeMinutes = 480U;

		internal const string ConfigRegistryKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\LatencyDetectionOptions";

		internal const string ThresholdValueAppend = "ThresholdMilliseconds";

		internal const string Enabled = "Enabled";

		internal const string WatsonThrottleMinutes = "WatsonThrottleMinutes";

		internal const string InitialWaitMinutes = "InitialWaitMinutes";

		internal const string RefreshOptionsIntervalSeconds = "RefreshOptionsIntervalSeconds";

		internal const string BacklogRetirementAgeMinutes = "BacklogRetirementAgeMinutes";

		internal const string BacklogSizeLimit = "BacklogSizeLimit";

		private const string EnableLatencyEventLoggingFlag = "EnableLatencyEventLogging";

		private static PerformanceReportingOptions singletonInstance = new PerformanceReportingOptions();

		private bool latencyDetectionEnabled;

		private DateTime nextRefresh;

		private object refreshLockObject = new object();
	}
}
