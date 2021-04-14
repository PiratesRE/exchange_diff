using System;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class MSExchangeCAFERemoteRegistryByPortRule : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "MSExchange CAS";
			}
		}

		protected override IndirectStrings DescriptionIndirectString
		{
			get
			{
				return IndirectStrings.IDS_FIREWALLRULE_DESC_MSEXCHANGECAFEREMOTEREGISTRY;
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
				return "139";
			}
		}

		private const string RuleApplicationRelativePath = "";
	}
}
