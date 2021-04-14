using System;
using System.IO;
using System.Security;

namespace Microsoft.Exchange.Diagnostics
{
	internal static class ConfigFiles
	{
		private static FileHandler Application
		{
			get
			{
				if (ConfigFiles.application == null)
				{
					ConfigFiles.application = new FileHandler(ConfigFiles.InternalGetConfigPath());
				}
				return ConfigFiles.application;
			}
		}

		internal static ConfigFileHandler Trace
		{
			get
			{
				if (ConfigFiles.trace == null)
				{
					lock (ConfigFiles.locker)
					{
						if (ConfigFiles.trace == null)
						{
							ConfigFiles.trace = new ConfigFileHandler("ExTraceConfiguration", "EnabledTraces.Config");
							ConfigFiles.Application.Changed += ConfigFiles.trace.UpdateConfigFilePath;
						}
					}
				}
				return ConfigFiles.trace;
			}
		}

		internal static ConfigFileHandler FaultInjection
		{
			get
			{
				if (ConfigFiles.faultInjection == null)
				{
					lock (ConfigFiles.locker)
					{
						if (ConfigFiles.faultInjection == null)
						{
							ConfigFiles.faultInjection = new ConfigFileHandler("ExFaultInjectionConfiguration", "FaultInjection.Config");
							ConfigFiles.Application.Changed += ConfigFiles.faultInjection.UpdateConfigFilePath;
						}
					}
				}
				return ConfigFiles.faultInjection;
			}
		}

		internal static ConfigFileHandler InMemory
		{
			get
			{
				if (ConfigFiles.inMemory == null)
				{
					lock (ConfigFiles.locker)
					{
						if (ConfigFiles.inMemory == null)
						{
							ConfigFiles.inMemory = new ConfigFileHandler("ExInMemoryTraceConfiguration", "EnabledInMemoryTraces.Config");
							ConfigFiles.Application.Changed += ConfigFiles.inMemory.UpdateConfigFilePath;
						}
					}
				}
				return ConfigFiles.inMemory;
			}
		}

		internal static void SetConfigSource(string configSource, string siteName)
		{
			ConfigFiles.Trace.SetConfigSource(configSource, siteName);
			ConfigFiles.InMemory.SetConfigSource(configSource, siteName);
			ConfigFiles.FaultInjection.SetConfigSource(configSource, siteName);
		}

		internal static string GetDefaultConfigFilePath()
		{
			return ConfigFiles.GetConfigFilePath("EnabledTraces.Config");
		}

		internal static string GetDefaultInMemoryConfigFilePath()
		{
			return ConfigFiles.GetConfigFilePath("EnabledInMemoryTraces.Config");
		}

		internal static string GetConfigFilePath(string fileName)
		{
			string text = null;
			try
			{
				text = ConfigFiles.GetSystemDriveDirectory();
			}
			catch (SecurityException)
			{
				text = null;
			}
			if (string.IsNullOrEmpty(text) || text.Length != 2)
			{
				return null;
			}
			text += ConfigFiles.DirectorySeparator;
			if (!Directory.Exists(text))
			{
				return null;
			}
			return Path.Combine(text, fileName);
		}

		private static char DirectorySeparator
		{
			get
			{
				return Path.DirectorySeparatorChar;
			}
		}

		private static string GetSystemDriveDirectory()
		{
			return Environment.GetEnvironmentVariable("SystemDrive");
		}

		private static string InternalGetConfigPath()
		{
			return AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
		}

		private const string TraceConfigurationFileKey = "ExTraceConfiguration";

		private const string InMemoryTraceConfigurationFileKey = "ExInMemoryTraceConfiguration";

		private const string FaultInjectionConfigurationFileKey = "ExFaultInjectionConfiguration";

		internal const string DefaultTraceConfigFileName = "EnabledTraces.Config";

		internal const string DefaultInMemoryTraceConfigFileName = "EnabledInMemoryTraces.Config";

		internal const string DefaultFaultInjectionConfigFileName = "FaultInjection.Config";

		private static object locker = new object();

		private static FileHandler application;

		private static ConfigFileHandler trace;

		private static ConfigFileHandler faultInjection;

		private static ConfigFileHandler inMemory;
	}
}
