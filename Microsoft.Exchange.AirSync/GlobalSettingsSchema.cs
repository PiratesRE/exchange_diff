using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Win32;

namespace Microsoft.Exchange.AirSync
{
	internal sealed class GlobalSettingsSchema
	{
		private static Dictionary<string, GlobalSettingsPropertyDefinition> PropertyMapping
		{
			get
			{
				if (GlobalSettingsSchema.propMap == null)
				{
					lock (GlobalSettingsSchema.staticLock)
					{
						if (GlobalSettingsSchema.propMap == null)
						{
							GlobalSettingsSchema.propMap = new Dictionary<string, GlobalSettingsPropertyDefinition>();
							Type typeFromHandle = typeof(GlobalSettingsSchema);
							foreach (FieldInfo fieldInfo in from x in typeFromHandle.GetTypeInfo().DeclaredFields
							where x.IsStatic && x.IsPublic && x.FieldType == typeof(GlobalSettingsPropertyDefinition)
							select x)
							{
								GlobalSettingsPropertyDefinition globalSettingsPropertyDefinition = (GlobalSettingsPropertyDefinition)fieldInfo.GetValue(null);
								GlobalSettingsSchema.propMap[globalSettingsPropertyDefinition.Name] = globalSettingsPropertyDefinition;
							}
						}
					}
				}
				return GlobalSettingsSchema.propMap;
			}
		}

		public static IList<GlobalSettingsPropertyDefinition> AllProperties
		{
			get
			{
				return GlobalSettingsSchema.PropertyMapping.Values.ToList<GlobalSettingsPropertyDefinition>();
			}
		}

		public static GlobalSettingsPropertyDefinition GetProperty(string name)
		{
			GlobalSettingsPropertyDefinition result = null;
			if (GlobalSettingsSchema.PropertyMapping.TryGetValue(name, out result))
			{
				return result;
			}
			return null;
		}

		internal static string GetAppSetting(GlobalSettingsPropertyDefinition propDef)
		{
			if (TestHooks.GlobalSettings_GetAppSetting != null)
			{
				return TestHooks.GlobalSettings_GetAppSetting(propDef);
			}
			return ConfigurationManager.AppSettings[propDef.Name];
		}

		internal static object GetRegistrySetting(GlobalSettingsPropertyDefinition propDef, string registryPath)
		{
			if (TestHooks.GlobalSettings_GetRegistrySetting != null)
			{
				return TestHooks.GlobalSettings_GetRegistrySetting(propDef, registryPath);
			}
			return GlobalSettingsSchema.ReadRegistryValue(propDef.Name, registryPath);
		}

		internal static bool GetFlightingSetting(GlobalSettingsPropertyDefinition propDef, IFeature feature)
		{
			if (TestHooks.GlobalSettings_GetFlightingSetting != null)
			{
				return (bool)TestHooks.GlobalSettings_GetFlightingSetting(propDef);
			}
			return feature.Enabled;
		}

		internal static bool GccGetStoredSecretKeysValid()
		{
			if (TestHooks.GccUtils_AreStoredSecurityKeysValid != null)
			{
				return TestHooks.GccUtils_AreStoredSecurityKeysValid();
			}
			return GccUtils.AreStoredSecretKeysValid();
		}

		private static GlobalSettingsPropertyDefinition CreateMaxWorkerThreadsPerProc()
		{
			int num;
			int num2;
			ThreadPool.GetMaxThreads(out num, out num2);
			num /= Environment.ProcessorCount;
			return GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("MaxWorkerThreadsPerProc", num, 1, num);
		}

		private static GlobalSettingsPropertyDefinition CreateConstrainedPropertyDefinition<T>(string name, T defaultValue, T minValue, T maxValue) where T : struct, IComparable
		{
			return GlobalSettingsSchema.CreateConstrainedPropertyDefinition<T>(name, defaultValue, minValue, maxValue, false, null);
		}

		private static GlobalSettingsPropertyDefinition CreateConstrainedPropertyDefinition<T>(string name, T defaultValue, T minValue, T maxValue, bool logMissingEntries, Func<GlobalSettingsPropertyDefinition, object> getter) where T : struct, IComparable
		{
			return new GlobalSettingsPropertyDefinition(name, typeof(T), defaultValue, new RangedValueConstraint<T>(minValue, maxValue), logMissingEntries, getter);
		}

		private static GlobalSettingsPropertyDefinition CreatePropertyDefinition(string name, Type type, object defaultValue)
		{
			return GlobalSettingsSchema.CreatePropertyDefinition(name, type, defaultValue, false, null);
		}

		private static GlobalSettingsPropertyDefinition CreatePropertyDefinition(string name, Type type, object defaultValue, bool logMissingEntries, Func<GlobalSettingsPropertyDefinition, object> getter)
		{
			return new GlobalSettingsPropertyDefinition(name, type, defaultValue, null, logMissingEntries, getter);
		}

		private static GlobalSettingsPropertyDefinition CreateVDirPropertyDefinition<T>(string name, T defaultValue, Func<ADMobileVirtualDirectory, T> getter)
		{
			return GlobalSettingsSchema.CreatePropertyDefinition(name, typeof(T), defaultValue, false, delegate(GlobalSettingsPropertyDefinition propDef2)
			{
				if (TestHooks.GlobalSettings_GetVDirSetting != null)
				{
					return TestHooks.GlobalSettings_GetVDirSetting(propDef2);
				}
				ADMobileVirtualDirectory admobileVirtualDirectory = ADNotificationManager.ADMobileVirtualDirectory;
				return (admobileVirtualDirectory == null) ? defaultValue : getter(admobileVirtualDirectory);
			});
		}

		private static object LoadRegistryBool(GlobalSettingsPropertyDefinition propDef, string registryPath)
		{
			object registrySetting = GlobalSettingsSchema.GetRegistrySetting(propDef, registryPath);
			if (registrySetting != null && registrySetting.GetType() == typeof(int))
			{
				return (int)registrySetting != 0;
			}
			return (bool)propDef.DefaultValue;
		}

		private static int LoadRegistryInt(GlobalSettingsPropertyDefinition propDef, string registryPath)
		{
			object registrySetting = GlobalSettingsSchema.GetRegistrySetting(propDef, registryPath);
			if (registrySetting != null && registrySetting.GetType() == typeof(int))
			{
				return (int)registrySetting;
			}
			return (int)propDef.DefaultValue;
		}

		private static object ReadRegistryValue(string keyName, string registryPath)
		{
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(registryPath))
				{
					if (registryKey != null)
					{
						return registryKey.GetValue(keyName);
					}
					AirSyncDiagnostics.LogEvent(AirSyncEventLogConstants.Tuple_GlobalValueRegistryValueMissing, new string[]
					{
						keyName,
						registryPath
					});
				}
			}
			catch (SecurityException ex)
			{
				AirSyncDiagnostics.LogEvent(AirSyncEventLogConstants.Tuple_GlobalValueRegistryReadFailure, new string[]
				{
					keyName,
					registryPath,
					ex.ToString()
				});
			}
			catch (UnauthorizedAccessException ex2)
			{
				AirSyncDiagnostics.LogEvent(AirSyncEventLogConstants.Tuple_GlobalValueRegistryReadFailure, new string[]
				{
					keyName,
					registryPath,
					ex2.ToString()
				});
			}
			return null;
		}

		private static object ConvertSecondsToTimeSpan(GlobalSettingsPropertyDefinition propDef)
		{
			int num = (int)GlobalSettingsPropertyDefinition.ConvertValueFromString(GlobalSettingsSchema.GetAppSetting(propDef), typeof(int), propDef.Name, (int)((TimeSpan)propDef.DefaultValue).TotalSeconds);
			return TimeSpan.FromSeconds((double)num);
		}

		private static object GetPartnerHostedOnly(GlobalSettingsPropertyDefinition propDef)
		{
			return DatacenterRegistry.IsPartnerHostedOnly();
		}

		private static object ConvertMinutesToTimeSpan(GlobalSettingsPropertyDefinition propDef)
		{
			int num = (int)GlobalSettingsPropertyDefinition.ConvertValueFromString(GlobalSettingsSchema.GetAppSetting(propDef), typeof(int), propDef.Name, (int)((TimeSpan)propDef.DefaultValue).TotalMinutes);
			return TimeSpan.FromMinutes((double)num);
		}

		private static object DeviceTypeParse(GlobalSettingsPropertyDefinition propDef)
		{
			string appSetting = GlobalSettingsSchema.GetAppSetting(propDef);
			if (string.IsNullOrWhiteSpace(appSetting))
			{
				return (string[])propDef.DefaultValue;
			}
			string[] array = (string[])propDef.DefaultValue;
			string[] array2 = appSetting.Split(new string[]
			{
				","
			}, StringSplitOptions.RemoveEmptyEntries);
			if (array.Length > 0)
			{
				string[] array3 = new string[array2.Length + array.Length];
				Array.Copy(array2, array3, array2.Length);
				Array.Copy(array, 0, array3, array2.Length, array.Length);
				return array3;
			}
			return array2;
		}

		private static List<string> ParseCommaSeparatedValuesIntoList(GlobalSettingsPropertyDefinition propDef)
		{
			string appSetting = GlobalSettingsSchema.GetAppSetting(propDef);
			if (string.IsNullOrEmpty(appSetting))
			{
				return propDef.DefaultValue as List<string>;
			}
			return new List<string>(appSetting.ToLower().Split(new char[]
			{
				','
			}, StringSplitOptions.RemoveEmptyEntries));
		}

		private static List<string> ParseSupportedIPMTypes(GlobalSettingsPropertyDefinition propDef)
		{
			string appSetting = GlobalSettingsSchema.GetAppSetting(propDef);
			if (string.IsNullOrEmpty(appSetting))
			{
				if (propDef.LogMissingEntries)
				{
					AirSyncDiagnostics.LogEvent(AirSyncEventLogConstants.Tuple_GlobalValueHasBeenDefaulted, new string[]
					{
						propDef.Name,
						propDef.DefaultValue.ToString()
					});
				}
				return (List<string>)propDef.DefaultValue;
			}
			char[] separator = new char[]
			{
				';'
			};
			string[] array = appSetting.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			List<string> list = new List<string>(array.Length);
			foreach (string text in array)
			{
				bool flag = true;
				foreach (char c in text)
				{
					if (!char.IsLetterOrDigit(c) && c != '.' && c != '-')
					{
						AirSyncDiagnostics.LogEvent(AirSyncEventLogConstants.Tuple_GlobalValueHasInvalidCharacters, new string[]
						{
							propDef.Name,
							text
						});
						flag = false;
						break;
					}
				}
				if (flag)
				{
					string text3 = text.ToUpperInvariant();
					if (text3.StartsWith("IPM.", StringComparison.Ordinal))
					{
						list.Add(text3);
					}
					else
					{
						AirSyncDiagnostics.LogEvent(AirSyncEventLogConstants.Tuple_CustomTypeDoesNotStartWithIPM, new string[]
						{
							propDef.Name,
							text
						});
					}
				}
			}
			if (list.Count == 0)
			{
				if (propDef.LogMissingEntries)
				{
					AirSyncDiagnostics.LogEvent(AirSyncEventLogConstants.Tuple_GlobalValueHasBeenDefaulted, new string[]
					{
						propDef.Name,
						propDef.DefaultValue.ToString()
					});
				}
				return (List<string>)propDef.DefaultValue;
			}
			return list;
		}

		private const char CommaSeparator = ',';

		private static Dictionary<string, GlobalSettingsPropertyDefinition> propMap = null;

		private static object staticLock = new object();

		public static GlobalSettingsPropertyDefinition UseTestBudget = GlobalSettingsSchema.CreatePropertyDefinition("UseTestBudget", typeof(bool), false);

		public static GlobalSettingsPropertyDefinition WriteProtocolLogDiagnostics = GlobalSettingsSchema.CreatePropertyDefinition("WriteProtocolLogDiagnostics", typeof(bool), false);

		public static GlobalSettingsPropertyDefinition HangingSyncHintCacheSize = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("HangingSyncHintCacheSize", 10000, 1, int.MaxValue);

		public static GlobalSettingsPropertyDefinition HangingSyncHintCacheTimeout = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<TimeSpan>("HangingSyncHintCacheTimeout", TimeSpan.FromMinutes(15.0), TimeSpan.Zero, TimeSpan.MaxValue, false, new Func<GlobalSettingsPropertyDefinition, object>(GlobalSettingsSchema.ConvertMinutesToTimeSpan));

		public static GlobalSettingsPropertyDefinition MaxNumOfFolders = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("MaxNumOfFolders", 100, 30, 5000);

		public static GlobalSettingsPropertyDefinition NumOfQueuedMailboxLogEntries = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("NumOfQueuedMailboxLogEntries", 15, 1, 200);

		public static GlobalSettingsPropertyDefinition MaxSizeOfMailboxLog = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("MaxSizeOfMailboxLog", 8000, 1000, 1000000);

		public static GlobalSettingsPropertyDefinition MaxNoOfPartnershipToAutoClean = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("MaxNoOfPartnershipToAutoClean", 10, 0, int.MaxValue);

		public static GlobalSettingsPropertyDefinition ADCacheRefreshInterval = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<TimeSpan>("ADCacheRefreshInterval", TimeSpan.FromSeconds(60.0), TimeSpan.FromSeconds(60.0), TimeSpan.MaxValue, false, new Func<GlobalSettingsPropertyDefinition, object>(GlobalSettingsSchema.ConvertSecondsToTimeSpan));

		public static GlobalSettingsPropertyDefinition MaxCleanUpExecutionTime = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<TimeSpan>("MaxCleanUpExecutionTime", TimeSpan.FromSeconds(45.0), TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(60.0), false, new Func<GlobalSettingsPropertyDefinition, object>(GlobalSettingsSchema.ConvertSecondsToTimeSpan));

		public static GlobalSettingsPropertyDefinition EventQueuePollingInterval = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<TimeSpan>("EventQueuePollingInterval", TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(600.0), false, new Func<GlobalSettingsPropertyDefinition, object>(GlobalSettingsSchema.ConvertSecondsToTimeSpan));

		public static GlobalSettingsPropertyDefinition MaxRetrievedItems = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("MaxRetrievedItems", 100, 1, int.MaxValue);

		public static GlobalSettingsPropertyDefinition MaxNoOfItemsMove = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("MaxNoOfItemsMove", 1000, 1, int.MaxValue);

		public static GlobalSettingsPropertyDefinition MaxWindowSize = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("MaxWindowSize", 100, 1, int.MaxValue);

		public static GlobalSettingsPropertyDefinition BudgetBackOffMinThreshold = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<TimeSpan>("BudgetBackOffMinThreshold", TimeSpan.FromSeconds(10.0), TimeSpan.FromSeconds(0.0), TimeSpan.MaxValue);

		public static GlobalSettingsPropertyDefinition AutoblockBackOffMediumThreshold = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<double>("AutoblockBackOffMediumThreshold", 0.5, 0.0, 1.0);

		public static GlobalSettingsPropertyDefinition AutoblockBackOffHighThreshold = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<double>("AutoblockBackOffHighThreshold", 0.75, 0.0, 1.0);

		public static GlobalSettingsPropertyDefinition MaxNumberOfClientOperations = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("MaxNumberOfClientOperations", 200, 1, int.MaxValue);

		public static GlobalSettingsPropertyDefinition MinRedirectProtocolVersion = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("MinRedirectProtocolVersion", 140, 120, 140);

		public static GlobalSettingsPropertyDefinition DeviceClassCacheMaxStartDelay = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<TimeSpan>("DeviceClassCacheMaxStartDelay", TimeSpan.FromSeconds(21600.0), TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(86400.0), false, new Func<GlobalSettingsPropertyDefinition, object>(GlobalSettingsSchema.ConvertSecondsToTimeSpan));

		public static GlobalSettingsPropertyDefinition DeviceClassCacheMaxADUploadCount = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("DeviceClassCacheMaxADUploadCount", 300, 1, 1000);

		public static GlobalSettingsPropertyDefinition DeviceClassCachePerOrgRefreshInterval = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("DeviceClassCachePerOrgRefreshInterval", 10800, 0, 86400);

		public static GlobalSettingsPropertyDefinition RequestCacheTimeInterval = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("RequestCacheTimeInterval", 10, 0, 1440);

		public static GlobalSettingsPropertyDefinition RequestCacheMaxCount = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("RequestCacheMaxCount", 5000, 0, int.MaxValue);

		public static GlobalSettingsPropertyDefinition DeviceClassPerOrgMaxADCount = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("DeviceClassPerOrgMaxADCount", 1000, 1, 10000);

		public static GlobalSettingsPropertyDefinition DeviceClassCacheADCleanupInterval = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("DeviceClassCacheADCleanupInterval", 90, 1, 730);

		public static GlobalSettingsPropertyDefinition DisableCaching = GlobalSettingsSchema.CreatePropertyDefinition("DisableCaching", typeof(bool), false);

		public static GlobalSettingsPropertyDefinition SyncLogEnabled = GlobalSettingsSchema.CreatePropertyDefinition("SyncLogEnabled", typeof(bool), true);

		public static GlobalSettingsPropertyDefinition SyncLogDirectory = GlobalSettingsSchema.CreatePropertyDefinition("SyncLogDirectory", typeof(string), string.Empty);

		public static GlobalSettingsPropertyDefinition EnableCredentialRequest = GlobalSettingsSchema.CreatePropertyDefinition("EnableCredentialRequest", typeof(bool), true);

		public static GlobalSettingsPropertyDefinition EnableMailboxLoggingVerboseMode = GlobalSettingsSchema.CreatePropertyDefinition("EnableMailboxLoggingVerboseMode", typeof(bool), false, false, delegate(GlobalSettingsPropertyDefinition propDef)
		{
			bool flightingSetting = GlobalSettingsSchema.GetFlightingSetting(propDef, VariantConfiguration.InvariantNoFlightingSnapshot.ActiveSync.MailboxLoggingVerboseMode);
			if (flightingSetting)
			{
				string appSetting = GlobalSettingsSchema.GetAppSetting(propDef);
				bool flag;
				return bool.TryParse(appSetting, out flag) ? flag : ((bool)propDef.DefaultValue);
			}
			return (bool)propDef.DefaultValue;
		});

		public static GlobalSettingsPropertyDefinition SchemaDirectory = GlobalSettingsSchema.CreatePropertyDefinition("SchemaDirectory", typeof(string), string.Empty, true, null);

		public static GlobalSettingsPropertyDefinition SchemaValidate = GlobalSettingsSchema.CreatePropertyDefinition("SchemaValidate", typeof(bool), true, true, (GlobalSettingsPropertyDefinition prop) => !string.Equals(GlobalSettingsSchema.GetAppSetting(prop), "off", StringComparison.OrdinalIgnoreCase));

		public static GlobalSettingsPropertyDefinition BlockNewMailboxes = GlobalSettingsSchema.CreatePropertyDefinition("BlockNewMailboxes", typeof(bool), false);

		public static GlobalSettingsPropertyDefinition BlockLegacyMailboxes = GlobalSettingsSchema.CreatePropertyDefinition("BlockLegacyMailboxes", typeof(bool), false);

		public static GlobalSettingsPropertyDefinition SkipAzureADCall = GlobalSettingsSchema.CreatePropertyDefinition("SkipAzureADCall", typeof(bool), false);

		public static GlobalSettingsPropertyDefinition ProxyVirtualDirectory = GlobalSettingsSchema.CreatePropertyDefinition("ProxyVirtualDirectory", typeof(string), "/Microsoft-Server-ActiveSync");

		public static GlobalSettingsPropertyDefinition BackOffThreshold = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("BackOffThreshold", 3, 1, 99999999);

		public static GlobalSettingsPropertyDefinition BackOffTimeOut = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("BackOffTimeOut", 60, 1, 600);

		public static GlobalSettingsPropertyDefinition BackOffErrorWindow = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("BackOffErrorWindow", 60, 1, 600);

		public static GlobalSettingsPropertyDefinition ProxyHandlerLongTimeout = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<TimeSpan>("ProxyHandlerLongTimeout", TimeSpan.FromSeconds(3600.0), TimeSpan.FromSeconds(60.0), TimeSpan.FromSeconds(9999.0), false, new Func<GlobalSettingsPropertyDefinition, object>(GlobalSettingsSchema.ConvertSecondsToTimeSpan));

		public static GlobalSettingsPropertyDefinition ProxyHandlerShortTimeout = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<TimeSpan>("ProxyHandlerShortTimeout", TimeSpan.FromSeconds(120.0), TimeSpan.FromSeconds(12.0), TimeSpan.FromSeconds(600.0), false, new Func<GlobalSettingsPropertyDefinition, object>(GlobalSettingsSchema.ConvertSecondsToTimeSpan));

		public static GlobalSettingsPropertyDefinition EarlyCompletionTolerance = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("EarlyCompletionTolerance", 500, 0, int.MaxValue);

		public static GlobalSettingsPropertyDefinition EarlyWakeupBufferTime = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("EarlyWakeupBufferTime", 10, 0, 59);

		public static GlobalSettingsPropertyDefinition ErrorResponseDelay = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("ErrorResponseDelay", 20, 0, 59);

		public static GlobalSettingsPropertyDefinition MaxThrottlingDelay = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("MaxThrottlingDelay", 70, 0, 300);

		public static GlobalSettingsPropertyDefinition ProxyConnectionPoolConnectionLimit = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("ProxyConnectionPoolConnectionLimit", 50000, 48, 200000);

		public static GlobalSettingsPropertyDefinition MaxCollectionsToLog = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("MaxCollectionsToLog", 100, 0, 1000);

		public static GlobalSettingsPropertyDefinition ProxyHeaders = GlobalSettingsSchema.CreatePropertyDefinition("ProxyHeaders", typeof(string[]), "PUBLIC,ALLOW,MS-SERVER-ACTIVESYNC,MS-ASPROTOCOLVERSIONS,MS-ASPROTOCOLCOMMANDS,CONTENT-TYPE,CONTENT-LENGTH,CONTENT-ENCODING".Split(new char[]
		{
			','
		}), false, delegate(GlobalSettingsPropertyDefinition propDef)
		{
			string appSetting = GlobalSettingsSchema.GetAppSetting(propDef);
			if (string.IsNullOrEmpty(appSetting))
			{
				return (string[])propDef.DefaultValue;
			}
			return appSetting.Split(new char[]
			{
				','
			});
		});

		public static GlobalSettingsPropertyDefinition DeviceTypesSupportingRedirect = GlobalSettingsSchema.CreatePropertyDefinition("DeviceTypesSupportingRedirect", typeof(string[]), "iPod,iPad,iPhone,WindowsPhone,WP,WP8,WindowsMail,EASProbeDeviceType,BlackBerry".Split(new char[]
		{
			','
		}), false, new Func<GlobalSettingsPropertyDefinition, object>(GlobalSettingsSchema.DeviceTypeParse));

		public static GlobalSettingsPropertyDefinition DeviceTypesWithBasicMDMNotification = GlobalSettingsSchema.CreatePropertyDefinition("DeviceTypesWithBasicMDMNotification", typeof(List<string>), new List<string>
		{
			"ipod",
			"ipad",
			"iphone"
		}, false, new Func<GlobalSettingsPropertyDefinition, object>(GlobalSettingsSchema.ParseCommaSeparatedValuesIntoList));

		public static GlobalSettingsPropertyDefinition DeviceTypesToParseOSVersion = GlobalSettingsSchema.CreatePropertyDefinition("DeviceTypesToParseOSVersion", typeof(List<string>), new List<string>
		{
			"samsung",
			"android"
		}, false, new Func<GlobalSettingsPropertyDefinition, object>(GlobalSettingsSchema.ParseCommaSeparatedValuesIntoList));

		public static GlobalSettingsPropertyDefinition NegativeDeviceStatusCacheExpirationInterval = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<TimeSpan>("NegativeDeviceStatusCacheExpirationInterval", TimeSpan.FromSeconds(30.0), TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(86400.0), false, delegate(GlobalSettingsPropertyDefinition propDef)
		{
			if (TestHooks.GlobalSettings_GetFlightingSetting != null)
			{
				return TestHooks.GlobalSettings_GetFlightingSetting(propDef);
			}
			return VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).ActiveSync.MdmNotification.NegativeDeviceStatusCacheExpirationInterval;
		});

		public static GlobalSettingsPropertyDefinition DeviceStatusCacheExpirationInterval = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<TimeSpan>("DeviceStatusCacheExpirationInterval", TimeSpan.FromSeconds(900.0), TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(86400.0), false, delegate(GlobalSettingsPropertyDefinition propDef)
		{
			if (TestHooks.GlobalSettings_GetFlightingSetting != null)
			{
				return TestHooks.GlobalSettings_GetFlightingSetting(propDef);
			}
			return VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).ActiveSync.MdmNotification.DeviceStatusCacheExpirationInternal;
		});

		public static GlobalSettingsPropertyDefinition ValidSingleNamespaceUrls = GlobalSettingsSchema.CreatePropertyDefinition("ValidSingleNamespaceUrls", typeof(List<string>), new List<string>
		{
			"sdfpilot.outlook.com",
			"outlook.office365.com",
			"partner.outlook.cn"
		}, false, new Func<GlobalSettingsPropertyDefinition, object>(GlobalSettingsSchema.ParseCommaSeparatedValuesIntoList));

		public static GlobalSettingsPropertyDefinition MdmEnrollmentUrl = GlobalSettingsSchema.CreatePropertyDefinition("MDMEnrollmentUrl", typeof(Uri), new Uri("http://go.microsoft.com/fwlink/?LinkId=396941"), false, delegate(GlobalSettingsPropertyDefinition propDef)
		{
			if (TestHooks.GlobalSettings_GetFlightingSetting != null)
			{
				return TestHooks.GlobalSettings_GetFlightingSetting(propDef);
			}
			return VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).ActiveSync.MdmNotification.EnrollmentUrl;
		});

		public static GlobalSettingsPropertyDefinition MdmActivationUrl = GlobalSettingsSchema.CreatePropertyDefinition("MDMActivationUrl", typeof(string), "https://{0}/manage/{1}/eas/activation?easid={2}&traceid={3}");

		public static GlobalSettingsPropertyDefinition MdmComplianceStatusUrl = GlobalSettingsSchema.CreatePropertyDefinition("MDMComplianceStatusUrl", typeof(Uri), new Uri("http://go.microsoft.com/fwlink/?LinkId=397185"), false, delegate(GlobalSettingsPropertyDefinition propDef)
		{
			if (TestHooks.GlobalSettings_GetFlightingSetting != null)
			{
				return TestHooks.GlobalSettings_GetFlightingSetting(propDef);
			}
			return VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).ActiveSync.MdmNotification.ComplianceStatusUrl;
		});

		public static GlobalSettingsPropertyDefinition ADRegistrationServiceUrl = GlobalSettingsSchema.CreatePropertyDefinition("ADRegistrationServiceUrl", typeof(string), "enterpriseregistration.windows.net", false, delegate(GlobalSettingsPropertyDefinition propDef)
		{
			if (TestHooks.GlobalSettings_GetFlightingSetting != null)
			{
				return TestHooks.GlobalSettings_GetFlightingSetting(propDef);
			}
			return VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).ActiveSync.MdmNotification.ADRegistrationServiceHost;
		});

		public static GlobalSettingsPropertyDefinition MdmEnrollmentUrlWithBasicSteps = GlobalSettingsSchema.CreatePropertyDefinition("MdmEnrollmentUrlWithBasicSteps", typeof(Uri), new Uri("http://aka.ms/deviceenroll?easID={0}"), false, delegate(GlobalSettingsPropertyDefinition propDef)
		{
			if (TestHooks.GlobalSettings_GetFlightingSetting != null)
			{
				return TestHooks.GlobalSettings_GetFlightingSetting(propDef);
			}
			return VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).ActiveSync.MdmNotification.EnrollmentUrlWithBasicSteps;
		});

		public static GlobalSettingsPropertyDefinition DisableDeviceHealthStatusCache = GlobalSettingsSchema.CreatePropertyDefinition("DisableDeviceHealthStatusCache", typeof(bool), false);

		public static GlobalSettingsPropertyDefinition DisableAadClientCache = GlobalSettingsSchema.CreatePropertyDefinition("DisableAadClientCache", typeof(bool), false);

		public static GlobalSettingsPropertyDefinition MdmActivationUrlWithBasicSteps = GlobalSettingsSchema.CreatePropertyDefinition("MdmActivationUrlWithBasicSteps", typeof(string), "companyportal://enrollment/mapping?easID={0}", false, delegate(GlobalSettingsPropertyDefinition propDef)
		{
			if (TestHooks.GlobalSettings_GetFlightingSetting != null)
			{
				return TestHooks.GlobalSettings_GetFlightingSetting(propDef);
			}
			return VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).ActiveSync.MdmNotification.ActivationUrlWithBasicSteps;
		});

		public static GlobalSettingsPropertyDefinition MaxWorkerThreadsPerProc = GlobalSettingsSchema.CreateMaxWorkerThreadsPerProc();

		public static GlobalSettingsPropertyDefinition MaxRequestsQueued = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("MaxRequestsQueued", 500, 1, 10000);

		public static GlobalSettingsPropertyDefinition MaxMailboxSearchResults = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("MaxMailboxSearchResults", 100, 10, 100000);

		public static GlobalSettingsPropertyDefinition MailboxSearchTimeout = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<TimeSpan>("MailboxSearchTimeout", TimeSpan.FromSeconds(30.0), TimeSpan.FromSeconds(2.0), TimeSpan.FromSeconds(150.0), false, new Func<GlobalSettingsPropertyDefinition, object>(GlobalSettingsSchema.ConvertSecondsToTimeSpan));

		public static GlobalSettingsPropertyDefinition MailboxSessionCacheInitialSize = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("MailboxSessionCacheInitialSize", 100, 0, 32000);

		public static GlobalSettingsPropertyDefinition MailboxSessionCacheMaxSize = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("MailboxSessionCacheMaxSize", 1000, 1, 32000);

		public static GlobalSettingsPropertyDefinition MailboxSessionCacheTimeout = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<TimeSpan>("MailboxSessionCacheTimeout", TimeSpan.FromMinutes(15.0), TimeSpan.Zero, TimeSpan.FromMinutes(1440.0), false, new Func<GlobalSettingsPropertyDefinition, object>(GlobalSettingsSchema.ConvertMinutesToTimeSpan));

		public static GlobalSettingsPropertyDefinition MailboxSearchTimeoutNoContentIndexing = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<TimeSpan>("MailboxSearchTimeoutNoContentIndexing", TimeSpan.FromSeconds(90.0), TimeSpan.FromSeconds(3.0), TimeSpan.FromSeconds(300.0), false, new Func<GlobalSettingsPropertyDefinition, object>(GlobalSettingsSchema.ConvertSecondsToTimeSpan));

		public static GlobalSettingsPropertyDefinition MaxClientSentBadItems = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("MaxClientSentBadItems", 4, 1, 10000);

		public static GlobalSettingsPropertyDefinition BadItemIncludeStackTrace = GlobalSettingsSchema.CreatePropertyDefinition("BadItemIncludeStackTrace", typeof(bool), false);

		public static GlobalSettingsPropertyDefinition BadItemIncludeEmailToText = GlobalSettingsSchema.CreatePropertyDefinition("BadItemIncludeEmailToText", typeof(bool), false);

		public static GlobalSettingsPropertyDefinition BadItemEmailToText = GlobalSettingsSchema.CreatePropertyDefinition("BadItemEmailToText", typeof(string), string.Empty, false, (GlobalSettingsPropertyDefinition propDef) => GlobalSettingsSchema.GetAppSetting(propDef) ?? string.Empty);

		public static GlobalSettingsPropertyDefinition SupportedIPMTypes = GlobalSettingsSchema.CreatePropertyDefinition("SupportedIPMTypes", typeof(List<string>), new List<string>(), true, new Func<GlobalSettingsPropertyDefinition, object>(GlobalSettingsSchema.ParseSupportedIPMTypes));

		public static GlobalSettingsPropertyDefinition MaxGALSearchResults = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("MaxGALSearchResults", 100, 10, 1000);

		public static GlobalSettingsPropertyDefinition MaxDocumentLibrarySearchResults = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("MaxDocumentLibrarySearchResults", 1000, 10, 10000);

		public static GlobalSettingsPropertyDefinition MaxDocumentDataSize = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("MaxDocumentDataSize", 10240000, 10240, int.MaxValue);

		public static GlobalSettingsPropertyDefinition MaxSMimeADDistributionListExpansion = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("MaxSMimeADDistributionListExpansion", 2000, 5, 65535);

		public static GlobalSettingsPropertyDefinition IrmEnabled = GlobalSettingsSchema.CreatePropertyDefinition("IrmEnabled", typeof(bool), true);

		public static GlobalSettingsPropertyDefinition MaxRmsTemplates = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("MaxRmsTemplates", 20, 0, 50);

		public static GlobalSettingsPropertyDefinition NegativeRmsTemplateCacheExpirationInterval = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<TimeSpan>("NegativeRmsTemplateCacheExpirationInterval", TimeSpan.FromSeconds(300.0), TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(86400.0), false, delegate(GlobalSettingsPropertyDefinition propDef)
		{
			string appSetting = GlobalSettingsSchema.GetAppSetting(propDef);
			int num;
			return int.TryParse(appSetting, out num) ? TimeSpan.FromSeconds((double)num) : ((TimeSpan)propDef.DefaultValue);
		});

		public static GlobalSettingsPropertyDefinition HeartBeatInterval = GlobalSettingsSchema.CreatePropertyDefinition("HeartbeatInterval", typeof(HeartBeatInterval), Microsoft.Exchange.AirSync.HeartBeatInterval.Default, false, (GlobalSettingsPropertyDefinition propDef) => Microsoft.Exchange.AirSync.HeartBeatInterval.Read());

		public static GlobalSettingsPropertyDefinition HeartbeatSampleSize = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("HeartbeatSampleSize", 200, 0, 10000);

		public static GlobalSettingsPropertyDefinition HeartbeatAlertThreshold = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("HeartbeatAlertThreshold", 540, 0, 3540);

		public static GlobalSettingsPropertyDefinition ADCacheExpirationTimeout = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<TimeSpan>("ADCacheExpirationTimeout", TimeSpan.FromSeconds(300.0), TimeSpan.FromSeconds(1.0), TimeSpan.MaxValue, false, new Func<GlobalSettingsPropertyDefinition, object>(GlobalSettingsSchema.ConvertSecondsToTimeSpan));

		public static GlobalSettingsPropertyDefinition ADCacheMaxOrgCount = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("ADCacheMaxOrgCount", 50000, 1, int.MaxValue);

		public static GlobalSettingsPropertyDefinition FullServerVersion = GlobalSettingsSchema.CreatePropertyDefinition("FullServerVersion", typeof(bool), false);

		public static GlobalSettingsPropertyDefinition VdirCacheTimeoutSeconds = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<TimeSpan>("VdirCacheTimeoutSeconds", TimeSpan.FromSeconds(900.0), TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(2147483647.0), false, new Func<GlobalSettingsPropertyDefinition, object>(GlobalSettingsSchema.ConvertSecondsToTimeSpan));

		public static GlobalSettingsPropertyDefinition AllowProxyingWithoutSsl = GlobalSettingsSchema.CreatePropertyDefinition("AllowProxyingWithoutSsl", typeof(bool), false, false, (GlobalSettingsPropertyDefinition propDef) => GlobalSettingsSchema.LoadRegistryBool(propDef, "SYSTEM\\CurrentControlSet\\Services\\MSExchange OWA"));

		public static GlobalSettingsPropertyDefinition HDPhotoCacheMaxSize = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("HDPhotoCacheMaxSize", 5000, 1, int.MaxValue);

		public static GlobalSettingsPropertyDefinition HDPhotoCacheExpirationTimeOut = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<TimeSpan>("HDPhotoCacheExpirationTimeOut", TimeSpan.FromSeconds(900.0), TimeSpan.FromSeconds(60.0), TimeSpan.FromSeconds(2147483647.0), false, new Func<GlobalSettingsPropertyDefinition, object>(GlobalSettingsSchema.ConvertSecondsToTimeSpan));

		public static GlobalSettingsPropertyDefinition MaxRequestExecutionTime = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<TimeSpan>("MaxRequestExecutionTime", TimeSpan.FromSeconds(90.0), TimeSpan.FromSeconds(0.0), TimeSpan.FromSeconds(120.0), false, new Func<GlobalSettingsPropertyDefinition, object>(GlobalSettingsSchema.ConvertSecondsToTimeSpan));

		public static GlobalSettingsPropertyDefinition HDPhotoDefaultSupportedResolution = GlobalSettingsSchema.CreatePropertyDefinition("HDPhotoDefaultSupportedResolution", typeof(UserPhotoResolution), UserPhotoResolution.HR96x96);

		public static GlobalSettingsPropertyDefinition HDPhotoMaxNumberOfRequestsToProcess = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("HDPhotoMaxNumberOfRequestsToProcess", 100, 0, int.MaxValue);

		public static GlobalSettingsPropertyDefinition AllowInternalUntrustedCerts = GlobalSettingsSchema.CreatePropertyDefinition("AllowInternalUntrustedCerts", typeof(bool), true, false, (GlobalSettingsPropertyDefinition propDef) => GlobalSettingsSchema.LoadRegistryBool(propDef, "SYSTEM\\CurrentControlSet\\Services\\MSExchange OWA"));

		public static GlobalSettingsPropertyDefinition AllowDirectPush = GlobalSettingsSchema.CreatePropertyDefinition("AllowDirectPush", typeof(GlobalSettings.DirectPushEnabled), GlobalSettings.DirectPushEnabled.On, false, delegate(GlobalSettingsPropertyDefinition propDef)
		{
			GlobalSettings.DirectPushEnabled directPushEnabled = (GlobalSettings.DirectPushEnabled)GlobalSettingsSchema.LoadRegistryInt(propDef, "SYSTEM\\CurrentControlSet\\Services\\MSExchange ActiveSync");
			if (directPushEnabled >= GlobalSettings.DirectPushEnabled.Off && directPushEnabled <= GlobalSettings.DirectPushEnabled.OnWithAddressCheck)
			{
				return directPushEnabled;
			}
			return propDef.DefaultValue;
		});

		public static GlobalSettingsPropertyDefinition IsMultiTenancyEnabled = GlobalSettingsSchema.CreatePropertyDefinition("IsMultiTenancyEnabled", typeof(bool), false, false, (GlobalSettingsPropertyDefinition propDef) => GlobalSettingsSchema.GetFlightingSetting(propDef, VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy));

		public static GlobalSettingsPropertyDefinition IsWindowsLiveIDEnabled = GlobalSettingsSchema.CreatePropertyDefinition("IsWindowsLiveIDEnabled", typeof(bool), false, false, (GlobalSettingsPropertyDefinition propDef) => GlobalSettingsSchema.GetFlightingSetting(propDef, VariantConfiguration.InvariantNoFlightingSnapshot.Global.WindowsLiveID));

		public static GlobalSettingsPropertyDefinition IsGCCEnabled = GlobalSettingsSchema.CreatePropertyDefinition("IsGCCEnabled", typeof(bool), false, false, (GlobalSettingsPropertyDefinition propDef) => GlobalSettingsSchema.GetFlightingSetting(propDef, VariantConfiguration.InvariantNoFlightingSnapshot.ActiveSync.GlobalCriminalCompliance));

		public static GlobalSettingsPropertyDefinition AreGccStoredSecretKeysValid = GlobalSettingsSchema.CreatePropertyDefinition("AreGccStoredSecretKeysValid", typeof(bool), false, false, (GlobalSettingsPropertyDefinition propDef) => GlobalSettingsSchema.GetFlightingSetting(propDef, VariantConfiguration.InvariantNoFlightingSnapshot.ActiveSync.GlobalCriminalCompliance) && GlobalSettingsSchema.GccGetStoredSecretKeysValid());

		public static GlobalSettingsPropertyDefinition BootstrapCABForWM61HostingURL = GlobalSettingsSchema.CreatePropertyDefinition("BootstrapCABForWM61HostingURL", typeof(string), "http://go.microsoft.com/fwlink/?LinkId=150061");

		public static GlobalSettingsPropertyDefinition MobileUpdateInformationURL = GlobalSettingsSchema.CreatePropertyDefinition("MobileUpdateInformationURL", typeof(string), "http://go.microsoft.com/fwlink/?LinkId=143155");

		public static GlobalSettingsPropertyDefinition AutoBlockWriteToAd = GlobalSettingsSchema.CreatePropertyDefinition("AutoBlockWriteToAd", typeof(bool), true);

		public static GlobalSettingsPropertyDefinition AutoBlockADWriteDelay = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("AutoBlockADWriteDelay", 3, 0, 24);

		public static GlobalSettingsPropertyDefinition ADDataSyncInterval = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("ADDataSyncInterval", 24, 0, 168);

		public static GlobalSettingsPropertyDefinition DeviceBehaviorCacheInitialSize = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("DeviceBehaviorCacheInitialSize", 100, 0, 32000);

		public static GlobalSettingsPropertyDefinition DeviceBehaviorCacheMaxSize = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("DeviceBehaviorCacheMaxSize", 1000, 1, 32000);

		public static GlobalSettingsPropertyDefinition DeviceBehaviorCacheTimeout = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("DeviceBehaviorCacheTimeout", 15, 0, 1440);

		public static GlobalSettingsPropertyDefinition BootstrapMailDeliveryDelay = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("BootstrapMailDeliveryDelay", 259200, 0, int.MaxValue);

		public static GlobalSettingsPropertyDefinition UpgradeGracePeriod = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("UpgradeGracePeriod", 10080, 0, int.MaxValue);

		public static GlobalSettingsPropertyDefinition DeviceDiscoveryPeriod = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("DeviceDiscoveryPeriod", 14, 0, int.MaxValue);

		public static GlobalSettingsPropertyDefinition OnlyOrganizersCanSendMeetingChanges = GlobalSettingsSchema.CreatePropertyDefinition("OnlyOrganizersCanSendMeetingChanges", typeof(bool), true);

		public static GlobalSettingsPropertyDefinition IsPartnerHostedOnly = GlobalSettingsSchema.CreatePropertyDefinition("IsPartnerHostedOnly", typeof(bool), false, false, new Func<GlobalSettingsPropertyDefinition, object>(GlobalSettingsSchema.GetPartnerHostedOnly));

		public static GlobalSettingsPropertyDefinition ExternalProxy = GlobalSettingsSchema.CreatePropertyDefinition("ExternalProxy", typeof(string), string.Empty);

		public static GlobalSettingsPropertyDefinition WriteActivityContextDiagnostics = GlobalSettingsSchema.CreatePropertyDefinition("WriteActivityContextDiagnostics", typeof(bool), false);

		public static GlobalSettingsPropertyDefinition WriteBudgetDiagnostics = GlobalSettingsSchema.CreatePropertyDefinition("WriteBudgetDiagnostics", typeof(bool), false);

		public static GlobalSettingsPropertyDefinition WriteExceptionDiagnostics = GlobalSettingsSchema.CreatePropertyDefinition("WriteExceptionDiagnostics", typeof(bool), false);

		public static GlobalSettingsPropertyDefinition LogCompressedExceptionDetails = GlobalSettingsSchema.CreatePropertyDefinition("LogCompressedExceptionDetails", typeof(bool), true);

		public static GlobalSettingsPropertyDefinition IncludeRequestInWatson = GlobalSettingsSchema.CreatePropertyDefinition("IncludeRequestInWatson", typeof(bool), false);

		public static GlobalSettingsPropertyDefinition SendWatsonReport = GlobalSettingsSchema.CreateVDirPropertyDefinition<bool>("SendWatsonReport", true, (ADMobileVirtualDirectory vdir) => vdir.SendWatsonReport);

		public static GlobalSettingsPropertyDefinition MeetingOrganizerCleanupTime = GlobalSettingsSchema.CreatePropertyDefinition("MeetingOrganizerCleanupTime", typeof(TimeSpan), TimeSpan.FromDays(1.0));

		public static GlobalSettingsPropertyDefinition MeetingOrganizerEntryLiveTime = GlobalSettingsSchema.CreatePropertyDefinition("MeetingOrganizerEntryLiveTime", typeof(TimeSpan), TimeSpan.FromDays(7.0));

		public static GlobalSettingsPropertyDefinition TimeTrackingEnabled = GlobalSettingsSchema.CreatePropertyDefinition("TimeTrackingEnabled", typeof(bool), true);

		public static GlobalSettingsPropertyDefinition MinGALSearchLength = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<int>("MinGALSearchLength", 4, 1, 256);

		public static GlobalSettingsPropertyDefinition RemoteDocumentsAllowedServers = GlobalSettingsSchema.CreateVDirPropertyDefinition<MultiValuedProperty<string>>("RemoteDocumentsAllowedServers", null, (ADMobileVirtualDirectory vdir) => vdir.RemoteDocumentsAllowedServers);

		public static GlobalSettingsPropertyDefinition RemoteDocumentsActionForUnknownServers = GlobalSettingsSchema.CreateVDirPropertyDefinition<RemoteDocumentsActions?>("RemoteDocumentsActionForUnknownServers", null, (ADMobileVirtualDirectory vdir) => vdir.RemoteDocumentsActionForUnknownServers);

		public static GlobalSettingsPropertyDefinition RemoteDocumentsBlockedServers = GlobalSettingsSchema.CreateVDirPropertyDefinition<MultiValuedProperty<string>>("RemoteDocumentsBlockedServers", null, (ADMobileVirtualDirectory vdir) => vdir.RemoteDocumentsBlockedServers);

		public static GlobalSettingsPropertyDefinition RemoteDocumentsInternalDomainSuffixList = GlobalSettingsSchema.CreateVDirPropertyDefinition<MultiValuedProperty<string>>("RemoteDocumentsInternalDomainSuffixList", null, (ADMobileVirtualDirectory vdir) => vdir.RemoteDocumentsInternalDomainSuffixList);

		public static GlobalSettingsPropertyDefinition EnableV160 = GlobalSettingsSchema.CreatePropertyDefinition("EnableV160", typeof(bool), false);

		public static GlobalSettingsPropertyDefinition MaxBackoffDuration = GlobalSettingsSchema.CreateConstrainedPropertyDefinition<TimeSpan>("MaxBackOffDuration", TimeSpan.FromSeconds(3600.0), TimeSpan.Zero, TimeSpan.FromSeconds(86400.0), false, new Func<GlobalSettingsPropertyDefinition, object>(GlobalSettingsSchema.ConvertSecondsToTimeSpan));

		public static GlobalSettingsPropertyDefinition AddBackOffReasonHeader = GlobalSettingsSchema.CreatePropertyDefinition("AddBackOffReasonHeader", typeof(bool), false);

		public static GlobalSettingsPropertyDefinition AllowFlightingOverrides = GlobalSettingsSchema.CreatePropertyDefinition("AllowFlightingOverrides", typeof(bool), false);

		public static GlobalSettingsPropertyDefinition DisableCharsetDetectionInCopyMessageContents = GlobalSettingsSchema.CreatePropertyDefinition("DisableCharsetDetectionInCopyMessageContents", typeof(bool), VariantConfiguration.InvariantNoFlightingSnapshot.ActiveSync.DisableCharsetDetectionInCopyMessageContents.Enabled, false, (GlobalSettingsPropertyDefinition propDef) => GlobalSettingsSchema.GetFlightingSetting(propDef, VariantConfiguration.InvariantNoFlightingSnapshot.ActiveSync.DisableCharsetDetectionInCopyMessageContents));

		public static GlobalSettingsPropertyDefinition UseOAuthMasterSidForSecurityContext = GlobalSettingsSchema.CreatePropertyDefinition("UseOAuthMasterSidForSecurityContext", typeof(bool), VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).ActiveSync.UseOAuthMasterSidForSecurityContext.Enabled, false, (GlobalSettingsPropertyDefinition propDef) => GlobalSettingsSchema.GetFlightingSetting(propDef, VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).ActiveSync.UseOAuthMasterSidForSecurityContext));

		public static GlobalSettingsPropertyDefinition GetGoidFromCalendarItemForMeetingResponse = GlobalSettingsSchema.CreatePropertyDefinition("GetGoidFromCalendarItemForMeetingResponse", typeof(bool), VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).ActiveSync.GetGoidFromCalendarItemForMeetingResponse.Enabled, false, (GlobalSettingsPropertyDefinition propDef) => GlobalSettingsSchema.GetFlightingSetting(propDef, VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).ActiveSync.GetGoidFromCalendarItemForMeetingResponse));

		internal static GlobalSettingsPropertyDefinition Supporting_MinHeartbeatInterval = GlobalSettingsSchema.CreatePropertyDefinition("MinHeartbeatInterval", typeof(int), 60);

		internal static GlobalSettingsPropertyDefinition Supporting_MaxHeartbeatInterval = GlobalSettingsSchema.CreatePropertyDefinition("MaxHeartbeatInterval", typeof(int), 3540);

		internal static GlobalSettingsPropertyDefinition ClientAccessRulesLogPeriodicEvent = GlobalSettingsSchema.CreatePropertyDefinition("ClientAccessRulesLogPeriodicEvent", typeof(bool), false);

		internal static GlobalSettingsPropertyDefinition ClientAccessRulesLatencyThreshold = GlobalSettingsSchema.CreatePropertyDefinition("ClientAccessRulesLatencyThreshold", typeof(double), 50.0);
	}
}
