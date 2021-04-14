using System;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class MSExchangeUMWorkerProcessNumberedByPort : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "UMWorkerProcess (GFW)";
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
				return "5065,5066,5067,5068";
			}
		}

		private const string RuleApplicationRelativePath = "";
	}
}
