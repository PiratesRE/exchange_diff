using System;
using System.IO;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class MSExchangeReplRPCEPMapFirewallRule : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "MSExchangerepl - RPCEPMap";
			}
		}

		protected override IndirectStrings DescriptionIndirectString
		{
			get
			{
				return IndirectStrings.IDS_FIREWALLRULE_DESC_MSEXCHANGEREPLRPCEPMAP;
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
				return "RPC-EPMap";
			}
		}

		private const string RuleApplicationRelativePath = "Bin\\msexchangerepl.exe";
	}
}
