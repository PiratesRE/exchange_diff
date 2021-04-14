using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class OSPRoleConfigurationInfo : InstallableUnitConfigurationInfo
	{
		public override string Name
		{
			get
			{
				return "OSPRole";
			}
		}

		public override LocalizedString DisplayName
		{
			get
			{
				return Strings.OSPRoleDisplayName;
			}
		}

		public override decimal Size
		{
			get
			{
				return RequiredDiskSpaceStatistics.OSPRole;
			}
		}
	}
}
