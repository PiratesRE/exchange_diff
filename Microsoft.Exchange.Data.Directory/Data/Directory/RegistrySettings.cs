using System;
using Microsoft.Exchange.Data.Directory.GlobalLocatorService;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	internal static class RegistrySettings
	{
		internal static class MSExchangeADAccess
		{
			private const string MsExchangeADAccessRegistryPath = "SYSTEM\\CurrentControlSet\\Services\\MSExchange ADAccess";
		}

		internal class ExchangeServerCurrentVersion
		{
			internal static GlsEnvironmentType GlsEnvironmentType
			{
				get
				{
					return Globals.GetEnumValueFromRegistry<GlsEnvironmentType>("SOFTWARE\\Microsoft\\ExchangeServer\\v15", RegistrySettings.ExchangeServerCurrentVersion.GlobalDirectoryEnvironmentTypeValue, GlsEnvironmentType.Prod, ExTraceGlobals.GLSTracer);
				}
			}

			public static string SmtpNextHopDomainFormat
			{
				get
				{
					string valueFromRegistry = Globals.GetValueFromRegistry<string>("SOFTWARE\\Microsoft\\ExchangeServer\\v15", RegistrySettings.ExchangeServerCurrentVersion.SmtpNextHopDomainFormatValue, null, ExTraceGlobals.GLSTracer);
					if (string.IsNullOrEmpty(valueFromRegistry))
					{
						throw new GlsPermanentException(DirectoryStrings.PermanentGlsError(string.Format("{0} is not present", RegistrySettings.ExchangeServerCurrentVersion.SmtpNextHopDomainFormatValue)));
					}
					return valueFromRegistry;
				}
			}

			internal static int GLSTenantCacheExpiry
			{
				get
				{
					return Globals.GetIntValueFromRegistry("SOFTWARE\\Microsoft\\ExchangeServer\\v15", RegistrySettings.ExchangeServerCurrentVersion.GLSTenantCacheExpiryValue, (int)TimeSpan.FromHours(1.0).TotalSeconds, 0);
				}
			}

			private static Random Random
			{
				get
				{
					if (RegistrySettings.ExchangeServerCurrentVersion.randomObject == null)
					{
						RegistrySettings.ExchangeServerCurrentVersion.randomObject = new Random();
					}
					return RegistrySettings.ExchangeServerCurrentVersion.randomObject;
				}
			}

			internal static int NotificationThrottlingTimeMinutes
			{
				get
				{
					return Globals.GetIntValueFromRegistry(RegistrySettings.ExchangeServerCurrentVersion.NotificationThrottlingRegistryKeyName, 15, 0) + RegistrySettings.ExchangeServerCurrentVersion.Random.Next(15);
				}
			}

			private const string registryKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15";

			private static readonly string GlobalDirectoryEnvironmentTypeValue = "GlobalDirectoryEnvironmentType";

			private static readonly string SmtpNextHopDomainFormatValue = "SmtpNextHopDomainFormat";

			private static readonly string GLSTenantCacheExpiryValue = "GLSTenantCacheExpiry";

			[ThreadStatic]
			private static Random randomObject;

			private static readonly string NotificationThrottlingRegistryKeyName = "NotificationThrottling";
		}
	}
}
