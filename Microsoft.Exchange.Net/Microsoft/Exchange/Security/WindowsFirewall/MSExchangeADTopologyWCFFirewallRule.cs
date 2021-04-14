using System;
using System.IO;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class MSExchangeADTopologyWCFFirewallRule : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "MSExchangeADTopology - WCF";
			}
		}

		protected override IndirectStrings DescriptionIndirectString
		{
			get
			{
				return IndirectStrings.IDS_FIREWALLRULE_DESC_MSEXCHANGEADTOPOLOGY_WCF;
			}
		}

		protected override string ApplicationPath
		{
			get
			{
				return Path.Combine(ExchangeFirewallRule.ExchangeInstallPath, "Bin\\Microsoft.Exchange.Directory.Topologyservice.exe");
			}
		}

		protected override string ServiceName
		{
			get
			{
				return "MSExchangeADTopology";
			}
		}

		protected override string LocalPorts
		{
			get
			{
				return "890";
			}
		}

		private const string RuleApplicationRelativePath = "Bin\\Microsoft.Exchange.Directory.Topologyservice.exe";
	}
}
