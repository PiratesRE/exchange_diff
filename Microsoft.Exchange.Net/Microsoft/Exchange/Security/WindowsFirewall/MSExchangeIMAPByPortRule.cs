using System;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class MSExchangeIMAPByPortRule : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "MSExchange - IMAP4 (GFW)";
			}
		}

		protected override IndirectStrings DescriptionIndirectString
		{
			get
			{
				return IndirectStrings.IDS_FIREWALLRULE_DESC_MSEXCHANGEIMAP4;
			}
		}

		protected override string ApplicationPath
		{
			get
			{
				return string.Empty;
			}
		}

		protected override string ServiceName
		{
			get
			{
				return string.Empty;
			}
		}

		protected override string LocalPorts
		{
			get
			{
				return "143,993";
			}
		}

		private const string RuleApplicationRelativePath = "";
	}
}
