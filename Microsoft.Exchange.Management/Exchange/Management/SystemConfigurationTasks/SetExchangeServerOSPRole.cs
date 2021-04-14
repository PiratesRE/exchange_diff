using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "ExchangeServerOSPRole", DefaultParameterSetName = "Identity")]
	public sealed class SetExchangeServerOSPRole : SetSystemConfigurationObjectTask<ServerIdParameter, Server>
	{
		public SetExchangeServerOSPRole()
		{
			this.Remove = new SwitchParameter(false);
		}

		[Parameter(Mandatory = true)]
		public ServerRole ServerRole
		{
			get
			{
				return (ServerRole)base.Fields["OSPServerRole"];
			}
			set
			{
				base.Fields["OSPServerRole"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Remove
		{
			get
			{
				return (SwitchParameter)base.Fields["OSPServerRoleRemove"];
			}
			set
			{
				base.Fields["OSPServerRoleRemove"] = value;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			Server server = (Server)base.PrepareDataObject();
			if (this.Remove)
			{
				server.CurrentServerRole &= ~this.ServerRole;
			}
			else
			{
				server.CurrentServerRole |= this.ServerRole;
				if (string.IsNullOrEmpty(server.Fqdn))
				{
					string localComputerFqdn = NativeHelpers.GetLocalComputerFqdn(true);
					TcpNetworkAddress value = new TcpNetworkAddress(NetworkProtocol.TcpIP, localComputerFqdn);
					server.NetworkAddress = new NetworkAddressCollection(value);
				}
			}
			return server;
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if ((ServerRole.OSP & this.ServerRole) != this.ServerRole)
			{
				base.WriteError(new ArgumentException(Strings.ErrorInvalidOSPServerRole, "ServerRole"), ErrorCategory.InvalidData, null);
			}
		}

		private const string OSPServerRoleField = "OSPServerRole";

		private const string OSPServerRoleRemoveField = "OSPServerRoleRemove";

		private const ServerRole SupportedOSPRoles = ServerRole.OSP;
	}
}
