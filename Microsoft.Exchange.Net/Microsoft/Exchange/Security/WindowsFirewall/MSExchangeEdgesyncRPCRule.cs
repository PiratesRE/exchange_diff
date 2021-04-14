using System;
using System.IO;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class MSExchangeEdgesyncRPCRule : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "MSExchangeEdgesync - RPC";
			}
		}

		protected override IndirectStrings DescriptionIndirectString
		{
			get
			{
				return IndirectStrings.IDS_FIREWALLRULE_DESC_MSEXCHANGEEDGESYNCRPC;
			}
		}

		protected override string ApplicationPath
		{
			get
			{
				return Path.Combine(ExchangeFirewallRule.ExchangeInstallPath, "Bin\\Microsoft.Exchange.EdgeSyncSvc.exe");
			}
		}

		protected override string ServiceName
		{
			get
			{
				return "MSExchangeEdgesync";
			}
		}

		protected override string LocalPorts
		{
			get
			{
				return "RPC";
			}
		}

		private const string RuleApplicationRelativePath = "Bin\\Microsoft.Exchange.EdgeSyncSvc.exe";
	}
}
