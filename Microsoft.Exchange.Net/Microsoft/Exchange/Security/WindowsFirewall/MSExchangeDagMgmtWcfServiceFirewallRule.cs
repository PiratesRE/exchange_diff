using System;
using System.IO;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class MSExchangeDagMgmtWcfServiceFirewallRule : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "MSExchangeDagMgmt WCF Monitoring Service";
			}
		}

		protected override IndirectStrings DescriptionIndirectString
		{
			get
			{
				return IndirectStrings.IDS_FIREWALLRULE_DESC_MSEXCHANGEDAGMGMTWEBSERVICE;
			}
		}

		protected override string ApplicationPath
		{
			get
			{
				return Path.Combine(ExchangeFirewallRule.ExchangeInstallPath, "Bin\\MSExchangeDagMgmt.exe");
			}
		}

		protected override string ServiceName
		{
			get
			{
				return "MSExchangeDagMgmt";
			}
		}

		protected override string LocalPorts
		{
			get
			{
				return "808";
			}
		}

		private const string RuleApplicationRelativePath = "Bin\\MSExchangeDagMgmt.exe";
	}
}
