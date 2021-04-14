using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Net
{
	internal class DefaultAppSettings : IAppSettings
	{
		private DefaultAppSettings()
		{
		}

		public static DefaultAppSettings Instance
		{
			get
			{
				DefaultAppSettings result;
				if ((result = DefaultAppSettings.instance) == null)
				{
					result = (DefaultAppSettings.instance = new DefaultAppSettings());
				}
				return result;
			}
		}

		string IAppSettings.PodRedirectTemplate
		{
			get
			{
				return null;
			}
		}

		string IAppSettings.SiteRedirectTemplate
		{
			get
			{
				return null;
			}
		}

		bool IAppSettings.TenantRedirectionEnabled
		{
			get
			{
				return false;
			}
		}

		bool IAppSettings.RedirectionEnabled
		{
			get
			{
				return true;
			}
		}

		int IAppSettings.MaxPowershellAppPoolConnections
		{
			get
			{
				return 0;
			}
		}

		string IAppSettings.ProvisioningCacheIdentification
		{
			get
			{
				return null;
			}
		}

		string IAppSettings.DedicatedMailboxPlansCustomAttributeName
		{
			get
			{
				return null;
			}
		}

		bool IAppSettings.DedicatedMailboxPlansEnabled
		{
			get
			{
				return false;
			}
		}

		bool IAppSettings.ShouldShowFismaBanner
		{
			get
			{
				return false;
			}
		}

		int IAppSettings.ThreadPoolMaxThreads
		{
			get
			{
				return AppSettings.DefaultThreadPoolMaxThreads;
			}
		}

		int IAppSettings.ThreadPoolMaxCompletionPorts
		{
			get
			{
				return AppSettings.DefaultThreadPoolMaxCompletionPorts;
			}
		}

		PSLanguageMode IAppSettings.PSLanguageMode
		{
			get
			{
				return PSLanguageMode.NoLanguage;
			}
		}

		string IAppSettings.SupportedEMCVersions
		{
			get
			{
				return null;
			}
		}

		bool IAppSettings.FailFastEnabled
		{
			get
			{
				return false;
			}
		}

		int IAppSettings.PSMaximumReceivedObjectSizeMB
		{
			get
			{
				return AppSettings.DefaultPSMaximumReceivedObjectSizeByte;
			}
		}

		int IAppSettings.PSMaximumReceivedDataSizePerCommandMB
		{
			get
			{
				return AppSettings.DefaultPSMaximumReceivedDataSizePerCommandByte;
			}
		}

		string IAppSettings.LogSubFolderName
		{
			get
			{
				return "Others";
			}
		}

		bool IAppSettings.LogEnabled
		{
			get
			{
				return true;
			}
		}

		string IAppSettings.LogDirectoryPath
		{
			get
			{
				return null;
			}
		}

		TimeSpan IAppSettings.LogFileAgeInDays
		{
			get
			{
				return AppSettings.DefaultLogFileAgeInDays;
			}
		}

		int IAppSettings.MaxLogDirectorySizeInGB
		{
			get
			{
				return 1;
			}
		}

		int IAppSettings.MaxLogFileSizeInMB
		{
			get
			{
				return 10;
			}
		}

		int IAppSettings.ThresholdToLogActivityLatency
		{
			get
			{
				return 1000;
			}
		}

		string IAppSettings.WebSiteName
		{
			get
			{
				throw new InvalidOperationException("WebSiteName is not supported in DefaultAppSettings.");
			}
		}

		string IAppSettings.VDirName
		{
			get
			{
				throw new InvalidOperationException("VDirName is not supported in DefaultAppSettings.");
			}
		}

		string IAppSettings.ConfigurationFilePath
		{
			get
			{
				throw new InvalidOperationException("ConfigurationFilePath is not supported in DefaultAppSettings.");
			}
		}

		int IAppSettings.LogCPUMemoryIntervalInMinutes
		{
			get
			{
				return AppSettings.DefaultLogCPUMemoryIntervalInMinutes;
			}
		}

		TimeSpan IAppSettings.SidsCacheTimeoutInHours
		{
			get
			{
				return AppSettings.DefaultSidsCacheTimeoutInHours;
			}
		}

		int IAppSettings.ClientAccessRulesLimit
		{
			get
			{
				return AppSettings.DefaultClientAccessRulesLimit;
			}
		}

		int IAppSettings.MaxCmdletRetryCnt
		{
			get
			{
				return AppSettings.DefaultMaxCmdletRetryCnt;
			}
		}

		private static DefaultAppSettings instance;
	}
}
