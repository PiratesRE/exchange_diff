using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	internal class RoleManager
	{
		static RoleManager()
		{
			RoleManager.roleList.Add(new BridgeheadRole());
			RoleManager.roleList.Add(new GatewayRole());
			RoleManager.roleList.Add(new ClientAccessRole());
			RoleManager.roleList.Add(new MailboxRole());
			RoleManager.roleList.Add(new UnifiedMessagingRole());
			RoleManager.roleList.Add(new FrontendTransportRole());
			RoleManager.roleList.Add(new AdminToolsRole());
			RoleManager.roleList.Add(new MonitoringRole());
			RoleManager.roleList.Add(new CentralAdminRole());
			RoleManager.roleList.Add(new CentralAdminDatabaseRole());
			RoleManager.roleList.Add(new CentralAdminFrontEndRole());
			RoleManager.roleList.Add(new LanguagePacksRole());
			RoleManager.roleList.Add(new CafeRole());
			RoleManager.roleList.Add(new FfoWebServiceRole());
			RoleManager.roleList.Add(new OSPRole());
		}

		public static RoleCollection Roles
		{
			get
			{
				return RoleManager.roleList;
			}
		}

		internal static SetupComponentInfoCollection GetRequiredComponents(string roleName, InstallationModes mode)
		{
			TaskLogger.LogEnter(new object[]
			{
				roleName
			});
			Role roleByName = RoleManager.GetRoleByName(roleName);
			RoleCollection currentRoles = RoleManager.GetCurrentRoles();
			currentRoles.Remove(roleByName);
			SetupComponentInfoCollection setupComponentInfoCollection = new SetupComponentInfoCollection();
			foreach (Role role in currentRoles)
			{
				setupComponentInfoCollection.AddRange(role.AllComponents);
			}
			SetupComponentInfoCollection allComponents = roleByName.AllComponents;
			SetupComponentInfoCollection setupComponentInfoCollection2 = new SetupComponentInfoCollection();
			if (mode == InstallationModes.BuildToBuildUpgrade)
			{
				setupComponentInfoCollection2.AddRange(allComponents);
			}
			else
			{
				using (List<SetupComponentInfo>.Enumerator enumerator2 = allComponents.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						SetupComponentInfo candidate = enumerator2.Current;
						bool flag = false;
						if (candidate.AlwaysExecute)
						{
							TaskLogger.Log(Strings.RunningAlwaysToRunComponent(candidate.Name));
							flag = true;
						}
						else if (setupComponentInfoCollection.Find((SetupComponentInfo sci) => sci.Name == candidate.Name) == null)
						{
							TaskLogger.Log(Strings.AddingUniqueComponent(candidate.Name));
							flag = true;
						}
						else
						{
							TaskLogger.Log(Strings.AlreadyConfiguredComponent(candidate.Name));
						}
						if (flag)
						{
							setupComponentInfoCollection2.Add(candidate);
						}
					}
				}
			}
			TaskLogger.LogExit();
			return setupComponentInfoCollection2;
		}

		public static Role GetRoleByName(string roleName)
		{
			if (!roleName.ToLower().Contains("role"))
			{
				roleName += "role";
			}
			foreach (Role role in RoleManager.Roles)
			{
				if (string.Compare(role.RoleName, roleName, true, CultureInfo.InvariantCulture) == 0)
				{
					return role;
				}
			}
			return null;
		}

		public static RoleCollection GetInstalledRoles()
		{
			RoleCollection roleCollection = new RoleCollection();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Role role in RoleManager.Roles)
			{
				if (role.IsInstalled)
				{
					roleCollection.Add(role);
					stringBuilder.Append(role.RoleName);
					stringBuilder.Append(" ");
				}
			}
			TaskLogger.Log(Strings.InstalledRoles(stringBuilder.ToString()));
			return roleCollection;
		}

		public static RoleCollection GetUnpackedRoles()
		{
			RoleCollection roleCollection = new RoleCollection();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Role role in RoleManager.Roles)
			{
				if (role.IsUnpacked)
				{
					roleCollection.Add(role);
					stringBuilder.Append(role.RoleName);
					stringBuilder.Append(" ");
				}
			}
			TaskLogger.Log(Strings.UnpackedRoles(stringBuilder.ToString()));
			return roleCollection;
		}

		public static RoleCollection GetUnpackedDatacenterRoles()
		{
			RoleCollection roleCollection = new RoleCollection();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Role role in RoleManager.Roles)
			{
				if (role.IsDatacenterUnpacked)
				{
					roleCollection.Add(role);
					stringBuilder.Append(role.RoleName);
					stringBuilder.Append(" ");
				}
			}
			TaskLogger.Log(Strings.UnpackedDatacenterRoles(stringBuilder.ToString()));
			return roleCollection;
		}

		public static RoleCollection GetCurrentRoles()
		{
			RoleCollection roleCollection = new RoleCollection();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Role role in RoleManager.Roles)
			{
				if (role.IsCurrent)
				{
					roleCollection.Add(role);
					stringBuilder.Append(role.RoleName);
					stringBuilder.Append(" ");
				}
			}
			TaskLogger.Log(Strings.CurrentRoles(stringBuilder.ToString()));
			return roleCollection;
		}

		internal static void Reset()
		{
			foreach (Role role in RoleManager.Roles)
			{
				role.Reset();
			}
		}

		protected static RoleCollection roleList = new RoleCollection();
	}
}
