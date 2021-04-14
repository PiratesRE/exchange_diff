using System;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Common
{
	public class RegistryConfigManager
	{
		private static void ReadAllConfigsIfRequired()
		{
			long ticks = DateTime.UtcNow.Ticks;
			if (RegistryConfigManager.lastAccessTicks != 0L && RegistryConfigManager.lastAccessTicks <= ticks)
			{
				if (ticks - RegistryConfigManager.lastAccessTicks <= RegistryConfigManager.RegistryReadIntervalTicks)
				{
					return;
				}
			}
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Diagnostics"))
				{
					if (registryKey != null)
					{
						object value = registryKey.GetValue("Iso2022JpEncodingOverride");
						if (value != null && value is int)
						{
							RegistryConfigManager.iso2022JpEncodingOverride = (int)value;
						}
						object value2 = registryKey.GetValue("HtmlEncapsulationOverride");
						if (value2 != null && value2 is int)
						{
							RegistryConfigManager.htmlEncapsulationOverride = (int)value2;
						}
					}
				}
			}
			catch (Exception)
			{
			}
			finally
			{
				RegistryConfigManager.lastAccessTicks = ticks;
			}
		}

		internal static int Iso2022JpEncodingOverride
		{
			get
			{
				RegistryConfigManager.ReadAllConfigsIfRequired();
				return RegistryConfigManager.iso2022JpEncodingOverride;
			}
		}

		internal static bool HtmlEncapsulationOverride
		{
			get
			{
				RegistryConfigManager.ReadAllConfigsIfRequired();
				return RegistryConfigManager.htmlEncapsulationOverride != 0;
			}
		}

		internal const string RegistryPath = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Diagnostics";

		internal const string Iso2022JpEncodingOverrideRegistryValueName = "Iso2022JpEncodingOverride";

		internal const string HtmlEncapsulationOverrideRegistryValueName = "HtmlEncapsulationOverride";

		internal static readonly long RegistryReadIntervalTicks = 600000000L;

		private static long lastAccessTicks;

		private static int iso2022JpEncodingOverride = 1;

		private static int htmlEncapsulationOverride = 1;
	}
}
