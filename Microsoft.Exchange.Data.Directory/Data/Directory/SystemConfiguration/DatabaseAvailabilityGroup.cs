using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Net;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class DatabaseAvailabilityGroup : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				if (this.schema == null)
				{
					this.schema = ObjectSchema.GetInstance<DatabaseAvailabilityGroupSchema>();
				}
				return this.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return DatabaseAvailabilityGroup.mostDerivedClass;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		public new string Name
		{
			get
			{
				return (string)this[DatabaseAvailabilityGroupSchema.Name];
			}
			internal set
			{
				this[DatabaseAvailabilityGroupSchema.Name] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> Servers
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[DatabaseAvailabilityGroupSchema.Servers];
			}
		}

		public FileShareWitnessServerName WitnessServer
		{
			get
			{
				if (this.FileShareWitness != null)
				{
					return FileShareWitnessServerName.Parse(this.FileShareWitness.ServerName);
				}
				return null;
			}
			set
			{
				this[DatabaseAvailabilityGroupSchema.WitnessServer] = UncFileSharePath.Parse("\\\\" + value.RawData + "\\TempShare");
			}
		}

		internal UncFileSharePath FileShareWitness
		{
			get
			{
				if (this[DatabaseAvailabilityGroupSchema.WitnessServer] != null)
				{
					return (UncFileSharePath)this[DatabaseAvailabilityGroupSchema.WitnessServer];
				}
				return null;
			}
		}

		internal static long EncodeNetworkSettings(DatabaseAvailabilityGroup.NetworkOption compress, DatabaseAvailabilityGroup.NetworkOption encrypt, bool manualNetConfig)
		{
			long num = (long)compress;
			long num2 = (long)encrypt << 4;
			long num3 = (manualNetConfig ? 1L : 0L) << 8;
			return num3 | num2 | num;
		}

		internal static void DecodeNetworkSettings(long networkSettings, out DatabaseAvailabilityGroup.NetworkOption compress, out DatabaseAvailabilityGroup.NetworkOption encrypt, out bool manualNetConfig)
		{
			int adVal = (int)(networkSettings & 15L);
			compress = DatabaseAvailabilityGroup.ConvertToNetworkOption(adVal);
			int adVal2 = (int)(networkSettings & 240L) >> 4;
			encrypt = DatabaseAvailabilityGroup.ConvertToNetworkOption(adVal2);
			if ((networkSettings & 256L) != 0L)
			{
				manualNetConfig = true;
				return;
			}
			manualNetConfig = false;
		}

		internal void SetWitnessServer(UncFileSharePath witnessServer, NonRootLocalLongFullPath witnessDirectory)
		{
			this[DatabaseAvailabilityGroupSchema.WitnessServer] = witnessServer;
			this[DatabaseAvailabilityGroupSchema.WitnessDirectory] = witnessDirectory;
		}

		public NonRootLocalLongFullPath WitnessDirectory
		{
			get
			{
				return (NonRootLocalLongFullPath)this[DatabaseAvailabilityGroupSchema.WitnessDirectory];
			}
			set
			{
				this[DatabaseAvailabilityGroupSchema.WitnessDirectory] = value;
			}
		}

		public FileShareWitnessServerName AlternateWitnessServer
		{
			get
			{
				if (this.AlternateFileShareWitness != null)
				{
					return FileShareWitnessServerName.Parse(this.AlternateFileShareWitness.ServerName);
				}
				return null;
			}
			set
			{
				this[DatabaseAvailabilityGroupSchema.AlternateWitnessServer] = UncFileSharePath.Parse("\\\\" + value.RawData + "\\TempShare");
			}
		}

		internal UncFileSharePath AlternateFileShareWitness
		{
			get
			{
				if (this[DatabaseAvailabilityGroupSchema.AlternateWitnessServer] != null)
				{
					return (UncFileSharePath)this[DatabaseAvailabilityGroupSchema.AlternateWitnessServer];
				}
				return null;
			}
		}

		internal void SetAlternateWitnessServer(UncFileSharePath alternateWitnessServer, NonRootLocalLongFullPath alternateWitnessDirectory)
		{
			this[DatabaseAvailabilityGroupSchema.AlternateWitnessServer] = alternateWitnessServer;
			this[DatabaseAvailabilityGroupSchema.AlternateWitnessDirectory] = alternateWitnessDirectory;
		}

		public NonRootLocalLongFullPath AlternateWitnessDirectory
		{
			get
			{
				return (NonRootLocalLongFullPath)this[DatabaseAvailabilityGroupSchema.AlternateWitnessDirectory];
			}
			set
			{
				this[DatabaseAvailabilityGroupSchema.AlternateWitnessDirectory] = value;
			}
		}

		internal long NetworkSettings
		{
			get
			{
				return (long)this[DatabaseAvailabilityGroupSchema.NetworkSettings];
			}
			set
			{
				this[DatabaseAvailabilityGroupSchema.NetworkSettings] = value;
			}
		}

		public DatabaseAvailabilityGroup.NetworkOption NetworkCompression
		{
			get
			{
				DatabaseAvailabilityGroup.NetworkOption result;
				DatabaseAvailabilityGroup.NetworkOption networkOption;
				bool flag;
				DatabaseAvailabilityGroup.DecodeNetworkSettings(this.NetworkSettings, out result, out networkOption, out flag);
				return result;
			}
			set
			{
				DatabaseAvailabilityGroup.NetworkOption networkOption;
				DatabaseAvailabilityGroup.NetworkOption encrypt;
				bool manualNetConfig;
				DatabaseAvailabilityGroup.DecodeNetworkSettings(this.NetworkSettings, out networkOption, out encrypt, out manualNetConfig);
				long networkSettings = DatabaseAvailabilityGroup.EncodeNetworkSettings(value, encrypt, manualNetConfig);
				this.NetworkSettings = networkSettings;
			}
		}

		public DatabaseAvailabilityGroup.NetworkOption NetworkEncryption
		{
			get
			{
				DatabaseAvailabilityGroup.NetworkOption networkOption;
				DatabaseAvailabilityGroup.NetworkOption result;
				bool flag;
				DatabaseAvailabilityGroup.DecodeNetworkSettings(this.NetworkSettings, out networkOption, out result, out flag);
				return result;
			}
			set
			{
				DatabaseAvailabilityGroup.NetworkOption compress;
				DatabaseAvailabilityGroup.NetworkOption networkOption;
				bool manualNetConfig;
				DatabaseAvailabilityGroup.DecodeNetworkSettings(this.NetworkSettings, out compress, out networkOption, out manualNetConfig);
				long networkSettings = DatabaseAvailabilityGroup.EncodeNetworkSettings(compress, value, manualNetConfig);
				this.NetworkSettings = networkSettings;
			}
		}

		public bool ManualDagNetworkConfiguration
		{
			get
			{
				DatabaseAvailabilityGroup.NetworkOption networkOption;
				DatabaseAvailabilityGroup.NetworkOption networkOption2;
				bool result;
				DatabaseAvailabilityGroup.DecodeNetworkSettings(this.NetworkSettings, out networkOption, out networkOption2, out result);
				return result;
			}
			set
			{
				DatabaseAvailabilityGroup.NetworkOption compress;
				DatabaseAvailabilityGroup.NetworkOption encrypt;
				bool flag;
				DatabaseAvailabilityGroup.DecodeNetworkSettings(this.NetworkSettings, out compress, out encrypt, out flag);
				long networkSettings = DatabaseAvailabilityGroup.EncodeNetworkSettings(compress, encrypt, value);
				this.NetworkSettings = networkSettings;
			}
		}

		private static DatabaseAvailabilityGroup.NetworkOption ConvertToNetworkOption(int adVal)
		{
			if (adVal < 0 || adVal > 3)
			{
				return DatabaseAvailabilityGroup.NetworkOption.InterSubnetOnly;
			}
			return (DatabaseAvailabilityGroup.NetworkOption)adVal;
		}

		internal bool IsDagEmpty()
		{
			using (MultiValuedProperty<ADObjectId>.Enumerator enumerator = this.Servers.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					ADObjectId adobjectId = enumerator.Current;
					return false;
				}
			}
			return true;
		}

		public DatacenterActivationModeOption DatacenterActivationMode
		{
			get
			{
				return (DatacenterActivationModeOption)this[DatabaseAvailabilityGroupSchema.DataCenterActivationMode];
			}
			set
			{
				this[DatabaseAvailabilityGroupSchema.DataCenterActivationMode] = value;
			}
		}

		public MultiValuedProperty<string> StoppedMailboxServers
		{
			get
			{
				return (MultiValuedProperty<string>)this[DatabaseAvailabilityGroupSchema.StoppedMailboxServers];
			}
			set
			{
				this[DatabaseAvailabilityGroupSchema.StoppedMailboxServers] = value;
			}
		}

		public MultiValuedProperty<string> StartedMailboxServers
		{
			get
			{
				return (MultiValuedProperty<string>)this[DatabaseAvailabilityGroupSchema.StartedMailboxServers];
			}
			set
			{
				this[DatabaseAvailabilityGroupSchema.StartedMailboxServers] = value;
			}
		}

		public MultiValuedProperty<IPAddress> DatabaseAvailabilityGroupIpv4Addresses
		{
			get
			{
				return (MultiValuedProperty<IPAddress>)this[DatabaseAvailabilityGroupSchema.DatabaseAvailabilityGroupIpv4Addresses];
			}
			set
			{
				this[DatabaseAvailabilityGroupSchema.DatabaseAvailabilityGroupIpv4Addresses] = value;
			}
		}

		public MultiValuedProperty<IPAddress> DatabaseAvailabilityGroupIpAddresses
		{
			get
			{
				return this.DatabaseAvailabilityGroupIpv4Addresses;
			}
			set
			{
				this.DatabaseAvailabilityGroupIpv4Addresses = value;
			}
		}

		public bool AllowCrossSiteRpcClientAccess
		{
			get
			{
				return (int)this[DatabaseAvailabilityGroupSchema.AllowCrossSiteRpcClientAccess] != 0;
			}
			set
			{
				this[DatabaseAvailabilityGroupSchema.AllowCrossSiteRpcClientAccess] = (value ? 1 : 0);
			}
		}

		public ADObjectId[] OperationalServers
		{
			get
			{
				return this.m_operationalServers;
			}
			internal set
			{
				this.m_operationalServers = value;
			}
		}

		public ADObjectId PrimaryActiveManager
		{
			get
			{
				return this.m_primaryActiveManager;
			}
			internal set
			{
				this.m_primaryActiveManager = value;
			}
		}

		public ADObjectId[] ServersInMaintenance
		{
			get
			{
				return this.m_serversInMaintenance;
			}
			internal set
			{
				this.m_serversInMaintenance = value;
			}
		}

		public DeferredFailoverEntry[] ServersInDeferredRecovery
		{
			get
			{
				return this.m_serversInDeferredRecovery;
			}
			internal set
			{
				this.m_serversInDeferredRecovery = value;
			}
		}

		public ThirdPartyReplicationMode ThirdPartyReplication
		{
			get
			{
				return (ThirdPartyReplicationMode)this[DatabaseAvailabilityGroupSchema.ThirdPartyReplication];
			}
			set
			{
				this[DatabaseAvailabilityGroupSchema.ThirdPartyReplication] = value;
			}
		}

		public ushort ReplicationPort
		{
			get
			{
				return (ushort)((int)this[DatabaseAvailabilityGroupSchema.ReplicationPort]);
			}
			internal set
			{
				this[DatabaseAvailabilityGroupSchema.ReplicationPort] = (int)value;
			}
		}

		public List<string> NetworkNames
		{
			get
			{
				return this.m_networkNames;
			}
		}

		public WitnessShareUsage? WitnessShareInUse
		{
			get
			{
				return this.m_witnessShareInUse;
			}
			internal set
			{
				this.m_witnessShareInUse = value;
			}
		}

		public ADObjectId DatabaseAvailabilityGroupConfiguration
		{
			get
			{
				return (ADObjectId)this[DatabaseAvailabilityGroupSchema.DatabaseAvailabilityGroupConfiguration];
			}
			set
			{
				this[DatabaseAvailabilityGroupSchema.DatabaseAvailabilityGroupConfiguration] = value;
			}
		}

		internal DagAutoDagFlags AutoDagFlags
		{
			get
			{
				return (DagAutoDagFlags)this[DatabaseAvailabilityGroupSchema.AutoDagFlags];
			}
			set
			{
				this[DatabaseAvailabilityGroupSchema.AutoDagFlags] = value;
			}
		}

		public Version AutoDagSchemaVersion
		{
			get
			{
				return (Version)this[DatabaseAvailabilityGroupSchema.AutoDagSchemaVersion];
			}
			internal set
			{
				this[DatabaseAvailabilityGroupSchema.AutoDagSchemaVersion] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int AutoDagDatabaseCopiesPerDatabase
		{
			get
			{
				return (int)this[DatabaseAvailabilityGroupSchema.AutoDagDatabaseCopiesPerDatabase];
			}
			set
			{
				this[DatabaseAvailabilityGroupSchema.AutoDagDatabaseCopiesPerDatabase] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int AutoDagDatabaseCopiesPerVolume
		{
			get
			{
				return (int)this[DatabaseAvailabilityGroupSchema.AutoDagDatabaseCopiesPerVolume];
			}
			set
			{
				this[DatabaseAvailabilityGroupSchema.AutoDagDatabaseCopiesPerVolume] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int AutoDagTotalNumberOfDatabases
		{
			get
			{
				return (int)this[DatabaseAvailabilityGroupSchema.AutoDagTotalNumberOfDatabases];
			}
			set
			{
				this[DatabaseAvailabilityGroupSchema.AutoDagTotalNumberOfDatabases] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int AutoDagTotalNumberOfServers
		{
			get
			{
				return (int)this[DatabaseAvailabilityGroupSchema.AutoDagTotalNumberOfServers];
			}
			set
			{
				this[DatabaseAvailabilityGroupSchema.AutoDagTotalNumberOfServers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public NonRootLocalLongFullPath AutoDagDatabasesRootFolderPath
		{
			get
			{
				return (NonRootLocalLongFullPath)this[DatabaseAvailabilityGroupSchema.AutoDagDatabasesRootFolderPath];
			}
			set
			{
				this[DatabaseAvailabilityGroupSchema.AutoDagDatabasesRootFolderPath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public NonRootLocalLongFullPath AutoDagVolumesRootFolderPath
		{
			get
			{
				return (NonRootLocalLongFullPath)this[DatabaseAvailabilityGroupSchema.AutoDagVolumesRootFolderPath];
			}
			set
			{
				this[DatabaseAvailabilityGroupSchema.AutoDagVolumesRootFolderPath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AutoDagAllServersInstalled
		{
			get
			{
				return (bool)this[DatabaseAvailabilityGroupSchema.AutoDagAllServersInstalled];
			}
			set
			{
				this[DatabaseAvailabilityGroupSchema.AutoDagAllServersInstalled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AutoDagAutoReseedEnabled
		{
			get
			{
				return (bool)this[DatabaseAvailabilityGroupSchema.AutoDagAutoReseedEnabled];
			}
			set
			{
				this[DatabaseAvailabilityGroupSchema.AutoDagAutoReseedEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AutoDagDiskReclaimerEnabled
		{
			get
			{
				return (bool)this[DatabaseAvailabilityGroupSchema.AutoDagDiskReclaimerEnabled];
			}
			set
			{
				this[DatabaseAvailabilityGroupSchema.AutoDagDiskReclaimerEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AutoDagBitlockerEnabled
		{
			get
			{
				return (bool)this[DatabaseAvailabilityGroupSchema.AutoDagBitlockerEnabled];
			}
			set
			{
				this[DatabaseAvailabilityGroupSchema.AutoDagBitlockerEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AutoDagFIPSCompliant
		{
			get
			{
				return (bool)this[DatabaseAvailabilityGroupSchema.AutoDagFIPSCompliant];
			}
			set
			{
				this[DatabaseAvailabilityGroupSchema.AutoDagFIPSCompliant] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ReplayLagManagerEnabled
		{
			get
			{
				return (bool)this[DatabaseAvailabilityGroupSchema.ReplayLagManagerEnabled];
			}
			set
			{
				this[DatabaseAvailabilityGroupSchema.ReplayLagManagerEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize? MailboxLoadBalanceMaximumEdbFileSize
		{
			get
			{
				return (ByteQuantifiedSize?)this[DatabaseAvailabilityGroupSchema.MailboxLoadBalanceMaximumEdbFileSize];
			}
			set
			{
				this[DatabaseAvailabilityGroupSchema.MailboxLoadBalanceMaximumEdbFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? MailboxLoadBalanceRelativeLoadCapacity
		{
			get
			{
				return (int?)this[DatabaseAvailabilityGroupSchema.MailboxLoadBalanceRelativeLoadCapacity];
			}
			set
			{
				this[DatabaseAvailabilityGroupSchema.MailboxLoadBalanceRelativeLoadCapacity] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? MailboxLoadBalanceOverloadedThreshold
		{
			get
			{
				return (int?)this[DatabaseAvailabilityGroupSchema.MailboxLoadBalanceOverloadedThreshold];
			}
			set
			{
				this[DatabaseAvailabilityGroupSchema.MailboxLoadBalanceOverloadedThreshold] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? MailboxLoadBalanceUnderloadedThreshold
		{
			get
			{
				return (int?)this[DatabaseAvailabilityGroupSchema.MailboxLoadBalanceUnderloadedThreshold];
			}
			set
			{
				this[DatabaseAvailabilityGroupSchema.MailboxLoadBalanceUnderloadedThreshold] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MailboxLoadBalanceEnabled
		{
			get
			{
				return (bool)this[DatabaseAvailabilityGroupSchema.MailboxLoadBalanceEnabled];
			}
			set
			{
				this[DatabaseAvailabilityGroupSchema.MailboxLoadBalanceEnabled] = value;
			}
		}

		public const ushort DefaultReplicationPort = 64327;

		public const DatabaseAvailabilityGroup.NetworkOption DefaultNetworkOption = DatabaseAvailabilityGroup.NetworkOption.InterSubnetOnly;

		public const DatacenterActivationModeOption DefaultDatacenterActivationMode = DatacenterActivationModeOption.Off;

		private static string mostDerivedClass = "msExchMDBAvailabilityGroup";

		public static readonly NonRootLocalLongFullPath DefaultAutoDagDatabasesRootFolderPath = NonRootLocalLongFullPath.Parse("C:\\ExchangeDatabases");

		public static readonly NonRootLocalLongFullPath DefaultAutoDagVolumesRootFolderPath = NonRootLocalLongFullPath.Parse("C:\\ExchangeVolumes");

		[NonSerialized]
		private DatabaseAvailabilityGroupSchema schema;

		private ADObjectId[] m_operationalServers;

		private ADObjectId m_primaryActiveManager;

		private ADObjectId[] m_serversInMaintenance;

		private DeferredFailoverEntry[] m_serversInDeferredRecovery;

		private List<string> m_networkNames = new List<string>();

		private WitnessShareUsage? m_witnessShareInUse = null;

		public enum NetworkOption
		{
			Disabled,
			Enabled,
			InterSubnetOnly,
			SeedOnly
		}
	}
}
