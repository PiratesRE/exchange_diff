using System;
using System.Globalization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.StoreConfigurableType;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MigrationBatchSchema : ObjectSchema
	{
		public static readonly ProviderPropertyDefinition Identity = new SimpleProviderPropertyDefinition("Identity", ExchangeObjectVersion.Exchange2010, typeof(MigrationBatchId), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition ObjectState = UserConfigurationObjectSchema.ObjectState;

		public static readonly ProviderPropertyDefinition ExchangeVersion = UserConfigurationObjectSchema.ExchangeVersion;

		public static readonly ProviderPropertyDefinition TotalCount = new SimpleProviderPropertyDefinition("TotalCount", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.TaskPopulated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition FinalizedCount = new SimpleProviderPropertyDefinition("FinalizedCount", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.TaskPopulated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition StoppedCount = new SimpleProviderPropertyDefinition("StoppedCount", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.TaskPopulated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition SyncedCount = new SimpleProviderPropertyDefinition("SyncedCount", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.TaskPopulated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition PendingCount = new SimpleProviderPropertyDefinition("PendingCount", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.TaskPopulated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition SubmittedByUser = new SimpleProviderPropertyDefinition("SubmittedByUser", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.TaskPopulated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition OwnerId = new SimpleProviderPropertyDefinition("OwnerId", ExchangeObjectVersion.Exchange2010, typeof(ADObjectId), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition OwnerExchangeObjectId = new SimpleProviderPropertyDefinition("OwnerExchangeObjectId", ExchangeObjectVersion.Exchange2010, typeof(Guid), PropertyDefinitionFlags.TaskPopulated, Guid.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition DelegatedAdminOwner = new SimpleProviderPropertyDefinition("DelegatedAdminOwner", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.TaskPopulated, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition CreationDateTime = new SimpleProviderPropertyDefinition("CreationDateTime", ExchangeObjectVersion.Exchange2010, typeof(DateTime), PropertyDefinitionFlags.TaskPopulated, DateTime.MinValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition CreationDateTimeUTC = new SimpleProviderPropertyDefinition("CreationDateTimeUTC", ExchangeObjectVersion.Exchange2010, typeof(DateTime), PropertyDefinitionFlags.TaskPopulated, DateTime.MinValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition StartDateTime = new SimpleProviderPropertyDefinition("StartDateTime", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition StartDateTimeUTC = new SimpleProviderPropertyDefinition("StartDateTimeUTC", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition FinalizedDateTime = new SimpleProviderPropertyDefinition("FinalizedDateTime", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition FinalizedDateTimeUTC = new SimpleProviderPropertyDefinition("FinalizedDateTimeUTC", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition BatchStatus = new SimpleProviderPropertyDefinition("BatchStatus", ExchangeObjectVersion.Exchange2010, typeof(MigrationBatchStatus), PropertyDefinitionFlags.TaskPopulated, MigrationBatchStatus.Failed, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition FailedCount = new SimpleProviderPropertyDefinition("MigrationErrorsCount", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.TaskPopulated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition FailedInitialSyncCount = new SimpleProviderPropertyDefinition("FailedInitialSyncCount", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.TaskPopulated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition FailedIncrementalSyncCount = new SimpleProviderPropertyDefinition("FailedIncrementalSyncCount", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.TaskPopulated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition NumValidationErrors = new SimpleProviderPropertyDefinition("NumValidationErrors", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.TaskPopulated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition ValidationErrors = new SimpleProviderPropertyDefinition("ValidationErrors", ExchangeObjectVersion.Exchange2010, typeof(MigrationBatchError), PropertyDefinitionFlags.MultiValued | PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition Message = new SimpleProviderPropertyDefinition("Message", ExchangeObjectVersion.Exchange2010, typeof(LocalizedString), PropertyDefinitionFlags.TaskPopulated, LocalizedString.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition NotificationEmails = new SimpleProviderPropertyDefinition("NotificationEmails", ExchangeObjectVersion.Exchange2010, typeof(SmtpAddress), PropertyDefinitionFlags.MultiValued | PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition ExcludedFolders = new SimpleProviderPropertyDefinition("ExcludedFolders", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.MultiValued | PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition MigrationType = new SimpleProviderPropertyDefinition("MigrationType", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.TaskPopulated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition BatchDirection = new SimpleProviderPropertyDefinition("BatchDirection", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.TaskPopulated, 2, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition SuppressErrors = new SimpleProviderPropertyDefinition("SuppressErrors", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.TaskPopulated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition ActiveCount = new SimpleProviderPropertyDefinition("ActiveCount", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.TaskPopulated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition ProvisionedCount = new SimpleProviderPropertyDefinition("ProvisionedCount", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.TaskPopulated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition Locale = new SimpleProviderPropertyDefinition("Locale", ExchangeObjectVersion.Exchange2010, typeof(CultureInfo), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition IsProvisioning = new SimpleProviderPropertyDefinition("IsProvisioning", ExchangeObjectVersion.Exchange2010, typeof(bool), PropertyDefinitionFlags.TaskPopulated, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition MigrationBatchFlags = new SimpleProviderPropertyDefinition("MigrationBatchFlags", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.TaskPopulated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition Reports = new SimpleProviderPropertyDefinition("Reports", ExchangeObjectVersion.Exchange2010, typeof(MigrationReportSet), PropertyDefinitionFlags.MultiValued | PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition DiagnosticInfo = new SimpleProviderPropertyDefinition("DiagnosticInfo", ExchangeObjectVersion.Exchange2010, typeof(string), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition SupportedActions = new SimpleProviderPropertyDefinition("SupportedActions", ExchangeObjectVersion.Exchange2010, typeof(MigrationBatchSupportedActions), PropertyDefinitionFlags.TaskPopulated, MigrationBatchSupportedActions.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition InitialSyncDateTime = new SimpleProviderPropertyDefinition("InitialSyncDateTime", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition InitialSyncDateTimeUTC = new SimpleProviderPropertyDefinition("InitialSyncDateTimeUTC", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition InitialSyncDuration = new SimpleProviderPropertyDefinition("InitialSyncDuration", ExchangeObjectVersion.Exchange2010, typeof(TimeSpan?), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition LastSyncedDateTimeUTC = new SimpleProviderPropertyDefinition("LastSyncedDateTimeUTC", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition LastSyncedDateTime = new SimpleProviderPropertyDefinition("LastSyncedDateTime", ExchangeObjectVersion.Exchange2010, typeof(DateTime?), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition SourceEndpoint = new SimpleProviderPropertyDefinition("SourceEndpoint", ExchangeObjectVersion.Exchange2012, typeof(MigrationEndpoint), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition TargetEndpoint = new SimpleProviderPropertyDefinition("TargetEndpoint", ExchangeObjectVersion.Exchange2012, typeof(MigrationEndpoint), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition BadItemLimit = new SimpleProviderPropertyDefinition("BadItemLimit", ExchangeObjectVersion.Exchange2012, typeof(Unlimited<int>), PropertyDefinitionFlags.None, Unlimited<int>.UnlimitedValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition LargeItemLimit = new SimpleProviderPropertyDefinition("LargeItemLimit", ExchangeObjectVersion.Exchange2012, typeof(Unlimited<int>), PropertyDefinitionFlags.None, Unlimited<int>.UnlimitedValue, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition SourcePublicFolderDatabase = new SimpleProviderPropertyDefinition("SourcePublicFolderDatabase", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition TargetDatabases = new SimpleProviderPropertyDefinition("TargetDatabases", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.MultiValued | PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition TargetArchiveDatabases = new SimpleProviderPropertyDefinition("TargetArchiveDatabases", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.MultiValued | PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition PrimaryOnly = new SimpleProviderPropertyDefinition("PrimaryOnly", ExchangeObjectVersion.Exchange2012, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition ArchiveOnly = new SimpleProviderPropertyDefinition("ArchiveOnly", ExchangeObjectVersion.Exchange2012, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition TargetDeliveryDomain = new SimpleProviderPropertyDefinition("TargetDeliveryDomain", ExchangeObjectVersion.Exchange2012, typeof(string), PropertyDefinitionFlags.None, string.Empty, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition SkipSteps = new SimpleProviderPropertyDefinition("SkipSteps", ExchangeObjectVersion.Exchange2012, typeof(SkippableMigrationSteps), PropertyDefinitionFlags.None, SkippableMigrationSteps.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition CurrentRetryCount = new SimpleProviderPropertyDefinition("CurrentRetryCount", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.TaskPopulated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition AutoRetryCount = new SimpleProviderPropertyDefinition("AutoRetryCount", ExchangeObjectVersion.Exchange2010, typeof(int), PropertyDefinitionFlags.TaskPopulated, 0, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition StartAfter = new SimpleProviderPropertyDefinition("StartAfter", ExchangeObjectVersion.Exchange2012, typeof(DateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition StartAfterUTC = new SimpleProviderPropertyDefinition("StartAfterUTC", ExchangeObjectVersion.Exchange2012, typeof(DateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition CompleteAfter = new SimpleProviderPropertyDefinition("CompleteAfter", ExchangeObjectVersion.Exchange2012, typeof(DateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition CompleteAfterUTC = new SimpleProviderPropertyDefinition("CompleteAfterUTC", ExchangeObjectVersion.Exchange2012, typeof(DateTime?), PropertyDefinitionFlags.None, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition AllowUnknownColumnsInCsv = new SimpleProviderPropertyDefinition("AllowUnknownColumnsInCsv", ExchangeObjectVersion.Exchange2012, typeof(bool), PropertyDefinitionFlags.None, false, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition ReportInterval = new SimpleProviderPropertyDefinition("ReportInterval", ExchangeObjectVersion.Exchange2010, typeof(TimeSpan?), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);

		public static readonly ProviderPropertyDefinition OriginalBatchId = new SimpleProviderPropertyDefinition("OriginalBatchId", ExchangeObjectVersion.Exchange2010, typeof(Guid?), PropertyDefinitionFlags.TaskPopulated, null, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None);
	}
}
