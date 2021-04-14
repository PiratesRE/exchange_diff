using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Win32;

namespace Microsoft.Exchange.Diagnostics
{
	public static class ExchangeBinaryFormatterFactory
	{
		public static BinaryFormatter CreateBinaryFormatter(SerializationBinder binder = null)
		{
			return new BinaryFormatter
			{
				Binder = new ChainedSerializationBinder(binder, ExchangeBinaryFormatterFactory.typeGatherer.Value, new string[0])
			};
		}

		public static BinaryFormatter CreateBinaryFormatter(SerializationBinder binder, params string[] allowList)
		{
			return new BinaryFormatter
			{
				Binder = new ChainedSerializationBinder(binder, ExchangeBinaryFormatterFactory.typeGatherer.Value, allowList)
			};
		}

		public static bool LoggingEnabled
		{
			get
			{
				return ExchangeBinaryFormatterFactory.loggingEnabled;
			}
			private set
			{
				ExchangeBinaryFormatterFactory.loggingEnabled = value;
			}
		}

		public static TimeSpan LogDumpInterval
		{
			get
			{
				return ExchangeBinaryFormatterFactory.logDumpInterval;
			}
			set
			{
				if (value != TimeSpan.Zero)
				{
					ExchangeBinaryFormatterFactory.logDumpInterval = value;
				}
			}
		}

		public static bool ClearAfterSave { get; private set; }

		public static bool IncludeStackTrace { get; private set; }

		public static string ConfigError { get; private set; }

		public static string GetLogDirectory()
		{
			using (RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32))
			{
				if (registryKey != null)
				{
					using (RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup"))
					{
						if (registryKey2 != null)
						{
							object value = registryKey2.GetValue("MsiInstallPath");
							return (value != null) ? Path.Combine(value.ToString(), "Logging", "TypeDeserialization", ExchangeBinaryFormatterFactory.GetProtocolName()) : null;
						}
					}
				}
			}
			return null;
		}

		public static void RefreshConfig(string config)
		{
			if (string.IsNullOrEmpty(config))
			{
				return;
			}
			ExchangeBinaryFormatterFactory.ConfigureDefaultSettings();
			if (config.StartsWith("LoggingDisabled", StringComparison.OrdinalIgnoreCase))
			{
				ExchangeBinaryFormatterFactory.LoggingEnabled = false;
				return;
			}
			if (config.StartsWith("LoggingEnabled", StringComparison.OrdinalIgnoreCase))
			{
				ExchangeBinaryFormatterFactory.LoggingEnabled = true;
				if (config.IndexOf(';') > -1)
				{
					string[] array = config.Split(new char[]
					{
						';'
					});
					if (array.Length > 1)
					{
						for (int i = 1; i < array.Length; i++)
						{
							if (array[i].Equals("IncludeStackTrace", StringComparison.OrdinalIgnoreCase))
							{
								ExchangeBinaryFormatterFactory.IncludeStackTrace = true;
							}
							if (array[i].Equals("ClearAfterSave", StringComparison.OrdinalIgnoreCase))
							{
								ExchangeBinaryFormatterFactory.ClearAfterSave = true;
							}
							if (array[i].StartsWith("LogDumpInterval", StringComparison.OrdinalIgnoreCase))
							{
								ExchangeBinaryFormatterFactory.LogDumpInterval = ExchangeBinaryFormatterFactory.GetTimespanForLogDumpInterval(array[i]);
							}
						}
						return;
					}
				}
			}
			else
			{
				ExchangeBinaryFormatterFactory.ConfigureDefaultSettings();
			}
		}

		private static IDeserializedTypesGatherer BuildTypeGatherer()
		{
			try
			{
				string logDirectory = ExchangeBinaryFormatterFactory.GetLogDirectory();
				if (logDirectory != null)
				{
					return new FileBasedDeserializedTypeGatherer(logDirectory, ExchangeBinaryFormatterFactory.LogDumpInterval)
					{
						AddStackTrace = ExchangeBinaryFormatterFactory.IncludeStackTrace,
						ClearAfterSave = ExchangeBinaryFormatterFactory.ClearAfterSave
					};
				}
			}
			catch
			{
			}
			return null;
		}

		private static string GetProtocolName()
		{
			string result;
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				result = (string.Equals(currentProcess.ProcessName, "w3wp", StringComparison.OrdinalIgnoreCase) ? ExchangeBinaryFormatterFactory.GetAppPoolId() : currentProcess.ProcessName);
			}
			return result;
		}

		private static string GetAppPoolId()
		{
			string text = Environment.GetEnvironmentVariable("APP_POOL_ID", EnvironmentVariableTarget.Process);
			if (string.IsNullOrWhiteSpace(text))
			{
				string input = Environment.CommandLine ?? string.Empty;
				Match match = Regex.Match(input, "-ap\\s+\"(?<appPool>\\w+)\"");
				if (match.Success)
				{
					text = match.Groups["appPool"].Value;
				}
			}
			if (!string.IsNullOrWhiteSpace(text))
			{
				return text;
			}
			return string.Empty;
		}

		private static void HandleConfigReadError(string error)
		{
			ExchangeBinaryFormatterFactory.ConfigError = DateTime.UtcNow + "-" + error;
			ExchangeBinaryFormatterFactory.ConfigureDefaultSettings();
		}

		private static void ConfigureDefaultSettings()
		{
			ExchangeBinaryFormatterFactory.LoggingEnabled = true;
			ExchangeBinaryFormatterFactory.LogDumpInterval = ExchangeBinaryFormatterFactory.defaultLogDumpInterval;
			ExchangeBinaryFormatterFactory.ClearAfterSave = false;
			ExchangeBinaryFormatterFactory.IncludeStackTrace = false;
		}

		private static TimeSpan GetTimespanForLogDumpInterval(string logDumpInterval)
		{
			TimeSpan timeSpan;
			if (logDumpInterval.IndexOf('=') <= -1 || !TimeSpan.TryParse(logDumpInterval.Substring(logDumpInterval.IndexOf('=') + 1), out timeSpan))
			{
				return ExchangeBinaryFormatterFactory.defaultLogDumpInterval;
			}
			if (!(timeSpan < ExchangeBinaryFormatterFactory.defaultLogDumpInterval))
			{
				return timeSpan;
			}
			return ExchangeBinaryFormatterFactory.defaultLogDumpInterval;
		}

		public const string LOGREGKEY = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Diagnostics";

		public const string LOGREGVALUE = "ExchangeBinaryFormatterFactory";

		private static readonly RegistryKeyChangeWatcher RegistryKeyChangeWatcher = new RegistryKeyChangeWatcher("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Diagnostics", "ExchangeBinaryFormatterFactory", new Action<string>(ExchangeBinaryFormatterFactory.RefreshConfig), new Action<string>(ExchangeBinaryFormatterFactory.HandleConfigReadError));

		private static readonly Lazy<IDeserializedTypesGatherer> typeGatherer = new Lazy<IDeserializedTypesGatherer>(() => ExchangeBinaryFormatterFactory.BuildTypeGatherer(), LazyThreadSafetyMode.PublicationOnly);

		private static readonly TimeSpan defaultLogDumpInterval = TimeSpan.FromMinutes(5.0);

		private static TimeSpan logDumpInterval = ExchangeBinaryFormatterFactory.defaultLogDumpInterval;

		private static bool loggingEnabled = true;
	}
}
