using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Win32;

namespace Microsoft.Exchange.Diagnostics
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ExchangeSetupContext
	{
		public static bool IsUnpacked
		{
			get
			{
				string value = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup", "MsiInstallPath", null);
				return !string.IsNullOrEmpty(value);
			}
		}

		public static string InstallPath
		{
			get
			{
				if (ExchangeSetupContext.installPath == null)
				{
					if (!ExchangeSetupContext.IsUnpacked)
					{
						throw new SetupVersionInformationCorruptException("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup");
					}
					ExchangeSetupContext.installPath = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup", "MsiInstallPath", null);
				}
				return ExchangeSetupContext.installPath;
			}
		}

		public static string AssemblyPath
		{
			get
			{
				if (ExchangeSetupContext.assemblyPath == null)
				{
					string location = Assembly.GetExecutingAssembly().Location;
					ExchangeSetupContext.assemblyPath = Path.GetDirectoryName(location);
				}
				return ExchangeSetupContext.assemblyPath;
			}
		}

		public static void UseAssemblyPathAsInstallPath()
		{
			if (!ExchangeSetupContext.IsUnpacked || !ExchangeSetupContext.assemblyPath.Equals(ExchangeSetupContext.BinPath, StringComparison.OrdinalIgnoreCase))
			{
				ExchangeSetupContext.installPath = ExchangeSetupContext.AssemblyPath;
				ExchangeSetupContext.installedVersion = ExchangeSetupContext.GetExecutingVersion();
				ExchangeSetupContext.useAssemblyPathAsBinPath = true;
			}
		}

		public static void ResetInstallPath()
		{
			ExchangeSetupContext.installPath = null;
			ExchangeSetupContext.installedVersion = null;
			ExchangeSetupContext.useAssemblyPathAsBinPath = false;
		}

		public static Version GetExecutingVersion()
		{
			if (ExchangeSetupContext.executingVersion == null)
			{
				string text = Path.Combine(ExchangeSetupContext.AssemblyPath, "ExSetup.exe");
				try
				{
					FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(text);
					if (string.IsNullOrEmpty(versionInfo.FileVersion))
					{
						throw new FileVersionNotFoundException(DiagnosticsResources.ExceptionFileVersionNotFound(text));
					}
					ExchangeSetupContext.executingVersion = new Version(versionInfo.FileVersion);
				}
				catch (FileNotFoundException)
				{
					throw new FileVersionNotFoundException(DiagnosticsResources.ExceptionWantedVersionButFileNotFound(text));
				}
			}
			return ExchangeSetupContext.executingVersion;
		}

		public static string SetupPath
		{
			get
			{
				return Path.Combine(ExchangeSetupContext.InstallPath, "Setup");
			}
		}

		public static string SetupDataPath
		{
			get
			{
				return Path.Combine(ExchangeSetupContext.SetupPath, "Data");
			}
		}

		public static string SetupLoggingPath
		{
			get
			{
				return ExchangeSetupContext.setupLoggingPath;
			}
		}

		public static string SetupLogFileName
		{
			get
			{
				return ExchangeSetupContext.setupLogFileName;
			}
		}

		public static string SetupLogFileNameForWatson
		{
			get
			{
				return ExchangeSetupContext.setupLogFileNameForWatson;
			}
		}

		public static string SetupPerfPath
		{
			get
			{
				return Path.Combine(ExchangeSetupContext.SetupPath, "Perf");
			}
		}

		public static string DataPath
		{
			get
			{
				return Path.Combine(ExchangeSetupContext.InstallPath, "Data");
			}
		}

		public static string ScriptPath
		{
			get
			{
				return Path.Combine(ExchangeSetupContext.InstallPath, "Scripts");
			}
		}

		public static string RemoteScriptPath
		{
			get
			{
				return Path.Combine(ExchangeSetupContext.InstallPath, "RemoteScripts");
			}
		}

		public static string BinPath
		{
			get
			{
				if (ExchangeSetupContext.useAssemblyPathAsBinPath)
				{
					return ExchangeSetupContext.InstallPath;
				}
				return Path.Combine(ExchangeSetupContext.InstallPath, "Bin");
			}
		}

		public static string DatacenterPath
		{
			get
			{
				return Path.Combine(ExchangeSetupContext.InstallPath, "Datacenter");
			}
		}

		public static string LoggingPath
		{
			get
			{
				return Path.Combine(ExchangeSetupContext.InstallPath, "Logging");
			}
		}

		public static string ResPath
		{
			get
			{
				return Path.Combine(ExchangeSetupContext.BinPath, "Res");
			}
		}

		public static string TransportDataPath
		{
			get
			{
				return Path.Combine(ExchangeSetupContext.InstallPath, "TransportRoles\\Data");
			}
		}

		public static string BinPerfProcessorPath
		{
			get
			{
				string environmentVariable = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
				return Path.Combine(Path.Combine(ExchangeSetupContext.BinPath, "Perf"), environmentVariable);
			}
		}

		public static Version InstalledVersion
		{
			get
			{
				if (null == ExchangeSetupContext.installedVersion)
				{
					try
					{
						using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\ExchangeServer\\v15\\Setup"))
						{
							int major = (int)registryKey.GetValue("MsiProductMajor");
							int minor = (int)registryKey.GetValue("MsiProductMinor");
							int build = (int)registryKey.GetValue("MsiBuildMajor");
							int revision = (int)registryKey.GetValue("MsiBuildMinor");
							ExchangeSetupContext.installedVersion = new Version(major, minor, build, revision);
						}
					}
					catch (Exception innerException)
					{
						throw new SetupVersionInformationCorruptException("Software\\Microsoft\\ExchangeServer\\v15\\Setup", innerException);
					}
				}
				return ExchangeSetupContext.installedVersion;
			}
		}

		public static string PSHostPath
		{
			get
			{
				if (ExchangeSetupContext.mshHostPath == null)
				{
					try
					{
						ExchangeSetupContext.mshHostPath = (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\PowerShell\\1\\PowerShellEngine", "ApplicationBase", null);
					}
					catch (Exception innerException)
					{
						throw new SetupVersionInformationCorruptException("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\PowerShell\\1\\PowerShellEngine", innerException);
					}
					if (ExchangeSetupContext.mshHostPath == null)
					{
						throw new SetupVersionInformationCorruptException("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\PowerShell\\1\\PowerShellEngine");
					}
				}
				return ExchangeSetupContext.mshHostPath;
			}
		}

		public static bool IsLonghornServer
		{
			get
			{
				return Environment.OSVersion.Version.Major == 6;
			}
		}

		private const string SetupInstallKey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup";

		private const string InstallPathName = "MsiInstallPath";

		private const string MshEngineInstallKey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\PowerShell\\1\\PowerShellEngine";

		private const string MshInstallPathName = "ApplicationBase";

		private const string OfficialVersionFile = "ExSetup.exe";

		private static string setupLoggingPath = Environment.ExpandEnvironmentVariables("%systemdrive%\\ExchangeSetupLogs");

		private static string setupLogFileName = "ExchangeSetup.log";

		private static string setupLogFileNameForWatson = "ExchangeSetupWatson.log";

		private static string mshHostPath;

		private static Version executingVersion;

		private static string assemblyPath;

		private static bool useAssemblyPathAsBinPath = false;

		private static string installPath;

		private static Version installedVersion;
	}
}
