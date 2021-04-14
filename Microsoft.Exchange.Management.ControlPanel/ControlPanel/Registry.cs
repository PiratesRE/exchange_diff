using System;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal static class Registry
	{
		static Registry()
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange OWA", false))
			{
				if (registryKey != null)
				{
					Registry.AllowInternalUntrustedCerts = registryKey.GetBoolean("AllowInternalUntrustedCerts", Registry.AllowInternalUntrustedCerts);
					Registry.AllowProxyingWithoutSsl = registryKey.GetBoolean("AllowProxyingWithoutSsl", Registry.AllowProxyingWithoutSsl);
					Registry.SslOffloaded = registryKey.GetBoolean("SSLOffloaded", Registry.SslOffloaded);
				}
			}
		}

		private static bool GetBoolean(this RegistryKey key, string valueName, bool defaultValue)
		{
			object value = key.GetValue(valueName, defaultValue);
			if (value is int)
			{
				return (int)value != 0;
			}
			return defaultValue;
		}

		private const string OwaRegistryPath = "SYSTEM\\CurrentControlSet\\Services\\MSExchange OWA";

		private const string AllowInternalUntrustedCertsKey = "AllowInternalUntrustedCerts";

		private const string AllowProxyingWithoutSslKey = "AllowProxyingWithoutSsl";

		private const string SslOffloadedKey = "SSLOffloaded";

		public static readonly bool AllowInternalUntrustedCerts = true;

		public static readonly bool AllowProxyingWithoutSsl = false;

		public static readonly bool SslOffloaded = false;
	}
}
