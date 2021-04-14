using System;
using System.Net;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class DatabaseAvailabilityGroupSchema : ADConfigurationObjectSchema
	{
		public static readonly Version AutoDagSchemaCurrentVersion = new Version(1, 0);

		public new static readonly ADPropertyDefinition Name = new ADPropertyDefinition("Name", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchMDBAvailabilityGroupName", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new ADObjectNameStringLengthConstraint(1, 15),
			ComputerNameCharacterConstraint.DefaultConstraint
		}, null, null);

		public static readonly ADPropertyDefinition Servers = new ADPropertyDefinition("Servers", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchMDBAvailabilityGroupBL", ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition WitnessServer = new ADPropertyDefinition("WitnessServer", ExchangeObjectVersion.Exchange2010, typeof(UncFileSharePath), "msExchFileShareWitness", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new NoLeadingOrTrailingWhitespaceConstraint(),
			new UIImpactStringLengthConstraint(1, 260)
		}, null, null);

		public static readonly ADPropertyDefinition WitnessDirectory = new ADPropertyDefinition("WitnessDirectory", ExchangeObjectVersion.Exchange2010, typeof(NonRootLocalLongFullPath), "msExchFileShareWitnessDirectory", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new NoLeadingOrTrailingWhitespaceConstraint(),
			new UIImpactStringLengthConstraint(1, 260)
		}, null, null);

		public static readonly ADPropertyDefinition AlternateWitnessServer = new ADPropertyDefinition("AlternateWitnessServer", ExchangeObjectVersion.Exchange2010, typeof(UncFileSharePath), "msExchAlternateFileShareWitness", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new NoLeadingOrTrailingWhitespaceConstraint()
		}, null, null);

		public static readonly ADPropertyDefinition AlternateWitnessDirectory = new ADPropertyDefinition("AlternateWitnessDirectory", ExchangeObjectVersion.Exchange2010, typeof(NonRootLocalLongFullPath), "msExchAlternateFileShareWitnessDirectory", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new NoLeadingOrTrailingWhitespaceConstraint(),
			new UIImpactStringLengthConstraint(1, 260)
		}, null, null);

		public static readonly ADPropertyDefinition NetworkSettings = new ADPropertyDefinition("NetworkSettings", ExchangeObjectVersion.Exchange2010, typeof(long), "msExchMDBAvailabilityGroupNetworkSettings", ADPropertyDefinitionFlags.PersistDefaultValue, DatabaseAvailabilityGroup.EncodeNetworkSettings(DatabaseAvailabilityGroup.NetworkOption.InterSubnetOnly, DatabaseAvailabilityGroup.NetworkOption.InterSubnetOnly, false), PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ThirdPartyReplication = new ADPropertyDefinition("ThirdPartyReplication", ExchangeObjectVersion.Exchange2010, typeof(ThirdPartyReplicationMode), "msExchThirdPartySynchronousReplication", ADPropertyDefinitionFlags.PersistDefaultValue, ThirdPartyReplicationMode.Disabled, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DataCenterActivationMode = new ADPropertyDefinition("DataCenterActivationMode", ExchangeObjectVersion.Exchange2010, typeof(DatacenterActivationModeOption), "msExchDatacenterActivationMode", ADPropertyDefinitionFlags.PersistDefaultValue, DatacenterActivationModeOption.Off, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition StoppedMailboxServers = new ADPropertyDefinition("StoppedMailboxServers", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchStoppedMailboxServers", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition StartedMailboxServers = new ADPropertyDefinition("StartedMailboxServers", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchStartedMailboxServers", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DatabaseAvailabilityGroupIpv4Addresses = new ADPropertyDefinition("DatabaseAvailabilityGroupIpv4Addresses", ExchangeObjectVersion.Exchange2010, typeof(IPAddress), "msExchMDBAvailabilityGroupIPv4Addresses", ADPropertyDefinitionFlags.MultiValued, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AllowCrossSiteRpcClientAccess = new ADPropertyDefinition("AllowCrossSiteRpcClientAccess", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchAllowCrossSiteRPCClientAccess", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AutoDagDatabaseCopiesPerDatabase = new ADPropertyDefinition("AutoDagDatabaseCopiesPerDatabase", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchAutoDAGParamDatabaseCopiesPerDatabase", ADPropertyDefinitionFlags.PersistDefaultValue, 1, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 16)
		}, null, null);

		public static readonly ADPropertyDefinition AutoDagDatabaseCopiesPerVolume = new ADPropertyDefinition("AutoDagDatabaseCopiesPerVolume", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchAutoDAGParamDatabaseCopiesPerVolume", ADPropertyDefinitionFlags.PersistDefaultValue, 1, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, int.MaxValue)
		}, null, null);

		public static readonly ADPropertyDefinition AutoDagDatabasesRootFolderPath = new ADPropertyDefinition("AutoDagDatabasesRootFolderPath", ExchangeObjectVersion.Exchange2010, typeof(NonRootLocalLongFullPath), "msExchAutoDAGParamDatabasesRootFolderPath", ADPropertyDefinitionFlags.PersistDefaultValue, DatabaseAvailabilityGroup.DefaultAutoDagDatabasesRootFolderPath, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			LocalLongFullPathLengthConstraint.LocalLongFullDirectoryPathLengthConstraint,
			new NoLeadingOrTrailingWhitespaceConstraint(),
			new AsciiCharactersOnlyConstraint()
		}, null, null);

		public static readonly ADPropertyDefinition AutoDagFlags = new ADPropertyDefinition("AutoDagFlags", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchAutoDAGParamFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AutoDagTotalNumberOfDatabases = new ADPropertyDefinition("AutoDagTotalNumberOfDatabases", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchAutoDAGParamTotalNumberOfDatabases", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AutoDagTotalNumberOfServers = new ADPropertyDefinition("AutoDagTotalNumberOfServers", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchAutoDAGParamTotalNumberOfServers", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AutoDagVolumesRootFolderPath = new ADPropertyDefinition("AutoDagVolumesRootFolderPath", ExchangeObjectVersion.Exchange2010, typeof(NonRootLocalLongFullPath), "msExchAutoDAGParamVolumesRootFolderPath", ADPropertyDefinitionFlags.PersistDefaultValue, DatabaseAvailabilityGroup.DefaultAutoDagVolumesRootFolderPath, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			LocalLongFullPathLengthConstraint.LocalLongFullDirectoryPathLengthConstraint,
			new NoLeadingOrTrailingWhitespaceConstraint(),
			new AsciiCharactersOnlyConstraint()
		}, null, null);

		public static readonly ADPropertyDefinition AutoDagSchemaVersion = new ADPropertyDefinition("AutoDagSchemaVersion", ExchangeObjectVersion.Exchange2010, typeof(Version), "msExchAutoDAGSchemaVersion", ADPropertyDefinitionFlags.PersistDefaultValue, DatabaseAvailabilityGroupSchema.AutoDagSchemaCurrentVersion, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ReplicationPort = new ADPropertyDefinition("ReplicationPort", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchMDBAvailabilityGroupReplicationPort", ADPropertyDefinitionFlags.PersistDefaultValue, 64327, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(1, 65536)
		}, null, null);

		public static readonly ADPropertyDefinition AutoDagAllServersInstalled = new ADPropertyDefinition("AutoDagAllServersInstalled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			DatabaseAvailabilityGroupSchema.AutoDagFlags
		}, null, ADObject.FlagGetterDelegate(DatabaseAvailabilityGroupSchema.AutoDagFlags, 8), ADObject.FlagSetterDelegate(DatabaseAvailabilityGroupSchema.AutoDagFlags, 8), null, null);

		public static readonly ADPropertyDefinition DatabaseAvailabilityGroupConfiguration = new ADPropertyDefinition("DatabaseAvailabilityGroupConfiguration", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchMDBAvailabilityGroupConfigurationLink", ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ReplayLagManagerEnabled = new ADPropertyDefinition("ReplayLagManagerEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			DatabaseAvailabilityGroupSchema.AutoDagFlags
		}, null, ADObject.FlagGetterDelegate(DatabaseAvailabilityGroupSchema.AutoDagFlags, 16), ADObject.FlagSetterDelegate(DatabaseAvailabilityGroupSchema.AutoDagFlags, 16), null, null);

		public static readonly ADPropertyDefinition AutoDagAutoReseedEnabled = new ADPropertyDefinition("AutoDagAutoReseedEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			DatabaseAvailabilityGroupSchema.AutoDagFlags
		}, null, ADObject.InvertFlagGetterDelegate(DatabaseAvailabilityGroupSchema.AutoDagFlags, 4), ADObject.InvertFlagSetterDelegate(DatabaseAvailabilityGroupSchema.AutoDagFlags, 4), null, null);

		public static readonly ADPropertyDefinition AutoDagDiskReclaimerEnabled = new ADPropertyDefinition("AutoDagDiskReclaimerEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			DatabaseAvailabilityGroupSchema.AutoDagFlags
		}, null, ADObject.InvertFlagGetterDelegate(DatabaseAvailabilityGroupSchema.AutoDagFlags, 32), ADObject.InvertFlagSetterDelegate(DatabaseAvailabilityGroupSchema.AutoDagFlags, 32), null, null);

		public static readonly ADPropertyDefinition AutoDagBitlockerEnabled = new ADPropertyDefinition("AutoDagBitlockerEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			DatabaseAvailabilityGroupSchema.AutoDagFlags
		}, null, ADObject.FlagGetterDelegate(DatabaseAvailabilityGroupSchema.AutoDagFlags, 64), ADObject.FlagSetterDelegate(DatabaseAvailabilityGroupSchema.AutoDagFlags, 64), null, null);

		public static readonly ADPropertyDefinition AutoDagFIPSCompliant = new ADPropertyDefinition("AutoDagFIPSCompliant", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			DatabaseAvailabilityGroupSchema.AutoDagFlags
		}, null, ADObject.FlagGetterDelegate(DatabaseAvailabilityGroupSchema.AutoDagFlags, 128), ADObject.FlagSetterDelegate(DatabaseAvailabilityGroupSchema.AutoDagFlags, 128), null, null);

		public static readonly ADPropertyDefinition ConfigurationXmlRaw = XMLSerializableBase.ConfigurationXmlRawProperty();

		public static readonly ADPropertyDefinition ConfigurationXml = XMLSerializableBase.ConfigurationXmlProperty<DatabaseAvailabilityGroupConfigXml>(DatabaseAvailabilityGroupSchema.ConfigurationXmlRaw);

		public static readonly ADPropertyDefinition MailboxLoadBalanceMaximumEdbFileSize = XMLSerializableBase.ConfigXmlProperty<DatabaseAvailabilityGroupConfigXml, ByteQuantifiedSize?>("MailboxLoadBalanceMaximumEdbFileSize", ExchangeObjectVersion.Exchange2007, DatabaseAvailabilityGroupSchema.ConfigurationXml, null, (DatabaseAvailabilityGroupConfigXml configXml) => configXml.MailboxLoadBalanceMaximumEdbFileSize, delegate(DatabaseAvailabilityGroupConfigXml configXml, ByteQuantifiedSize? value)
		{
			configXml.MailboxLoadBalanceMaximumEdbFileSize = value;
		}, null, null);

		public static readonly ADPropertyDefinition MailboxLoadBalanceRelativeLoadCapacity = XMLSerializableBase.ConfigXmlProperty<DatabaseAvailabilityGroupConfigXml, int?>("MailboxLoadBalanceRelativeLoadCapacity", ExchangeObjectVersion.Exchange2007, DatabaseAvailabilityGroupSchema.ConfigurationXml, null, (DatabaseAvailabilityGroupConfigXml configXml) => configXml.MailboxLoadBalanceRelativeCapacity, delegate(DatabaseAvailabilityGroupConfigXml configXml, int? value)
		{
			configXml.MailboxLoadBalanceRelativeCapacity = value;
		}, null, null);

		public static readonly ADPropertyDefinition MailboxLoadBalanceOverloadedThreshold = XMLSerializableBase.ConfigXmlProperty<DatabaseAvailabilityGroupConfigXml, int?>("MailboxLoadBalanceOverloadedThreshold", ExchangeObjectVersion.Exchange2007, DatabaseAvailabilityGroupSchema.ConfigurationXml, null, (DatabaseAvailabilityGroupConfigXml configXml) => configXml.MailboxLoadBalanceOverloadThreshold, delegate(DatabaseAvailabilityGroupConfigXml configXml, int? value)
		{
			configXml.MailboxLoadBalanceOverloadThreshold = value;
		}, null, null);

		public static readonly ADPropertyDefinition MailboxLoadBalanceUnderloadedThreshold = XMLSerializableBase.ConfigXmlProperty<DatabaseAvailabilityGroupConfigXml, int?>("MailboxLoadBalanceUnderloadedThreshold", ExchangeObjectVersion.Exchange2007, DatabaseAvailabilityGroupSchema.ConfigurationXml, null, (DatabaseAvailabilityGroupConfigXml configXml) => configXml.MailboxLoadBalanceMinimumBalancingThreshold, delegate(DatabaseAvailabilityGroupConfigXml configXml, int? value)
		{
			configXml.MailboxLoadBalanceMinimumBalancingThreshold = value;
		}, null, null);

		public static readonly ADPropertyDefinition MailboxLoadBalanceEnabled = XMLSerializableBase.ConfigXmlProperty<DatabaseAvailabilityGroupConfigXml, bool>("MailboxLoadBalanceEnabled", ExchangeObjectVersion.Exchange2007, DatabaseAvailabilityGroupSchema.ConfigurationXml, false, (DatabaseAvailabilityGroupConfigXml configXml) => configXml.MailboxLoadBalanceEnabled, delegate(DatabaseAvailabilityGroupConfigXml configXml, bool value)
		{
			configXml.MailboxLoadBalanceEnabled = value;
		}, null, null);
	}
}
