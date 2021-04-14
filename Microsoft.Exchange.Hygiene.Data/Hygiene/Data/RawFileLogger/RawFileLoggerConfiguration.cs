using System;
using System.Configuration;
using System.Web.Configuration;

namespace Microsoft.Exchange.Hygiene.Data.RawFileLogger
{
	internal class RawFileLoggerConfiguration : ConfigurationSection
	{
		public static string SectionName
		{
			get
			{
				return "RawFileLogger";
			}
		}

		public static RawFileLoggerConfiguration Instance
		{
			get
			{
				if (RawFileLoggerConfiguration.instance == null)
				{
					RawFileLoggerConfiguration.instance = (((RawFileLoggerConfiguration)WebConfigurationManager.GetSection(RawFileLoggerConfiguration.SectionName)) ?? new RawFileLoggerConfiguration());
				}
				return RawFileLoggerConfiguration.instance;
			}
		}

		[ConfigurationProperty("maximumLogAge", IsRequired = false, DefaultValue = "10.00:00:00")]
		public TimeSpan MaximumLogAge
		{
			get
			{
				return (TimeSpan)base["maximumLogAge"];
			}
			set
			{
				base["maximumLogAge"] = value;
			}
		}

		[ConfigurationProperty("maximumLogDirectorySize", IsRequired = false, DefaultValue = 107374182400L)]
		[LongValidator(ExcludeRange = false, MinValue = 0L, MaxValue = 214748364800L)]
		public long MaximumLogDirectorySize
		{
			get
			{
				return (long)base["maximumLogDirectorySize"];
			}
			set
			{
				base["maximumLogDirectorySize"] = value;
			}
		}

		[LongValidator(ExcludeRange = false, MinValue = 0L, MaxValue = 1073741824L)]
		[ConfigurationProperty("maximumLogFileSize", IsRequired = false, DefaultValue = 31457280L)]
		public long MaximumLogFileSize
		{
			get
			{
				return (long)base["maximumLogFileSize"];
			}
			set
			{
				base["maximumLogFileSize"] = value;
			}
		}

		[ConfigurationProperty("logBufferSize", IsRequired = false, DefaultValue = 1024)]
		[IntegerValidator(ExcludeRange = false, MinValue = 0)]
		public int LogBufferSize
		{
			get
			{
				return (int)base["logBufferSize"];
			}
			set
			{
				base["logBufferSize"] = value;
			}
		}

		[ConfigurationProperty("bufferFlushInterval", IsRequired = false, DefaultValue = "00:00:30")]
		public TimeSpan LogBufferFlushInterval
		{
			get
			{
				return (TimeSpan)base["bufferFlushInterval"];
			}
			set
			{
				base["bufferFlushInterval"] = value;
			}
		}

		private const string MaximumLogAgeKey = "maximumLogAge";

		private const string MaximumLogDirectorySizeKey = "maximumLogDirectorySize";

		private const string MaximumLogFileSizeKey = "maximumLogFileSize";

		private const string LogBufferSizeKey = "logBufferSize";

		private const string LogBufferFlushIntervalKey = "bufferFlushInterval";

		private static RawFileLoggerConfiguration instance;
	}
}
