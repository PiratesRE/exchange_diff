using System;
using System.IO;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class MSExchangeMailboxAssistantsRPCEPMapFirewallRule : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "MSExchangeMailboxAssistants - RPCEPMap";
			}
		}

		protected override IndirectStrings DescriptionIndirectString
		{
			get
			{
				return IndirectStrings.IDS_FIREWALLRULE_DESC_MSEXCHANGEMAILBOXASSISTANTSRPCEPMAP;
			}
		}

		protected override string ApplicationPath
		{
			get
			{
				return Path.Combine(ExchangeFirewallRule.ExchangeInstallPath, "Bin\\MSExchangeMailboxAssistants.exe");
			}
		}

		protected override string ServiceName
		{
			get
			{
				return "MSExchangeMailboxAssistants";
			}
		}

		protected override string LocalPorts
		{
			get
			{
				return "RPC-EPMap";
			}
		}

		private const string RuleApplicationRelativePath = "Bin\\MSExchangeMailboxAssistants.exe";
	}
}
