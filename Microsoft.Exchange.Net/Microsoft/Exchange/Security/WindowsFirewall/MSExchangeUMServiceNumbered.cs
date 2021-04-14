using System;
using System.IO;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class MSExchangeUMServiceNumbered : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "UMService";
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
				return Path.Combine(ExchangeFirewallRule.ExchangeInstallPath, "Bin\\UMService.exe");
			}
		}

		protected override string ServiceName
		{
			get
			{
				return "UMService";
			}
		}

		protected override string LocalPorts
		{
			get
			{
				return "5060,5061,5062,5063";
			}
		}

		private const string RuleApplicationRelativePath = "Bin\\UMService.exe";
	}
}
