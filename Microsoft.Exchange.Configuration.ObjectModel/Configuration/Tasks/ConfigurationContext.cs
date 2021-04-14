using System;
using System.IO;
using System.Net;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[XmlRoot("configurationContext")]
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class ConfigurationContext
	{
		public ConfigurationContext() : this(null, null)
		{
		}

		public ConfigurationContext(string serverName, string instanceName)
		{
			this.instance = new ConfigurationContext.InstanceContext(serverName, instanceName);
		}

		[XmlElement("instance")]
		public ConfigurationContext.InstanceContext Instance
		{
			get
			{
				return this.instance;
			}
			set
			{
				this.instance = value;
			}
		}

		private ConfigurationContext.InstanceContext instance;

		[ClassAccessLevel(AccessLevel.MSInternal)]
		internal class Setup
		{
			public static bool IsUnpacked
			{
				get
				{
					return ExchangeSetupContext.IsUnpacked;
				}
			}

			public static string InstallPath
			{
				get
				{
					return ExchangeSetupContext.InstallPath;
				}
			}

			public static string AssemblyPath
			{
				get
				{
					return ExchangeSetupContext.AssemblyPath;
				}
			}

			public static void UseAssemblyPathAsInstallPath()
			{
				ExchangeSetupContext.UseAssemblyPathAsInstallPath();
			}

			public static void ResetInstallPath()
			{
				ExchangeSetupContext.ResetInstallPath();
			}

			public static Version GetExecutingVersion()
			{
				return ExchangeSetupContext.GetExecutingVersion();
			}

			public static string SetupPath
			{
				get
				{
					return ExchangeSetupContext.SetupPath;
				}
			}

			public static string SetupDataPath
			{
				get
				{
					return ExchangeSetupContext.SetupDataPath;
				}
			}

			public static string SetupLoggingPath
			{
				get
				{
					return ExchangeSetupContext.SetupLoggingPath;
				}
			}

			public static string SetupLogFileName
			{
				get
				{
					return ExchangeSetupContext.SetupLogFileName;
				}
			}

			public static string SetupLogFileNameForWatson
			{
				get
				{
					return ExchangeSetupContext.SetupLogFileNameForWatson;
				}
			}

			public static string SetupPerfPath
			{
				get
				{
					return ExchangeSetupContext.SetupPerfPath;
				}
			}

			public static string DataPath
			{
				get
				{
					return ExchangeSetupContext.DataPath;
				}
			}

			public static string ScriptPath
			{
				get
				{
					return ExchangeSetupContext.ScriptPath;
				}
			}

			public static string RemoteScriptPath
			{
				get
				{
					return ExchangeSetupContext.RemoteScriptPath;
				}
			}

			public static string BinPath
			{
				get
				{
					return ExchangeSetupContext.BinPath;
				}
			}

			public static string DatacenterPath
			{
				get
				{
					return ExchangeSetupContext.DatacenterPath;
				}
			}

			public static string LoggingPath
			{
				get
				{
					return ExchangeSetupContext.LoggingPath;
				}
			}

			public static string ResPath
			{
				get
				{
					return ExchangeSetupContext.ResPath;
				}
			}

			public static string TransportDataPath
			{
				get
				{
					return ExchangeSetupContext.TransportDataPath;
				}
			}

			public static string BinPerfProcessorPath
			{
				get
				{
					return ExchangeSetupContext.BinPerfProcessorPath;
				}
			}

			public static Version InstalledVersion
			{
				get
				{
					return ExchangeSetupContext.InstalledVersion;
				}
			}

			public static string PSHostPath
			{
				get
				{
					return ExchangeSetupContext.PSHostPath;
				}
			}

			public static bool IsLonghornServer
			{
				get
				{
					return Environment.OSVersion.Version.Major == 6;
				}
			}

			public static string FipsBinPath
			{
				get
				{
					return Path.Combine(ExchangeSetupContext.InstallPath, "FIP-FS\\Bin");
				}
			}

			public static string FipsDataPath
			{
				get
				{
					return Path.Combine(ExchangeSetupContext.InstallPath, "FIP-FS\\Data");
				}
			}

			public static string TorusPath
			{
				get
				{
					return "D:\\ManagedTools\\cmdlets";
				}
			}

			public static string TorusRemoteScriptPath
			{
				get
				{
					return "D:\\ManagedTools\\RemoteScripts";
				}
			}

			public static string TorusCmdletAssembly
			{
				get
				{
					return "Microsoft.Office.Datacenter.Torus.Cmdlets.dll";
				}
			}
		}

		[ClassAccessLevel(AccessLevel.MSInternal)]
		internal class InstanceContext
		{
			public InstanceContext() : this(null, null)
			{
			}

			internal InstanceContext(string serverName, string instanceName)
			{
				if (!string.IsNullOrEmpty(serverName))
				{
					this.serverName = serverName;
					this.dataSource = serverName;
				}
				else
				{
					this.serverName = ConfigurationContext.InstanceContext.defaultServerName;
					this.dataSource = ".";
				}
				if (!string.IsNullOrEmpty(instanceName))
				{
					this.instanceName = instanceName;
					this.dataSource = this.dataSource + "\\" + instanceName;
				}
			}

			[XmlElement("serverName")]
			public string ServerName
			{
				get
				{
					return this.serverName;
				}
			}

			[XmlElement("instanceName")]
			public string InstanceName
			{
				get
				{
					return this.instanceName;
				}
			}

			[XmlIgnore]
			public string DataSource
			{
				get
				{
					return this.dataSource;
				}
			}

			private string serverName;

			private static string defaultServerName = Dns.GetHostEntry(Environment.MachineName).HostName;

			private string instanceName;

			private string dataSource;
		}
	}
}
