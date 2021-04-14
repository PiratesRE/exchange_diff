using System;
using System.IO;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class RemoteIISAdminFirewallRule : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "inetinfo";
			}
		}

		protected override IndirectStrings DescriptionIndirectString
		{
			get
			{
				return IndirectStrings.IDS_FIREWALLRULE_DESC_REMOTEIISADMIN;
			}
		}

		protected override string ApplicationPath
		{
			get
			{
				return Path.Combine(Environment.SystemDirectory, "inetsrv\\inetinfo.exe");
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
				return "RPC";
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

		private const string RuleApplicationRelativePath = "inetsrv\\inetinfo.exe";
	}
}
