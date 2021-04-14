using System;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class LegacyDatabaseSchema : ADLegacyVersionableObjectSchema
	{
		public static readonly ADPropertyDefinition AllowFileRestore = new ADPropertyDefinition("AllowFileRestore", ExchangeObjectVersion.Exchange2003, typeof(bool), "msExchPatchMDB", ADPropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition CopyEdbFilePath = SharedPropertyDefinitions.CopyEdbFilePath;

		public static readonly ADPropertyDefinition DatabaseCreated = new ADPropertyDefinition("DatabaseCreated", ExchangeObjectVersion.Exchange2003, typeof(bool), "msExchDatabaseCreated", ADPropertyDefinitionFlags.PersistDefaultValue | ADPropertyDefinitionFlags.DoNotProvisionalClone, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

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

		public static readonly ADPropertyDefinition FixedFont = new ADPropertyDefinition("FixedFont", ExchangeObjectVersion.Exchange2003, typeof(bool), "msExchConvertToFixedFont", ADPropertyDefinitionFlags.PersistDefaultValue, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition HasLocalCopyValue = new ADPropertyDefinition("HasLocalCopyValue", ExchangeObjectVersion.Exchange2007, typeof(int), "msExchHasLocalCopy", ADPropertyDefinitionFlags.PersistDefaultValue, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

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

		public static readonly ADPropertyDefinition RestoreInProgress = new ADPropertyDefinition("RestoreInProgress", ExchangeObjectVersion.Exchange2003, typeof(bool), "msExchDatabaseBeingRestored", ADPropertyDefinitionFlags.PersistDefaultValue | ADPropertyDefinitionFlags.DoNotProvisionalClone, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition Server = SharedPropertyDefinitions.Server;

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

		public static readonly ADPropertyDefinition AdministrativeGroup = new ADPropertyDefinition("AdministrativeGroup", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.Id
		}, null, new GetterDelegate(LegacyDatabase.AdministrativeGroupGetter), null, null, null);

		public static readonly ADPropertyDefinition HasLocalCopy = new ADPropertyDefinition("HasLocalCopy", ExchangeObjectVersion.Exchange2007, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			LegacyDatabaseSchema.HasLocalCopyValue
		}, new CustomFilterBuilderDelegate(LegacyDatabase.HasLocalCopyFilterBuilder), new GetterDelegate(LegacyDatabase.HasLocalCopyGetter), new SetterDelegate(LegacyDatabase.HasLocalCopySetter), null, null);

		public static readonly ADPropertyDefinition MaintenanceSchedule = new ADPropertyDefinition("MaintenanceSchedule", ExchangeObjectVersion.Exchange2003, typeof(Schedule), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			LegacyDatabaseSchema.MaintenanceScheduleBitmaps,
			LegacyDatabaseSchema.MaintenanceScheduleMode
		}, null, new GetterDelegate(LegacyDatabase.MaintenanceScheduleGetter), new SetterDelegate(LegacyDatabase.MaintenanceScheduleSetter), null, null);

		public static readonly ADPropertyDefinition MountAtStartup = new ADPropertyDefinition("MountAtStartup", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, true, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			LegacyDatabaseSchema.EdbOfflineAtStartup
		}, new CustomFilterBuilderDelegate(LegacyDatabase.MountAtStartupFilterBuilder), (IPropertyBag propertyBag) => !(bool)propertyBag[LegacyDatabaseSchema.EdbOfflineAtStartup], delegate(object value, IPropertyBag propertyBag)
		{
			propertyBag[LegacyDatabaseSchema.EdbOfflineAtStartup] = !(bool)value;
		}, null, null);

		public static readonly ADPropertyDefinition Organization = new ADPropertyDefinition("Organization", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.Id
		}, null, new GetterDelegate(LegacyDatabase.OrganizationGetter), null, null, null);

		public static readonly ADPropertyDefinition QuotaNotificationSchedule = new ADPropertyDefinition("QuotaNotificationSchedule", ExchangeObjectVersion.Exchange2003, typeof(Schedule), null, ADPropertyDefinitionFlags.Calculated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			LegacyDatabaseSchema.QuotaNotificationScheduleBitmaps,
			LegacyDatabaseSchema.QuotaNotificationMode
		}, null, new GetterDelegate(LegacyDatabase.QuotaNotificationScheduleGetter), new SetterDelegate(LegacyDatabase.QuotaNotificationScheduleSetter), null, null);

		public static readonly ADPropertyDefinition RetainDeletedItemsUntilBackup = new ADPropertyDefinition("RetainDeletedItemsUntilBackup", ExchangeObjectVersion.Exchange2003, typeof(bool), null, ADPropertyDefinitionFlags.Calculated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			LegacyDatabaseSchema.DelItemAfterBackupEnum
		}, new CustomFilterBuilderDelegate(LegacyDatabase.RetainDeletedItemsUntilBackupFilterBuilder), new GetterDelegate(LegacyDatabase.RetainDeletedItemsUntilBackupGetter), new SetterDelegate(LegacyDatabase.RetainDeletedItemsUntilBackupSetter), null, null);

		public static readonly ADPropertyDefinition ServerName = new ADPropertyDefinition("ServerName", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			LegacyDatabaseSchema.Server
		}, null, new GetterDelegate(LegacyDatabase.ServerNameGetter), null, null, null);

		public static readonly ADPropertyDefinition StorageGroup = new ADPropertyDefinition("StorageGroup", ExchangeObjectVersion.Exchange2003, typeof(ADObjectId), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated | ADPropertyDefinitionFlags.ValidateInFirstOrganization, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.Id
		}, null, new GetterDelegate(LegacyDatabase.StorageGroupGetter), null, null, null);

		public static readonly ADPropertyDefinition StorageGroupName = new ADPropertyDefinition("StorageGroupName", ExchangeObjectVersion.Exchange2003, typeof(string), null, ADPropertyDefinitionFlags.ReadOnly | ADPropertyDefinitionFlags.Calculated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, new ProviderPropertyDefinition[]
		{
			ADObjectSchema.Id
		}, null, new GetterDelegate(LegacyDatabase.StorageGroupNameGetter), null, null, null);

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
		}, new CustomFilterBuilderDelegate(ADObject.DummyCustomFilterBuilderDelegate), new GetterDelegate(LegacyDatabase.DatabaseNameGetter), new SetterDelegate(LegacyDatabase.DatabaseNameSetter), null, null);

		public static readonly ADPropertyDefinition DisplayName = SharedPropertyDefinitions.OptionalDisplayName;
	}
}
