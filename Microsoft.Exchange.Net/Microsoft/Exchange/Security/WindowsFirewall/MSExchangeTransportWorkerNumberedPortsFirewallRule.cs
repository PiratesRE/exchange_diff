using System;
using System.IO;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class MSExchangeTransportWorkerNumberedPortsFirewallRule : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "MSExchangeTransportWorker";
			}
		}

		protected override IndirectStrings DescriptionIndirectString
		{
			get
			{
				return IndirectStrings.IDS_FIREWALLRULE_DESC_MSEXCHANGETRANSPORTWORKERNUMBERED;
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
				return "25,465,587,2525";
			}
		}

		private const string RuleApplicationRelativePath = "Bin\\edgetransport.exe";
	}
}
