using System;
using System.IO;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class MSExchangeABNumberedFirewallRule : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "MSExchangeAB-RpcHttp";
			}
		}

		protected override IndirectStrings DescriptionIndirectString
		{
			get
			{
				return IndirectStrings.IDS_FIREWALLRULE_DESC_MSEXCHANGEAB;
			}
		}

		protected override string ApplicationPath
		{
			get
			{
				return Path.Combine(ExchangeFirewallRule.ExchangeInstallPath, "Bin\\Microsoft.Exchange.RpcClientAccess.Service.exe");
			}
		}

		protected override string ServiceName
		{
			get
			{
				return "MSExchangeRPC";
			}
		}

		protected override string LocalPorts
		{
			get
			{
				return "6002,6004";
			}
		}

		private const string RuleApplicationRelativePath = "Bin\\Microsoft.Exchange.RpcClientAccess.Service.exe";
	}
}
