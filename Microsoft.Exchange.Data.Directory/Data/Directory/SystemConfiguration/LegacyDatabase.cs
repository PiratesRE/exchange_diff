using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class LegacyDatabase : ADLegacyVersionableObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return LegacyDatabase.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return LegacyDatabase.mostDerivedClass;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new OrFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, MailboxDatabase.MostDerivedClass),
					new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, PublicFolderDatabase.MostDerivedClass)
				});
			}
		}

		protected override void ValidateWrite(List<ValidationError> errors)
		{
			base.ValidateWrite(errors);
			if (!base.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2007))
			{
				AsciiCharactersOnlyConstraint asciiCharactersOnlyConstraint = new AsciiCharactersOnlyConstraint();
				PropertyConstraintViolationError propertyConstraintViolationError = asciiCharactersOnlyConstraint.Validate(this.Name, LegacyDatabaseSchema.Name, null);
				if (propertyConstraintViolationError != null)
				{
					errors.Add(propertyConstraintViolationError);
				}
				propertyConstraintViolationError = asciiCharactersOnlyConstraint.Validate(this.EdbFilePath, LegacyDatabaseSchema.EdbFilePath, null);
				if (propertyConstraintViolationError != null)
				{
					errors.Add(propertyConstraintViolationError);
				}
			}
			if (null != this.EdbFilePath && this.EdbFilePath.IsPathInRootDirectory)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorEdbFilePathInRoot(this.EdbFilePath.PathName), this.Identity, string.Empty));
			}
			if (null == this.CopyEdbFilePath == this.HasLocalCopy)
			{
				if (null == this.CopyEdbFilePath)
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.ErrorProperty1EqValue1WhileProperty2EqValue2(LegacyDatabaseSchema.CopyEdbFilePath.Name, "null", LegacyDatabaseSchema.HasLocalCopy.Name, this.HasLocalCopy.ToString()), this.Identity, string.Empty));
				}
				else
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.ErrorProperty1NeValue1WhileProperty2EqValue2(LegacyDatabaseSchema.CopyEdbFilePath.Name, "null", LegacyDatabaseSchema.HasLocalCopy.Name, this.HasLocalCopy.ToString()), this.Identity, string.Empty));
				}
			}
			if (this.HasLocalCopy && null != this.CopyEdbFilePath && null != this.EdbFilePath)
			{
				string text = Path.GetFileName(this.EdbFilePath.PathName);
				string text2 = Path.GetFileName(this.CopyEdbFilePath.PathName);
				if (!string.Equals(text, text2, StringComparison.OrdinalIgnoreCase))
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.ErrorProductFileNameDifferentFromCopyFileName(text, text2), this.Identity, string.Empty));
				}
				text = Path.GetDirectoryName(this.EdbFilePath.PathName);
				text2 = Path.GetDirectoryName(this.CopyEdbFilePath.PathName);
				if (string.Equals(text, text2, StringComparison.OrdinalIgnoreCase))
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.ErrorProductFileDirectoryIdenticalWithCopyFileDirectory(text), this.Identity, string.Empty));
				}
				if (this.CopyEdbFilePath.IsPathInRootDirectory)
				{
					errors.Add(new ObjectValidationError(DirectoryStrings.ErrorEdbFilePathInRoot(this.CopyEdbFilePath.PathName), this.Identity, string.Empty));
				}
			}
			if (base.Id.DomainId != null && base.Id.Depth - base.Id.DomainId.Depth < 8)
			{
				errors.Add(new ObjectValidationError(DirectoryStrings.ErrorInvalidDNDepth, this.Identity, string.Empty));
			}
		}

		public ADObjectId AdministrativeGroup
		{
			get
			{
				return (ADObjectId)this[LegacyDatabaseSchema.AdministrativeGroup];
			}
		}

		[Parameter(Mandatory = false)]
		public bool AllowFileRestore
		{
			get
			{
				return (bool)this[LegacyDatabaseSchema.AllowFileRestore];
			}
			set
			{
				this[LegacyDatabaseSchema.AllowFileRestore] = value;
			}
		}

		public bool? BackupInProgress
		{
			get
			{
				return this.databaseBackupInProgress;
			}
			internal set
			{
				this.databaseBackupInProgress = value;
			}
		}

		public EdbFilePath CopyEdbFilePath
		{
			get
			{
				return (EdbFilePath)this[LegacyDatabaseSchema.CopyEdbFilePath];
			}
			internal set
			{
				this[LegacyDatabaseSchema.CopyEdbFilePath] = value;
			}
		}

		public bool DatabaseCreated
		{
			get
			{
				return (bool)this[LegacyDatabaseSchema.DatabaseCreated];
			}
			internal set
			{
				this[LegacyDatabaseSchema.DatabaseCreated] = value;
			}
		}

		internal DeliveryMechanisms DeliveryMechanism
		{
			get
			{
				return (DeliveryMechanisms)this[LegacyDatabaseSchema.DeliveryMechanism];
			}
		}

		public string Description
		{
			get
			{
				return (string)this[LegacyDatabaseSchema.Description];
			}
		}

		public EdbFilePath EdbFilePath
		{
			get
			{
				return (EdbFilePath)this[LegacyDatabaseSchema.EdbFilePath];
			}
			internal set
			{
				this[LegacyDatabaseSchema.EdbFilePath] = value;
			}
		}

		public string ExchangeLegacyDN
		{
			get
			{
				return (string)this[LegacyDatabaseSchema.ExchangeLegacyDN];
			}
			internal set
			{
				this[LegacyDatabaseSchema.ExchangeLegacyDN] = value;
			}
		}

		internal bool FixedFont
		{
			get
			{
				return (bool)this[LegacyDatabaseSchema.FixedFont];
			}
			set
			{
				this[LegacyDatabaseSchema.FixedFont] = value;
			}
		}

		public bool HasLocalCopy
		{
			get
			{
				return (bool)this[LegacyDatabaseSchema.HasLocalCopy];
			}
			internal set
			{
				this[LegacyDatabaseSchema.HasLocalCopy] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan DeletedItemRetention
		{
			get
			{
				return (EnhancedTimeSpan)this[LegacyDatabaseSchema.DeletedItemRetention];
			}
			set
			{
				this[LegacyDatabaseSchema.DeletedItemRetention] = value;
			}
		}

		public bool? SnapshotLastFullBackup
		{
			get
			{
				return this.snapshotLastFullBackup;
			}
			internal set
			{
				this.snapshotLastFullBackup = value;
			}
		}

		public bool? SnapshotLastIncrementalBackup
		{
			get
			{
				return this.snapshotLastIncrementalBackup;
			}
			internal set
			{
				this.snapshotLastIncrementalBackup = value;
			}
		}

		public bool? SnapshotLastDifferentialBackup
		{
			get
			{
				return this.snapshotLastDifferentialBackup;
			}
			internal set
			{
				this.snapshotLastDifferentialBackup = value;
			}
		}

		public bool? SnapshotLastCopyBackup
		{
			get
			{
				return this.snapshotLastCopyBackup;
			}
			internal set
			{
				this.snapshotLastCopyBackup = value;
			}
		}

		public DateTime? LastFullBackup
		{
			get
			{
				return this.databaseLastFullBackup;
			}
			internal set
			{
				this.databaseLastFullBackup = value;
			}
		}

		public DateTime? LastIncrementalBackup
		{
			get
			{
				return this.databaseLastIncrementalBackup;
			}
			internal set
			{
				this.databaseLastIncrementalBackup = value;
			}
		}

		public DateTime? LastDifferentialBackup
		{
			get
			{
				return this.databaseLastDifferentialBackup;
			}
			internal set
			{
				this.databaseLastDifferentialBackup = value;
			}
		}

		public DateTime? LastCopyBackup
		{
			get
			{
				return this.databaseLastCopyBackup;
			}
			internal set
			{
				this.databaseLastCopyBackup = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Schedule MaintenanceSchedule
		{
			get
			{
				return (Schedule)this[LegacyDatabaseSchema.MaintenanceSchedule];
			}
			set
			{
				this[LegacyDatabaseSchema.MaintenanceSchedule] = value;
			}
		}

		internal ScheduleMode MaintenanceScheduleMode
		{
			get
			{
				return (ScheduleMode)this[LegacyDatabaseSchema.MaintenanceScheduleMode];
			}
		}

		internal int MaxCachedViews
		{
			get
			{
				return (int)this[LegacyDatabaseSchema.MaxCachedViews];
			}
		}

		[Parameter(Mandatory = false)]
		public bool MountAtStartup
		{
			get
			{
				return (bool)this[LegacyDatabaseSchema.MountAtStartup];
			}
			set
			{
				this[LegacyDatabaseSchema.MountAtStartup] = value;
			}
		}

		public bool? Mounted
		{
			get
			{
				return this.databaseMounted;
			}
			internal set
			{
				this.databaseMounted = value;
			}
		}

		internal bool? OnlineMaintenanceInProgress
		{
			get
			{
				return this.databaseOnlineMaintenanceInProgress;
			}
			set
			{
				this.databaseOnlineMaintenanceInProgress = value;
			}
		}

		public ADObjectId Organization
		{
			get
			{
				return (ADObjectId)this[LegacyDatabaseSchema.Organization];
			}
		}

		internal ScheduleMode QuotaNotificationMode
		{
			get
			{
				return (ScheduleMode)this[LegacyDatabaseSchema.QuotaNotificationMode];
			}
		}

		[Parameter(Mandatory = false)]
		public Schedule QuotaNotificationSchedule
		{
			get
			{
				return (Schedule)this[LegacyDatabaseSchema.QuotaNotificationSchedule];
			}
			set
			{
				this[LegacyDatabaseSchema.QuotaNotificationSchedule] = value;
			}
		}

		internal bool RestoreInProgress
		{
			get
			{
				return (bool)this[LegacyDatabaseSchema.RestoreInProgress];
			}
			set
			{
				this[LegacyDatabaseSchema.RestoreInProgress] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RetainDeletedItemsUntilBackup
		{
			get
			{
				return (bool)this[LegacyDatabaseSchema.RetainDeletedItemsUntilBackup];
			}
			set
			{
				this[LegacyDatabaseSchema.RetainDeletedItemsUntilBackup] = value;
			}
		}

		public ADObjectId Server
		{
			get
			{
				return (ADObjectId)this[LegacyDatabaseSchema.Server];
			}
			internal set
			{
				this[LegacyDatabaseSchema.Server] = value;
			}
		}

		public string ServerName
		{
			get
			{
				return (string)this[LegacyDatabaseSchema.ServerName];
			}
		}

		internal bool SMimeSignatureEnabled
		{
			get
			{
				return (bool)this[LegacyDatabaseSchema.SMimeSignatureEnabled];
			}
			set
			{
				this[LegacyDatabaseSchema.SMimeSignatureEnabled] = value;
			}
		}

		public ADObjectId StorageGroup
		{
			get
			{
				return (ADObjectId)this[LegacyDatabaseSchema.StorageGroup];
			}
		}

		public string StorageGroupName
		{
			get
			{
				return (string)this[LegacyDatabaseSchema.StorageGroupName];
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> IssueWarningQuota
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[LegacyDatabaseSchema.IssueWarningQuota];
			}
			set
			{
				this[LegacyDatabaseSchema.IssueWarningQuota] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan EventHistoryRetentionPeriod
		{
			get
			{
				return (EnhancedTimeSpan)this[LegacyDatabaseSchema.EventHistoryRetentionPeriod];
			}
			set
			{
				this[LegacyDatabaseSchema.EventHistoryRetentionPeriod] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public new string Name
		{
			get
			{
				return (string)this[LegacyDatabaseSchema.Name];
			}
			set
			{
				this[LegacyDatabaseSchema.Name] = value;
				this[ADConfigurationObjectSchema.AdminDisplayName] = value;
				this[LegacyDatabaseSchema.DisplayName] = value;
			}
		}

		internal Server GetServer()
		{
			return base.Session.Read<Server>(this.Server);
		}

		internal StorageGroup GetStorageGroup()
		{
			return base.Session.Read<StorageGroup>(this.StorageGroup);
		}

		internal override void StampPersistableDefaultValues()
		{
			if (!base.IsModified(LegacyDatabaseSchema.MaintenanceSchedule))
			{
				this.MaintenanceSchedule = Schedule.DailyFrom1AMTo5AM;
			}
			if (!base.IsModified(LegacyDatabaseSchema.QuotaNotificationSchedule))
			{
				this.QuotaNotificationSchedule = Schedule.Daily1AM;
			}
			if (!base.IsModified(LegacyDatabaseSchema.IssueWarningQuota))
			{
				this.IssueWarningQuota = new Unlimited<ByteQuantifiedSize>(ByteQuantifiedSize.FromMB(1945UL));
			}
			base.StampPersistableDefaultValues();
		}

		internal static ValidationError[] ValidateAscendingQuotas(PropertyBag propertyBag, ProviderPropertyDefinition[] ascendingQuotaDefinitions, ObjectId identity)
		{
			List<ValidationError> list = new List<ValidationError>(ascendingQuotaDefinitions.Length);
			ProviderPropertyDefinition providerPropertyDefinition = null;
			Unlimited<ByteQuantifiedSize> unlimited = default(Unlimited<ByteQuantifiedSize>);
			int num = 0;
			while (ascendingQuotaDefinitions.Length > num)
			{
				ProviderPropertyDefinition providerPropertyDefinition2 = ascendingQuotaDefinitions[num];
				Unlimited<ByteQuantifiedSize> unlimited2 = (Unlimited<ByteQuantifiedSize>)propertyBag[providerPropertyDefinition2];
				if (!unlimited2.IsUnlimited)
				{
					if (providerPropertyDefinition != null && 0 < unlimited.CompareTo(unlimited2))
					{
						if (propertyBag.IsChanged(providerPropertyDefinition))
						{
							list.Add(new ObjectValidationError(DirectoryStrings.ErrorProperty1GtProperty2(providerPropertyDefinition.Name, unlimited.ToString(), providerPropertyDefinition2.Name, unlimited2.ToString()), identity, string.Empty));
						}
						else
						{
							list.Add(new ObjectValidationError(DirectoryStrings.ErrorProperty1LtProperty2(providerPropertyDefinition2.Name, unlimited2.ToString(), providerPropertyDefinition.Name, unlimited.ToString()), identity, string.Empty));
						}
					}
					providerPropertyDefinition = providerPropertyDefinition2;
					unlimited = unlimited2;
				}
				num++;
			}
			return list.ToArray();
		}

		internal static void InternalAssertComparisonFilter(SinglePropertyFilter filter, PropertyDefinition propertyDefinition)
		{
			string name = propertyDefinition.Name;
			ComparisonFilter comparisonFilter = filter as ComparisonFilter;
			if (comparisonFilter == null)
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedFilterForProperty(name, filter.GetType(), typeof(ComparisonFilter)));
			}
			object propertyValue = comparisonFilter.PropertyValue;
			if (propertyValue == null)
			{
				throw new ArgumentNullException("filter.PropertyValue");
			}
			Type type = propertyValue.GetType();
			if (type != propertyDefinition.Type)
			{
				throw new ADFilterException(DirectoryStrings.ExceptionUnsupportedPropertyValueType(name, type, propertyDefinition.Type));
			}
		}

		internal static object AdministrativeGroupGetter(IPropertyBag propertyBag)
		{
			object result;
			try
			{
				result = ((ADObjectId)propertyBag[ADObjectSchema.Id]).AncestorDN(2);
			}
			catch (NullReferenceException ex)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("AdministrativeGroup", ex.Message), LegacyDatabaseSchema.AdministrativeGroup, propertyBag[ADObjectSchema.Id]), ex);
			}
			catch (InvalidOperationException ex2)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("AdministrativeGroup", ex2.Message), LegacyDatabaseSchema.AdministrativeGroup, propertyBag[ADObjectSchema.Id]), ex2);
			}
			return result;
		}

		internal static QueryFilter HasLocalCopyFilterBuilder(SinglePropertyFilter filter)
		{
			LegacyDatabase.InternalAssertComparisonFilter(filter, LegacyDatabaseSchema.HasLocalCopy);
			ComparisonFilter comparisonFilter = filter as ComparisonFilter;
			ComparisonOperator comparisonOperator = comparisonFilter.ComparisonOperator;
			if (comparisonOperator != ComparisonOperator.Equal && ComparisonOperator.NotEqual != comparisonOperator)
			{
				throw new ADFilterException(DirectoryStrings.ExceptionInvalidBitwiseComparison(LegacyDatabaseSchema.HasLocalCopy.Name, comparisonOperator.ToString()));
			}
			bool flag = (bool)comparisonFilter.PropertyValue;
			bool flag2 = ComparisonOperator.Equal == comparisonOperator;
			QueryFilter queryFilter = new BitMaskAndFilter(LegacyDatabaseSchema.HasLocalCopyValue, 1UL);
			if (flag ^ flag2)
			{
				queryFilter = new NotFilter(queryFilter);
			}
			return queryFilter;
		}

		internal static object HasLocalCopyGetter(IPropertyBag propertyBag)
		{
			object obj = propertyBag[LegacyDatabaseSchema.HasLocalCopyValue];
			if (obj != null)
			{
				return 0 != (1 & (int)obj);
			}
			return null;
		}

		internal static void HasLocalCopySetter(object value, IPropertyBag propertyBag)
		{
			object obj = propertyBag[LegacyDatabaseSchema.HasLocalCopyValue];
			int num = (obj == null) ? ((int)LegacyDatabaseSchema.HasLocalCopyValue.DefaultValue) : ((int)propertyBag[LegacyDatabaseSchema.HasLocalCopyValue]);
			propertyBag[LegacyDatabaseSchema.HasLocalCopyValue] = (((bool)value) ? (1 | num) : (-2 & num));
		}

		internal static object MaintenanceScheduleGetter(IPropertyBag propertyBag)
		{
			return propertyBag[LegacyDatabaseSchema.MaintenanceScheduleBitmaps];
		}

		internal static void MaintenanceScheduleSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[LegacyDatabaseSchema.MaintenanceScheduleBitmaps] = value;
			if (value == null)
			{
				propertyBag[LegacyDatabaseSchema.MaintenanceScheduleMode] = ScheduleMode.Never;
				return;
			}
			propertyBag[LegacyDatabaseSchema.MaintenanceScheduleMode] = ((Schedule)value).Mode;
		}

		internal static QueryFilter MountAtStartupFilterBuilder(SinglePropertyFilter filter)
		{
			LegacyDatabase.InternalAssertComparisonFilter(filter, LegacyDatabaseSchema.MountAtStartup);
			ComparisonFilter comparisonFilter = filter as ComparisonFilter;
			return new ComparisonFilter(comparisonFilter.ComparisonOperator, LegacyDatabaseSchema.EdbOfflineAtStartup, !(bool)comparisonFilter.PropertyValue);
		}

		internal static object OrganizationGetter(IPropertyBag propertyBag)
		{
			object result;
			try
			{
				result = ((ADObjectId)propertyBag[ADObjectSchema.Id]).AncestorDN(4);
			}
			catch (NullReferenceException ex)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("Organization", ex.Message), LegacyDatabaseSchema.Organization, propertyBag[ADObjectSchema.Id]), ex);
			}
			catch (InvalidOperationException ex2)
			{
				throw new DataValidationException(new PropertyValidationError(DirectoryStrings.CannotCalculateProperty("Organization", ex2.Message), LegacyDatabaseSchema.Organization, propertyBag[ADObjectSchema.Id]), ex2);
			}
			return result;
		}

		internal static object QuotaNotificationScheduleGetter(IPropertyBag propertyBag)
		{
			return propertyBag[LegacyDatabaseSchema.QuotaNotificationScheduleBitmaps];
		}

		internal static void QuotaNotificationScheduleSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[LegacyDatabaseSchema.QuotaNotificationScheduleBitmaps] = value;
			if (value == null)
			{
				propertyBag[LegacyDatabaseSchema.QuotaNotificationMode] = ScheduleMode.Never;
				return;
			}
			propertyBag[LegacyDatabaseSchema.QuotaNotificationMode] = ((Schedule)value).Mode;
		}

		internal static QueryFilter RetainDeletedItemsUntilBackupFilterBuilder(SinglePropertyFilter filter)
		{
			LegacyDatabase.InternalAssertComparisonFilter(filter, LegacyDatabaseSchema.RetainDeletedItemsUntilBackup);
			ComparisonFilter comparisonFilter = filter as ComparisonFilter;
			return new ComparisonFilter(comparisonFilter.ComparisonOperator, LegacyDatabaseSchema.DelItemAfterBackupEnum, ((bool)comparisonFilter.PropertyValue) ? Microsoft.Exchange.Data.Directory.Recipient.DeletedItemRetention.RetainUntilBackupOrCustomPeriod : Microsoft.Exchange.Data.Directory.Recipient.DeletedItemRetention.RetainForCustomPeriod);
		}

		internal static object RetainDeletedItemsUntilBackupGetter(IPropertyBag propertyBag)
		{
			return Microsoft.Exchange.Data.Directory.Recipient.DeletedItemRetention.RetainUntilBackupOrCustomPeriod == (DeletedItemRetention)propertyBag[LegacyDatabaseSchema.DelItemAfterBackupEnum];
		}

		internal static void RetainDeletedItemsUntilBackupSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[LegacyDatabaseSchema.DelItemAfterBackupEnum] = (((bool)value) ? Microsoft.Exchange.Data.Directory.Recipient.DeletedItemRetention.RetainUntilBackupOrCustomPeriod : Microsoft.Exchange.Data.Directory.Recipient.DeletedItemRetention.RetainForCustomPeriod);
		}

		internal static object ServerNameGetter(IPropertyBag propertyBag)
		{
			ADObjectId adobjectId = (ADObjectId)propertyBag[LegacyDatabaseSchema.Server];
			if (adobjectId == null)
			{
				return LegacyDatabaseSchema.ServerName.DefaultValue;
			}
			return adobjectId.Name;
		}

		internal static object StorageGroupGetter(IPropertyBag propertyBag)
		{
			ADObjectId adobjectId = (ADObjectId)propertyBag[ADObjectSchema.Id];
			if (adobjectId == null)
			{
				return LegacyDatabaseSchema.StorageGroup.DefaultValue;
			}
			return adobjectId.Parent;
		}

		internal static object StorageGroupNameGetter(IPropertyBag propertyBag)
		{
			ADObjectId adobjectId = (ADObjectId)LegacyDatabase.StorageGroupGetter(propertyBag);
			if (adobjectId == null)
			{
				return LegacyDatabaseSchema.StorageGroupName.DefaultValue;
			}
			return adobjectId.Name;
		}

		internal static object DatabaseNameGetter(IPropertyBag propertyBag)
		{
			return propertyBag[ADObjectSchema.RawName];
		}

		internal static void DatabaseNameSetter(object value, IPropertyBag propertyBag)
		{
			propertyBag[ADObjectSchema.RawName] = value;
		}

		private static LegacyDatabaseSchema schema = ObjectSchema.GetInstance<LegacyDatabaseSchema>();

		private static string mostDerivedClass = "msExchMDB";

		private bool? databaseMounted;

		private bool? databaseOnlineMaintenanceInProgress;

		private bool? databaseBackupInProgress;

		private bool? snapshotLastFullBackup;

		private bool? snapshotLastIncrementalBackup;

		private bool? snapshotLastDifferentialBackup;

		private bool? snapshotLastCopyBackup;

		private DateTime? databaseLastFullBackup;

		private DateTime? databaseLastIncrementalBackup;

		private DateTime? databaseLastDifferentialBackup;

		private DateTime? databaseLastCopyBackup;
	}
}
