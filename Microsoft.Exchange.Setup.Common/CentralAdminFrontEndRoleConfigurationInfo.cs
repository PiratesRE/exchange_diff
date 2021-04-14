using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CentralAdminFrontEndRoleConfigurationInfo : InstallableUnitConfigurationInfo
	{
		public override string Name
		{
			get
			{
				return "CentralAdminFrontEndRole";
			}
		}

		public override LocalizedString DisplayName
		{
			get
			{
				return Strings.CentralAdminFrontEndRoleDisplayName;
			}
		}

		public override decimal Size
		{
			get
			{
				return RequiredDiskSpaceStatistics.CentralAdminFrontEndRole;
			}
		}
	}
}
