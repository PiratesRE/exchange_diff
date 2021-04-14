using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class DatabaseSchema : ADLegacyVersionableObjectSchema
	{
		public static readonly ADPropertyDefinition AllowFileRestore = new ADPropertyDefinition("AllowFileRestore", ExchangeObjectVersion.Exchange2003, typeof(bool), "msExchPatchMDB", ADPropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AdminDisplayVersion = new ADPropertyDefinition("AdminDisplayVersion", ExchangeObjectVersion.Exchange2003, typeof(ServerVersion), null, ADPropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition BackgroundDatabaseMaintenance = new ADPropertyDefinition("BackgroundDatabaseMaintenance", ExchangeObjectVersion.Exchange2010, typeof(bool), "msExchESEParamBackgroundDatabaseMaintenance", ADPropertyDefinitionFlags.PersistDefaultValue, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ReplayBackgroundDatabaseMaintenance = new ADPropertyDefinition("ReplayBackgroundDatabaseMaintenance", ExchangeObjectVersion.Exchange2010, typeof(bool?), "msExchESEParamReplayBackgroundDatabaseMaintenance", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition BackgroundDatabaseMaintenanceSerialization = new ADPropertyDefinition("BackgroundDatabaseMaintenanceSerialization", ExchangeObjectVersion.Exchange2010, typeof(bool?), "msExchESEParamBackgroundDatabaseMaintenanceSerialization", ADPropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition BackgroundDatabaseMaintenanceDelay = new ADPropertyDefinition("BackgroundDatabaseMaintenanceDelay", ExchangeObjectVersion.Exchange2010, typeof(int?), "msExchESEParamBackgroundDatabaseMaintenanceDelay", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ReplayBackgroundDatabaseMaintenanceDelay = new ADPropertyDefinition("ReplayBackgroundDatabaseMaintenanceDelay", ExchangeObjectVersion.Exchange2010, typeof(int?), "msExchESEParamReplayBackgroundDatabaseMaintenanceDelay", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MimimumBackgroundDatabaseMaintenanceInterval = new ADPropertyDefinition("MimimumBackgroundDatabaseMaintenanceInterval", ExchangeObjectVersion.Exchange2010, typeof(int?), "msExchESEParamBackgroundDatabaseMaintenanceIntervalMin", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaximumBackgroundDatabaseMaintenanceInterval = new ADPropertyDefinition("MaximumBackgroundDatabaseMaintenanceInterval", ExchangeObjectVersion.Exchange2010, typeof(int?), "msExchESEParamBackgroundDatabaseMaintenanceIntervalMax", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DatabaseCreated = new ADPropertyDefinition("DatabaseCreated", ExchangeObjectVersion.Exchange2003, typeof(bool), "msExchDatabaseCreated", ADPropertyDefinitionFlags.PersistDefaultValue | ADPropertyDefinitionFlags.DoNotProvisionalClone, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DatabaseBL = new ADPropertyDefinition("DatabaseBL", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), "homeMDBBL", ADPropertyDefinitionFlags.MultiValued | ADPropertyDefinitionFlags.FilterOnly | ADPropertyDefinitionFlags.DoNotProvisionalClone | ADPropertyDefinitionFlags.BackLink, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DelItemAfterBackupEnum = new ADPropertyDefinition("DelItemAfterBackupEnum", ExchangeObjectVersion.Exchange2003, typeof(DeletedItemRetention), "deletedItemFlags", ADPropertyDefinitionFlags.PersistDefaultValue, Microsoft.Exchange.Data.Directory.Recipient.DeletedItemRetention.RetainForCustomPeriod, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(DeletedItemRetention))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DeliveryMechanism = new ADPropertyDefinition("DeliveryMechanism", ExchangeObjectVersion.Exchange2003, typeof(DeliveryMechanisms), "deliveryMechanism", ADPropertyDefinitionFlags.Mandatory | ADPropertyDefinitionFlags.PersistDefaultValue, DeliveryMechanisms.MessageStore, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(DeliveryMechanisms))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Description = SharedPropertyDefinitions.Description;

		public static readonly ADPropertyDefinition EdbFilePath = SharedPropertyDefinitions.EdbFilePath;

		public static readonly ADPropertyDefinition EdbOfflineAtStartup = new ADPropertyDefinition("EdbOfflineAtStartup", ExchangeObjectVersion.Exchange2003, typeof(bool), "msExchEDBOffline", ADPropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ExchangeLegacyDN = SharedPropertyDefinitions.ExchangeLegacyDN;

		public static readonly ADPropertyDefinition DisplayName = SharedPropertyDefinitions.OptionalDisplayName;

		public static readonly ADPropertyDefinition FixedFont = new ADPropertyDefinition("FixedFont", ExchangeObjectVersion.Exchange2003, typeof(bool), "msExchConvertToFixedFont", ADPropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DeletedItemRetention = new ADPropertyDefinition("DeletedItemRetention", ExchangeObjectVersion.Exchange2003, typeof(EnhancedTimeSpan), "garbageCollPeriod", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromDays(14.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.Zero, EnhancedTimeSpan.FromSeconds(2147483647.0)),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaintenanceScheduleBitmaps = SharedPropertyDefinitions.MaintenanceScheduleBitmaps;

		public static readonly ADPropertyDefinition MaintenanceScheduleMode = new ADPropertyDefinition("MaintenanceScheduleMode", ExchangeObjectVersion.Exchange2003, typeof(ScheduleMode), "activationStyle", ADPropertyDefinitionFlags.PersistDefaultValue, ScheduleMode.Never, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(ScheduleMode))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaxCachedViews = new ADPropertyDefinition("MaxCachedViews", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchMaxCachedViews", ADPropertyDefinitionFlags.PersistDefaultValue, 11, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition QuotaNotificationMode = new ADPropertyDefinition("QuotaNotificationMode", ExchangeObjectVersion.Exchange2003, typeof(ScheduleMode), "quotaNotificationStyle", ADPropertyDefinitionFlags.PersistDefaultValue, ScheduleMode.Never, new PropertyDefinitionConstraint[]
		{
			new EnumValueDefinedConstraint(typeof(ScheduleMode))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition QuotaNotificationScheduleBitmaps = SharedPropertyDefinitions.QuotaNotificationScheduleBitmaps;

		public static readonly ADPropertyDefinition Recovery = new ADPropertyDefinition("Recovery", ExchangeObjectVersion.Exchange2003, typeof(bool), "msExchRestore", ADPropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RestoreInProgress = new ADPropertyDefinition("RestoreInProgress", ExchangeObjectVersion.Exchange2003, typeof(bool), "msExchDatabaseBeingRestored", ADPropertyDefinitionFlags.PersistDefaultValue | ADPropertyDefinitionFlags.DoNotProvisionalClone, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Server = SharedPropertyDefinitions.Server;

		public static readonly ADPropertyDefinition MasterServerOrAvailabilityGroup = new ADPropertyDefinition("MasterServerOrAvailabilityGroup", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), "msExchMasterServerOrAvailabilityGroup", ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition SMimeSignatureEnabled = new ADPropertyDefinition("SMimeSignatureEnabled", ExchangeObjectVersion.Exchange2003, typeof(bool), "msExchDownGradeMultipartSigned", ADPropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition IssueWarningQuota = new ADPropertyDefinition("IssueWarningQuota", ExchangeObjectVersion.Exchange2003, typeof(Unlimited<ByteQuantifiedSize>), ByteQuantifiedSize.KilobyteQuantifierProvider, "mDBStorageQuota", ADPropertyDefinitionFlags.None, Unlimited<ByteQuantifiedSize>.UnlimitedValue, new PropertyDefinitionConstraint[]
		{
			new RangedUnlimitedConstraint<ByteQuantifiedSize>(ByteQuantifiedSize.FromKB(0UL), ByteQuantifiedSize.FromKB(2147483647UL))
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition EventHistoryRetentionPeriod = new ADPropertyDefinition("EventHistoryRetentionPeriod", ExchangeObjectVersion.Exchange2007, typeof(EnhancedTimeSpan), "msExchEventHistoryRetentionPeriod", ADPropertyDefinitionFlags.PersistDefaultValue, EnhancedTimeSpan.FromSeconds(604800.0), new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<EnhancedTimeSpan>(EnhancedTimeSpan.FromSeconds(1.0), EnhancedTimeSpan.FromSeconds(2592000.0)),
			new EnhancedTimeSpanUnitConstraint(EnhancedTimeSpan.OneSecond)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DatabaseGroup = new ADPropertyDefinition("DatabaseGroup", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchDatabaseGroup", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		internal static readonly ADPropertyDefinition MailboxPublicFolderDatabase = SharedPropertyDefinitions.MailboxPublicFolderDatabase;

		public static readonly ADPropertyDefinition LogFilePrefix = new ADPropertyDefinition("LogFilePrefix", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchESEParamBaseName", ADPropertyDefinitionFlags.DoNotProvisionalClone, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CircularLoggingEnabledValue = new ADPropertyDefinition("CircularLoggingEnabledValue", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchESEParamCircularLog", ADPropertyDefinitionFlags.PersistDefaultValue, 0, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 1)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LogFolderPath = new ADPropertyDefinition("LogFolderPath", ExchangeObjectVersion.Exchange2010, typeof(NonRootLocalLongFullPath), "msExchESEParamLogFilePath", ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			LocalLongFullPathLengthConstraint.LocalLongFullDirectoryPathLengthConstraint,
			new NoLeadingOrTrailingWhitespaceConstraint()
		}, null, null);

		public static readonly ADPropertyDefinition SystemFolderPath = new ADPropertyDefinition("SystemFolderPath", ExchangeObjectVersion.Exchange2010, typeof(NonRootLocalLongFullPath), "msExchESEParamSystemPath", ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			LocalLongFullPathLengthConstraint.LocalLongFullDirectoryPathLengthConstraint,
			new NoLeadingOrTrailingWhitespaceConstraint()
		}, null, null);

		public static readonly ADPropertyDefinition TemporaryDataFolderPath = new ADPropertyDefinition("TemporaryDataFolderPath", ExchangeObjectVersion.Exchange2010, typeof(NonRootLocalLongFullPath), "msExchESEparamTempPath", ADPropertyDefinitionFlags.DoNotProvisionalClone, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition EventLogSourceID = new ADPropertyDefinition("EventLogSourceID", ExchangeObjectVersion.Exchange2010, typeof(string), "msExchESEParamEventSource", ADPropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ZeroDatabasePagesValue = new ADPropertyDefinition("ZeroDatabasePagesValue", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchESEParamZeroDatabaseDuringBackup", ADPropertyDefinitionFlags.PersistDefaultValue, 0, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 1)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LogFileSize = new ADPropertyDefinition("LogFileSize", ExchangeObjectVersion.Exchange2003, typeof(int), "msExchESEParamLogFileSize", ADPropertyDefinitionFlags.PersistDefaultValue, 1024, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LogBuffers = new ADPropertyDefinition("LogBuffers", ExchangeObjectVersion.Exchange2010, typeof(int?), "msExchESEparamLogBuffers", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaximumOpenTables = new ADPropertyDefinition("MaximumOpenTables", ExchangeObjectVersion.Exchange2010, typeof(int?), "msExchESEparamMaxOpenTables", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaximumTemporaryTables = new ADPropertyDefinition("MaximumTemporaryTables", ExchangeObjectVersion.Exchange2010, typeof(int?), "msExchESEparamMaxTemporaryTables", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaximumCursors = new ADPropertyDefinition("MaximumCursors", ExchangeObjectVersion.Exchange2010, typeof(int?), "msExchESEparamMaxCursors", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaximumSessions = new ADPropertyDefinition("MaximumSessions", ExchangeObjectVersion.Exchange2010, typeof(int?), "msExchESEparamMaxSessions", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaximumVersionStorePages = new ADPropertyDefinition("MaximumVersionStorePages", ExchangeObjectVersion.Exchange2010, typeof(int?), "msExchESEparamMaxVerPages", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition PreferredVersionStorePages = new ADPropertyDefinition("PreferredVersionStorePages", ExchangeObjectVersion.Exchange2010, typeof(int?), "msExchESEparamPreferredVerPages", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DatabaseExtensionSize = new ADPropertyDefinition("DatabaseExtensionSize", ExchangeObjectVersion.Exchange2010, typeof(int?), "msExchESEParamDbExtensionSize", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition LogCheckpointDepth = new ADPropertyDefinition("LogCheckpointDepth", ExchangeObjectVersion.Exchange2010, typeof(int?), "msExchESEParamCheckpointDepthMax", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ReplayCheckpointDepth = new ADPropertyDefinition("ReplayCheckpointDepth", ExchangeObjectVersion.Exchange2010, typeof(int?), "msExchESEParamReplayCheckpointDepthMax", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CachePriority = new ADPropertyDefinition("CachePriority", ExchangeObjectVersion.Exchange2010, typeof(int?), "msExchESEParamCachePriority", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 100)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ReplayCachePriority = new ADPropertyDefinition("ReplayCachePriority", ExchangeObjectVersion.Exchange2010, typeof(int?), "msExchESEParamReplayCachePriority", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, 100)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CachedClosedTables = new ADPropertyDefinition("CachedClosedTables", ExchangeObjectVersion.Exchange2010, typeof(int?), "msExchESEparamCachedClosedTables", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaximumPreReadPages = new ADPropertyDefinition("MaximumPreReadPages", ExchangeObjectVersion.Exchange2010, typeof(int?), "msExchESEParamPreReadIOMax", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition MaximumReplayPreReadPages = new ADPropertyDefinition("MaximumReplayPreReadPages", ExchangeObjectVersion.Exchange2010, typeof(int?), "msExchESEParamReplayPreReadIOMax", ADPropertyDefinitionFlags.None, null, new PropertyDefinitionConstraint[]
		{
			new RangedValueConstraint<int>(0, int.MaxValue)
		}, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition DataMoveReplicationConstraintDefinition = new ADPropertyDefinition("DataMoveReplicationConstraint", ExchangeObjectVersion.Exchange2007, typeof(DataMoveReplicationConstraintParameter), "msExchDataMoveReplicationConstraint", ADPropertyDefinitionFlags.None, DataMoveReplicationConstraintParameter.SecondCopy, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition ConfigurationXMLRaw = XMLSerializableBase.ConfigurationXmlRawProperty();

		public static readonly ADPropertyDefinition ConfigurationXML = XMLSerializableBase.ConfigurationXmlProperty<DatabaseConfigXml>(DatabaseSchema.ConfigurationXMLRaw);

		public static readonly ADPropertyDefinition MailboxProvisioningAttributes = new ADPropertyDefinition("MailboxProvisioningAttributes", ExchangeObjectVersion.Exchange2010, typeof(MailboxProvisioningAttributes), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			DatabaseSchema.ConfigurationXMLRaw,
			ADObjectSchema.RawName,
			DatabaseSchema.Server,
			DatabaseSchema.MasterServerOrAvailabilityGroup
		}, null, new GetterDelegate(Database.MailboxProvisioningAttributesGetter), new SetterDelegate(Database.MailboxProvisioningAttributesSetter), null, null);

		public static readonly ADPropertyDefinition AdministrativeGroup = new ADPropertyDefinition("AdministrativeGroup", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.Id
		}, null, new GetterDelegate(Database.AdministrativeGroupGetter), null, null, null);

		public static readonly ADPropertyDefinition MaintenanceSchedule = new ADPropertyDefinition("MaintenanceSchedule", ExchangeObjectVersion.Exchange2003, typeof(Schedule), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			DatabaseSchema.MaintenanceScheduleBitmaps,
			DatabaseSchema.MaintenanceScheduleMode
		}, null, new GetterDelegate(Database.MaintenanceScheduleGetter), new SetterDelegate(Database.MaintenanceScheduleSetter), null, null);

		public static readonly ADPropertyDefinition MountAtStartup = new ADPropertyDefinition("MountAtStartup", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			DatabaseSchema.EdbOfflineAtStartup
		}, new CustomFilterBuilderDelegate(Database.MountAtStartupFilterBuilder), (IPropertyBag propertyBag) => !(bool)propertyBag[DatabaseSchema.EdbOfflineAtStartup], delegate(object value, IPropertyBag propertyBag)
		{
			propertyBag[DatabaseSchema.EdbOfflineAtStartup] = !(bool)value;
		}, null, null);

		public static readonly ADPropertyDefinition CircularLoggingEnabled = new ADPropertyDefinition("CircularLoggingEnabled", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			DatabaseSchema.CircularLoggingEnabledValue
		}, null, (IPropertyBag propertyBag) => 0 != (int)propertyBag[DatabaseSchema.CircularLoggingEnabledValue], delegate(object value, IPropertyBag propertyBag)
		{
			propertyBag[DatabaseSchema.CircularLoggingEnabledValue] = (((bool)value) ? 1 : 0);
		}, null, null);

		public static readonly ADPropertyDefinition Organization = new ADPropertyDefinition("Organization", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.Id
		}, null, new GetterDelegate(Database.OrganizationGetter), null, null, null);

		public static readonly ADPropertyDefinition QuotaNotificationSchedule = new ADPropertyDefinition("QuotaNotificationSchedule", ExchangeObjectVersion.Exchange2003, typeof(Schedule), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			DatabaseSchema.QuotaNotificationScheduleBitmaps,
			DatabaseSchema.QuotaNotificationMode
		}, null, new GetterDelegate(Database.QuotaNotificationScheduleGetter), new SetterDelegate(Database.QuotaNotificationScheduleSetter), null, null);

		public static readonly ADPropertyDefinition RetainDeletedItemsUntilBackup = new ADPropertyDefinition("RetainDeletedItemsUntilBackup", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			DatabaseSchema.DelItemAfterBackupEnum
		}, new CustomFilterBuilderDelegate(Database.RetainDeletedItemsUntilBackupFilterBuilder), new GetterDelegate(Database.RetainDeletedItemsUntilBackupGetter), new SetterDelegate(Database.RetainDeletedItemsUntilBackupSetter), null, null);

		public static readonly ADPropertyDefinition MasterServerOrAvailabilityGroupName = new ADPropertyDefinition("MasterServerOrAvailabilityGroupName", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			DatabaseSchema.MasterServerOrAvailabilityGroup
		}, null, new GetterDelegate(Database.MasterServerOrAvailabilityGroupNameGetter), null, null, null);

		public static readonly ADPropertyDefinition ServerName = new ADPropertyDefinition("ServerName", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			DatabaseSchema.Server
		}, null, new GetterDelegate(Database.ServerNameGetter), null, null, null);

		public new static readonly ADPropertyDefinition Name = new ADPropertyDefinition("Name", ExchangeObjectVersion.Exchange2003, typeof(string), "name", ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, new PropertyDefinitionConstraint[]
		{
			new NoLeadingOrTrailingWhitespaceConstraint(),
			new ADObjectNameStringLengthConstraint(1, 64),
			new ContainingNonWhitespaceConstraint(),
			new ADObjectNameCharacterConstraint(new char[]
			{
				'\\',
				'/',
				'=',
				';',
				'\0',
				'\n'
			})
		}, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.RawName
		}, new CustomFilterBuilderDelegate(ADObject.DummyCustomFilterBuilderDelegate), new GetterDelegate(Database.DatabaseNameGetter), new SetterDelegate(Database.DatabaseNameSetter), null, null);

		public static readonly ADPropertyDefinition IsExchange2009OrLater = new ADPropertyDefinition("IsExchange2009OrLater", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.ExchangeVersion
		}, null, new GetterDelegate(Database.IsExchange2009OrLaterGetter), null, null, null);

		public static readonly ADPropertyDefinition RpcClientAccessServerExchangeLegacyDN = new ADPropertyDefinition("RpcClientAccessServerExchangeLegacyDN", ExchangeObjectVersion.Exchange2010, typeof(string), null, ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.DoNotProvisionalClone, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.ObjectClass,
			DatabaseSchema.ExchangeLegacyDN
		}, null, new GetterDelegate(Database.RpcClientAccessServerExchangeLegacyDNGetter), new SetterDelegate(Database.RpcClientAccessServerExchangeLegacyDNSetter), null, null);

		public static readonly ADPropertyDefinition AutoDagFlags = new ADPropertyDefinition("AutoDagFlags", ExchangeObjectVersion.Exchange2010, typeof(int), "msExchAutoDAGParamDatabaseFlags", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AutoDatabaseMountDialType = new ADPropertyDefinition("AutoDatabaseMountDialType", ExchangeObjectVersion.Exchange2010, typeof(AutoDatabaseMountDial), "msExchDataLossForAutoDatabaseMount", ADPropertyDefinitionFlags.PersistDefaultValue, AutoDatabaseMountDial.GoodAvailability, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition AutoDagExcludeFromMonitoring = new ADPropertyDefinition("AutoDagExcludeFromMonitoring", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			DatabaseSchema.AutoDagFlags
		}, null, ADObject.FlagGetterDelegate(DatabaseSchema.AutoDagFlags, 1), ADObject.FlagSetterDelegate(DatabaseSchema.AutoDagFlags, 1), null, null);

		public static readonly ADPropertyDefinition InvalidDatabaseCopiesAllowed = new ADPropertyDefinition("InvalidDatabaseCopiesAllowed", ExchangeObjectVersion.Exchange2010, typeof(bool), null, ADPropertyDefinitionFlags.TaskPopulated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);
	}
}
