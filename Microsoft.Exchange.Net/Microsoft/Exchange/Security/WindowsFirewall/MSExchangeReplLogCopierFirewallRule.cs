using System;
using System.IO;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class MSExchangeReplLogCopierFirewallRule : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "MSExchangerepl - Log Copier";
			}
		}

		protected override IndirectStrings DescriptionIndirectString
		{
			get
			{
				return IndirectStrings.IDS_FIREWALLRULE_DESC_MSEXCHANGEREPLPORTCOPIER;
			}
		}

		protected override string ApplicationPath
		{
			get
			{
				return Path.Combine(ExchangeFirewallRule.ExchangeInstallPath, "Bin\\msexchangerepl.exe");
			}
		}

		protected override string ServiceName
		{
			get
			{
				return "MSExchangerepl";
			}
		}

		protected override string LocalPorts
		{
			get
			{
				return "64327";
			}
		}

		private const string RuleApplicationRelativePath = "Bin\\msexchangerepl.exe";
	}
}
