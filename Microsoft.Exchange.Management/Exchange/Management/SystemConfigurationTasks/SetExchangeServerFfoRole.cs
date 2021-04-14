using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "ExchangeServerFfoRole", DefaultParameterSetName = "Identity")]
	public sealed class SetExchangeServerFfoRole : SetSystemConfigurationObjectTask<ServerIdParameter, Server>
	{
		public SetExchangeServerFfoRole()
		{
			this.Remove = new SwitchParameter(false);
		}

		[Parameter(Mandatory = true)]
		public ServerRole ServerRole
		{
			get
			{
				return (ServerRole)base.Fields["FfoServerRole"];
			}
			set
			{
				base.Fields["FfoServerRole"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Remove
		{
			get
			{
				return (SwitchParameter)base.Fields["FfoServerRoleRemove"];
			}
			set
			{
				base.Fields["FfoServerRoleRemove"] = value;
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
			if ((ServerRole.FfoWebService & this.ServerRole) != this.ServerRole)
			{
				base.WriteError(new ArgumentException(Strings.ErrorInvalidFfoServerRole, "ServerRole"), ErrorCategory.InvalidData, null);
			}
		}

		private const string FfoServerRoleField = "FfoServerRole";

		private const string FfoServerRoleRemoveField = "FfoServerRoleRemove";

		private const ServerRole SupportedFfoRoles = ServerRole.FfoWebService;
	}
}
