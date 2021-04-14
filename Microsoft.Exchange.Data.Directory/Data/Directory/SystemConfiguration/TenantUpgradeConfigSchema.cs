using System;
using System.Configuration;
using Microsoft.Exchange.Data.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class TenantUpgradeConfigSchema : ConfigSchemaBase
	{
		public override string Name
		{
			get
			{
				return "TenantUpgrade";
			}
		}

		public override string SectionName
		{
			get
			{
				return "TenantUpgradeConfiguration";
			}
		}

		[ConfigurationProperty("IsUpgradingBrokerEnabled", DefaultValue = false)]
		public bool IsUpgradingBrokerEnabled
		{
			get
			{
				return (bool)base["IsUpgradingBrokerEnabled"];
			}
			set
			{
				base["IsUpgradingBrokerEnabled"] = value;
			}
		}

		[ConfigurationProperty("MaxConcurrentUpgradingThreadsPerServer", DefaultValue = "4")]
		public uint MaxConcurrentUpgradingThreadsPerServer
		{
			get
			{
				return (uint)base["MaxConcurrentUpgradingThreadsPerServer"];
			}
			set
			{
				base["MaxConcurrentUpgradingThreadsPerServer"] = value;
			}
		}

		[ConfigurationProperty("UpgradingBrokerPollIntervalInMinutes", DefaultValue = "1440")]
		public uint UpgradingBrokerPollIntervalInMinutes
		{
			get
			{
				return (uint)base["UpgradingBrokerPollIntervalInMinutes"];
			}
			set
			{
				base["UpgradingBrokerPollIntervalInMinutes"] = value;
			}
		}

		[ConfigurationProperty("GetWorkItemsRetryIntervalInMinutes", DefaultValue = "2")]
		public uint GetWorkItemsRetryIntervalInMinutes
		{
			get
			{
				return (uint)base["GetWorkItemsRetryIntervalInMinutes"];
			}
			set
			{
				base["GetWorkItemsRetryIntervalInMinutes"] = value;
			}
		}

		[ConfigurationProperty("StartUpgradeRetries", DefaultValue = "2")]
		public uint StartUpgradeRetries
		{
			get
			{
				return (uint)base["StartUpgradeRetries"];
			}
			set
			{
				base["StartUpgradeRetries"] = value;
			}
		}

		[ConfigurationProperty("StartUpgradeThresholdInSeconds", DefaultValue = "120")]
		public uint StartUpgradeThresholdInSeconds
		{
			get
			{
				return (uint)base["StartUpgradeThresholdInSeconds"];
			}
			set
			{
				base["StartUpgradeThresholdInSeconds"] = value;
			}
		}

		[ConfigurationProperty("DelayBeforeCompletionInSeconds", DefaultValue = "30")]
		public uint DelayBeforeCompletionInSeconds
		{
			get
			{
				return (uint)base["DelayBeforeCompletionInSeconds"];
			}
			set
			{
				base["DelayBeforeCompletionInSeconds"] = value;
			}
		}

		[ConfigurationProperty("ExpectedMajorVersion", DefaultValue = "15")]
		public uint ExpectedMajorVersion
		{
			get
			{
				return (uint)base["ExpectedMajorVersion"];
			}
			set
			{
				base["ExpectedMajorVersion"] = value;
			}
		}

		[ConfigurationProperty("ExpectedMinorVersion", DefaultValue = "0")]
		public uint ExpectedMinorVersion
		{
			get
			{
				return (uint)base["ExpectedMinorVersion"];
			}
			set
			{
				base["ExpectedMinorVersion"] = value;
			}
		}

		[ConfigurationProperty("SkipIfExpectedVersionDoesntMatchAssembly", DefaultValue = true)]
		public bool SkipIfExpectedVersionDoesntMatchAssembly
		{
			get
			{
				return (bool)base["SkipIfExpectedVersionDoesntMatchAssembly"];
			}
			set
			{
				base["SkipIfExpectedVersionDoesntMatchAssembly"] = value;
			}
		}

		[ConfigurationProperty("UpgradeSchedule", DefaultValue = "Never")]
		public string UpgradeSchedule
		{
			get
			{
				return (string)base["UpgradeSchedule"];
			}
			set
			{
				base["UpgradeSchedule"] = value;
			}
		}

		[ConfigurationProperty("PreventServiceStopIfUpgradeInProgress", DefaultValue = false)]
		public bool PreventServiceStopIfUpgradeInProgress
		{
			get
			{
				return (bool)base["PreventServiceStopIfUpgradeInProgress"];
			}
			set
			{
				base["PreventServiceStopIfUpgradeInProgress"] = value;
			}
		}

		[ConfigurationProperty("StuckTenantDetectionThresholdInMinutes", DefaultValue = "60")]
		public uint StuckTenantDetectionThresholdInMinutes
		{
			get
			{
				return (uint)base["StuckTenantDetectionThresholdInMinutes"];
			}
			set
			{
				base["StuckTenantDetectionThresholdInMinutes"] = value;
			}
		}

		[ConfigurationProperty("NumberOfFailuresBeforeBlackListing", DefaultValue = "5")]
		public uint NumberOfFailuresBeforeBlackListing
		{
			get
			{
				return (uint)base["NumberOfFailuresBeforeBlackListing"];
			}
			set
			{
				base["NumberOfFailuresBeforeBlackListing"] = value;
			}
		}

		[ConfigurationProperty("EnableFileLogging", DefaultValue = false)]
		public bool EnableFileLogging
		{
			get
			{
				return (bool)base["EnableFileLogging"];
			}
			set
			{
				base["EnableFileLogging"] = value;
			}
		}

		[ConfigurationProperty("EnableADHealthMonitoring", DefaultValue = true)]
		public bool EnableADHealthMonitoring
		{
			get
			{
				return (bool)base["EnableADHealthMonitoring"];
			}
			set
			{
				base["EnableADHealthMonitoring"] = value;
			}
		}

		[ConfigurationProperty("EnableUpdateThrottling", DefaultValue = false)]
		public bool EnableUpdateThrottling
		{
			get
			{
				return (bool)base["EnableUpdateThrottling"];
			}
			set
			{
				base["EnableUpdateThrottling"] = value;
			}
		}

		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			ExTraceGlobals.TenantUpgradeServiceletTracer.TraceDebug<string, string>(0L, "Unrecognized configuration attribute {0}={1}", name, value);
			return base.OnDeserializeUnrecognizedAttribute(name, value);
		}

		public static class Setting
		{
			public const string IsUpgradingBrokerEnabled = "IsUpgradingBrokerEnabled";

			public const string MaxConcurrentUpgradingThreadsPerServer = "MaxConcurrentUpgradingThreadsPerServer";

			public const string UpgradingBrokerPollIntervalInMinutes = "UpgradingBrokerPollIntervalInMinutes";

			public const string GetWorkItemsRetryIntervalInMinutes = "GetWorkItemsRetryIntervalInMinutes";

			public const string StartUpgradeRetries = "StartUpgradeRetries";

			public const string StartUpgradeThresholdInSeconds = "StartUpgradeThresholdInSeconds";

			public const string DelayBeforeCompletionInSeconds = "DelayBeforeCompletionInSeconds";

			public const string ExpectedMajorVersion = "ExpectedMajorVersion";

			public const string ExpectedMinorVersion = "ExpectedMinorVersion";

			public const string SkipIfExpectedVersionDoesntMatchAssembly = "SkipIfExpectedVersionDoesntMatchAssembly";

			public const string UpgradeSchedule = "UpgradeSchedule";

			public const string PreventServiceStopIfUpgradeInProgress = "PreventServiceStopIfUpgradeInProgress";

			public const string StuckTenantDetectionThresholdInMinutes = "StuckTenantDetectionThresholdInMinutes";

			public const string NumberOfFailuresBeforeBlackListing = "NumberOfFailuresBeforeBlackListing";

			public const string EnableFileLogging = "EnableFileLogging";

			public const string EnableADHealthMonitoring = "EnableADHealthMonitoring";

			public const string EnableUpdateThrottling = "EnableUpdateThrottling";
		}
	}
}
