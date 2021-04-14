using System;
using System.IO;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class MSExchangeUMCallRouterNumbered : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "UMCallRouter";
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
				return Path.Combine(ExchangeFirewallRule.ExchangeInstallPath, "FrontEnd\\CallRouter\\Microsoft.Exchange.UM.CallRouter.exe");
			}
		}

		protected override string ServiceName
		{
			get
			{
				return "UMCallRouter";
			}
		}

		protected override string LocalPorts
		{
			get
			{
				return "5060,5061";
			}
		}

		private const string RuleApplicationRelativePath = "FrontEnd\\CallRouter\\Microsoft.Exchange.UM.CallRouter.exe";
	}
}
