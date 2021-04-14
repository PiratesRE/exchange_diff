using System;
using System.IO;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class MSExchangeCAMOMConnectorFirewallRule : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "MSExchangeCAMOMConnector - RPC";
			}
		}

		protected override IndirectStrings DescriptionIndirectString
		{
			get
			{
				return IndirectStrings.IDS_FIREWALLRULE_DESC_MSEXCHANGECAMOMCONNECTORRPC;
			}
		}

		protected override string ApplicationPath
		{
			get
			{
				return Path.Combine(ExchangeFirewallRule.ExchangeInstallPath, "Bin\\Microsoft.Exchange.Management.CentralAdmin.MomConnector.exe");
			}
		}

		protected override string ServiceName
		{
			get
			{
				return "MSExchangeCAMOMConnector";
			}
		}

		protected override string LocalPorts
		{
			get
			{
				return "RPC";
			}
		}

		private const string RuleApplicationRelativePath = "Bin\\Microsoft.Exchange.Management.CentralAdmin.MomConnector.exe";
	}
}
