using System;
using System.IO;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class MSExchangeIMAP4BeFirewallRule : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "MSExchangeIMAP4BE";
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
				return Path.Combine(ExchangeFirewallRule.ExchangeInstallPath, "ClientAccess\\PopImap\\Microsoft.Exchange.Imap4Service.exe");
			}
		}

		protected override string ServiceName
		{
			get
			{
				return "MSExchangeIMAP4BE";
			}
		}

		protected override string LocalPorts
		{
			get
			{
				return "9933,1993";
			}
		}

		private const string RuleApplicationRelativePath = "ClientAccess\\PopImap\\Microsoft.Exchange.Imap4Service.exe";
	}
}
