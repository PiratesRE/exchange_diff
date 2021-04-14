using System;
using System.IO;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class MSExchangePOP3FirewallRule : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "MSExchangePOP3";
			}
		}

		protected override IndirectStrings DescriptionIndirectString
		{
			get
			{
				return IndirectStrings.IDS_FIREWALLRULE_DESC_MSEXCHANGEPOP3;
			}
		}

		protected override string ApplicationPath
		{
			get
			{
				return Path.Combine(ExchangeFirewallRule.ExchangeInstallPath, "FrontEnd\\PopImap\\Microsoft.Exchange.Pop3Service.exe");
			}
		}

		protected override string ServiceName
		{
			get
			{
				return "MSExchangePOP3";
			}
		}

		protected override string LocalPorts
		{
			get
			{
				return "110,995";
			}
		}

		private const string RuleApplicationRelativePath = "FrontEnd\\PopImap\\Microsoft.Exchange.Pop3Service.exe";
	}
}
