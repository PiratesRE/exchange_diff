using System;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class MSExchangeMbxByPort : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "MSExchangeIS (GFW)";
			}
		}

		protected override IndirectStrings DescriptionIndirectString
		{
			get
			{
				return IndirectStrings.IDS_FIREWALLRULE_DESC_MSEXCHANGEIS;
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
				return "6001,6002,6004,64327";
			}
		}

		private const string RuleApplicationRelativePath = "";
	}
}
