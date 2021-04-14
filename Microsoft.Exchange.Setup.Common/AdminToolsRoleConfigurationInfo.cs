using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AdminToolsRoleConfigurationInfo : InstallableUnitConfigurationInfo
	{
		public override string Name
		{
			get
			{
				return "AdminToolsRole";
			}
		}

		public override LocalizedString DisplayName
		{
			get
			{
				return Strings.AdminToolsRoleDisplayName;
			}
		}

		public override decimal Size
		{
			get
			{
				return RequiredDiskSpaceStatistics.AdminTools;
			}
		}
	}
}
