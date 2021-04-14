using System;
using System.IO;
using System.Security;
using Microsoft.Exchange.Diagnostics.Components.Common;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Transport
{
	internal sealed class InternalConfiguration
	{
		static InternalConfiguration()
		{
			InternalConfiguration.ReadEaiConfig();
		}

		internal static void ReadEaiConfig()
		{
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Diagnostics"))
				{
					if (registryKey != null)
					{
						object value = registryKey.GetValue("EaiEnabled");
						if (value != null)
						{
							bool.TryParse(value.ToString(), out InternalConfiguration.eaiEnabled);
						}
					}
				}
			}
			catch (SecurityException exception)
			{
				InternalConfiguration.HandleRegistryReadError(exception);
			}
			catch (UnauthorizedAccessException exception2)
			{
				InternalConfiguration.HandleRegistryReadError(exception2);
			}
			catch (IOException exception3)
			{
				InternalConfiguration.HandleRegistryReadError(exception3);
			}
		}

		public static bool IsEaiEnabled()
		{
			return InternalConfiguration.eaiEnabled;
		}

		private static void HandleRegistryReadError(Exception exception)
		{
			ExTraceGlobals.CommonTracer.TraceError<string, string>(0L, "Exception occurred while reading EAI configuration from registry.{0}{1}", Environment.NewLine, exception.ToString());
		}

		internal const string RegistryPath = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Diagnostics";

		internal const string EaiEnabledRegistryValueName = "EaiEnabled";

		internal const bool EaiEnabledDefaultValue = false;

		private static bool eaiEnabled;
	}
}
