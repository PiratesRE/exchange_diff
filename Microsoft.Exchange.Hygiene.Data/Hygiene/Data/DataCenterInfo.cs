using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Hygiene.Data.DataProvider;
using Microsoft.Win32;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal class DataCenterInfo
	{
		public static FFORole GetConfiguredFFORolesBitMask()
		{
			if (DataCenterInfo.configuredFfoRolesBitMask == null)
			{
				DataCenterInfo.LoadFfoConfiguredRoles();
			}
			return DataCenterInfo.configuredFfoRolesBitMask.Value;
		}

		public static IEnumerable<FFORole> GetConfiguredFFORoles()
		{
			if (DataCenterInfo.configuredFfoRolesBitMask == null)
			{
				DataCenterInfo.LoadFfoConfiguredRoles();
			}
			return DataCenterInfo.configuredFfoRoles;
		}

		public static EXORole GetConfiguredEXORolesBitMask()
		{
			if (DataCenterInfo.configuredExoRolesBitMask == null)
			{
				DataCenterInfo.LoadExoConfiguredRoles();
			}
			return DataCenterInfo.configuredExoRolesBitMask.Value;
		}

		public static IEnumerable<EXORole> GetConfiguredEXORoles()
		{
			if (DataCenterInfo.configuredExoRolesBitMask == null)
			{
				DataCenterInfo.LoadExoConfiguredRoles();
			}
			return DataCenterInfo.configuredExoRoles;
		}

		private static void LoadFfoConfiguredRoles()
		{
			DataCenterInfo.configuredFfoRoles = new FFORole[0];
			DataCenterInfo.configuredFfoRolesBitMask = new FFORole?(FFORole.None);
			EnvironmentStrategy environmentStrategy = new EnvironmentStrategy();
			if (environmentStrategy.IsForefrontForOffice() || environmentStrategy.IsForefrontDALOverrideUseSQL())
			{
				string[] configRoles = null;
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\ExchangeLabs"))
				{
					if (registryKey != null)
					{
						string text = (string)registryKey.GetValue("Role");
						if (!string.IsNullOrWhiteSpace(text))
						{
							configRoles = text.Split(new char[]
							{
								','
							});
						}
					}
				}
				if (configRoles != null && configRoles.Length != 0)
				{
					DataCenterInfo.configuredFfoRoles = (from FFORole ffoRole in Enum.GetValues(typeof(FFORole))
					where configRoles.Any((string configRole) => configRole.Equals(ffoRole.ToString(), StringComparison.OrdinalIgnoreCase))
					select ffoRole).ToArray<FFORole>();
					DataCenterInfo.configuredFfoRolesBitMask = DataCenterInfo.configuredFfoRoles.Aggregate(DataCenterInfo.configuredFfoRolesBitMask, (FFORole? current, FFORole next) => current |= next);
				}
			}
		}

		private static void LoadExoConfiguredRoles()
		{
			DataCenterInfo.configuredExoRoles = new EXORole[0];
			DataCenterInfo.configuredExoRolesBitMask = new EXORole?(EXORole.None);
			EnvironmentStrategy environmentStrategy = new EnvironmentStrategy();
			if (!environmentStrategy.IsForefrontForOffice())
			{
				string[] allInstalledMsis = null;
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15"))
				{
					if (registryKey != null)
					{
						allInstalledMsis = registryKey.GetSubKeyNames();
					}
				}
				if (allInstalledMsis != null && allInstalledMsis.Length != 0)
				{
					DataCenterInfo.configuredExoRoles = (from EXORole exoRole in Enum.GetValues(typeof(EXORole))
					where allInstalledMsis.Any((string installedMsi) => installedMsi.Equals(exoRole.ToString(), StringComparison.OrdinalIgnoreCase))
					select exoRole).ToArray<EXORole>();
					DataCenterInfo.configuredExoRolesBitMask = DataCenterInfo.configuredExoRoles.Aggregate(DataCenterInfo.configuredExoRolesBitMask, (EXORole? current, EXORole next) => current |= next);
				}
			}
		}

		internal const string ExchangeLabsKey = "Software\\Microsoft\\ExchangeLabs";

		internal const string ExchangeServerVersionRootKey = "SOFTWARE\\Microsoft\\ExchangeServer\\v15";

		internal const string RoleRegValue = "Role";

		private static FFORole? configuredFfoRolesBitMask;

		private static FFORole[] configuredFfoRoles;

		private static EXORole? configuredExoRolesBitMask;

		private static EXORole[] configuredExoRoles;
	}
}
