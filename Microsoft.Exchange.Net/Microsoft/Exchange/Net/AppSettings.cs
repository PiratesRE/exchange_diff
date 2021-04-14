using System;
using System.Management.Automation;
using System.Web;

namespace Microsoft.Exchange.Net
{
	internal static class AppSettings
	{
		internal static IAppSettings Current
		{
			get
			{
				if (EventLogConstants.IsPowerShellWebService)
				{
					return PswsAppSettings.Instance;
				}
				if (HttpContext.Current != null)
				{
					return AutoLoadAppSettings.Instance;
				}
				if (AppSettings.manualLoadAppSettings == null)
				{
					return DefaultAppSettings.Instance;
				}
				return AppSettings.manualLoadAppSettings;
			}
		}

		internal static bool RpsAuthZAppSettingsInitialized
		{
			get
			{
				return AppSettings.manualLoadAppSettings != null;
			}
		}

		internal static void InitializeManualLoadAppSettings(string connectionUri, Action postLoadAction)
		{
			if (AppSettings.manualLoadAppSettings != null)
			{
				return;
			}
			lock (AppSettings.SyncObj)
			{
				if (AppSettings.manualLoadAppSettings == null)
				{
					AppSettings.manualLoadAppSettings = new ManualLoadAppSettings(connectionUri);
					postLoadAction();
				}
			}
		}

		internal const string PodRedirectTemplateAppSettingKey = "PodRedirectTemplate";

		internal const string SiteRedirectTemplateAppSettingKey = "SiteRedirectTemplate";

		internal const string TenantRedirectionEnabledAppSettingKey = "TenantRedirectionEnabled";

		internal const string RedirectionEnabledAppSettingKey = "RedirectionEnabled";

		internal const string MaxPowershellAppPoolConnectionsAppSettingKey = "MaxPowershellAppPoolConnections";

		internal const string ProvisioningCacheIdentificationAppSettingKey = "ProvisioningCacheIdentification";

		internal const string DedicatedMailboxPlansCustomAttributeNameAppSettingKey = "DedicatedMailboxPlansCustomAttributeName";

		internal const string DedicatedMailboxPlansEnabledAppSettingKey = "DedicatedMailboxPlansEnabled";

		internal const string ShouldShowFismaBannerAppSettingKey = "ShouldShowFismaBanner";

		internal const string MaxWorkerThreadsAppSettingKey = "ThreadPool.MaxWorkerThreads";

		internal const string MaxCompletionPortThreadsAppSettingKey = "ThreadPool.MaxCompletionPortThreads";

		internal const string PSLanguageModeAppSettingKey = "PSLanguageMode";

		internal const string SupportedEMCVersionsAppSettingKey = "SupportedEMCVersions";

		internal const string FailFastEnabledAppSettingKey = "FailFastEnabled";

		internal const string LogSubFolderNameAppSettingKey = "LogSubFolderName";

		internal const string LogEnabledAppSettingKey = "LogEnabled";

		internal const string CustomLogFolderPathAppSettingsKey = "ConfigurationCoreLogger.LogFolder";

		internal const string LogFileAgeInDaysAppSettingKey = "LogFileAgeInDays";

		internal const string MaxLogDirectorySizeInGBAppSettingsKey = "MaxLogDirectorySizeInGB";

		internal const string MaxLogFileSizeInMBAppSettingsKey = "MaxLogFileSizeInMB";

		internal const string LogDirectoryPathAppSettingKey = "LogDirectoryPath";

		internal const string ThresholdToLogActivityLatencyAppSettingsKey = "ThresholdToLogActivityLatency";

		internal const string LogCPUMemoryIntervalInMinutesAppSettingsKey = "LogCPUMemoryIntervalInMinutes";

		internal const string SidsCacheTimeoutInHoursAppSettingKey = "SidsCacheTimeoutInHours";

		internal const string ClientAccessRulesLimitAppSettingsKey = "ClientAccessRulesLimit";

		internal const string MaxCmdletRetryCntAppSettingsKey = "MaxCmdletRetryCnt";

		internal const string DefaultPodRedirectTemplate = null;

		internal const string DefaultSiteRedirectTemplate = null;

		internal const bool DefaultTenantRedirectionEnabled = false;

		internal const bool DefaultRedirectionEnabled = true;

		internal const int DefaultMaxPowershellAppPoolConnections = 0;

		internal const string DefaultProvisioningCacheIdentification = null;

		internal const string DefaultDedicatedMailboxPlansCustomAttributeName = null;

		internal const bool DefaultDedicatedMailboxPlansEnabled = false;

		internal const bool DefaultShouldShowFismaBanner = false;

		internal const string DefaultSupportedEMCVersions = null;

		internal const bool DefaultFailFastEnabled = false;

		internal const string DefaultLogDirectoryPath = null;

		internal const PSLanguageMode DefaultLanguageMode = PSLanguageMode.NoLanguage;

		internal const string DefaultLogSubFolderName = "Others";

		internal const bool DefaultLogEnabled = true;

		internal const int DefaultMaxLogDirectorySizeInGB = 1;

		internal const int DefaultThresholdToLogActivityLatency = 1000;

		internal const int DefaultMaxLogFileSizeInMB = 10;

		internal static readonly int DefaultThreadPoolMaxThreads = Environment.ProcessorCount * 150;

		internal static readonly int DefaultThreadPoolMaxCompletionPorts = Environment.ProcessorCount * 150;

		internal static readonly int DefaultPSMaximumReceivedObjectSizeByte = 78643200;

		internal static readonly int DefaultPSMaximumReceivedDataSizePerCommandByte = 524288000;

		internal static readonly TimeSpan DefaultLogFileAgeInDays = TimeSpan.FromDays(30.0);

		internal static readonly int DefaultLogCPUMemoryIntervalInMinutes = 5;

		internal static readonly TimeSpan DefaultSidsCacheTimeoutInHours = TimeSpan.FromHours(24.0);

		internal static readonly int DefaultClientAccessRulesLimit = 20;

		internal static readonly int DefaultMaxCmdletRetryCnt = 2;

		private static readonly object SyncObj = new object();

		private static ManualLoadAppSettings manualLoadAppSettings;
	}
}
