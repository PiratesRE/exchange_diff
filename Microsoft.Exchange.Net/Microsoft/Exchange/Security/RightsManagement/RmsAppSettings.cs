using System;
using System.Configuration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Security.RightsManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RmsAppSettings
	{
		private RmsAppSettings()
		{
			this.activeAgentCapDeferInterval = RmsAppSettings.GetConfigTimeSpan("RmsActiveAgentCapDeferInterval", TimeSpan.FromSeconds(1.0), TimeSpan.FromHours(1.0), TimeSpan.FromSeconds(10.0));
			this.transientErrorDeferInterval = RmsAppSettings.GetConfigTimeSpan("RmsTransientErrorDeferInterval", TimeSpan.FromSeconds(1.0), TimeSpan.FromHours(1.0), TimeSpan.FromSeconds(10.0));
			this.encryptionTransientErrorDeferInterval = RmsAppSettings.GetConfigTimeSpan("RmsEncryptionTransientErrorDeferInterval", TimeSpan.FromSeconds(1.0), TimeSpan.FromHours(1.0), TimeSpan.FromMinutes(10.0));
			this.jrdTransientErrorDeferInterval = RmsAppSettings.GetConfigTimeSpan("RmsJrdTransientErrorDeferInterval", TimeSpan.FromSeconds(1.0), TimeSpan.FromHours(1.0), TimeSpan.FromMinutes(5.0));
			int configInt = RmsAppSettings.GetConfigInt("RmsRacClcCacheSizeInMB", 1, 50, 10);
			this.racClcCacheSizeInBytes = (long)(configInt * 1048576);
			this.racClcCacheExpirationInterval = RmsAppSettings.GetConfigTimeSpan("RmsRacClcCacheExpirationInterval", TimeSpan.FromMinutes(1.0), TimeSpan.FromDays(30.0), TimeSpan.FromDays(10.0));
			this.racClcStoreExpirationInterval = RmsAppSettings.GetConfigTimeSpan("RmsRacClcStoreExpirationInterval", TimeSpan.FromMinutes(1.0), TimeSpan.FromDays(365.0), TimeSpan.FromMinutes(0.0));
			this.maxRacClcEntryCount = RmsAppSettings.GetConfigInt("RmsMaxRacClcEntryCount", 1, 10000, 5000);
			this.maxServerInfoEntryCount = RmsAppSettings.GetConfigInt("RmsMaxServerInfoEntryCount", 1, 10000, 5000);
			int configInt2 = RmsAppSettings.GetConfigInt("RmsTemplateCacheSizeInMB", 1, 50, 20);
			this.templateCacheSizeInBytes = (long)(configInt2 * 1048576);
			this.templateCacheExpirationInterval = RmsAppSettings.GetConfigTimeSpan("RmsTemplateCacheExpirationInterval", TimeSpan.FromMinutes(1.0), TimeSpan.FromDays(1.0), TimeSpan.FromDays(1.0));
			this.negativeServerInfoCacheExpirationInterval = RmsAppSettings.GetConfigTimeSpan("RmsNegativeServerInfoCacheExpirationInterval", TimeSpan.FromMinutes(1.0), TimeSpan.FromDays(1.0), TimeSpan.FromMinutes(5.0));
			this.timeoutForRmsSoapQueries = RmsAppSettings.GetConfigTimeSpan("RmsSoapQueriesTimeOut", TimeSpan.FromSeconds(10.0), TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(30.0));
			this.isSamlAuthenticationEnabledForInternalRMS = RmsAppSettings.GetConfigBool("RmsEnableSamlAuthenticationforInternalRMS", false);
			this.acquireLicenseBatchSize = RmsAppSettings.GetConfigInt("AcquireLicenseBatchSize", 1, 100, 100);
		}

		public static RmsAppSettings Instance
		{
			get
			{
				RmsAppSettings rmsAppSettings = RmsAppSettings.instance;
				if (rmsAppSettings == null)
				{
					lock (RmsAppSettings.syncRoot)
					{
						rmsAppSettings = RmsAppSettings.instance;
						if (rmsAppSettings == null)
						{
							rmsAppSettings = new RmsAppSettings();
							RmsAppSettings.instance = rmsAppSettings;
						}
					}
				}
				return rmsAppSettings;
			}
		}

		public TimeSpan TransientErrorDeferInterval
		{
			get
			{
				return this.transientErrorDeferInterval;
			}
		}

		public TimeSpan EncryptionTransientErrorDeferInterval
		{
			get
			{
				return this.encryptionTransientErrorDeferInterval;
			}
		}

		public TimeSpan JrdTransientErrorDeferInterval
		{
			get
			{
				return this.jrdTransientErrorDeferInterval;
			}
		}

		public TimeSpan ActiveAgentCapDeferInterval
		{
			get
			{
				return this.activeAgentCapDeferInterval;
			}
		}

		public long RacClcCacheSizeInBytes
		{
			get
			{
				return this.racClcCacheSizeInBytes;
			}
		}

		public TimeSpan RacClcCacheExpirationInterval
		{
			get
			{
				return this.racClcCacheExpirationInterval;
			}
		}

		public TimeSpan RacClcStoreExpirationInterval
		{
			get
			{
				return this.racClcStoreExpirationInterval;
			}
		}

		public int MaxRacClcEntryCount
		{
			get
			{
				return this.maxRacClcEntryCount;
			}
		}

		public int MaxServerInfoEntryCount
		{
			get
			{
				return this.maxServerInfoEntryCount;
			}
		}

		public long TemplateCacheSizeInBytes
		{
			get
			{
				return this.templateCacheSizeInBytes;
			}
		}

		public TimeSpan TemplateCacheExpirationInterval
		{
			get
			{
				return this.templateCacheExpirationInterval;
			}
		}

		public TimeSpan NegativeServerInfoCacheExpirationInterval
		{
			get
			{
				return this.negativeServerInfoCacheExpirationInterval;
			}
		}

		public TimeSpan RmsSoapQueriesTimeout
		{
			get
			{
				return this.timeoutForRmsSoapQueries;
			}
		}

		public bool IsSamlAuthenticationEnabledForInternalRMS
		{
			get
			{
				return this.isSamlAuthenticationEnabledForInternalRMS;
			}
			set
			{
				this.isSamlAuthenticationEnabledForInternalRMS = value;
			}
		}

		public int AcquireLicenseBatchSize
		{
			get
			{
				return this.acquireLicenseBatchSize;
			}
		}

		private static TimeSpan GetConfigTimeSpan(string label, TimeSpan min, TimeSpan max, TimeSpan defaultValue)
		{
			string text;
			try
			{
				text = ConfigurationManager.AppSettings[label];
			}
			catch (ConfigurationErrorsException)
			{
				return defaultValue;
			}
			TimeSpan timeSpan = defaultValue;
			if (string.IsNullOrEmpty(text) || !TimeSpan.TryParse(text, out timeSpan) || timeSpan < min || timeSpan > max)
			{
				return defaultValue;
			}
			return timeSpan;
		}

		private static int GetConfigInt(string label, int min, int max, int defaultValue)
		{
			string text;
			try
			{
				text = ConfigurationManager.AppSettings[label];
			}
			catch (ConfigurationErrorsException)
			{
				return defaultValue;
			}
			int num = defaultValue;
			if (string.IsNullOrEmpty(text) || !int.TryParse(text, out num) || num < min || num > max)
			{
				return defaultValue;
			}
			return num;
		}

		private static bool GetConfigBool(string label, bool defaultValue)
		{
			string value;
			try
			{
				value = ConfigurationManager.AppSettings[label];
			}
			catch (ConfigurationErrorsException)
			{
				return defaultValue;
			}
			bool result;
			if (!bool.TryParse(value, out result))
			{
				return defaultValue;
			}
			return result;
		}

		private const int NumberOfBytesPerMB = 1048576;

		private static object syncRoot = new object();

		private static RmsAppSettings instance;

		private TimeSpan transientErrorDeferInterval;

		private TimeSpan encryptionTransientErrorDeferInterval;

		private TimeSpan jrdTransientErrorDeferInterval;

		private TimeSpan activeAgentCapDeferInterval;

		private long racClcCacheSizeInBytes;

		private TimeSpan racClcCacheExpirationInterval;

		private TimeSpan racClcStoreExpirationInterval;

		private int maxRacClcEntryCount;

		private int maxServerInfoEntryCount;

		private long templateCacheSizeInBytes;

		private TimeSpan templateCacheExpirationInterval;

		private TimeSpan negativeServerInfoCacheExpirationInterval;

		private TimeSpan timeoutForRmsSoapQueries;

		private bool isSamlAuthenticationEnabledForInternalRMS;

		private int acquireLicenseBatchSize;
	}
}
