using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.HA.DirectoryServices
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ADDatabaseAvailabilityGroupWrapper : ADObjectWrapperBase, IADDatabaseAvailabilityGroup, IADObjectCommon
	{
		private ADDatabaseAvailabilityGroupWrapper(DatabaseAvailabilityGroup dag) : base(dag)
		{
			this.DatacenterActivationMode = (DatacenterActivationModeOption)dag[DatabaseAvailabilityGroupSchema.DataCenterActivationMode];
			this.ThirdPartyReplication = (ThirdPartyReplicationMode)dag[DatabaseAvailabilityGroupSchema.ThirdPartyReplication];
			this.Servers = (MultiValuedProperty<ADObjectId>)dag[DatabaseAvailabilityGroupSchema.Servers];
			this.StoppedMailboxServers = (MultiValuedProperty<string>)dag[DatabaseAvailabilityGroupSchema.StoppedMailboxServers];
			this.StartedMailboxServers = (MultiValuedProperty<string>)dag[DatabaseAvailabilityGroupSchema.StartedMailboxServers];
			this.AutoDagVolumesRootFolderPath = (NonRootLocalLongFullPath)dag[DatabaseAvailabilityGroupSchema.AutoDagVolumesRootFolderPath];
			this.AutoDagDatabasesRootFolderPath = (NonRootLocalLongFullPath)dag[DatabaseAvailabilityGroupSchema.AutoDagDatabasesRootFolderPath];
			this.AutoDagDatabaseCopiesPerVolume = (int)dag[DatabaseAvailabilityGroupSchema.AutoDagDatabaseCopiesPerVolume];
			this.AutoDagDatabaseCopiesPerDatabase = (int)dag[DatabaseAvailabilityGroupSchema.AutoDagDatabaseCopiesPerDatabase];
			this.AutoDagTotalNumberOfDatabases = (int)dag[DatabaseAvailabilityGroupSchema.AutoDagTotalNumberOfDatabases];
			this.AutoDagTotalNumberOfServers = (int)dag[DatabaseAvailabilityGroupSchema.AutoDagTotalNumberOfServers];
			this.ReplayLagManagerEnabled = (bool)dag[DatabaseAvailabilityGroupSchema.ReplayLagManagerEnabled];
			this.AutoDagAutoReseedEnabled = (bool)dag[DatabaseAvailabilityGroupSchema.AutoDagAutoReseedEnabled];
			this.AutoDagDiskReclaimerEnabled = (bool)dag[DatabaseAvailabilityGroupSchema.AutoDagDiskReclaimerEnabled];
			this.AutoDagBitlockerEnabled = (bool)dag[DatabaseAvailabilityGroupSchema.AutoDagBitlockerEnabled];
			this.AutoDagFIPSCompliant = (bool)dag[DatabaseAvailabilityGroupSchema.AutoDagFIPSCompliant];
			this.AllowCrossSiteRpcClientAccess = ((int)dag[DatabaseAvailabilityGroupSchema.AllowCrossSiteRpcClientAccess] != 0);
			this.ReplicationPort = dag.ReplicationPort;
			long networkSettings = (long)dag[DatabaseAvailabilityGroupSchema.NetworkSettings];
			DatabaseAvailabilityGroup.NetworkOption networkCompression;
			DatabaseAvailabilityGroup.NetworkOption networkEncryption;
			bool manualDagNetworkConfiguration;
			DatabaseAvailabilityGroup.DecodeNetworkSettings(networkSettings, out networkCompression, out networkEncryption, out manualDagNetworkConfiguration);
			this.NetworkCompression = networkCompression;
			this.NetworkEncryption = networkEncryption;
			this.ManualDagNetworkConfiguration = manualDagNetworkConfiguration;
		}

		public static ADDatabaseAvailabilityGroupWrapper CreateWrapper(DatabaseAvailabilityGroup dag)
		{
			if (dag == null)
			{
				return null;
			}
			return new ADDatabaseAvailabilityGroupWrapper(dag);
		}

		public DatacenterActivationModeOption DatacenterActivationMode { get; private set; }

		public ThirdPartyReplicationMode ThirdPartyReplication { get; private set; }

		public MultiValuedProperty<string> StartedMailboxServerFqdns { get; private set; }

		public MultiValuedProperty<ADObjectId> Servers { get; private set; }

		public NonRootLocalLongFullPath AutoDagVolumesRootFolderPath { get; private set; }

		public NonRootLocalLongFullPath AutoDagDatabasesRootFolderPath { get; private set; }

		public int AutoDagDatabaseCopiesPerVolume { get; private set; }

		public int AutoDagDatabaseCopiesPerDatabase { get; private set; }

		public int AutoDagTotalNumberOfDatabases { get; private set; }

		public int AutoDagTotalNumberOfServers { get; private set; }

		public bool ReplayLagManagerEnabled { get; private set; }

		public bool AutoDagAutoReseedEnabled { get; private set; }

		public bool AutoDagDiskReclaimerEnabled { get; private set; }

		public bool AutoDagBitlockerEnabled { get; private set; }

		public bool AutoDagFIPSCompliant { get; private set; }

		public MultiValuedProperty<string> StoppedMailboxServers { get; private set; }

		public MultiValuedProperty<string> StartedMailboxServers { get; private set; }

		public bool AllowCrossSiteRpcClientAccess { get; private set; }

		public bool ManualDagNetworkConfiguration { get; private set; }

		public ushort ReplicationPort { get; private set; }

		public DatabaseAvailabilityGroup.NetworkOption NetworkCompression { get; private set; }

		public DatabaseAvailabilityGroup.NetworkOption NetworkEncryption { get; private set; }
	}
}
