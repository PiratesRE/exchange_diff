using System;
using System.IO;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class MSExchangeMigrationRPCFirewallRule : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "MSExchangeMigration - RPC";
			}
		}

		protected override IndirectStrings DescriptionIndirectString
		{
			get
			{
				return IndirectStrings.IDS_FIREWALLRULE_DESC_MSEXCHANGEMIGRATIONRPC;
			}
		}

		protected override string ApplicationPath
		{
			get
			{
				return Path.Combine(ExchangeFirewallRule.ExchangeInstallPath, "Bin\\msexchangemigration.exe");
			}
		}

		protected override string ServiceName
		{
			get
			{
				return "MSExchangeMigration";
			}
		}

		protected override string LocalPorts
		{
			get
			{
				return "RPC";
			}
		}

		private const string RuleApplicationRelativePath = "Bin\\msexchangemigration.exe";
	}
}
