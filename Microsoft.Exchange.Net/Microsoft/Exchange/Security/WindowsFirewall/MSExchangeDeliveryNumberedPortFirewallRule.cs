using System;
using System.IO;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class MSExchangeDeliveryNumberedPortFirewallRule : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "MSExchangeDelivery";
			}
		}

		protected override IndirectStrings DescriptionIndirectString
		{
			get
			{
				return IndirectStrings.IDS_FIREWALLRULE_DESC_MSEXCHANGETRANSPORTDELIVERYNUMBERED;
			}
		}

		protected override string ApplicationPath
		{
			get
			{
				return Path.Combine(ExchangeFirewallRule.ExchangeInstallPath, "Bin\\MSExchangeDelivery.exe");
			}
		}

		protected override string ServiceName
		{
			get
			{
				return "MSExchangeDelivery";
			}
		}

		protected override string LocalPorts
		{
			get
			{
				return "475";
			}
		}

		private const string RuleApplicationRelativePath = "Bin\\MSExchangeDelivery.exe";
	}
}
