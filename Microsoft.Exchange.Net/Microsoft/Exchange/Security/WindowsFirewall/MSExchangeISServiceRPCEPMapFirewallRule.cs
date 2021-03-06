using System;
using System.IO;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class MSExchangeISServiceRPCEPMapFirewallRule : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "MSExchangeIS - RPCEPMap";
			}
		}

		protected override IndirectStrings DescriptionIndirectString
		{
			get
			{
				return IndirectStrings.IDS_FIREWALLRULE_DESC_MSEXCHANGEIS;
			}
		}

		protected override string ApplicationPath
		{
			get
			{
				return Path.Combine(ExchangeFirewallRule.ExchangeInstallPath, "Bin\\Microsoft.Exchange.Store.Service.exe");
			}
		}

		protected override string ServiceName
		{
			get
			{
				return "MSExchangeIS";
			}
		}

		protected override string LocalPorts
		{
			get
			{
				return "RPC-EPMap";
			}
		}

		private const string RuleApplicationRelativePath = "Bin\\Microsoft.Exchange.Store.Service.exe";
	}
}
