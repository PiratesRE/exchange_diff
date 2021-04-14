using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal static class DNDepth
	{
		internal const int MsExchPublicMDB = 8;

		internal const int MsExchPrivateMDB = 8;

		internal const int MsExchStorageGroup = 10;

		internal const int MsExchVirtualDirectory = 11;

		internal const int ProtocolCfg = 11;

		internal const int MsExchInformationStore = 9;

		internal const int MsExchExchangeServer = 8;

		internal const int MsExchServersContainer = 7;

		internal const int MsExchRoutingGroup = 8;

		internal const int TenantConfigurationUnitInDomainNC = 3;

		internal const int TenantConfigurationUnitInConfigNC = 6;

		internal const int AddressBookContainer = 6;

		internal const int MsExchAdminGroup = 6;

		internal const int MsExchAdminGroupContainer = 5;

		internal const int MsExchOrganizationContainer = 4;

		internal const int MsExchConfigurationContainer = 3;

		internal const int UserContainer = 1;

		internal const int Domain = 0;
	}
}
