using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Setup.Common
{
	internal class ClientAccessRoleConfigurationInfo : InstallableUnitConfigurationInfo
	{
		public override string Name
		{
			get
			{
				return "ClientAccessRole";
			}
		}

		public override LocalizedString DisplayName
		{
			get
			{
				return Strings.ClientAccessRoleDisplayName;
			}
		}

		public override decimal Size
		{
			get
			{
				return RequiredDiskSpaceStatistics.ClientAccessRole;
			}
		}
	}
}
