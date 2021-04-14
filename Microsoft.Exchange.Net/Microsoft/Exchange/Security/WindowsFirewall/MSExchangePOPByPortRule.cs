﻿using System;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class MSExchangePOPByPortRule : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "MSExchange - POP3 (GFW)";
			}
		}

		protected override IndirectStrings DescriptionIndirectString
		{
			get
			{
				return IndirectStrings.IDS_FIREWALLRULE_DESC_MSEXCHANGEPOP3;
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
				return "110,995";
			}
		}

		private const string RuleApplicationRelativePath = "";
	}
}
