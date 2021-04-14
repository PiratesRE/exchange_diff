using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Net
{
	internal interface IAppSettings
	{
		string PodRedirectTemplate { get; }

		string SiteRedirectTemplate { get; }

		bool TenantRedirectionEnabled { get; }

		bool RedirectionEnabled { get; }

		int MaxPowershellAppPoolConnections { get; }

		string ProvisioningCacheIdentification { get; }

		string DedicatedMailboxPlansCustomAttributeName { get; }

		bool DedicatedMailboxPlansEnabled { get; }

		bool ShouldShowFismaBanner { get; }

		int ThreadPoolMaxThreads { get; }

		int ThreadPoolMaxCompletionPorts { get; }

		PSLanguageMode PSLanguageMode { get; }

		string SupportedEMCVersions { get; }

		bool FailFastEnabled { get; }

		int PSMaximumReceivedObjectSizeMB { get; }

		int PSMaximumReceivedDataSizePerCommandMB { get; }

		string LogSubFolderName { get; }

		bool LogEnabled { get; }

		string LogDirectoryPath { get; }

		TimeSpan LogFileAgeInDays { get; }

		int MaxLogDirectorySizeInGB { get; }

		int MaxLogFileSizeInMB { get; }

		int ThresholdToLogActivityLatency { get; }

		int MaxCmdletRetryCnt { get; }

		string WebSiteName { get; }

		string VDirName { get; }

		string ConfigurationFilePath { get; }

		int LogCPUMemoryIntervalInMinutes { get; }

		TimeSpan SidsCacheTimeoutInHours { get; }

		int ClientAccessRulesLimit { get; }
	}
}
