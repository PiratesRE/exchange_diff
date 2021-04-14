using System;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AirSync
{
	internal sealed class GlobalSettingsHandler : ExchangeDiagnosableWrapper<GlobalSettingsResult>
	{
		protected override string UsageText
		{
			get
			{
				return "The GlobalSettings handler is a diagnostics handler that returns information about the current values exposed through the GlobalSettings singleton. These values are not necessarily the same as the values in web.config as they can be replaced due to parse or range errors, missing values, etc...";
			}
		}

		protected override string UsageSample
		{
			get
			{
				return "Example 1: Return all values.\r\n   Get-ExchangeDiagnosticInfo -Process MSExchangeSyncAppPool -Component GlobalSettings\r\n\r\nExample 2: Return a specific appSetting.\r\n   Get-ExchangeDiagnosticInfo -Process MSExchangeSyncAppPool -Component GlobalSettings -Argument propertyName=<AppSettingName>\r\n\r\nExample 3: Return all properties matching a name pattern.\r\n   Get-ExchangeDiagnosticInfo -Process MSExchangeSyncAppPool -Component GlobalSettings -Argument propertyName=*PartialString*\r\n\r\nExample 4: Return all properties that are not set to default value.\r\n   Get-ExchangeDiagnosticInfo -Process MSExchangeSyncAppPool -Component GlobalSettings -Argument notDefault\r\n\r\nYou can also combine the two arguments together, separated by a comma such as \"propertyName=*Timeout*,notDefault";
			}
		}

		public static GlobalSettingsHandler GetInstance()
		{
			if (GlobalSettingsHandler.instance == null)
			{
				lock (GlobalSettingsHandler.lockObject)
				{
					if (GlobalSettingsHandler.instance == null)
					{
						GlobalSettingsHandler.instance = new GlobalSettingsHandler();
					}
				}
			}
			return GlobalSettingsHandler.instance;
		}

		private GlobalSettingsHandler()
		{
		}

		protected override string ComponentName
		{
			get
			{
				return "GlobalSettings";
			}
		}

		internal override GlobalSettingsResult GetExchangeDiagnosticsInfoData(DiagnosableParameters argument)
		{
			string text = null;
			bool returnOnlySettingsThatAreNotDefault = false;
			if (!string.IsNullOrEmpty(argument.Argument))
			{
				string[] array = argument.Argument.Split(new char[]
				{
					','
				});
				foreach (string text2 in array)
				{
					if (text2.StartsWith("propertyname=", StringComparison.OrdinalIgnoreCase))
					{
						text = text2.Substring("propertyname=".Length);
						text = base.RemoveQuotes(text);
					}
					else if (text2.StartsWith("notdefault"))
					{
						returnOnlySettingsThatAreNotDefault = true;
					}
				}
			}
			return GlobalSettingsResult.Create(text, returnOnlySettingsThatAreNotDefault);
		}

		private const string propertyNamePrefix = "propertyname=";

		private const string notDefaultPrefix = "notdefault";

		private static GlobalSettingsHandler instance;

		private static object lockObject = new object();
	}
}
