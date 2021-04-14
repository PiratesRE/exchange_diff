using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Reflection;
using Microsoft.Exchange.AddressBook.EventLog;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AddressBook.Service;
using Microsoft.Win32;

namespace Microsoft.Exchange.AddressBook.Service
{
	internal static class Configuration
	{
		internal static string NspiHttpPort { get; private set; }

		internal static string RfrHttpPort { get; private set; }

		internal static bool EnablePhoneticSort { get; private set; }

		internal static bool ProtocolLoggingEnabled { get; private set; }

		internal static bool ApplyHourPrecision { get; private set; }

		internal static ByteQuantifiedSize MaxDirectorySize { get; private set; }

		internal static string LogFilePath { get; private set; }

		internal static int MaxRetentionPeriod { get; private set; }

		internal static ByteQuantifiedSize PerFileMaxSize { get; private set; }

		internal static bool CrashOnUnhandledException { get; private set; }

		internal static int ModCacheExpiryTimeInSeconds { get; private set; }

		internal static string NspiTestServer { get; private set; }

		internal static bool SuppressNspiEndpointRegistration { get; private set; }

		internal static int AverageLatencySamples { get; private set; }

		internal static bool EncryptionRequired { get; private set; }

		internal static bool ServiceEnabled { get; private set; }

		internal static ADObjectId MicrosoftExchangeConfigurationRoot { get; private set; }

		internal static ADObjectId ConfigNamingContext { get; private set; }

		internal static TimeSpan? ADTimeout { get; private set; }

		internal static TimeSpan MaxExecutionTime { get; private set; }

		internal static bool UseDefaultAppConfig
		{
			get
			{
				return Configuration.useDefaultAppConfig;
			}
			set
			{
				Configuration.useDefaultAppConfig = value;
			}
		}

		internal static bool IsDatacenter { get; private set; }

		internal static void Initialize(ExEventLog eventLog, Action stopService)
		{
			Configuration.GeneralTracer.TraceDebug(0L, "Configuration.Initialize");
			Configuration.ServiceEnabled = true;
			Configuration.stopService = stopService;
			Configuration.eventLog = eventLog;
			Configuration.EncryptionRequired = true;
			Configuration.LoadAppConfig();
			Configuration.LoadRegistryConfig();
			if (Configuration.LoadADConfiguration())
			{
				Configuration.ExchangeRpcClientAccessChangeNotification(null);
			}
			Configuration.IsDatacenter = Datacenter.IsMicrosoftHostedOnly(true);
		}

		internal static void Terminate()
		{
			if (Configuration.exchangeRCANotificationCookie != null)
			{
				ADNotificationAdapter.UnregisterChangeNotification(Configuration.exchangeRCANotificationCookie);
				Configuration.exchangeRCANotificationCookie = null;
			}
		}

		private static bool LoadADConfiguration()
		{
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 196, "LoadADConfiguration", "f:\\15.00.1497\\sources\\dev\\DoMT\\src\\Service\\Configuration.cs");
				tenantOrTopologyConfigurationSession.ServerTimeout = Configuration.ADTimeout;
				ConfigurationContainer configurationContainer = tenantOrTopologyConfigurationSession.Read<ConfigurationContainer>(tenantOrTopologyConfigurationSession.ConfigurationNamingContext);
				if (configurationContainer != null)
				{
					Guid objectGuid = (Guid)configurationContainer[ADObjectSchema.Guid];
					Configuration.ConfigNamingContext = new ADObjectId(tenantOrTopologyConfigurationSession.ConfigurationNamingContext.DistinguishedName, objectGuid);
				}
				Configuration.MicrosoftExchangeConfigurationRoot = Configuration.ConfigNamingContext.GetDescendantId("Services", "Microsoft Exchange", new string[0]);
			});
			if (!adoperationResult.Succeeded)
			{
				Configuration.ServiceEnabled = false;
			}
			return Configuration.ServiceEnabled;
		}

		private static void LoadAppConfig()
		{
			Configuration.settingsAppConfig = null;
			Configuration.settingsDefaultConfig = null;
			Configuration.ProtocolLoggingEnabled = Configuration.GetConfigBool("ProtocolLoggingEnabled", false);
			Configuration.ApplyHourPrecision = Configuration.GetConfigBool("ApplyHourPrecision", true);
			Configuration.MaxDirectorySize = Configuration.GetConfigByteQuantifiedSize("MaxDirectorySize", ByteQuantifiedSize.Parse("1GB"));
			Configuration.LogFilePath = Configuration.GetConfigString("LogFilePath", ProtocolLog.DefaultLogFilePath);
			Configuration.MaxRetentionPeriod = Configuration.GetConfigInt("MaxRetentionPeriod", 720);
			Configuration.PerFileMaxSize = Configuration.GetConfigByteQuantifiedSize("PerFileMaxSize", ByteQuantifiedSize.Parse("10MB"));
			Configuration.CrashOnUnhandledException = Configuration.GetConfigBool("CrashOnUnhandledException", false);
			Configuration.ModCacheExpiryTimeInSeconds = Configuration.GetConfigInt("ModCacheExpiryTimeInSeconds", 7200);
			Configuration.AverageLatencySamples = Configuration.GetConfigInt("AverageLatencySamples", 1024);
			Configuration.NspiTestServer = Configuration.GetConfigString("NspiTestServer", string.Empty);
			Configuration.SuppressNspiEndpointRegistration = Configuration.GetConfigBool("SuppressNspiEndpointRegistration", false);
			Configuration.ADTimeout = new TimeSpan?(TimeSpan.FromSeconds((double)Configuration.GetConfigInt("ADTimeout", 30)));
			Configuration.MaxExecutionTime = TimeSpan.FromSeconds((double)Configuration.GetConfigInt("MaxExecutionTime", 15));
		}

		private static void LoadRegistryConfig()
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchangeAB\\Parameters"))
			{
				Configuration.NspiHttpPort = Configuration.GetRegistryString(registryKey, "NspiHttpPort", "6004");
				Configuration.RfrHttpPort = Configuration.GetRegistryString(registryKey, "RfrHttpPort", "6002");
				Configuration.EnablePhoneticSort = Configuration.GetRegistryBoolean(registryKey, "EnablePhoneticSort", false);
			}
		}

		private static void ExchangeRpcClientAccessChangeNotification(ADNotificationEventArgs args)
		{
			Configuration.GeneralTracer.TraceDebug(0L, "ExchangeRpcClientAccessChangeNotification called");
			ADNotificationAdapter.TryRunADOperation(delegate()
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 280, "ExchangeRpcClientAccessChangeNotification", "f:\\15.00.1497\\sources\\dev\\DoMT\\src\\Service\\Configuration.cs");
				topologyConfigurationSession.ServerTimeout = Configuration.ADTimeout;
				Server server = topologyConfigurationSession.ReadLocalServer();
				if (server == null)
				{
					Configuration.GeneralTracer.TraceError(0L, "Failed to find local server in AD");
					Configuration.eventLog.LogEvent(AddressBookEventLogConstants.Tuple_FailedToFindLocalServerInAD, string.Empty, new object[0]);
					if (args == null)
					{
						Configuration.ServiceEnabled = false;
						return;
					}
					Configuration.stopService();
					return;
				}
				else
				{
					ExchangeRpcClientAccess exchangeRpcClientAccess = topologyConfigurationSession.Read<ExchangeRpcClientAccess>(ExchangeRpcClientAccess.FromServerId(server.Id));
					if (exchangeRpcClientAccess != null)
					{
						if (Configuration.exchangeRCANotificationCookie == null)
						{
							Configuration.exchangeRCANotificationCookie = ADNotificationAdapter.RegisterChangeNotification<ExchangeRpcClientAccess>(exchangeRpcClientAccess.Id, new ADNotificationCallback(Configuration.ExchangeRpcClientAccessChangeNotification));
						}
						if (Configuration.EncryptionRequired != exchangeRpcClientAccess.EncryptionRequired)
						{
							Configuration.GeneralTracer.TraceDebug<bool, bool>(0L, "Changing EncryptionRequired from {0} to {1}", Configuration.EncryptionRequired, exchangeRpcClientAccess.EncryptionRequired);
							Configuration.EncryptionRequired = exchangeRpcClientAccess.EncryptionRequired;
						}
						return;
					}
					Configuration.GeneralTracer.TraceDebug(0L, "ExchangeRpcClientAccess disabled");
					Configuration.eventLog.LogEvent(AddressBookEventLogConstants.Tuple_ExchangeRpcClientAccessDisabled, string.Empty, new object[0]);
					if (args == null)
					{
						Configuration.ServiceEnabled = false;
						return;
					}
					Configuration.stopService();
					return;
				}
			});
		}

		private static bool GetConfigBool(string key, bool defaultValue)
		{
			string configurationValue = Configuration.GetConfigurationValue(key);
			if (string.IsNullOrEmpty(configurationValue))
			{
				Configuration.GeneralTracer.TraceDebug<string, bool>(0L, "Config[{0}]: {1} (default)", key, defaultValue);
				return defaultValue;
			}
			if (configurationValue.Equals("true", StringComparison.OrdinalIgnoreCase))
			{
				Configuration.GeneralTracer.TraceDebug<string>(0L, "Config[{0}]: true", key);
				return true;
			}
			if (configurationValue.Equals("false", StringComparison.OrdinalIgnoreCase))
			{
				Configuration.GeneralTracer.TraceDebug<string>(0L, "Config[{0}]: false", key);
				return false;
			}
			Configuration.GeneralTracer.TraceError<string, string, bool>(0L, "Config[{0}]: {1} (Invalid: defaulting to {2})", key, configurationValue, defaultValue);
			return defaultValue;
		}

		private static int GetConfigInt(string key, int defaultValue)
		{
			string configurationValue = Configuration.GetConfigurationValue(key);
			int num;
			if (string.IsNullOrEmpty(configurationValue) || !int.TryParse(configurationValue, out num))
			{
				Configuration.GeneralTracer.TraceDebug<string, int>(0L, "Config[{0}]: {1} (default)", key, defaultValue);
				return defaultValue;
			}
			Configuration.GeneralTracer.TraceDebug<string, int>(0L, "Config[{0}]: {1}", key, num);
			return num;
		}

		private static string GetConfigString(string key, string defaultValue)
		{
			string configurationValue = Configuration.GetConfigurationValue(key);
			if (configurationValue == null)
			{
				Configuration.GeneralTracer.TraceDebug<string, string>(0L, "Config[{0}]: {1} (default)", key, defaultValue);
				return defaultValue;
			}
			Configuration.GeneralTracer.TraceDebug<string, string>(0L, "Config[{0}]: {1}", key, configurationValue);
			return configurationValue;
		}

		private static string GetRegistryString(RegistryKey regkey, string key, string defaultValue)
		{
			string text = null;
			if (regkey != null)
			{
				text = (regkey.GetValue(key) as string);
			}
			if (text == null)
			{
				Configuration.GeneralTracer.TraceDebug<string, string>(0L, "Registry[{0}]: {1} (default)", key, defaultValue);
				return defaultValue;
			}
			Configuration.GeneralTracer.TraceDebug<string, string>(0L, "Registry[{0}]: {1}", key, text);
			return text;
		}

		private static bool GetRegistryBoolean(RegistryKey regkey, string key, bool defaultValue)
		{
			if (regkey == null)
			{
				Configuration.GeneralTracer.TraceDebug<string, bool>(0L, "Registry[{0}]: {1} (default)", key, defaultValue);
				return defaultValue;
			}
			bool result;
			try
			{
				result = Convert.ToBoolean(regkey.GetValue(key, defaultValue));
			}
			catch (Exception ex)
			{
				Configuration.GeneralTracer.TraceDebug<string>(0L, "Exception raised. Using default value. [Exception Message] ", ex.ToString());
				Configuration.GeneralTracer.TraceDebug<string, bool>(0L, "Registry[{0}]: {1} (default)", key, defaultValue);
				result = defaultValue;
			}
			return result;
		}

		private static ByteQuantifiedSize GetConfigByteQuantifiedSize(string key, ByteQuantifiedSize defaultValue)
		{
			string configurationValue = Configuration.GetConfigurationValue(key);
			ByteQuantifiedSize byteQuantifiedSize;
			if (string.IsNullOrEmpty(configurationValue) || !ByteQuantifiedSize.TryParse(configurationValue, out byteQuantifiedSize))
			{
				Configuration.GeneralTracer.TraceDebug<string, ByteQuantifiedSize>(0L, "Config[{0}]: {1} (default)", key, defaultValue);
				return defaultValue;
			}
			Configuration.GeneralTracer.TraceDebug<string, ByteQuantifiedSize>(0L, "Config[{0}]: {1}", key, byteQuantifiedSize);
			return byteQuantifiedSize;
		}

		private static string GetConfigurationValue(string key)
		{
			object settings = Configuration.Settings;
			if (settings != null)
			{
				KeyValueConfigurationCollection keyValueConfigurationCollection = settings as KeyValueConfigurationCollection;
				if (keyValueConfigurationCollection != null)
				{
					KeyValueConfigurationElement keyValueConfigurationElement = keyValueConfigurationCollection[key];
					if (keyValueConfigurationElement == null)
					{
						return null;
					}
					return keyValueConfigurationElement.Value;
				}
				else
				{
					NameValueCollection nameValueCollection = settings as NameValueCollection;
					if (nameValueCollection != null)
					{
						return nameValueCollection[key];
					}
				}
			}
			return null;
		}

		private static object Settings
		{
			get
			{
				if (Configuration.UseDefaultAppConfig)
				{
					if (Configuration.settingsDefaultConfig == null)
					{
						ConfigurationManager.RefreshSection("appSettings");
						Configuration.settingsDefaultConfig = ConfigurationManager.AppSettings;
					}
					return Configuration.settingsDefaultConfig;
				}
				if (Configuration.settingsAppConfig == null)
				{
					string location = Assembly.GetExecutingAssembly().Location;
					Configuration.GeneralTracer.TraceDebug<string>(0L, "Loading configuration file: {0}", location);
					Configuration configuration = ConfigurationManager.OpenExeConfiguration(location);
					Configuration.settingsAppConfig = configuration.AppSettings.Settings;
				}
				return Configuration.settingsAppConfig;
			}
		}

		private const string RegistryParameters = "SYSTEM\\CurrentControlSet\\Services\\MSExchangeAB\\Parameters";

		private const string SectionName = "appSettings";

		internal static readonly Trace GeneralTracer = ExTraceGlobals.GeneralTracer;

		private static Action stopService;

		private static ExEventLog eventLog;

		private static ADNotificationRequestCookie exchangeRCANotificationCookie;

		private static bool useDefaultAppConfig = false;

		private static KeyValueConfigurationCollection settingsAppConfig = null;

		private static NameValueCollection settingsDefaultConfig = null;
	}
}
