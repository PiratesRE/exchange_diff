using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.RpcClientAccess.Diagnostics
{
	internal sealed class ProtocolLogConfiguration
	{
		public ProtocolLogConfiguration(ConfigurationPropertyBag propertyBag)
		{
			propertyBag.Freeze();
			this.configurationPropertyBag = propertyBag;
			this.isLoggingEnabledCache = propertyBag.Get<bool>(ProtocolLogConfiguration.Schema.IsLoggingEnabled);
			this.enabledTagsCache = propertyBag.Get<ProtocolLoggingTag>(ProtocolLogConfiguration.Schema.EnabledTags);
		}

		public static void SetDefaults(string logFilePath, string logTypeName, string logFilePrefix, string logComponent)
		{
			ProtocolLogConfiguration.defaultLogFilePath = logFilePath;
			ProtocolLogConfiguration.defaultLogTypeName = logTypeName;
			ProtocolLogConfiguration.defaultLogFilePrefix = logFilePrefix;
			ProtocolLogConfiguration.defaultLogComponent = logComponent;
		}

		public string SoftwareName
		{
			get
			{
				return "Microsoft Exchange";
			}
		}

		public string SoftwareVersion
		{
			get
			{
				return "15.00.1497.012";
			}
		}

		public string LogTypeName
		{
			get
			{
				return ProtocolLogConfiguration.defaultLogTypeName ?? "RCA Protocol Logs";
			}
		}

		public string LogFilePrefix
		{
			get
			{
				return ProtocolLogConfiguration.defaultLogFilePrefix ?? "RCA_";
			}
		}

		public string LogComponent
		{
			get
			{
				return ProtocolLogConfiguration.defaultLogComponent ?? "RCAProtocolLogs";
			}
		}

		public bool IsEnabled
		{
			get
			{
				return this.isLoggingEnabledCache;
			}
		}

		public string LogFilePath
		{
			get
			{
				return this.configurationPropertyBag.Get<string>(ProtocolLogConfiguration.Schema.LogFilePath).Replace(ProtocolLogConfiguration.ExchangeInstallDirEnvironmentVariable, this.configurationPropertyBag.Get<string>(ProtocolLogConfiguration.Schema.ExchangeInstallPath));
			}
		}

		public TimeSpan AgeQuota
		{
			get
			{
				return this.configurationPropertyBag.Get<TimeSpan>(ProtocolLogConfiguration.Schema.AgeQuota);
			}
		}

		public long DirectorySizeQuota
		{
			get
			{
				return this.configurationPropertyBag.Get<long>(ProtocolLogConfiguration.Schema.DirectorySizeQuota);
			}
		}

		public long PerFileSizeQuota
		{
			get
			{
				return this.configurationPropertyBag.Get<long>(ProtocolLogConfiguration.Schema.PerFileSizeQuota);
			}
		}

		public bool ApplyHourPrecision
		{
			get
			{
				return this.configurationPropertyBag.Get<bool>(ProtocolLogConfiguration.Schema.ApplyHourPrecision);
			}
		}

		public ProtocolLoggingTag EnabledTags
		{
			get
			{
				return this.enabledTagsCache;
			}
		}

		internal const string LogPathConfigKey = "LogPath";

		internal const string LoggingEnabledConfigKey = "ProtocolLoggingEnabled";

		internal const string LoggingTagConfigKey = "LoggingTag";

		private const string SoftwareNameValue = "Microsoft Exchange";

		private const string SoftwareVersionValue = "15.00.1497.012";

		private const string LogTypeNameValue = "RCA Protocol Logs";

		private const string LogFilePrefixValue = "RCA_";

		private const string LogComponentValue = "RCAProtocolLogs";

		public static ProtocolLogConfiguration Default = new ProtocolLogConfiguration(new ConfigurationPropertyBag(null, null));

		internal static string ExchangeInstallDirEnvironmentVariable = "%ExchangeInstallDir%";

		private static string defaultLogFilePath = null;

		private static string defaultLogTypeName = null;

		private static string defaultLogFilePrefix = null;

		private static string defaultLogComponent = null;

		private readonly ConfigurationPropertyBag configurationPropertyBag;

		private readonly bool isLoggingEnabledCache;

		private readonly ProtocolLoggingTag enabledTagsCache;

		internal class Schema : ConfigurationSchema<ProtocolLogConfiguration.Schema>
		{
			private static bool TryKilobytesToBytes(ulong kiloBytes, out long bytes)
			{
				bytes = (long)(kiloBytes * 1024UL);
				return kiloBytes <= 9007199254740991UL;
			}

			private static readonly ConfigurationSchema.TryConvert<string, ulong> ulongTryParse = new ConfigurationSchema.TryConvert<string, ulong>(ulong.TryParse);

			internal static readonly ConfigurationSchema.Property<string> ExchangeInstallPath = ConfigurationSchema.Property<string>.Declare(ConfigurationSchema<ProtocolLogConfiguration.Schema>.ConstantDataSource, () => ExchangeSetupContext.InstallPath);

			private static ConfigurationSchema.AppSettingsDataSource appSettings = new ConfigurationSchema.AppSettingsDataSource(ConfigurationSchema<ProtocolLogConfiguration.Schema>.AllDataSources);

			internal static ConfigurationSchema.Property<bool> IsLoggingEnabled = ConfigurationSchema.Property<bool>.Declare<string, string, string>(ProtocolLogConfiguration.Schema.appSettings, "ProtocolLoggingEnabled", new ConfigurationSchema.TryConvert<string, bool>(bool.TryParse), false);

			internal static ConfigurationSchema.Property<string> LogFilePath = ConfigurationSchema.Property<string>.Declare<string, string>(ProtocolLogConfiguration.Schema.appSettings, "LogPath", ProtocolLogConfiguration.defaultLogFilePath ?? "%ExchangeInstallDir%\\Logging\\RPC Client Access\\");

			internal static ConfigurationSchema.Property<TimeSpan> AgeQuota = ConfigurationSchema.Property<TimeSpan>.Declare<string, string, ulong>(ProtocolLogConfiguration.Schema.appSettings, "MaxRetentionPeriod", ProtocolLogConfiguration.Schema.ulongTryParse, delegate(ulong hours, out TimeSpan value)
			{
				value = TimeSpan.FromHours(hours);
				return true;
			}, TimeSpan.FromHours(720.0));

			internal static ConfigurationSchema.Property<long> DirectorySizeQuota = ConfigurationSchema.Property<long>.Declare<string, string, ulong>(ProtocolLogConfiguration.Schema.appSettings, "MaxDirectorySize", ProtocolLogConfiguration.Schema.ulongTryParse, new ConfigurationSchema.TryConvert<ulong, long>(ProtocolLogConfiguration.Schema.TryKilobytesToBytes), 1073741824L);

			internal static ConfigurationSchema.Property<long> PerFileSizeQuota = ConfigurationSchema.Property<long>.Declare<string, string, ulong>(ProtocolLogConfiguration.Schema.appSettings, "PerFileMaxSize", ProtocolLogConfiguration.Schema.ulongTryParse, new ConfigurationSchema.TryConvert<ulong, long>(ProtocolLogConfiguration.Schema.TryKilobytesToBytes), 10485760L);

			internal static ConfigurationSchema.Property<bool> ApplyHourPrecision = ConfigurationSchema.Property<bool>.Declare<string, string, string>(ProtocolLogConfiguration.Schema.appSettings, "ApplyHourPrecision", new ConfigurationSchema.TryConvert<string, bool>(bool.TryParse), true);

			internal static ConfigurationSchema.Property<ProtocolLoggingTag> EnabledTags = ConfigurationSchema.Property<ProtocolLoggingTag>.Declare<string, string, string>(ProtocolLogConfiguration.Schema.appSettings, "LoggingTag", delegate(string enumString, out ProtocolLoggingTag value)
			{
				return EnumValidator.TryParse<ProtocolLoggingTag>(enumString, EnumParseOptions.IgnoreCase, out value);
			}, ProtocolLoggingTag.ConnectDisconnect | ProtocolLoggingTag.ApplicationData | ProtocolLoggingTag.Failures | ProtocolLoggingTag.Logon);
		}
	}
}
