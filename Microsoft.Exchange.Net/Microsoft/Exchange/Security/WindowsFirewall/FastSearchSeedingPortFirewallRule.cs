using System;
using System.IO;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class FastSearchSeedingPortFirewallRule : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "MSExchangeSearch - Seeding";
			}
		}

		protected override IndirectStrings DescriptionIndirectString
		{
			get
			{
				return IndirectStrings.IDS_FIREWALLRULE_DESC_FASTSEARCHSEEDINGPORT_IN;
			}
		}

		protected override string ApplicationPath
		{
			get
			{
				return Path.Combine(ExchangeFirewallRule.ExchangeInstallPath, "Bin\\Search\\Ceres\\Runtime\\1.0\\NodeRunner.exe");
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
				return "3870,3871,3872,3873,3874,3875,3876,3877,3878,3879,3880,3881,3882,3883,3884,3885,3886,3887,3888,3889";
			}
		}

		private const string RuleApplicationRelativePath = "Bin\\Search\\Ceres\\Runtime\\1.0\\NodeRunner.exe";
	}
}
