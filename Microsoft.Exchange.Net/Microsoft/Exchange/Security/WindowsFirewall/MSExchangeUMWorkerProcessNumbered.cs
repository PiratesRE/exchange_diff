using System;
using System.IO;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class MSExchangeUMWorkerProcessNumbered : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "UMWorkerProcess";
			}
		}

		protected override IndirectStrings DescriptionIndirectString
		{
			get
			{
				return IndirectStrings.IDS_FIREWALLRULE_DESC_UMWORKERPROCESSNUMBERED;
			}
		}

		protected override string ApplicationPath
		{
			get
			{
				return Path.Combine(ExchangeFirewallRule.ExchangeInstallPath, "Bin\\UMWorkerProcess.exe");
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
				return "5065,5066,5067,5068";
			}
		}

		private const string RuleApplicationRelativePath = "Bin\\UMWorkerProcess.exe";
	}
}
