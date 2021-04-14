using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class BridgeheadRoleConfigurationInfo : InstallableUnitConfigurationInfo
	{
		public override string Name
		{
			get
			{
				return "BridgeheadRole";
			}
		}

		public override LocalizedString DisplayName
		{
			get
			{
				return Strings.BridgeheadRoleDisplayName;
			}
		}

		public override decimal Size
		{
			get
			{
				return RequiredDiskSpaceStatistics.BridgeheadRole;
			}
		}

		public bool StartTransportService
		{
			get
			{
				return InstallableUnitConfigurationInfo.SetupContext.StartTransportService;
			}
		}

		public ServerIdParameter LegacyRoutingServerId
		{
			get
			{
				return this.legacyRoutingServerId;
			}
			set
			{
				this.legacyRoutingServerId = value;
			}
		}

		public bool DisableAMFiltering
		{
			get
			{
				return InstallableUnitConfigurationInfo.SetupContext.DisableAMFiltering;
			}
			set
			{
				InstallableUnitConfigurationInfo.SetupContext.DisableAMFiltering = value;
			}
		}

		private ServerIdParameter legacyRoutingServerId;
	}
}
