using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring
{
	internal static class MpServerRoles
	{
		internal static ServerRole GetLocalE12ServerRolesFromRegistry()
		{
			ServerRole serverRole = ServerRole.None;
			using (RegistryKey localMachine = Registry.LocalMachine)
			{
				using (RegistryKey registryKey = localMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15", RegistryKeyPermissionCheck.Default))
				{
					if (registryKey != null)
					{
						foreach (MpServerRoles.RoleOnRegistry roleOnRegistry in MpServerRoles.E12MpRolesOnRegistry)
						{
							using (RegistryKey registryKey2 = registryKey.OpenSubKey(roleOnRegistry.SubKey))
							{
								if (registryKey2 != null)
								{
									serverRole |= roleOnRegistry.RoleFlag;
								}
							}
						}
					}
				}
			}
			return serverRole;
		}

		internal static string DisplayRoleName(ServerRole role)
		{
			if (role <= ServerRole.UnifiedMessaging)
			{
				switch (role)
				{
				case ServerRole.Mailbox:
					return Strings.MailboxRoleDisplayName;
				case ServerRole.Cafe | ServerRole.Mailbox:
					break;
				case ServerRole.ClientAccess:
					return Strings.ClientAccessRoleDisplayName;
				default:
					if (role == ServerRole.UnifiedMessaging)
					{
						return Strings.UnifiedMessagingRoleDisplayName;
					}
					break;
				}
			}
			else
			{
				if (role == ServerRole.HubTransport)
				{
					return Strings.BridgeheadRoleDisplayName;
				}
				if (role == ServerRole.Edge)
				{
					return Strings.GatewayRoleDisplayName;
				}
			}
			return string.Empty;
		}

		internal static readonly ServerRole[] ValidE12MpRoles = new ServerRole[]
		{
			ServerRole.Mailbox,
			ServerRole.ClientAccess,
			ServerRole.UnifiedMessaging,
			ServerRole.HubTransport,
			ServerRole.Edge
		};

		private static readonly MpServerRoles.RoleOnRegistry[] E12MpRolesOnRegistry = new MpServerRoles.RoleOnRegistry[]
		{
			new MpServerRoles.RoleOnRegistry(ServerRole.HubTransport, "HubTransportRole"),
			new MpServerRoles.RoleOnRegistry(ServerRole.ClientAccess, "ClientAccessRole"),
			new MpServerRoles.RoleOnRegistry(ServerRole.Edge, "EdgeTransportRole"),
			new MpServerRoles.RoleOnRegistry(ServerRole.Mailbox, "MailboxRole"),
			new MpServerRoles.RoleOnRegistry(ServerRole.UnifiedMessaging, "UnifiedMessagingRole")
		};

		internal struct RoleOnRegistry
		{
			public ServerRole RoleFlag
			{
				get
				{
					return this.roleFlag;
				}
			}

			public string SubKey
			{
				get
				{
					return this.subKey;
				}
			}

			public RoleOnRegistry(ServerRole role, string subKey)
			{
				this = default(MpServerRoles.RoleOnRegistry);
				this.roleFlag = role;
				this.subKey = subKey;
			}

			public const string RootSubKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15";

			private ServerRole roleFlag;

			private string subKey;
		}
	}
}
