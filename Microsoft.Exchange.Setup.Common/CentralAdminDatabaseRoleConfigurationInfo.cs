using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CentralAdminDatabaseRoleConfigurationInfo : InstallableUnitConfigurationInfo
	{
		public override string Name
		{
			get
			{
				return "CentralAdminDatabaseRole";
			}
		}

		public override LocalizedString DisplayName
		{
			get
			{
				return Strings.CentralAdminDatabaseRoleDisplayName;
			}
		}

		public override decimal Size
		{
			get
			{
				return RequiredDiskSpaceStatistics.CentralAdminDatabaseRole;
			}
		}
	}
}
