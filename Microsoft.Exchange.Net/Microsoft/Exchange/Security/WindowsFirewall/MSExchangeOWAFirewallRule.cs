using System;
using System.IO;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class MSExchangeOWAFirewallRule : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "MSExchangeOWAAppPool";
			}
		}

		protected override IndirectStrings DescriptionIndirectString
		{
			get
			{
				return IndirectStrings.IDS_FIREWALLRULE_DESC_MSEXCHANGEOWA;
			}
		}

		protected override string ApplicationPath
		{
			get
			{
				return Path.Combine(Environment.SystemDirectory, "inetsrv\\w3wp.exe");
			}
		}

		protected override string ServiceName
		{
			get
			{
				return "MSExchangeOWAAppPool";
			}
		}

		protected override string LocalPorts
		{
			get
			{
				return "5075,5076,5077";
			}
		}

		protected override bool InhibitApplicationPath
		{
			get
			{
				return false;
			}
		}

		protected override bool InhibitServiceName
		{
			get
			{
				return false;
			}
		}

		private const string RuleApplicationRelativePath = "inetsrv\\w3wp.exe";
	}
}
