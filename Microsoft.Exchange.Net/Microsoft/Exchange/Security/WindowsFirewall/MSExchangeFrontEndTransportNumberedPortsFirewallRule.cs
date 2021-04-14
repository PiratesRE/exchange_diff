using System;
using System.IO;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class MSExchangeFrontEndTransportNumberedPortsFirewallRule : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "MSExchangeFrontendTransport";
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
				return Path.Combine(ExchangeFirewallRule.ExchangeInstallPath, "Bin\\MSExchangeFrontendTransport.exe");
			}
		}

		protected override string ServiceName
		{
			get
			{
				return "MSExchangeFrontendTransport";
			}
		}

		protected override string LocalPorts
		{
			get
			{
				return "25,587,717";
			}
		}

		private const string RuleApplicationRelativePath = "Bin\\MSExchangeFrontendTransport.exe";
	}
}
