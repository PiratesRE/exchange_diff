using System;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ApplicationLogic.Diagnostics
{
	internal sealed class SettingOverrideSyncHandler : ExchangeDiagnosableWrapper<XElement>
	{
		protected override string UsageText
		{
			get
			{
				return "Diagnostic info for Variant Configuration.";
			}
		}

		protected override string UsageSample
		{
			get
			{
				return "Get a list of current overrides for MSExchangeMailboxAssistants process:\r\n                         Get-ExchangeDiagnosticInfo -ProcessName MSExchangeMailboxAssistants -Component VariantConfiguration -Argument overrides\r\n\r\n                         Refresh overrides and show up-to-date ones:\r\n                         Get-ExchangeDiagnosticInfo -ProcessName MSExchangeMailboxAssistants -Component VariantConfiguration -Argument refresh\r\n\r\n                         Don't specify any arguments to see a full list of supported arguments:\r\n                         Get-ExchangeDiagnosticInfo -ProcessName MSExchangeMailboxAssistants -Component VariantConfiguration";
			}
		}

		public static SettingOverrideSyncHandler GetInstance()
		{
			if (SettingOverrideSyncHandler.instance == null)
			{
				lock (SettingOverrideSyncHandler.lockObject)
				{
					if (SettingOverrideSyncHandler.instance == null)
					{
						SettingOverrideSyncHandler.instance = new SettingOverrideSyncHandler();
					}
				}
			}
			return SettingOverrideSyncHandler.instance;
		}

		private SettingOverrideSyncHandler()
		{
			SettingOverrideSync.Instance.Start(true);
		}

		protected override string ComponentName
		{
			get
			{
				return SettingOverrideSync.Instance.GetDiagnosticComponentName();
			}
		}

		internal override XElement GetExchangeDiagnosticsInfoData(DiagnosableParameters argument)
		{
			return SettingOverrideSync.Instance.GetDiagnosticInfo(argument);
		}

		protected override void InternalOnStop()
		{
			SettingOverrideSync.Instance.Stop();
		}

		private static SettingOverrideSyncHandler instance;

		private static object lockObject = new object();
	}
}
