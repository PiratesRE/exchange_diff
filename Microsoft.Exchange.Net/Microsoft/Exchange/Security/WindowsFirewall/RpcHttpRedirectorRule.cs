using System;
using System.IO;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class RpcHttpRedirectorRule : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "RpcHttpLBS";
			}
		}

		protected override IndirectStrings DescriptionIndirectString
		{
			get
			{
				return IndirectStrings.IDS_FIREWALLRULE_DESC_RPCHTTPLBS;
			}
		}

		protected override string ApplicationPath
		{
			get
			{
				return Path.Combine(Environment.SystemDirectory, "System32\\Svchost.exe");
			}
		}

		protected override string ServiceName
		{
			get
			{
				return "RpcHttpLBS";
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

		private const string RuleApplicationRelativePath = "System32\\Svchost.exe";
	}
}
