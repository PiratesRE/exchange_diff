using System;
using System.IO;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class MSExchangePOP3BeFirewallRule : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "MSExchangePOP3BE";
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
				return Path.Combine(ExchangeFirewallRule.ExchangeInstallPath, "ClientAccess\\PopImap\\Microsoft.Exchange.Pop3Service.exe");
			}
		}

		protected override string ServiceName
		{
			get
			{
				return "MSExchangePOP3BE";
			}
		}

		protected override string LocalPorts
		{
			get
			{
				return "9955,1995";
			}
		}

		private const string RuleApplicationRelativePath = "ClientAccess\\PopImap\\Microsoft.Exchange.Pop3Service.exe";
	}
}
