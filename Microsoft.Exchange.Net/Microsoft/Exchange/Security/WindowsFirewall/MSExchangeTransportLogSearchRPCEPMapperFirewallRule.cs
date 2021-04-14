using System;
using System.IO;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class MSExchangeTransportLogSearchRPCEPMapperFirewallRule : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "MSExchangeTransportLogSearch - RPCEPMap";
			}
		}

		protected override IndirectStrings DescriptionIndirectString
		{
			get
			{
				return IndirectStrings.IDS_FIREWALLRULE_DESC_MSEXCHANGETRANSPORTLOGSEARCHRPCEPMAP;
			}
		}

		protected override string ApplicationPath
		{
			get
			{
				return Path.Combine(ExchangeFirewallRule.ExchangeInstallPath, "Bin\\MSExchangeTransportLogSearch.exe");
			}
		}

		protected override string ServiceName
		{
			get
			{
				return "MSExchangeTransportLogSearch";
			}
		}

		protected override string LocalPorts
		{
			get
			{
				return "RPC-EPMap";
			}
		}

		private const string RuleApplicationRelativePath = "Bin\\MSExchangeTransportLogSearch.exe";
	}
}
