using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Win32;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public static class OwaRegistryKeys
	{
		public static string IMImplementationDllPath
		{
			get
			{
				return OwaRegistryKeys.GetStringValue(OwaRegistryKeys.implementationDllPathKey);
			}
		}

		public static string IMImplementationDllPathKey
		{
			get
			{
				return OwaRegistryKeys.implementationDllPathKey.Name;
			}
		}

		public static string InstalledOwaVersion
		{
			get
			{
				return OwaRegistryKeys.GetStringValue(OwaRegistryKeys.owaVersion);
			}
		}

		public static string InstalledNextOwaVersion
		{
			get
			{
				return OwaRegistryKeys.GetStringValue(OwaRegistryKeys.nextOwaVersion);
			}
		}

		public static void Initialize()
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "OwaRegistryKeys.Initialize");
			foreach (string text in OwaRegistryKeys.keySetMap.Keys)
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(text))
				{
					if (registryKey != null)
					{
						foreach (OwaRegistryKey owaRegistryKey in OwaRegistryKeys.keySetMap[text])
						{
							OwaRegistryKeys.keyValueCache[owaRegistryKey] = OwaRegistryKeys.ReadKeyValue(registryKey, owaRegistryKey);
						}
					}
				}
			}
		}

		public static void UpdateOwaSetupVersionsCache()
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(OwaRegistryKeys.OwaSetupInstallKey))
			{
				if (registryKey != null)
				{
					foreach (OwaRegistryKey owaRegistryKey in OwaRegistryKeys.keySetMap[OwaRegistryKeys.OwaSetupInstallKey])
					{
						OwaRegistryKeys.keyValueCache[owaRegistryKey] = OwaRegistryKeys.ReadKeyValue(registryKey, owaRegistryKey);
					}
				}
			}
		}

		private static object ReadKeyValue(RegistryKey keyContainer, OwaRegistryKey owaKey)
		{
			ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "Reading registry key \"{0}\"", owaKey.Name);
			object obj;
			if (owaKey.KeyType == typeof(int))
			{
				obj = keyContainer.GetValue(owaKey.Name, owaKey.DefaultValue);
				if (obj.GetType() != typeof(int))
				{
					obj = null;
				}
			}
			else if (owaKey.KeyType == typeof(uint))
			{
				obj = keyContainer.GetValue(owaKey.Name, owaKey.DefaultValue);
				if (obj.GetType() != typeof(uint))
				{
					obj = null;
				}
			}
			else if (owaKey.KeyType == typeof(string))
			{
				obj = keyContainer.GetValue(owaKey.Name, owaKey.DefaultValue);
				if (obj.GetType() != typeof(string))
				{
					obj = null;
				}
			}
			else
			{
				if (!(owaKey.KeyType == typeof(bool)))
				{
					return null;
				}
				object value = keyContainer.GetValue(owaKey.Name, owaKey.DefaultValue);
				if (value.GetType() != typeof(int))
				{
					obj = null;
				}
				else
				{
					obj = ((int)value != 0);
				}
			}
			if (obj == null)
			{
				ExTraceGlobals.CoreTracer.TraceDebug(0L, "Couldn't find key or key format/type was incorrect, using default value");
				obj = owaKey.DefaultValue;
			}
			ExTraceGlobals.CoreTracer.TraceDebug<string, object>(0L, "Configuration registry key \"{0}\" read with value=\"{1}\"", owaKey.Name, obj);
			return obj;
		}

		private static T GetValue<T>(OwaRegistryKey key)
		{
			object obj = null;
			if (OwaRegistryKeys.keyValueCache.TryGetValue(key, out obj))
			{
				return (T)((object)obj);
			}
			return (T)((object)key.DefaultValue);
		}

		private static string GetStringValue(OwaRegistryKey key)
		{
			return OwaRegistryKeys.GetValue<string>(key);
		}

		private static bool GetBoolValue(OwaRegistryKey key)
		{
			return OwaRegistryKeys.GetValue<bool>(key);
		}

		private static int GetIntValue(OwaRegistryKey key)
		{
			return OwaRegistryKeys.GetValue<int>(key);
		}

		private static uint GetUIntValue(OwaRegistryKey key)
		{
			return OwaRegistryKeys.GetValue<uint>(key);
		}

		internal static readonly string OwaSetupInstallKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup";

		internal static readonly string OwaRegKeyBase = "SYSTEM\\CurrentControlSet\\Services\\MSExchange OWA";

		private static OwaRegistryKey implementationDllPathKey = new OwaRegistryKey("ImplementationDLLPath", typeof(string), string.Empty);

		private static OwaRegistryKey owaVersion = new OwaRegistryKey("OwaVersion", typeof(string), string.Empty);

		private static OwaRegistryKey nextOwaVersion = new OwaRegistryKey("NextOwaVersion", typeof(string), string.Empty);

		private static List<OwaRegistryKey> owaIMKeys = new List<OwaRegistryKey>(1)
		{
			OwaRegistryKeys.implementationDllPathKey
		};

		private static List<OwaRegistryKey> owaSetupKeys = new List<OwaRegistryKey>(2)
		{
			OwaRegistryKeys.owaVersion,
			OwaRegistryKeys.nextOwaVersion
		};

		private static Dictionary<string, List<OwaRegistryKey>> keySetMap = new Dictionary<string, List<OwaRegistryKey>>
		{
			{
				OwaRegistryKeys.OwaRegKeyBase + "\\InstantMessaging",
				OwaRegistryKeys.owaIMKeys
			},
			{
				OwaRegistryKeys.OwaSetupInstallKey,
				OwaRegistryKeys.owaSetupKeys
			}
		};

		private static int keySetMapTotalCount = OwaRegistryKeys.keySetMap.Sum((KeyValuePair<string, List<OwaRegistryKey>> kvPair) => kvPair.Value.Count);

		private static Dictionary<OwaRegistryKey, object> keyValueCache = new Dictionary<OwaRegistryKey, object>(OwaRegistryKeys.keySetMapTotalCount);
	}
}
