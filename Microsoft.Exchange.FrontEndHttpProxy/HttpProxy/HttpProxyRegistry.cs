using System;
using System.Security;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Win32;

namespace Microsoft.Exchange.HttpProxy
{
	internal static class HttpProxyRegistry
	{
		private static bool GetOWARegistryValue(string valueName, bool defaultValue)
		{
			bool result;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange OWA", false))
				{
					object value = registryKey.GetValue(valueName);
					if (value == null || !(value is int))
					{
						result = defaultValue;
					}
					else
					{
						result = ((int)value != 0);
					}
				}
			}
			catch (SecurityException)
			{
				ExTraceGlobals.VerboseTracer.TraceError<string, bool>(0L, "[HttpProxyRegistry::GetOWARegistryValue] Security exception encountered while retrieving {0} registry value.  Defaulting to {1}", valueName, defaultValue);
				result = defaultValue;
			}
			catch (UnauthorizedAccessException)
			{
				ExTraceGlobals.VerboseTracer.TraceError<string, bool>(0L, "[HttpProxyRegistry::GetOWARegistryValue] Unauthorized exception encountered while retrieving {0} registry value.  Defaulting to {1}", valueName, defaultValue);
				result = defaultValue;
			}
			return result;
		}

		internal const string MSExchangeOWARegistryPath = "SYSTEM\\CurrentControlSet\\Services\\MSExchange OWA";

		public static readonly LazyMember<bool> OwaAllowInternalUntrustedCerts = new LazyMember<bool>(() => HttpProxyRegistry.GetOWARegistryValue("AllowInternalUntrustedCerts", true));

		public static readonly LazyMember<bool> AreGccStoredSecretKeysValid = new LazyMember<bool>(() => HttpProxyRegistry.AreGccStoredSecretKeysValid.Member);
	}
}
