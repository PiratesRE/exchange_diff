using System;
using System.IO;
using System.Runtime.InteropServices;
using Interop.NetFw;
using Microsoft.Win32;

namespace Microsoft.Exchange.Security.WindowsFirewall
{
	[CLSCompliant(false)]
	public abstract class ExchangeFirewallRule
	{
		internal string Name
		{
			get
			{
				if (string.IsNullOrEmpty(this.name))
				{
					this.name = string.Format("{0} ({1}-{2})", this.ComponentName, this.GetProtocolString(), this.GetDirectionString());
				}
				return this.name;
			}
		}

		internal NetFwRule NetFwFirewallRule
		{
			get
			{
				if (this.netfwFirewallRule == null)
				{
					try
					{
						this.netfwFirewallRule = this.FirewallPolicy.Rules.Item(this.Name);
					}
					catch (FileNotFoundException)
					{
						this.netfwFirewallRule = null;
					}
					catch (COMException)
					{
						this.netfwFirewallRule = null;
					}
				}
				return this.netfwFirewallRule;
			}
		}

		protected abstract string ComponentName { get; }

		protected abstract IndirectStrings DescriptionIndirectString { get; }

		protected abstract string ApplicationPath { get; }

		protected abstract string ServiceName { get; }

		protected abstract string LocalPorts { get; }

		protected virtual bool InhibitApplicationPath
		{
			get
			{
				return false;
			}
		}

		protected virtual bool InhibitServiceName
		{
			get
			{
				return true;
			}
		}

		protected virtual NET_FW_RULE_DIRECTION_ Direction
		{
			get
			{
				return 1;
			}
		}

		protected virtual NET_FW_IP_PROTOCOL_ Protocol
		{
			get
			{
				return 6;
			}
		}

		protected virtual IndirectStrings GroupingIndirectString
		{
			get
			{
				return IndirectStrings.IDS_FIREWALLGROUP_EXCHANGE;
			}
		}

		protected virtual string LocalAddresses
		{
			get
			{
				return "*";
			}
		}

		protected virtual string RemoteAddresses
		{
			get
			{
				return "*";
			}
		}

		protected virtual string RemotePorts
		{
			get
			{
				return "*";
			}
		}

		protected virtual NET_FW_PROFILE_TYPE2_ Profile
		{
			get
			{
				return int.MaxValue;
			}
		}

		protected virtual NET_FW_ACTION_ Action
		{
			get
			{
				return 1;
			}
		}

		protected virtual bool Enabled
		{
			get
			{
				return true;
			}
		}

		protected virtual bool EdgeTraversal
		{
			get
			{
				return false;
			}
		}

		private string Grouping
		{
			get
			{
				return "@%ExchangeInstallPath%\\Bin\\FirewallRes.dll,-" + (int)this.GroupingIndirectString;
			}
		}

		private string Description
		{
			get
			{
				return "@%ExchangeInstallPath%\\Bin\\FirewallRes.dll,-" + (int)this.DescriptionIndirectString;
			}
		}

		private NetFwPolicy2 FirewallPolicy
		{
			get
			{
				if (this.firewallPolicy == null)
				{
					this.firewallPolicy = new NetFwPolicy2Class();
				}
				return this.firewallPolicy;
			}
		}

		internal bool IsInstalled
		{
			get
			{
				return this.isInstalled;
			}
		}

		internal void Add()
		{
			if (this.NetFwFirewallRule == null)
			{
				NetFwRule netFwRule = new NetFwRuleClass();
				netFwRule.Name = this.Name;
				netFwRule.Description = this.Description;
				netFwRule.Grouping = this.Grouping;
				if (!this.InhibitApplicationPath && !string.IsNullOrEmpty(this.ApplicationPath))
				{
					netFwRule.ApplicationName = this.ApplicationPath;
				}
				if (!this.InhibitServiceName && !string.IsNullOrEmpty(this.ServiceName))
				{
					netFwRule.serviceName = this.ServiceName;
				}
				netFwRule.Protocol = this.Protocol;
				netFwRule.Direction = this.Direction;
				netFwRule.Profiles = this.Profile;
				netFwRule.Action = this.Action;
				netFwRule.Enabled = this.Enabled;
				netFwRule.LocalAddresses = this.LocalAddresses;
				netFwRule.LocalPorts = this.LocalPorts;
				netFwRule.RemoteAddresses = this.RemoteAddresses;
				netFwRule.RemotePorts = this.RemotePorts;
				netFwRule.EdgeTraversal = this.EdgeTraversal;
				this.FirewallPolicy.Rules.Add(netFwRule);
			}
			this.isInstalled = true;
		}

		internal void Remove()
		{
			this.netfwFirewallRule = null;
			if (this.NetFwFirewallRule != null)
			{
				this.FirewallPolicy.Rules.Remove(this.Name);
			}
			this.isInstalled = false;
		}

		internal void SetLocalPort(string portString)
		{
			this.netfwFirewallRule = null;
			if (this.NetFwFirewallRule != null)
			{
				try
				{
					this.NetFwFirewallRule.LocalPorts = portString;
				}
				catch (COMException ex)
				{
					uint errorCode = (uint)ex.ErrorCode;
					uint num = errorCode;
					if (num == 2147500035U)
					{
						throw new InvalidOperationException("Invalid pointer while trying to set firewall local ports");
					}
					if (num == 2147942405U)
					{
						throw new UnauthorizedAccessException("Unauthorized to set firewall local ports");
					}
					switch (num)
					{
					case 2147942413U:
						throw new ArgumentException("Invalid firewall local ports parameter");
					case 2147942414U:
						throw new OutOfMemoryException("Out of memory while trying to set firewall local ports");
					default:
						throw;
					}
				}
			}
		}

		internal string GetLocalPorts()
		{
			this.netfwFirewallRule = null;
			if (this.NetFwFirewallRule != null)
			{
				try
				{
					return this.NetFwFirewallRule.LocalPorts;
				}
				catch (COMException ex)
				{
					uint errorCode = (uint)ex.ErrorCode;
					uint num = errorCode;
					if (num == 2147500035U)
					{
						throw new InvalidOperationException("Invalid pointer while trying to get firewall local ports");
					}
					if (num == 2147942405U)
					{
						throw new UnauthorizedAccessException("Unauthorized to get firewall local ports");
					}
					if (num != 2147942414U)
					{
						throw;
					}
					throw new OutOfMemoryException("Out of memory while trying to get firewall local ports");
				}
			}
			return null;
		}

		internal string ToStringDefaultValue()
		{
			return string.Format("{0}{1}Protocol={2} Direction={3}{4}LocalPorts='{5}'{6}EdgeTraverse={7}{8}InhibitApplicationPath={9} InhibitServiceName={10}{11}'{12}'{13}'{14}'{15}'{16}'{17}'{18}'", new object[]
			{
				this.ComponentName,
				Environment.NewLine,
				this.GetProtocolString(),
				this.GetDirectionString(),
				Environment.NewLine,
				this.LocalPorts,
				Environment.NewLine,
				this.EdgeTraversal,
				Environment.NewLine,
				this.InhibitApplicationPath,
				this.InhibitServiceName,
				Environment.NewLine,
				this.ApplicationPath,
				Environment.NewLine,
				this.ServiceName,
				Environment.NewLine,
				this.Description,
				Environment.NewLine,
				this.Grouping
			});
		}

		internal string ToStringInstalledValue()
		{
			return string.Format("{0}{1}Protocol={2} Direction={3}{4}LocalPorts='{5}'{6}EdgeTraverse={7}{8}InhibitApplicationPath={9} InhibitServiceName={10}{11}'{12}'{13}'{14}'{15}'{16}'{17}'{18}'", new object[]
			{
				this.ComponentName,
				Environment.NewLine,
				this.GetProtocolString(),
				this.GetDirectionString(),
				Environment.NewLine,
				this.GetLocalPorts(),
				Environment.NewLine,
				this.EdgeTraversal,
				Environment.NewLine,
				this.InhibitApplicationPath,
				this.InhibitServiceName,
				Environment.NewLine,
				this.ApplicationPath,
				Environment.NewLine,
				this.ServiceName,
				Environment.NewLine,
				this.Description,
				Environment.NewLine,
				this.Grouping
			});
		}

		private static string GetExchangeInstallPath()
		{
			return (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup", "MsiInstallPath", null);
		}

		private string GetProtocolString()
		{
			string result = string.Empty;
			NET_FW_IP_PROTOCOL_ protocol = this.Protocol;
			if (protocol != 6)
			{
				if (protocol != 17)
				{
					if (protocol == 256)
					{
						result = "ANY";
					}
				}
				else
				{
					result = "UDP";
				}
			}
			else
			{
				result = "TCP";
			}
			return result;
		}

		private string GetDirectionString()
		{
			string result = string.Empty;
			switch (this.Direction)
			{
			case 1:
				result = "In";
				break;
			case 2:
				result = "Out";
				break;
			}
			return result;
		}

		internal const string AnyPort = "*";

		internal const string RpcPorts = "RPC";

		internal const string RPCEpMapPorts = "RPC-EPMap";

		internal const string TeredoPorts = "Teredo";

		protected const string AnyAddress = "*";

		protected const string LocalSubnetAddress = "LocalSubnet";

		protected const string DNSAddress = "DNS";

		private const string ProtocolAny = "ANY";

		private const string ProtocolTcp = "TCP";

		private const string ProtocolUdp = "UDP";

		private const string DirectionIn = "In";

		private const string DirectionOut = "Out";

		private const string RuleNameFormat = "{0} ({1}-{2})";

		private const string IndirectStringPrefix = "@%ExchangeInstallPath%\\Bin\\FirewallRes.dll,-";

		protected static readonly string ExchangeInstallPath = ExchangeFirewallRule.GetExchangeInstallPath();

		private string name;

		private NetFwPolicy2 firewallPolicy;

		private NetFwRule netfwFirewallRule;

		private bool isInstalled;
	}
}
