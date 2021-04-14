using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class InstallableUnitConfigurationInfoManager
	{
		static InstallableUnitConfigurationInfoManager()
		{
			InstallableUnitConfigurationInfoManager.installableConfigurationInfos.Add("BridgeheadRole", new BridgeheadRoleConfigurationInfo());
			InstallableUnitConfigurationInfoManager.installableConfigurationInfos.Add("ClientAccessRole", new ClientAccessRoleConfigurationInfo());
			InstallableUnitConfigurationInfoManager.installableConfigurationInfos.Add("MailboxRole", new MailboxRoleConfigurationInfo());
			InstallableUnitConfigurationInfoManager.installableConfigurationInfos.Add("GatewayRole", new GatewayRoleConfigurationInfo());
			InstallableUnitConfigurationInfoManager.installableConfigurationInfos.Add("UnifiedMessagingRole", new UnifiedMessagingRoleConfigurationInfo());
			InstallableUnitConfigurationInfoManager.installableConfigurationInfos.Add("FrontendTransportRole", new FrontendTransportRoleConfigurationInfo());
			InstallableUnitConfigurationInfoManager.installableConfigurationInfos.Add("CafeRole", new CafeRoleConfigurationInfo());
			InstallableUnitConfigurationInfoManager.installableConfigurationInfos.Add("AdminToolsRole", new AdminToolsRoleConfigurationInfo());
			InstallableUnitConfigurationInfoManager.installableConfigurationInfos.Add("CentralAdminDatabaseRole", new CentralAdminDatabaseRoleConfigurationInfo());
			InstallableUnitConfigurationInfoManager.installableConfigurationInfos.Add("CentralAdminRole", new CentralAdminRoleConfigurationInfo());
			InstallableUnitConfigurationInfoManager.installableConfigurationInfos.Add("CentralAdminFrontEndRole", new CentralAdminFrontEndRoleConfigurationInfo());
			InstallableUnitConfigurationInfoManager.installableConfigurationInfos.Add("MonitoringRole", new MonitoringRoleConfigurationInfo());
			InstallableUnitConfigurationInfoManager.installableConfigurationInfos.Add("OSPRole", new OSPRoleConfigurationInfo());
			InstallableUnitConfigurationInfoManager.installableConfigurationInfos.Add("UmLanguagePack", new UmLanguagePackConfigurationInfo());
			InstallableUnitConfigurationInfoManager.installableConfigurationInfos.Add("LanguagePacks", new LanguagePackConfigurationInfo());
		}

		public static InstallableUnitConfigurationInfo GetInstallableUnitConfigurationInfoByName(string installableUnitName)
		{
			InstallableUnitConfigurationInfo result = null;
			if (InstallableUnitConfigurationInfoManager.installableConfigurationInfos.ContainsKey(installableUnitName))
			{
				result = InstallableUnitConfigurationInfoManager.installableConfigurationInfos[installableUnitName];
			}
			return result;
		}

		public static bool IsRoleBasedConfigurableInstallableUnit(string installableUnitName)
		{
			return InstallableUnitConfigurationInfoManager.IsServerRoleBasedConfigurableInstallableUnit(installableUnitName) || installableUnitName == "AdminToolsRole";
		}

		public static bool IsServerRoleBasedConfigurableInstallableUnit(string installableUnitName)
		{
			return installableUnitName == "BridgeheadRole" || installableUnitName == "ClientAccessRole" || installableUnitName == "GatewayRole" || installableUnitName == "MailboxRole" || installableUnitName == "UnifiedMessagingRole" || installableUnitName == "FrontendTransportRole" || installableUnitName == "CafeRole" || installableUnitName == "CentralAdminRole" || installableUnitName == "CentralAdminDatabaseRole" || installableUnitName == "CentralAdminFrontEndRole" || installableUnitName == "MonitoringRole" || installableUnitName == "OSPRole";
		}

		public static bool IsUmLanguagePackInstallableUnit(string installableUnitName)
		{
			return installableUnitName.StartsWith("UmLanguagePack");
		}

		public static bool IsLanguagePacksInstallableUnit(string installableUnitName)
		{
			return installableUnitName.StartsWith("LanguagePacks");
		}

		public static void InitializeUmLanguagePacksConfigurationInfo(params CultureInfo[] cultures)
		{
			foreach (CultureInfo umlang in cultures)
			{
				string umLanguagePackNameForCultureInfo = UmLanguagePackConfigurationInfo.GetUmLanguagePackNameForCultureInfo(umlang);
				if (InstallableUnitConfigurationInfoManager.GetInstallableUnitConfigurationInfoByName(umLanguagePackNameForCultureInfo) == null)
				{
					InstallableUnitConfigurationInfoManager.AddInstallableUnit(UmLanguagePackConfigurationInfo.GetUmLanguagePackNameForCultureInfo(umlang), new UmLanguagePackConfigurationInfo(umlang));
				}
			}
		}

		public static void AddInstallableUnit(string name, InstallableUnitConfigurationInfo info)
		{
			InstallableUnitConfigurationInfoManager.installableConfigurationInfos.Add(name, info);
		}

		private static Dictionary<string, InstallableUnitConfigurationInfo> installableConfigurationInfos = new Dictionary<string, InstallableUnitConfigurationInfo>();
	}
}
