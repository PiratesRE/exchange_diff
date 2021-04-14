using System;
using System.IO;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class MSExchangeTransportWorkerRPCFirewallRule : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "MSExchangeTransportWorker - RPC";
			}
		}

		protected override IndirectStrings DescriptionIndirectString
		{
			get
			{
				return IndirectStrings.IDS_FIREWALLRULE_DESC_MSEXCHANGETRANSPORTWORKERRPC;
			}
		}

		protected override string ApplicationPath
		{
			get
			{
				return Path.Combine(ExchangeFirewallRule.ExchangeInstallPath, "Bin\\edgetransport.exe");
			}
		}

		protected override string ServiceName
		{
			get
			{
				return "MSExchangeTransportWorker";
			}
		}

		protected override string LocalPorts
		{
			get
			{
				return "RPC";
			}
		}

		private const string RuleApplicationRelativePath = "Bin\\edgetransport.exe";
	}
}
