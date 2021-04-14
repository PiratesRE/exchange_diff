using System;

namespace Microsoft.Exchange.EdgeSync.Ehf
{
	internal class EhfSyncAppConfig
	{
		public virtual int BatchSize
		{
			get
			{
				return EhfSyncAppConfig.AppConfigBatchSize;
			}
		}

		public virtual int MaxMessageSize
		{
			get
			{
				return EhfSyncAppConfig.AppConfigMaxMessageSize;
			}
		}

		public virtual int TransientExceptionRetryCount
		{
			get
			{
				return EhfSyncAppConfig.AppConfigTransientExceptionRetryCount;
			}
		}

		public virtual int EhfAdminSyncMaxAccumulatedChangeSize
		{
			get
			{
				return EhfSyncAppConfig.AppConfigEhfAdminSyncMaxAccumulatedChangeSize;
			}
		}

		public virtual int EhfAdminSyncMaxAccumulatedDeleteChangeSize
		{
			get
			{
				return EhfSyncAppConfig.AppConfigEhfAdminSyncMaxAccumulatedDeleteChangeSize;
			}
		}

		public virtual int EhfAdminSyncMaxFailureCount
		{
			get
			{
				return EhfSyncAppConfig.AppConfigEhfAdminSyncMaxFailureCount;
			}
		}

		public virtual int EhfAdminSyncMaxTargetAdminStateSize
		{
			get
			{
				return EhfSyncAppConfig.AppConfigEhfAdminSyncMaxTargetAdminStateSize;
			}
		}

		public virtual int EhfAdminSyncTransientFailureRetryThreshold
		{
			get
			{
				return EhfSyncAppConfig.AppConfigEhfAdminSyncTransientFailureRetryThreshold;
			}
		}

		public virtual TimeSpan EhfAdminSyncInterval
		{
			get
			{
				return EhfSyncAppConfig.AppConfigEhfAdminSyncInterval;
			}
		}

		public virtual TimeSpan RequestTimeout
		{
			get
			{
				return EhfSyncAppConfig.AppConfigRequestTimeout;
			}
		}

		private static int GetMaxMessageSizeFromAppConfig()
		{
			return EhfSyncAppConfig.GetConfigInt("EhfMaxMessageSize", 10240, 256000, 102400);
		}

		private static int GetBatchSizeFromAppConfig()
		{
			return EhfSyncAppConfig.GetConfigInt("EhfBatchSize", 1, 100, 20);
		}

		private static TimeSpan GetRequestTimeoutFromAppConfig()
		{
			TimeSpan lowerBound = TimeSpan.FromSeconds(1.0);
			TimeSpan upperBound = TimeSpan.FromMinutes(60.0);
			return EhfSyncAppConfig.GetConfigTimeSpan("EhfRequestTimeout", lowerBound, upperBound, EhfSyncAppConfig.DefaultRequestTimeout);
		}

		private static int GetTransientExceptionRetryCountFromAppConfig()
		{
			return EhfSyncAppConfig.GetConfigInt("TransientExceptionRetryCount", 0, 100, 3);
		}

		private static int GetAdminSyncTransientFailureRetryThresholdFromAppConfig()
		{
			return EhfSyncAppConfig.GetConfigInt("EhfAdminSyncTransientFailureRetryThreshold", 0, int.MaxValue, 10);
		}

		private static int GetAdminSyncMaxFailureCountFromAppConfig()
		{
			return EhfSyncAppConfig.GetConfigInt("EhfAdminSyncMaxFailureCount", 0, int.MaxValue, 10);
		}

		private static int GetAdminSyncMaxTargetAdminStateSizeFromAppConfig()
		{
			return EhfSyncAppConfig.GetConfigInt("EhfAdminSyncMaxTargetAdminStateSize", 0, 400, 50);
		}

		private static int GetEhfAdminSyncMaxAccumulatedChangeSizeFromAppConfig()
		{
			return EhfSyncAppConfig.GetConfigInt("EhfAdminSyncMaxAccumulatedChangeSize", 0, int.MaxValue, 100);
		}

		private static int GetEhfAdminSyncMaxAccumulatedDeleteChangeSizeFromAppConfig()
		{
			return EhfSyncAppConfig.GetConfigInt("EhfAdminSyncMaxAccumulatedDeleteChangeSize", 0, int.MaxValue, 1000);
		}

		private static TimeSpan GetEhfAdminSyncIntervalFromAppConfig()
		{
			TimeSpan lowerBound = TimeSpan.FromSeconds(1.0);
			TimeSpan maxValue = TimeSpan.MaxValue;
			return EhfSyncAppConfig.GetConfigTimeSpan("EhfAdminSyncInterval", lowerBound, maxValue, EhfSyncAppConfig.DefaultEhfAdminSyncInterval);
		}

		private static int GetConfigInt(string label, int lowerBound, int upperBound, int defaultValue)
		{
			if (EdgeSyncSvc.EdgeSync == null || EdgeSyncSvc.EdgeSync.AppConfig == null)
			{
				return defaultValue;
			}
			return EdgeSyncSvc.EdgeSync.AppConfig.GetConfigInt(label, lowerBound, upperBound, defaultValue);
		}

		private static TimeSpan GetConfigTimeSpan(string label, TimeSpan lowerBound, TimeSpan upperBound, TimeSpan defaultValue)
		{
			if (EdgeSyncSvc.EdgeSync == null || EdgeSyncSvc.EdgeSync.AppConfig == null)
			{
				return defaultValue;
			}
			return EdgeSyncSvc.EdgeSync.AppConfig.GetConfigTimeSpan(label, lowerBound, upperBound, defaultValue);
		}

		private const int DefaultMaxMessageSize = 102400;

		private const int DefaultBatchSize = 20;

		private const int DefaultEhfAdminSyncTransientFailureRetryThreshold = 10;

		private const int DefaultEhfAdminSyncMaxFailureCount = 10;

		private const int DefaultEhfAdminSyncMaxTargetAdminStateSize = 50;

		private const int DefaultEhfAdminSyncMaxAccumulatedChangeSize = 100;

		private const int DefaultEhfAdminSyncMaxAccumulatedDeleteChangeSize = 1000;

		public static readonly TimeSpan AppConfigEhfAdminSyncInterval = EhfSyncAppConfig.GetEhfAdminSyncIntervalFromAppConfig();

		public static readonly int AppConfigEhfAdminSyncMaxFailureCount = EhfSyncAppConfig.GetAdminSyncMaxFailureCountFromAppConfig();

		public static readonly int AppConfigEhfAdminSyncMaxTargetAdminStateSize = EhfSyncAppConfig.GetAdminSyncMaxTargetAdminStateSizeFromAppConfig();

		public static readonly int AppConfigEhfAdminSyncTransientFailureRetryThreshold = EhfSyncAppConfig.GetAdminSyncTransientFailureRetryThresholdFromAppConfig();

		public static readonly int AppConfigEhfAdminSyncMaxAccumulatedChangeSize = EhfSyncAppConfig.GetEhfAdminSyncMaxAccumulatedChangeSizeFromAppConfig();

		public static readonly int AppConfigEhfAdminSyncMaxAccumulatedDeleteChangeSize = EhfSyncAppConfig.GetEhfAdminSyncMaxAccumulatedDeleteChangeSizeFromAppConfig();

		private static readonly TimeSpan DefaultRequestTimeout = TimeSpan.FromMinutes(2.0);

		private static readonly TimeSpan DefaultEhfAdminSyncInterval = TimeSpan.FromMinutes(3.0);

		private static readonly int AppConfigMaxMessageSize = EhfSyncAppConfig.GetMaxMessageSizeFromAppConfig();

		private static readonly int AppConfigBatchSize = EhfSyncAppConfig.GetBatchSizeFromAppConfig();

		private static readonly TimeSpan AppConfigRequestTimeout = EhfSyncAppConfig.GetRequestTimeoutFromAppConfig();

		private static readonly int AppConfigTransientExceptionRetryCount = EhfSyncAppConfig.GetTransientExceptionRetryCountFromAppConfig();
	}
}
