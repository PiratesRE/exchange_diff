using System;
using System.Security;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Win32;

namespace Microsoft.Exchange.Net
{
	internal static class SslConfiguration
	{
		internal static bool AllowInternalUntrustedCerts
		{
			get
			{
				if (!SslConfiguration.attemptedToReadFromRegistry)
				{
					SslConfiguration.ReadFromRegistry();
				}
				return SslConfiguration.allowInternalUntrustedCerts;
			}
		}

		internal static bool AllowExternalUntrustedCerts
		{
			get
			{
				if (!SslConfiguration.attemptedToReadFromRegistry)
				{
					SslConfiguration.ReadFromRegistry();
				}
				return SslConfiguration.allowExternalUntrustedCerts;
			}
		}

		private static void ReadFromRegistry()
		{
			lock (SslConfiguration.locker)
			{
				if (!SslConfiguration.attemptedToReadFromRegistry)
				{
					RegistryKey registryKey = null;
					try
					{
						registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange OWA");
						if (registryKey != null)
						{
							SslConfiguration.allowInternalUntrustedCerts = SslConfiguration.ReadBoolValue(registryKey, "AllowInternalUntrustedCerts", SslConfiguration.allowInternalUntrustedCertsDefault);
							SslConfiguration.allowExternalUntrustedCerts = SslConfiguration.ReadBoolValue(registryKey, "AllowExternalUntrustedCerts", SslConfiguration.allowExternalUntrustedCertsDefault);
						}
						else
						{
							SslConfiguration.ConfigurationTracer.TraceError(0L, "Error reading SSL configuration values from Registry: keyContainer is null. Using default values.");
						}
					}
					catch (SecurityException arg)
					{
						SslConfiguration.ConfigurationTracer.TraceError<SecurityException>(0L, "Exception {0} encountered while reading SSL configuration values from Registry. Using default values.", arg);
					}
					finally
					{
						if (registryKey != null)
						{
							registryKey.Close();
						}
						SslConfiguration.attemptedToReadFromRegistry = true;
					}
				}
			}
		}

		private static bool ReadBoolValue(RegistryKey keyContainer, string valueName, bool defaultValue)
		{
			if (keyContainer != null)
			{
				object value = keyContainer.GetValue(valueName);
				if (value != null && value is int)
				{
					SslConfiguration.ConfigurationTracer.TraceDebug<string, object>(0L, "Registry value {0} was found. Its value is {1}", valueName, value);
					return (int)value != 0;
				}
				SslConfiguration.ConfigurationTracer.TraceDebug<string, bool>(0L, "Registry value {0} was not found or invalid. Returning default value: {1}.", valueName, defaultValue);
			}
			else
			{
				SslConfiguration.ConfigurationTracer.TraceDebug<string, bool>(0L, "Container {0} was not found in registry. Returning default value: {1}.", "SYSTEM\\CurrentControlSet\\Services\\MSExchange OWA", defaultValue);
			}
			return defaultValue;
		}

		private const string containerPath = "SYSTEM\\CurrentControlSet\\Services\\MSExchange OWA";

		private const string allowInternalUntrustedCertsName = "AllowInternalUntrustedCerts";

		private const string allowExternalUntrustedCertsName = "AllowExternalUntrustedCerts";

		private static bool allowInternalUntrustedCertsDefault = true;

		private static bool allowExternalUntrustedCertsDefault = false;

		private static bool allowInternalUntrustedCerts = SslConfiguration.allowInternalUntrustedCertsDefault;

		private static bool allowExternalUntrustedCerts = SslConfiguration.allowExternalUntrustedCertsDefault;

		private static readonly Trace ConfigurationTracer = ExTraceGlobals.ConfigurationTracer;

		private static bool attemptedToReadFromRegistry = false;

		private static object locker = new object();
	}
}
