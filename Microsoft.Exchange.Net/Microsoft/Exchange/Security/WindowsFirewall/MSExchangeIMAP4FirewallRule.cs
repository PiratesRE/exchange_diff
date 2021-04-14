using System;
using System.IO;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class MSExchangeIMAP4FirewallRule : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "MSExchangeIMAP4";
			}
		}

		protected override IndirectStrings DescriptionIndirectString
		{
			get
			{
				return IndirectStrings.IDS_FIREWALLRULE_DESC_MSEXCHANGEIMAP4;
			}
		}

		protected override string ApplicationPath
		{
			get
			{
				return Path.Combine(ExchangeFirewallRule.ExchangeInstallPath, "FrontEnd\\PopImap\\Microsoft.Exchange.Imap4Service.exe");
			}
		}

		protected override string ServiceName
		{
			get
			{
				return "MSExchangeIMAP4";
			}
		}

		protected override string LocalPorts
		{
			get
			{
				return "143,993";
			}
		}

		private const string RuleApplicationRelativePath = "FrontEnd\\PopImap\\Microsoft.Exchange.Imap4Service.exe";
	}
}
