using System;
using System.IO;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class MSExchangeMailboxReplicationFirewallRule : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "MSExchangeMailboxReplication";
			}
		}

		protected override IndirectStrings DescriptionIndirectString
		{
			get
			{
				return IndirectStrings.IDS_FIREWALLRULE_DESC_MSEXCHANGEMAILBOXREPLICATION;
			}
		}

		protected override string ApplicationPath
		{
			get
			{
				return Path.Combine(ExchangeFirewallRule.ExchangeInstallPath, "Bin\\MSExchangeMailboxReplication.exe");
			}
		}

		protected override string ServiceName
		{
			get
			{
				return "MSExchangeMailboxReplication";
			}
		}

		protected override string LocalPorts
		{
			get
			{
				return "808";
			}
		}

		private const string RuleApplicationRelativePath = "Bin\\MSExchangeMailboxReplication.exe";
	}
}
