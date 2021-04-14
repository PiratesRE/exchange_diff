using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Microsoft.Exchange.Diagnostics.Service.Common
{
	public static class ServerRole
	{
		[MethodImpl(MethodImplOptions.NoInlining)]
		static ServerRole()
		{
			object obj;
			if (CommonUtils.TryGetRegistryValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Diagnostics", "RoleName", string.Empty, out obj))
			{
				string text = obj.ToString().Trim();
				if (!string.IsNullOrEmpty(text))
				{
					ServerRole.installedRoles.Add(text);
				}
			}
			Assembly callingAssembly = Assembly.GetCallingAssembly();
			if (callingAssembly == null)
			{
				ServerRole.version = Environment.OSVersion.Version;
			}
			else
			{
				object[] customAttributes = callingAssembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false);
				if (customAttributes.Length != 1 || !ServerRole.TryParseVersion(((AssemblyFileVersionAttribute)customAttributes[0]).Version, out ServerRole.version))
				{
					ServerRole.version = Environment.OSVersion.Version;
				}
			}
			foreach (ServerRole.Role role in ServerRole.roles)
			{
				ServerRole.CheckRole(role.Name, role.RegistryKey, role.RegistryValue);
			}
			if (ServerRole.installedRoles.Count == 0)
			{
				ServerRole.installedRoles.Add("Other");
			}
		}

		public static bool IsRole(string roleName)
		{
			return ServerRole.installedRoles.Contains(roleName, StringComparer.OrdinalIgnoreCase);
		}

		public static List<string> GetRoleAndVersion(out Version roleVersion)
		{
			roleVersion = ServerRole.version;
			return ServerRole.installedRoles;
		}

		private static void CheckRole(string role, string registryKey, string valueName)
		{
			object obj;
			if (CommonUtils.TryGetRegistryValue(registryKey, valueName, null, out obj))
			{
				ServerRole.installedRoles.Add(role);
				if (valueName != "ConfiguredVersion")
				{
					return;
				}
				if (!ServerRole.TryParseVersion(obj.ToString(), out ServerRole.version))
				{
					throw new FormatException("Unable to parse the version of this machine.");
				}
			}
		}

		private static bool TryParseVersion(string input, out Version result)
		{
			bool result2 = false;
			int major = 0;
			int minor = 0;
			int build = 0;
			int revision = 0;
			if (!string.IsNullOrEmpty(input))
			{
				string[] array = input.Split(new char[]
				{
					'.'
				});
				if (array.Length >= 2)
				{
					for (int i = 0; i < array.Length; i++)
					{
						switch (i)
						{
						case 0:
							int.TryParse(array[0], out major);
							break;
						case 1:
							int.TryParse(array[1], out minor);
							break;
						case 2:
							int.TryParse(array[2], out build);
							break;
						case 3:
							int.TryParse(array[3], out revision);
							break;
						}
					}
					result2 = true;
				}
			}
			result = new Version(major, minor, build, revision);
			return result2;
		}

		private const string ExchangeRegistryValueName = "ConfiguredVersion";

		private const string ExchangeServerRegistryKey = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15";

		private static readonly ServerRole.Role[] roles = new ServerRole.Role[]
		{
			new ServerRole.Role("DomainController", "HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\services\\NTDS\\Parameters", "DSA Database file"),
			new ServerRole.Role("Cafe", "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\CafeRole", "ConfiguredVersion"),
			new ServerRole.Role("ClientAccess", "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ClientAccessRole", "ConfiguredVersion"),
			new ServerRole.Role("Mailbox", "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\MailboxRole", "ConfiguredVersion"),
			new ServerRole.Role("UnifiedMessaging", "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\UnifiedMessagingRole", "ConfiguredVersion"),
			new ServerRole.Role("HubTransport", "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\HubTransportRole", "ConfiguredVersion"),
			new ServerRole.Role("CentralAdmin", "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\CentralAdminRole", "ConfiguredVersion"),
			new ServerRole.Role("FfoWebService", "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\FfoWebServiceRole", "ConfiguredVersion"),
			new ServerRole.Role("Arr", "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\IIS Extensions\\Application Request Routing", "Install"),
			new ServerRole.Role("AuxECS", "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15AuxECS", "ConfiguredVersion")
		};

		private static List<string> installedRoles = new List<string>();

		private static Version version;

		private class Role
		{
			public Role(string name, string registrykey, string registryValue)
			{
				this.name = name;
				this.registryKey = registrykey;
				this.registryValue = registryValue;
			}

			public string Name
			{
				get
				{
					return this.name;
				}
			}

			public string RegistryKey
			{
				get
				{
					return this.registryKey;
				}
			}

			public string RegistryValue
			{
				get
				{
					return this.registryValue;
				}
			}

			private readonly string name;

			private readonly string registryKey;

			private readonly string registryValue;
		}
	}
}
