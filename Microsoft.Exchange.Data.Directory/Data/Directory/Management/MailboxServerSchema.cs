using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	internal class MailboxServerSchema : ADPresentationSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<ActiveDirectoryServerSchema>();
		}

		public static readonly ADPropertyDefinition CalendarRepairWorkCycle = ActiveDirectoryServerSchema.CalendarRepairWorkCycle;

		public static readonly ADPropertyDefinition CalendarRepairWorkCycleCheckpoint = ActiveDirectoryServerSchema.CalendarRepairWorkCycleCheckpoint;

		public static readonly ADPropertyDefinition SharingPolicyWorkCycle = ActiveDirectoryServerSchema.SharingPolicyWorkCycle;

		public static readonly ADPropertyDefinition SharingPolicyWorkCycleCheckpoint = ActiveDirectoryServerSchema.SharingPolicyWorkCycleCheckpoint;

		public static readonly ADPropertyDefinition SharingSyncWorkCycle = ActiveDirectoryServerSchema.SharingSyncWorkCycle;

		public static readonly ADPropertyDefinition SharingSyncWorkCycleCheckpoint = ActiveDirectoryServerSchema.SharingSyncWorkCycleCheckpoint;

		public static readonly ADPropertyDefinition PublicFolderWorkCycle = ActiveDirectoryServerSchema.PublicFolderWorkCycle;

		public static readonly ADPropertyDefinition PublicFolderWorkCycleCheckpoint = ActiveDirectoryServerSchema.PublicFolderWorkCycleCheckpoint;

		public static readonly ADPropertyDefinition SiteMailboxWorkCycle = ActiveDirectoryServerSchema.SiteMailboxWorkCycle;

		public static readonly ADPropertyDefinition SiteMailboxWorkCycleCheckpoint = ActiveDirectoryServerSchema.SiteMailboxWorkCycleCheckpoint;

		public static readonly ADPropertyDefinition ManagedFolderWorkCycle = ActiveDirectoryServerSchema.ManagedFolderWorkCycle;

		public static readonly ADPropertyDefinition ManagedFolderWorkCycleCheckpoint = ActiveDirectoryServerSchema.ManagedFolderWorkCycleCheckpoint;

		public static readonly ADPropertyDefinition MailboxAssociationReplicationWorkCycle = ActiveDirectoryServerSchema.MailboxAssociationReplicationWorkCycle;

		public static readonly ADPropertyDefinition MailboxAssociationReplicationWorkCycleCheckpoint = ActiveDirectoryServerSchema.MailboxAssociationReplicationWorkCycleCheckpoint;

		public static readonly ADPropertyDefinition GroupMailboxWorkCycle = ActiveDirectoryServerSchema.GroupMailboxWorkCycle;

		public static readonly ADPropertyDefinition GroupMailboxWorkCycleCheckpoint = ActiveDirectoryServerSchema.GroupMailboxWorkCycleCheckpoint;

		public static readonly ADPropertyDefinition TopNWorkCycle = ActiveDirectoryServerSchema.TopNWorkCycle;

		public static readonly ADPropertyDefinition TopNWorkCycleCheckpoint = ActiveDirectoryServerSchema.TopNWorkCycleCheckpoint;

		public static readonly ADPropertyDefinition UMReportingWorkCycle = ActiveDirectoryServerSchema.UMReportingWorkCycle;

		public static readonly ADPropertyDefinition UMReportingWorkCycleCheckpoint = ActiveDirectoryServerSchema.UMReportingWorkCycleCheckpoint;

		public static readonly ADPropertyDefinition InferenceTrainingWorkCycle = ActiveDirectoryServerSchema.InferenceTrainingWorkCycle;

		public static readonly ADPropertyDefinition InferenceTrainingWorkCycleCheckpoint = ActiveDirectoryServerSchema.InferenceTrainingWorkCycleCheckpoint;

		public static readonly ADPropertyDefinition DataPath = ServerSchema.DataPath;

		public static readonly ADPropertyDefinition DirectoryProcessorWorkCycle = ActiveDirectoryServerSchema.DirectoryProcessorWorkCycle;

		public static readonly ADPropertyDefinition DirectoryProcessorWorkCycleCheckpoint = ActiveDirectoryServerSchema.DirectoryProcessorWorkCycleCheckpoint;

		public static readonly ADPropertyDefinition OABGeneratorWorkCycle = ActiveDirectoryServerSchema.OABGeneratorWorkCycle;

		public static readonly ADPropertyDefinition OABGeneratorWorkCycleCheckpoint = ActiveDirectoryServerSchema.OABGeneratorWorkCycleCheckpoint;

		public static readonly ADPropertyDefinition InferenceDataCollectionWorkCycle = ActiveDirectoryServerSchema.InferenceDataCollectionWorkCycle;

		public static readonly ADPropertyDefinition InferenceDataCollectionWorkCycleCheckpoint = ActiveDirectoryServerSchema.InferenceDataCollectionWorkCycleCheckpoint;

		public static readonly ADPropertyDefinition PeopleRelevanceWorkCycle = ActiveDirectoryServerSchema.PeopleRelevanceWorkCycle;

		public static readonly ADPropertyDefinition PeopleRelevanceWorkCycleCheckpoint = ActiveDirectoryServerSchema.PeopleRelevanceWorkCycleCheckpoint;

		public static readonly ADPropertyDefinition SharePointSignalStoreWorkCycle = ActiveDirectoryServerSchema.SharePointSignalStoreWorkCycle;

		public static readonly ADPropertyDefinition SharePointSignalStoreWorkCycleCheckpoint = ActiveDirectoryServerSchema.SharePointSignalStoreWorkCycleCheckpoint;

		public static readonly ADPropertyDefinition PeopleCentricTriageWorkCycle = ActiveDirectoryServerSchema.PeopleCentricTriageWorkCycle;

		public static readonly ADPropertyDefinition PeopleCentricTriageWorkCycleCheckpoint = ActiveDirectoryServerSchema.PeopleCentricTriageWorkCycleCheckpoint;

		public static readonly ADPropertyDefinition MailboxProcessorWorkCycle = ActiveDirectoryServerSchema.MailboxProcessorWorkCycle;

		public static readonly ADPropertyDefinition StoreDsMaintenanceWorkCycle = ActiveDirectoryServerSchema.StoreDsMaintenanceWorkCycle;

		public static readonly ADPropertyDefinition StoreDsMaintenanceWorkCycleCheckpoint = ActiveDirectoryServerSchema.StoreDsMaintenanceWorkCycleCheckpoint;

		public static readonly ADPropertyDefinition StoreIntegrityCheckWorkCycle = ActiveDirectoryServerSchema.StoreIntegrityCheckWorkCycle;

		public static readonly ADPropertyDefinition StoreIntegrityCheckWorkCycleCheckpoint = ActiveDirectoryServerSchema.StoreIntegrityCheckWorkCycleCheckpoint;

		public static readonly ADPropertyDefinition StoreMaintenanceWorkCycle = ActiveDirectoryServerSchema.StoreMaintenanceWorkCycle;

		public static readonly ADPropertyDefinition StoreMaintenanceWorkCycleCheckpoint = ActiveDirectoryServerSchema.StoreMaintenanceWorkCycleCheckpoint;

		public static readonly ADPropertyDefinition StoreScheduledIntegrityCheckWorkCycle = ActiveDirectoryServerSchema.StoreScheduledIntegrityCheckWorkCycle;

		public static readonly ADPropertyDefinition StoreScheduledIntegrityCheckWorkCycleCheckpoint = ActiveDirectoryServerSchema.StoreScheduledIntegrityCheckWorkCycleCheckpoint;

		public static readonly ADPropertyDefinition StoreUrgentMaintenanceWorkCycle = ActiveDirectoryServerSchema.StoreUrgentMaintenanceWorkCycle;

		public static readonly ADPropertyDefinition StoreUrgentMaintenanceWorkCycleCheckpoint = ActiveDirectoryServerSchema.StoreUrgentMaintenanceWorkCycleCheckpoint;

		public static readonly ADPropertyDefinition JunkEmailOptionsCommitterWorkCycle = ActiveDirectoryServerSchema.JunkEmailOptionsCommitterWorkCycle;

		public static readonly ADPropertyDefinition ProbeTimeBasedAssistantWorkCycle = ActiveDirectoryServerSchema.ProbeTimeBasedAssistantWorkCycle;

		public static readonly ADPropertyDefinition ProbeTimeBasedAssistantWorkCycleCheckpoint = ActiveDirectoryServerSchema.ProbeTimeBasedAssistantWorkCycleCheckpoint;

		public static readonly ADPropertyDefinition SearchIndexRepairTimeBasedAssistantWorkCycle = ActiveDirectoryServerSchema.SearchIndexRepairTimeBasedAssistantWorkCycle;

		public static readonly ADPropertyDefinition SearchIndexRepairTimeBasedAssistantWorkCycleCheckpoint = ActiveDirectoryServerSchema.SearchIndexRepairTimeBasedAssistantWorkCycleCheckpoint;

		public static readonly ADPropertyDefinition DarTaskStoreTimeBasedAssistantWorkCycle = ActiveDirectoryServerSchema.DarTaskStoreTimeBasedAssistantWorkCycle;

		public static readonly ADPropertyDefinition DarTaskStoreTimeBasedAssistantWorkCycleCheckpoint = ActiveDirectoryServerSchema.DarTaskStoreTimeBasedAssistantWorkCycleCheckpoint;

		public static readonly ADPropertyDefinition SharingPolicySchedule = ActiveDirectoryServerSchema.SharingPolicySchedule;

		public static readonly ADPropertyDefinition CalendarRepairMissingItemFixDisabled = ActiveDirectoryServerSchema.CalendarRepairMissingItemFixDisabled;

		public static readonly ADPropertyDefinition CalendarRepairLogEnabled = ActiveDirectoryServerSchema.CalendarRepairLogEnabled;

		public static readonly ADPropertyDefinition CalendarRepairLogSubjectLoggingEnabled = ActiveDirectoryServerSchema.CalendarRepairLogSubjectLoggingEnabled;

		public static readonly ADPropertyDefinition CalendarRepairLogPath = ActiveDirectoryServerSchema.CalendarRepairLogPath;

		public static readonly ADPropertyDefinition CalendarRepairIntervalEndWindow = ActiveDirectoryServerSchema.CalendarRepairIntervalEndWindow;

		public static readonly ADPropertyDefinition CalendarRepairLogFileAgeLimit = ActiveDirectoryServerSchema.CalendarRepairLogFileAgeLimit;

		public static readonly ADPropertyDefinition CalendarRepairLogDirectorySizeLimit = ActiveDirectoryServerSchema.CalendarRepairLogDirectorySizeLimit;

		public static readonly ADPropertyDefinition CalendarRepairMode = ActiveDirectoryServerSchema.CalendarRepairMode;

		public static readonly ADPropertyDefinition ElcSchedule = ServerSchema.ElcSchedule;

		public static readonly ADPropertyDefinition ElcAuditLogPath = ActiveDirectoryServerSchema.ElcAuditLogPath;

		public static readonly ADPropertyDefinition ElcAuditLogFileAgeLimit = ActiveDirectoryServerSchema.ElcAuditLogFileAgeLimit;

		public static readonly ADPropertyDefinition ElcAuditLogDirectorySizeLimit = ActiveDirectoryServerSchema.ElcAuditLogDirectorySizeLimit;

		public static readonly ADPropertyDefinition ElcAuditLogFileSizeLimit = ActiveDirectoryServerSchema.ElcAuditLogFileSizeLimit;

		public static readonly ADPropertyDefinition MAPIEncryptionRequired = ActiveDirectoryServerSchema.MAPIEncryptionRequired;

		public static readonly ADPropertyDefinition ExpirationAuditLogEnabled = ActiveDirectoryServerSchema.ExpirationAuditLogEnabled;

		public static readonly ADPropertyDefinition AutocopyAuditLogEnabled = ActiveDirectoryServerSchema.AutocopyAuditLogEnabled;

		public static readonly ADPropertyDefinition FolderAuditLogEnabled = ActiveDirectoryServerSchema.FolderAuditLogEnabled;

		public static readonly ADPropertyDefinition ElcSubjectLoggingEnabled = ActiveDirectoryServerSchema.ElcSubjectLoggingEnabled;

		public static readonly ADPropertyDefinition SubmissionServerOverrideLIst = ServerSchema.SubmissionServerOverrideList;

		public static readonly ADPropertyDefinition AutoDatabaseMountDialType = ActiveDirectoryServerSchema.AutoDatabaseMountDialType;

		public static readonly ADPropertyDefinition IsPhoneticSupportEnabled = ServerSchema.IsPhoneticSupportEnabled;

		public static readonly ADPropertyDefinition Locale = ServerSchema.Locale;

		public static readonly ADPropertyDefinition MigrationLogLoggingLevel = ActiveDirectoryServerSchema.MigrationLogLoggingLevel;

		public static readonly ADPropertyDefinition MigrationLogFilePath = ActiveDirectoryServerSchema.MigrationLogFilePath;

		public static readonly ADPropertyDefinition MigrationLogMaxAge = ActiveDirectoryServerSchema.MigrationLogMaxAge;

		public static readonly ADPropertyDefinition MigrationLogMaxDirectorySize = ActiveDirectoryServerSchema.MigrationLogMaxDirectorySize;

		public static readonly ADPropertyDefinition MigrationLogMaxFileSize = ActiveDirectoryServerSchema.MigrationLogMaxFileSize;

		public static readonly ADPropertyDefinition DatabaseAvailabilityGroup = ServerSchema.DatabaseAvailabilityGroup;

		public static readonly ADPropertyDefinition ForceGroupMetricsGeneration = ActiveDirectoryServerSchema.ForceGroupMetricsGeneration;

		public static readonly ADPropertyDefinition TransportSyncDispatchEnabled = ServerSchema.TransportSyncDispatchEnabled;

		public static readonly ADPropertyDefinition MaxTransportSyncDispatchers = ServerSchema.MaxTransportSyncDispatchers;

		public static readonly ADPropertyDefinition TransportSyncLogEnabled = ServerSchema.TransportSyncMailboxLogEnabled;

		public static readonly ADPropertyDefinition TransportSyncLogLoggingLevel = ServerSchema.TransportSyncMailboxLogLoggingLevel;

		public static readonly ADPropertyDefinition TransportSyncLogFilePath = ServerSchema.TransportSyncMailboxLogFilePath;

		public static readonly ADPropertyDefinition TransportSyncLogMaxAge = ServerSchema.TransportSyncMailboxLogMaxAge;

		public static readonly ADPropertyDefinition TransportSyncLogMaxDirectorySize = ServerSchema.TransportSyncMailboxLogMaxDirectorySize;

		public static readonly ADPropertyDefinition TransportSyncLogMaxFileSize = ServerSchema.TransportSyncMailboxLogMaxFileSize;

		public static readonly ADPropertyDefinition TransportSyncMailboxHealthLogEnabled = ActiveDirectoryServerSchema.TransportSyncMailboxHealthLogEnabled;

		public static readonly ADPropertyDefinition TransportSyncMailboxHealthLogFilePath = ActiveDirectoryServerSchema.TransportSyncMailboxHealthLogFilePath;

		public static readonly ADPropertyDefinition TransportSyncMailboxHealthLogMaxAge = ActiveDirectoryServerSchema.TransportSyncMailboxHealthLogMaxAge;

		public static readonly ADPropertyDefinition TransportSyncMailboxHealthLogMaxDirectorySize = ActiveDirectoryServerSchema.TransportSyncMailboxHealthLogMaxDirectorySize;

		public static readonly ADPropertyDefinition TransportSyncMailboxHealthLogMaxFileSize = ActiveDirectoryServerSchema.TransportSyncMailboxHealthLogMaxFileSize;

		public static readonly ADPropertyDefinition DatabaseCopyAutoActivationPolicy = ActiveDirectoryServerSchema.DatabaseCopyAutoActivationPolicy;

		public static readonly ADPropertyDefinition DatabaseCopyActivationDisabledAndMoveNow = ActiveDirectoryServerSchema.DatabaseCopyActivationDisabledAndMoveNow;

		public static readonly ADPropertyDefinition AutoDagServerConfigured = ActiveDirectoryServerSchema.AutoDagServerConfigured;

		public static readonly ADPropertyDefinition FaultZone = ActiveDirectoryServerSchema.FaultZone;

		public static readonly ADPropertyDefinition AutoDagFlags = ActiveDirectoryServerSchema.AutoDagFlags;

		public static readonly ADPropertyDefinition IsExcludedFromProvisioning = ActiveDirectoryServerSchema.IsExcludedFromProvisioning;

		public static readonly ADPropertyDefinition MaxActiveMailboxDatabases = ServerSchema.MaxActiveMailboxDatabases;

		public static readonly ADPropertyDefinition MaxPreferredActiveDatabases = ServerSchema.MaxPreferredActiveDatabases;

		public static readonly ADPropertyDefinition ComponentStates = ServerSchema.ComponentStates;

		public static readonly ADPropertyDefinition AdminDisplayVersion = ServerSchema.AdminDisplayVersion;

		public static readonly ADPropertyDefinition CurrentServerRole = ServerSchema.CurrentServerRole;

		public static readonly ADPropertyDefinition ExchangeLegacyServerRole = ServerSchema.ExchangeLegacyServerRole;

		public static readonly ADPropertyDefinition IsE15OrLater = ServerSchema.IsE15OrLater;
	}
}
