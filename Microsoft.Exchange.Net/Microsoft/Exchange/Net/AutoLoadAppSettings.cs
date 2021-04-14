using System;
using System.Management.Automation;
using Microsoft.Exchange.Diagnostics.Components.Configuration.Core;

namespace Microsoft.Exchange.Net
{
	public class AutoLoadAppSettings : IAppSettings
	{
		protected AutoLoadAppSettings()
		{
		}

		public static AutoLoadAppSettings Instance
		{
			get
			{
				AutoLoadAppSettings result;
				if ((result = AutoLoadAppSettings.instance) == null)
				{
					result = (AutoLoadAppSettings.instance = new AutoLoadAppSettings());
				}
				return result;
			}
		}

		string IAppSettings.PodRedirectTemplate
		{
			get
			{
				return AutoLoadAppSettings.PopRedirectTemplateEntry.Value;
			}
		}

		string IAppSettings.SiteRedirectTemplate
		{
			get
			{
				return AutoLoadAppSettings.SiteRedirectTemplateEntry.Value;
			}
		}

		bool IAppSettings.TenantRedirectionEnabled
		{
			get
			{
				return AutoLoadAppSettings.TenantRedirectionEnabledEntry.Value;
			}
		}

		bool IAppSettings.RedirectionEnabled
		{
			get
			{
				return AutoLoadAppSettings.RedirectionEnabledEntry.Value;
			}
		}

		int IAppSettings.MaxPowershellAppPoolConnections
		{
			get
			{
				throw new NotSupportedException("MaxPowershellAppPoolConnections is not supposed to be used in AutoLoadAppSettings.");
			}
		}

		string IAppSettings.ProvisioningCacheIdentification
		{
			get
			{
				return AutoLoadAppSettings.ProvisioningCacheIdentificationEntry.Value;
			}
		}

		string IAppSettings.DedicatedMailboxPlansCustomAttributeName
		{
			get
			{
				return AutoLoadAppSettings.DedicatedMailboxPlansCustomAttributeNameEntry.Value;
			}
		}

		bool IAppSettings.DedicatedMailboxPlansEnabled
		{
			get
			{
				return AutoLoadAppSettings.DedicatedMailboxPlansEnabledEntry.Value;
			}
		}

		bool IAppSettings.ShouldShowFismaBanner
		{
			get
			{
				return AutoLoadAppSettings.ShouldShowFismaBannerEntry.Value;
			}
		}

		int IAppSettings.ThreadPoolMaxThreads
		{
			get
			{
				throw new NotSupportedException("ThreadPoolMaxThreads is not supposed to be used in RemotePS AutoLoadAppSettings.");
			}
		}

		int IAppSettings.ThreadPoolMaxCompletionPorts
		{
			get
			{
				throw new NotSupportedException("ThreadPoolMaxCompletionPorts is not supposed to be used in RemotePS AutoLoadAppSettings.");
			}
		}

		PSLanguageMode IAppSettings.PSLanguageMode
		{
			get
			{
				throw new NotSupportedException("PSLanguageMode is not supposed to be used in RemotePS AutoLoadAppSettings.");
			}
		}

		string IAppSettings.SupportedEMCVersions
		{
			get
			{
				throw new NotSupportedException("SupportedEMCVersions is not supposed to be used in RemotePS AutoLoadAppSettings.");
			}
		}

		bool IAppSettings.FailFastEnabled
		{
			get
			{
				return AutoLoadAppSettings.FailFastEnabledEntry.Value;
			}
		}

		int IAppSettings.PSMaximumReceivedObjectSizeMB
		{
			get
			{
				throw new NotSupportedException("PSMaximumReceivedObjectSizeMB is not supposed to be used in RemotePS AutoLoadAppSettings.");
			}
		}

		int IAppSettings.PSMaximumReceivedDataSizePerCommandMB
		{
			get
			{
				throw new NotSupportedException("PSMaximumReceivedDataSizePerCommandMB is not supposed to be used in RemotePS AutoLoadAppSettings.");
			}
		}

		string IAppSettings.WebSiteName
		{
			get
			{
				throw new NotSupportedException("WebSiteName is not supposed to be used in RemotePS AutoLoadAppSettings.");
			}
		}

		string IAppSettings.VDirName
		{
			get
			{
				throw new NotSupportedException("VDirName is not supposed to be used in RemotePS AutoLoadAppSettings.");
			}
		}

		string IAppSettings.ConfigurationFilePath
		{
			get
			{
				throw new NotSupportedException("ConfigurationFilePath is not supposed to be used in RemotePS AutoLoadAppSettings.");
			}
		}

		string IAppSettings.LogSubFolderName
		{
			get
			{
				return AutoLoadAppSettings.LogSubFolderNameEntry.Value;
			}
		}

		bool IAppSettings.LogEnabled
		{
			get
			{
				return AutoLoadAppSettings.LogEnabledEntry.Value;
			}
		}

		string IAppSettings.LogDirectoryPath
		{
			get
			{
				return AutoLoadAppSettings.LogDirectoryPathEntry.Value;
			}
		}

		int IAppSettings.MaxLogDirectorySizeInGB
		{
			get
			{
				return AutoLoadAppSettings.MaxLogDirectorySizeInGBEntry.Value;
			}
		}

		int IAppSettings.MaxLogFileSizeInMB
		{
			get
			{
				return AutoLoadAppSettings.MaxLogFileSizeInMBEntry.Value;
			}
		}

		TimeSpan IAppSettings.LogFileAgeInDays
		{
			get
			{
				return AutoLoadAppSettings.LogFileAgeInDaysEntry.Value;
			}
		}

		int IAppSettings.ThresholdToLogActivityLatency
		{
			get
			{
				return AutoLoadAppSettings.ThresholdToLogActivityLatencyEntry.Value;
			}
		}

		int IAppSettings.LogCPUMemoryIntervalInMinutes
		{
			get
			{
				return AutoLoadAppSettings.LogCPUMemoryIntervalInMinutesEntry.Value;
			}
		}

		public TimeSpan SidsCacheTimeoutInHours
		{
			get
			{
				return AutoLoadAppSettings.SidsCacheTimeoutInHoursEntry.Value;
			}
		}

		int IAppSettings.ClientAccessRulesLimit
		{
			get
			{
				return AutoLoadAppSettings.ClientAccessRulesLimitEntry.Value;
			}
		}

		int IAppSettings.MaxCmdletRetryCnt
		{
			get
			{
				int value = AutoLoadAppSettings.MaxCmdletRetryCntEntry.Value;
				if (value >= 0)
				{
					return value;
				}
				return 0;
			}
		}

		private static readonly StringAppSettingsEntry PopRedirectTemplateEntry = new StringAppSettingsEntry("PodRedirectTemplate", null, ExTraceGlobals.InstrumentationTracer);

		private static readonly StringAppSettingsEntry SiteRedirectTemplateEntry = new StringAppSettingsEntry("SiteRedirectTemplate", null, ExTraceGlobals.InstrumentationTracer);

		private static readonly StringAppSettingsEntry ProvisioningCacheIdentificationEntry = new StringAppSettingsEntry("ProvisioningCacheIdentification", null, ExTraceGlobals.InstrumentationTracer);

		private static readonly StringAppSettingsEntry DedicatedMailboxPlansCustomAttributeNameEntry = new StringAppSettingsEntry("DedicatedMailboxPlansCustomAttributeName", null, ExTraceGlobals.InstrumentationTracer);

		private static readonly BoolAppSettingsEntry DedicatedMailboxPlansEnabledEntry = new BoolAppSettingsEntry("DedicatedMailboxPlansEnabled", false, ExTraceGlobals.InstrumentationTracer);

		private static readonly BoolAppSettingsEntry TenantRedirectionEnabledEntry = new BoolAppSettingsEntry("TenantRedirectionEnabled", false, ExTraceGlobals.InstrumentationTracer);

		private static readonly BoolAppSettingsEntry RedirectionEnabledEntry = new BoolAppSettingsEntry("RedirectionEnabled", true, ExTraceGlobals.InstrumentationTracer);

		private static readonly BoolAppSettingsEntry FailFastEnabledEntry = new BoolAppSettingsEntry("FailFastEnabled", false, ExTraceGlobals.InstrumentationTracer);

		private static readonly StringAppSettingsEntry LogSubFolderNameEntry = new StringAppSettingsEntry("LogSubFolderName", "Others", ExTraceGlobals.InstrumentationTracer);

		private static readonly BoolAppSettingsEntry LogEnabledEntry = new BoolAppSettingsEntry("LogEnabled", true, ExTraceGlobals.InstrumentationTracer);

		private static readonly StringAppSettingsEntry LogDirectoryPathEntry = new StringAppSettingsEntry("LogDirectoryPath", null, ExTraceGlobals.InstrumentationTracer);

		private static readonly TimeSpanAppSettingsEntry LogFileAgeInDaysEntry = new TimeSpanAppSettingsEntry("LogFileAgeInDays", TimeSpanUnit.Days, AppSettings.DefaultLogFileAgeInDays, ExTraceGlobals.InstrumentationTracer);

		private static readonly IntAppSettingsEntry MaxLogDirectorySizeInGBEntry = new IntAppSettingsEntry("MaxLogDirectorySizeInGB", 1, ExTraceGlobals.InstrumentationTracer);

		private static readonly IntAppSettingsEntry MaxLogFileSizeInMBEntry = new IntAppSettingsEntry("MaxLogFileSizeInMB", 10, ExTraceGlobals.InstrumentationTracer);

		private static readonly IntAppSettingsEntry ThresholdToLogActivityLatencyEntry = new IntAppSettingsEntry("ThresholdToLogActivityLatency", 1000, ExTraceGlobals.InstrumentationTracer);

		private static readonly BoolAppSettingsEntry ShouldShowFismaBannerEntry = new BoolAppSettingsEntry("ShouldShowFismaBanner", false, ExTraceGlobals.InstrumentationTracer);

		private static readonly IntAppSettingsEntry LogCPUMemoryIntervalInMinutesEntry = new IntAppSettingsEntry("LogCPUMemoryIntervalInMinutes", AppSettings.DefaultLogCPUMemoryIntervalInMinutes, ExTraceGlobals.InstrumentationTracer);

		private static readonly TimeSpanAppSettingsEntry SidsCacheTimeoutInHoursEntry = new TimeSpanAppSettingsEntry("SidsCacheTimeoutInHours", TimeSpanUnit.Hours, AppSettings.DefaultSidsCacheTimeoutInHours, ExTraceGlobals.InstrumentationTracer);

		private static readonly IntAppSettingsEntry ClientAccessRulesLimitEntry = new IntAppSettingsEntry("ClientAccessRulesLimit", AppSettings.DefaultClientAccessRulesLimit, ExTraceGlobals.InstrumentationTracer);

		private static readonly IntAppSettingsEntry MaxCmdletRetryCntEntry = new IntAppSettingsEntry("MaxCmdletRetryCnt", AppSettings.DefaultMaxCmdletRetryCnt, ExTraceGlobals.InstrumentationTracer);

		private static AutoLoadAppSettings instance;
	}
}
