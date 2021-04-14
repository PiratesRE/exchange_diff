using System;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class MSExchangeCASToMBXProxyPortFirewallRule : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "MSExchange CAS-MBX Proxy";
			}
		}

		protected override IndirectStrings DescriptionIndirectString
		{
			get
			{
				return IndirectStrings.IDS_FIREWALLRULE_DESC_MSEXCHANGECASTOMBXPROXYPORT;
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
				return "81,444";
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

		private const string RuleApplicationRelativePath = "";
	}
}
