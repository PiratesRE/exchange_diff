using System;
using System.IO;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class MSExchangeISWorkerRPCFirewallRule : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "MSExchangeIS - RPC";
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
				return Path.Combine(ExchangeFirewallRule.ExchangeInstallPath, "Bin\\Microsoft.Exchange.Store.Worker.exe");
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
				return "RPC";
			}
		}

		private const string RuleApplicationRelativePath = "Bin\\Microsoft.Exchange.Store.Worker.exe";
	}
}
