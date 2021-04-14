using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.IO;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.SharePointSignalStore;
using Microsoft.Exchange.Transport.LoggingCommon;
using Microsoft.Win32;

namespace Microsoft.Exchange.Hygiene.Data.RawFileLogger
{
	internal class RawFileLogger : DisposeTrackableBase
	{
		private RawFileLogger(string logPrefixType, Version version, string logPrefix) : this(logPrefixType, version, logPrefix, RawFileLogger.GetLogPath(logPrefixType))
		{
		}

		private RawFileLogger(string logPrefixType, Version version, string logPrefix, string logPath)
		{
			this.logSchemaMapping = RawFileLogger.GetLogSchema(logPrefixType, version);
			long maximumLogFileSize = RawFileLoggerConfiguration.Instance.MaximumLogFileSize;
			long maximumLogDirectorySize = RawFileLoggerConfiguration.Instance.MaximumLogDirectorySize;
			TimeSpan maximumLogAge = RawFileLoggerConfiguration.Instance.MaximumLogAge;
			int logBufferSize = RawFileLoggerConfiguration.Instance.LogBufferSize;
			TimeSpan logBufferFlushInterval = RawFileLoggerConfiguration.Instance.LogBufferFlushInterval;
			this.log = new Log(logPrefix, new LogHeaderFormatter(this.logSchemaMapping), "Microsoft.Exchange.Hygiene.Data.RawFileLogger", false);
			this.log.Configure(logPath, maximumLogAge, maximumLogDirectorySize, maximumLogFileSize, logBufferSize, logBufferFlushInterval);
		}

		public static RawFileLogger GetNonCachedLogger(string logPrefixType, string logDirectoryPath, Version version)
		{
			return new RawFileLogger(logPrefixType, version, logPrefixType, logDirectoryPath);
		}

		public static RawFileLogger GetLogger(string logPrefixType, Version version)
		{
			string logPrefix = RawFileLogger.GetLogPrefix(logPrefixType, version);
			if (!RawFileLogger.instances.ContainsKey(logPrefix))
			{
				RawFileLogger rawFileLogger = new RawFileLogger(logPrefixType, version, logPrefix);
				if (!RawFileLogger.instances.TryAdd(logPrefix, rawFileLogger))
				{
					rawFileLogger.Dispose();
				}
			}
			return RawFileLogger.instances[logPrefix];
		}

		public static RawFileLogger GetLogger(string logPrefixType, string logDirectoryPath, Version version)
		{
			string logPrefix = RawFileLogger.GetLogPrefix(logPrefixType, version);
			if (!RawFileLogger.instances.ContainsKey(logPrefix))
			{
				RawFileLogger rawFileLogger = new RawFileLogger(logPrefixType, version, logPrefix, logDirectoryPath);
				if (!RawFileLogger.instances.TryAdd(logPrefix, rawFileLogger))
				{
					rawFileLogger.Dispose();
				}
			}
			return RawFileLogger.instances[logPrefix];
		}

		public static void Cleanup()
		{
			foreach (RawFileLogger rawFileLogger in RawFileLogger.instances.Values)
			{
				rawFileLogger.Dispose();
			}
			RawFileLogger.instances.Clear();
		}

		public void WriteLogData(byte[][] logData)
		{
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.logSchemaMapping, true, false);
			for (int i = 0; i < logData.GetLength(0); i++)
			{
				logRowFormatter[i] = logData[i];
			}
			this.log.Append(logRowFormatter, -1);
		}

		public void Flush()
		{
			this.log.Flush();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<RawFileLogger>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.log != null)
			{
				this.log.Flush();
				this.log.Close();
			}
		}

		private static string GetLogPrefix(string logPrefixType, Version version)
		{
			return string.Format("{0}_{1}_{2}_{3}_{4}_", new object[]
			{
				logPrefixType,
				version.Major,
				version.Minor,
				version.MajorRevision,
				version.MinorRevision
			});
		}

		private static LogSchema GetLogSchema(string logPrefixType, Version version)
		{
			string[] fields;
			if (logPrefixType.StartsWith("MSGTRK", StringComparison.OrdinalIgnoreCase) || logPrefixType.StartsWith("MSGTRKMD", StringComparison.OrdinalIgnoreCase) || logPrefixType.StartsWith("MSGTRKMS", StringComparison.OrdinalIgnoreCase) || logPrefixType.StartsWith("MSGTRACECOMBO", StringComparison.OrdinalIgnoreCase) || logPrefixType.StartsWith("SYSPRB", StringComparison.OrdinalIgnoreCase))
			{
				fields = (from field in MessageTrackingSchema.MessageTrackingEvent.Fields
				where field.BuildAdded <= version
				select field into csvfield
				select csvfield.Name).ToArray<string>();
			}
			else if (logPrefixType.StartsWith("ASYNCQUEUE", StringComparison.OrdinalIgnoreCase))
			{
				fields = (from field in AsyncQueueLogLineSchema.DefaultSchema.Fields
				where field.BuildAdded <= version
				select field into csvfield
				select csvfield.Name).ToArray<string>();
			}
			else if (logPrefixType.StartsWith("SYNCTR", StringComparison.OrdinalIgnoreCase) || logPrefixType.StartsWith("SYNCADCP", StringComparison.OrdinalIgnoreCase))
			{
				fields = (from field in TenantSettingSchema.Schema.Fields
				where field.BuildAdded <= version
				select field into csvfield
				select csvfield.Name).ToArray<string>();
			}
			else if (logPrefixType.StartsWith("SPAMDIGEST", StringComparison.OrdinalIgnoreCase))
			{
				fields = (from field in SpamDigestLogSchema.DefaultSchema.Fields
				where field.BuildAdded <= version
				select field into csvfield
				select csvfield.Name).ToArray<string>();
			}
			else
			{
				if (!logPrefixType.StartsWith("OFFICEGRAPH", StringComparison.OrdinalIgnoreCase))
				{
					throw new ArgumentException(string.Format("Unsupported log prefix: {0}", logPrefixType));
				}
				fields = (from field in OfficeGraphLogSchema.Schema.Fields
				where field.BuildAdded <= version
				select field into csvfield
				select csvfield.Name).ToArray<string>();
			}
			return new LogSchema("Microsoft.Exchange.Hygiene.Data.RawFileLogger", version.ToString(), "Raw File Logger", fields);
		}

		private static string GetLogPath(string logPrefixType)
		{
			bool flag = false;
			string text = null;
			string key;
			switch (key = logPrefixType.ToUpper())
			{
			case "MSGTRK":
			case "MSGTRKMD":
			case "MSGTRKMS":
			case "MSGTRACECOMBO":
			case "SYSPRB":
			case "SYNCTR":
			case "SYNCADCP":
			{
				string text2 = "ExoMessageTraceLogPath";
				flag = true;
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeLabs"))
				{
					if (registryKey != null)
					{
						object value = registryKey.GetValue(text2);
						if (value != null)
						{
							text = value.ToString();
						}
					}
				}
				if (string.IsNullOrWhiteSpace(text))
				{
					throw new ConfigurationErrorsException(string.Format("Log Path is not set in Registry: {0}[{1}]", "SOFTWARE\\Microsoft\\ExchangeLabs", text2));
				}
				if (flag)
				{
					text = Path.Combine(text, logPrefixType);
				}
				return text;
			}
			}
			throw new ArgumentException(string.Format("Unsupported log prefix: {0}", logPrefixType));
		}

		private const string LogComponentName = "Microsoft.Exchange.Hygiene.Data.RawFileLogger";

		private const string LogPathRegistryPath = "SOFTWARE\\Microsoft\\ExchangeLabs";

		private static ConcurrentDictionary<string, RawFileLogger> instances = new ConcurrentDictionary<string, RawFileLogger>();

		private readonly LogSchema logSchemaMapping;

		private readonly Log log;
	}
}
