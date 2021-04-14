using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.HA.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IADDatabaseAvailabilityGroup : IADObjectCommon
	{
		DatacenterActivationModeOption DatacenterActivationMode { get; }

		ThirdPartyReplicationMode ThirdPartyReplication { get; }

		MultiValuedProperty<ADObjectId> Servers { get; }

		NonRootLocalLongFullPath AutoDagVolumesRootFolderPath { get; }

		NonRootLocalLongFullPath AutoDagDatabasesRootFolderPath { get; }

		int AutoDagDatabaseCopiesPerVolume { get; }

		int AutoDagDatabaseCopiesPerDatabase { get; }

		int AutoDagTotalNumberOfDatabases { get; }

		int AutoDagTotalNumberOfServers { get; }

		bool ReplayLagManagerEnabled { get; }

		bool AutoDagAutoReseedEnabled { get; }

		bool AutoDagDiskReclaimerEnabled { get; }

		bool AutoDagBitlockerEnabled { get; }

		bool AutoDagFIPSCompliant { get; }

		ushort ReplicationPort { get; }

		MultiValuedProperty<string> StoppedMailboxServers { get; }

		MultiValuedProperty<string> StartedMailboxServers { get; }

		bool AllowCrossSiteRpcClientAccess { get; }

		bool ManualDagNetworkConfiguration { get; }

		DatabaseAvailabilityGroup.NetworkOption NetworkCompression { get; }

		DatabaseAvailabilityGroup.NetworkOption NetworkEncryption { get; }
	}
}
