using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ActivityLoggingConfig : IActivityLoggingConfig
	{
		public static IActivityLoggingConfig Instance
		{
			get
			{
				return ActivityLoggingConfig.InstanceHook.Value;
			}
		}

		public TimeSpan MaxLogFileAge
		{
			get
			{
				return this.maxLogFileAgeInternal;
			}
		}

		public ByteQuantifiedSize MaxLogDirectorySize
		{
			get
			{
				return this.maxLogDirectorySizeInternal;
			}
		}

		public ByteQuantifiedSize MaxLogFileSize
		{
			get
			{
				return this.maxLogFileSizeInternal;
			}
		}

		public bool IsDumpCollectionEnabled
		{
			get
			{
				return this.isDumpCollectionEnabled;
			}
		}

		internal ActivityLoggingConfig()
		{
			this.maxLogFileAgeInternal = ActivityLoggingConfig.GetMaxLogFileAgeOrDefault();
			this.maxLogDirectorySizeInternal = ActivityLoggingConfig.GetMaxLogDirectorySizeOrDefault();
			this.maxLogFileSizeInternal = ActivityLoggingConfig.GetMaxLogFileSizeOrDefault();
			this.isDumpCollectionEnabled = ActivityLoggingConfig.GetIsDumpCollectionEnabled();
		}

		private static ByteQuantifiedSize GetMaxLogFileSizeOrDefault()
		{
			ulong value = RegistryReader.Instance.GetValue<ulong>(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Inference\\ActivityLogging", "MaxLogFileSizeInMB", ActivityLoggingConfig.MaxLogFileSizeDefault.ToMB());
			return ByteQuantifiedSize.FromMB(value);
		}

		private static ByteQuantifiedSize GetMaxLogDirectorySizeOrDefault()
		{
			ulong value = RegistryReader.Instance.GetValue<ulong>(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Inference\\ActivityLogging", "MaxLogDirectorySizeInMB", ActivityLoggingConfig.MaxLogDirectorySizeDefault.ToMB());
			return ByteQuantifiedSize.FromMB(value);
		}

		private static TimeSpan GetMaxLogFileAgeOrDefault()
		{
			string value = RegistryReader.Instance.GetValue<string>(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Inference\\ActivityLogging", "MaxLogFileAge", string.Empty);
			TimeSpan result;
			if (string.IsNullOrEmpty(value) || !TimeSpan.TryParse(value, out result))
			{
				return ActivityLoggingConfig.MaxLogFileAgeDefault;
			}
			return result;
		}

		private static bool GetIsDumpCollectionEnabled()
		{
			return RegistryReader.Instance.GetValue<bool>(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Inference\\ActivityLogging", "IsDumpCollectionEnabled", ActivityLoggingConfig.IsDumpCollectionEnabledDefault);
		}

		private const string RegkeyName = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Inference\\ActivityLogging";

		private const string MaxLogFileAgeRegkeyValueName = "MaxLogFileAge";

		private const string MaxLogDirectorySizeInMBRegkeyValueName = "MaxLogDirectorySizeInMB";

		private const string MaxLogFileSizeInMBRegkeyValueName = "MaxLogFileSizeInMB";

		private const string IsDumpCollectionEnabledRegkeyValueName = "IsDumpCollectionEnabled";

		private static readonly TimeSpan MaxLogFileAgeDefault = TimeSpan.FromDays(14.0);

		private static readonly ByteQuantifiedSize MaxLogDirectorySizeDefault = ByteQuantifiedSize.FromGB(3UL);

		private static readonly ByteQuantifiedSize MaxLogFileSizeDefault = ByteQuantifiedSize.FromMB(10UL);

		private static readonly bool IsDumpCollectionEnabledDefault = false;

		internal static readonly Hookable<IActivityLoggingConfig> InstanceHook = Hookable<IActivityLoggingConfig>.Create(true, new ActivityLoggingConfig());

		private readonly TimeSpan maxLogFileAgeInternal;

		private readonly ByteQuantifiedSize maxLogDirectorySizeInternal;

		private readonly ByteQuantifiedSize maxLogFileSizeInternal;

		private readonly bool isDumpCollectionEnabled;
	}
}
