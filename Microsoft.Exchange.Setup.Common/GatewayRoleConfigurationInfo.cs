using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class GatewayRoleConfigurationInfo : InstallableUnitConfigurationInfo
	{
		public override string Name
		{
			get
			{
				return "GatewayRole";
			}
		}

		public override LocalizedString DisplayName
		{
			get
			{
				return Strings.GatewayRoleDisplayName;
			}
		}

		public override decimal Size
		{
			get
			{
				return RequiredDiskSpaceStatistics.GatewayRole;
			}
		}

		public bool StartTransportService
		{
			get
			{
				return InstallableUnitConfigurationInfo.SetupContext.StartTransportService;
			}
		}

		public ushort AdamLdapPort
		{
			get
			{
				return InstallableUnitConfigurationInfo.SetupContext.AdamLdapPort;
			}
			set
			{
				InstallableUnitConfigurationInfo.SetupContext.AdamLdapPort = value;
			}
		}

		public ushort AdamSslPort
		{
			get
			{
				return InstallableUnitConfigurationInfo.SetupContext.AdamSslPort;
			}
			set
			{
				InstallableUnitConfigurationInfo.SetupContext.AdamSslPort = value;
			}
		}
	}
}
