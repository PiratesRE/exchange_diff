using System;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class MSExchangeUMCallRouterNumberedByPort : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "UMCallRouter (GFW)";
			}
		}

		protected override IndirectStrings DescriptionIndirectString
		{
			get
			{
				return IndirectStrings.IDS_FIREWALLRULE_DESC_UMCALLROUTERNUMBERED;
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
				return "5060,5061";
			}
		}

		private const string RuleApplicationRelativePath = "";
	}
}
