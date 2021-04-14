using System;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class MSExchangeUMServiceNumberedByPort : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "UMService (GFW)";
			}
		}

		protected override IndirectStrings DescriptionIndirectString
		{
			get
			{
				return IndirectStrings.IDS_FIREWALLRULE_DESC_UMSERVICENUMBERED;
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
				return "5060,5061,5062,5063";
			}
		}

		private const string RuleApplicationRelativePath = "";
	}
}
