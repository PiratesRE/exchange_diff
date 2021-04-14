using System;
using System.IO;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public sealed class RpcEndPointMapperFirewallRule : ExchangeFirewallRule
	{
		protected override string ComponentName
		{
			get
			{
				return "RPC Endpoint Mapper";
			}
		}

		protected override IndirectStrings DescriptionIndirectString
		{
			get
			{
				return IndirectStrings.IDS_FIREWALLRULE_DESC_RPC_ENDPOINT_MAPPER;
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
				return "RPCSS";
			}
		}

		protected override string LocalPorts
		{
			get
			{
				return "RPC-EPMap";
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
