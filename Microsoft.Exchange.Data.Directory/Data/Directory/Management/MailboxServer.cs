using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public class MailboxServer : ADPresentationObject
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return MailboxServer.schema;
			}
		}

		public MailboxServer()
		{
		}

		public MailboxServer(Server dataObject) : base(dataObject)
		{
		}

		public new string Name
		{
			get
			{
				return (string)this[ADObjectSchema.Name];
			}
		}

		public LocalLongFullPath DataPath
		{
			get
			{
				return (LocalLongFullPath)this[MailboxServerSchema.DataPath];
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? CalendarRepairWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.CalendarRepairWorkCycle];
			}
			set
			{
				this[MailboxServerSchema.CalendarRepairWorkCycle] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? CalendarRepairWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.CalendarRepairWorkCycleCheckpoint];
			}
			set
			{
				this[MailboxServerSchema.CalendarRepairWorkCycleCheckpoint] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? SharingPolicyWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.SharingPolicyWorkCycle];
			}
			set
			{
				this[MailboxServerSchema.SharingPolicyWorkCycle] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? SharingPolicyWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.SharingPolicyWorkCycleCheckpoint];
			}
			set
			{
				this[MailboxServerSchema.SharingPolicyWorkCycleCheckpoint] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? SharingSyncWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.SharingSyncWorkCycle];
			}
			set
			{
				this[MailboxServerSchema.SharingSyncWorkCycle] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? SharingSyncWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.SharingSyncWorkCycleCheckpoint];
			}
			set
			{
				this[MailboxServerSchema.SharingSyncWorkCycleCheckpoint] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? PublicFolderWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.PublicFolderWorkCycle];
			}
			set
			{
				this[MailboxServerSchema.PublicFolderWorkCycle] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? PublicFolderWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.PublicFolderWorkCycleCheckpoint];
			}
			set
			{
				this[MailboxServerSchema.PublicFolderWorkCycleCheckpoint] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? SiteMailboxWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.SiteMailboxWorkCycle];
			}
			set
			{
				this[MailboxServerSchema.SiteMailboxWorkCycle] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? SiteMailboxWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.SiteMailboxWorkCycleCheckpoint];
			}
			set
			{
				this[MailboxServerSchema.SiteMailboxWorkCycleCheckpoint] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? ManagedFolderWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.ManagedFolderWorkCycle];
			}
			set
			{
				this[MailboxServerSchema.ManagedFolderWorkCycle] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? ManagedFolderWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.ManagedFolderWorkCycleCheckpoint];
			}
			set
			{
				this[MailboxServerSchema.ManagedFolderWorkCycleCheckpoint] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? MailboxAssociationReplicationWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.MailboxAssociationReplicationWorkCycle];
			}
			set
			{
				this[MailboxServerSchema.MailboxAssociationReplicationWorkCycle] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? MailboxAssociationReplicationWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.MailboxAssociationReplicationWorkCycleCheckpoint];
			}
			set
			{
				this[MailboxServerSchema.MailboxAssociationReplicationWorkCycleCheckpoint] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? GroupMailboxWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.GroupMailboxWorkCycle];
			}
			set
			{
				this[MailboxServerSchema.GroupMailboxWorkCycle] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? GroupMailboxWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.GroupMailboxWorkCycleCheckpoint];
			}
			set
			{
				this[MailboxServerSchema.GroupMailboxWorkCycleCheckpoint] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? TopNWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.TopNWorkCycle];
			}
			set
			{
				this[MailboxServerSchema.TopNWorkCycle] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? TopNWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.TopNWorkCycleCheckpoint];
			}
			set
			{
				this[MailboxServerSchema.TopNWorkCycleCheckpoint] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? UMReportingWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.UMReportingWorkCycle];
			}
			set
			{
				this[MailboxServerSchema.UMReportingWorkCycle] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? UMReportingWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.UMReportingWorkCycleCheckpoint];
			}
			set
			{
				this[MailboxServerSchema.UMReportingWorkCycleCheckpoint] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? InferenceTrainingWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.InferenceTrainingWorkCycle];
			}
			set
			{
				this[MailboxServerSchema.InferenceTrainingWorkCycle] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? InferenceTrainingWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.InferenceTrainingWorkCycleCheckpoint];
			}
			set
			{
				this[MailboxServerSchema.InferenceTrainingWorkCycleCheckpoint] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? DirectoryProcessorWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.DirectoryProcessorWorkCycle];
			}
			set
			{
				this[MailboxServerSchema.DirectoryProcessorWorkCycle] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? DirectoryProcessorWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.DirectoryProcessorWorkCycleCheckpoint];
			}
			set
			{
				this[MailboxServerSchema.DirectoryProcessorWorkCycleCheckpoint] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? OABGeneratorWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.OABGeneratorWorkCycle];
			}
			set
			{
				this[MailboxServerSchema.OABGeneratorWorkCycle] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? OABGeneratorWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.OABGeneratorWorkCycleCheckpoint];
			}
			set
			{
				this[MailboxServerSchema.OABGeneratorWorkCycleCheckpoint] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? InferenceDataCollectionWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.InferenceDataCollectionWorkCycle];
			}
			set
			{
				this[MailboxServerSchema.InferenceDataCollectionWorkCycle] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? InferenceDataCollectionWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.InferenceDataCollectionWorkCycleCheckpoint];
			}
			set
			{
				this[MailboxServerSchema.InferenceDataCollectionWorkCycleCheckpoint] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? PeopleRelevanceWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.PeopleRelevanceWorkCycle];
			}
			set
			{
				this[MailboxServerSchema.PeopleRelevanceWorkCycle] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? PeopleRelevanceWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.PeopleRelevanceWorkCycleCheckpoint];
			}
			set
			{
				this[MailboxServerSchema.PeopleRelevanceWorkCycleCheckpoint] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? SharePointSignalStoreWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.SharePointSignalStoreWorkCycle];
			}
			set
			{
				this[MailboxServerSchema.SharePointSignalStoreWorkCycle] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? SharePointSignalStoreWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.SharePointSignalStoreWorkCycleCheckpoint];
			}
			set
			{
				this[MailboxServerSchema.SharePointSignalStoreWorkCycleCheckpoint] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? PeopleCentricTriageWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.PeopleCentricTriageWorkCycle];
			}
			set
			{
				this[MailboxServerSchema.PeopleCentricTriageWorkCycle] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? PeopleCentricTriageWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.PeopleCentricTriageWorkCycleCheckpoint];
			}
			set
			{
				this[MailboxServerSchema.PeopleCentricTriageWorkCycleCheckpoint] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? MailboxProcessorWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.MailboxProcessorWorkCycle];
			}
			set
			{
				this[MailboxServerSchema.MailboxProcessorWorkCycle] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? StoreDsMaintenanceWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.StoreDsMaintenanceWorkCycle];
			}
			set
			{
				this[MailboxServerSchema.StoreDsMaintenanceWorkCycle] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? StoreDsMaintenanceWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.StoreDsMaintenanceWorkCycleCheckpoint];
			}
			set
			{
				this[MailboxServerSchema.StoreDsMaintenanceWorkCycleCheckpoint] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? StoreIntegrityCheckWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.StoreIntegrityCheckWorkCycle];
			}
			set
			{
				this[MailboxServerSchema.StoreIntegrityCheckWorkCycle] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? StoreIntegrityCheckWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.StoreIntegrityCheckWorkCycleCheckpoint];
			}
			set
			{
				this[MailboxServerSchema.StoreIntegrityCheckWorkCycleCheckpoint] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? StoreMaintenanceWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.StoreMaintenanceWorkCycle];
			}
			set
			{
				this[MailboxServerSchema.StoreMaintenanceWorkCycle] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? StoreMaintenanceWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.StoreMaintenanceWorkCycleCheckpoint];
			}
			set
			{
				this[MailboxServerSchema.StoreMaintenanceWorkCycleCheckpoint] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? StoreScheduledIntegrityCheckWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.StoreScheduledIntegrityCheckWorkCycle];
			}
			set
			{
				this[MailboxServerSchema.StoreScheduledIntegrityCheckWorkCycle] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? StoreScheduledIntegrityCheckWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.StoreScheduledIntegrityCheckWorkCycleCheckpoint];
			}
			set
			{
				this[MailboxServerSchema.StoreScheduledIntegrityCheckWorkCycleCheckpoint] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? StoreUrgentMaintenanceWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.StoreUrgentMaintenanceWorkCycle];
			}
			set
			{
				this[MailboxServerSchema.StoreUrgentMaintenanceWorkCycle] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? StoreUrgentMaintenanceWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.StoreUrgentMaintenanceWorkCycleCheckpoint];
			}
			set
			{
				this[MailboxServerSchema.StoreUrgentMaintenanceWorkCycleCheckpoint] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? JunkEmailOptionsCommitterWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.JunkEmailOptionsCommitterWorkCycle];
			}
			set
			{
				this[MailboxServerSchema.JunkEmailOptionsCommitterWorkCycle] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? ProbeTimeBasedAssistantWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.ProbeTimeBasedAssistantWorkCycle];
			}
			set
			{
				this[MailboxServerSchema.ProbeTimeBasedAssistantWorkCycle] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? ProbeTimeBasedAssistantWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.ProbeTimeBasedAssistantWorkCycleCheckpoint];
			}
			set
			{
				this[MailboxServerSchema.ProbeTimeBasedAssistantWorkCycleCheckpoint] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? SearchIndexRepairTimeBasedAssistantWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.SearchIndexRepairTimeBasedAssistantWorkCycle];
			}
			set
			{
				this[MailboxServerSchema.SearchIndexRepairTimeBasedAssistantWorkCycle] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? SearchIndexRepairTimeBasedAssistantWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.SearchIndexRepairTimeBasedAssistantWorkCycleCheckpoint];
			}
			set
			{
				this[MailboxServerSchema.SearchIndexRepairTimeBasedAssistantWorkCycleCheckpoint] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? DarTaskStoreTimeBasedAssistantWorkCycle
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.DarTaskStoreTimeBasedAssistantWorkCycle];
			}
			set
			{
				this[MailboxServerSchema.DarTaskStoreTimeBasedAssistantWorkCycle] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan? DarTaskStoreTimeBasedAssistantWorkCycleCheckpoint
		{
			get
			{
				return (EnhancedTimeSpan?)this[MailboxServerSchema.DarTaskStoreTimeBasedAssistantWorkCycleCheckpoint];
			}
			set
			{
				this[MailboxServerSchema.DarTaskStoreTimeBasedAssistantWorkCycleCheckpoint] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ScheduleInterval[] SharingPolicySchedule
		{
			get
			{
				return (ScheduleInterval[])this[MailboxServerSchema.SharingPolicySchedule];
			}
			set
			{
				this[MailboxServerSchema.SharingPolicySchedule] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool CalendarRepairMissingItemFixDisabled
		{
			get
			{
				return (bool)this[MailboxServerSchema.CalendarRepairMissingItemFixDisabled];
			}
			set
			{
				this[MailboxServerSchema.CalendarRepairMissingItemFixDisabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool CalendarRepairLogEnabled
		{
			get
			{
				return (bool)this[MailboxServerSchema.CalendarRepairLogEnabled];
			}
			set
			{
				this[MailboxServerSchema.CalendarRepairLogEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool CalendarRepairLogSubjectLoggingEnabled
		{
			get
			{
				return (bool)this[MailboxServerSchema.CalendarRepairLogSubjectLoggingEnabled];
			}
			set
			{
				this[MailboxServerSchema.CalendarRepairLogSubjectLoggingEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath CalendarRepairLogPath
		{
			get
			{
				return (LocalLongFullPath)this[MailboxServerSchema.CalendarRepairLogPath];
			}
			set
			{
				this[MailboxServerSchema.CalendarRepairLogPath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int CalendarRepairIntervalEndWindow
		{
			get
			{
				return (int)this[MailboxServerSchema.CalendarRepairIntervalEndWindow];
			}
			set
			{
				this[MailboxServerSchema.CalendarRepairIntervalEndWindow] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan CalendarRepairLogFileAgeLimit
		{
			get
			{
				return (EnhancedTimeSpan)this[MailboxServerSchema.CalendarRepairLogFileAgeLimit];
			}
			set
			{
				this[MailboxServerSchema.CalendarRepairLogFileAgeLimit] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> CalendarRepairLogDirectorySizeLimit
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxServerSchema.CalendarRepairLogDirectorySizeLimit];
			}
			set
			{
				this[MailboxServerSchema.CalendarRepairLogDirectorySizeLimit] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public CalendarRepairType CalendarRepairMode
		{
			get
			{
				return (CalendarRepairType)this[MailboxServerSchema.CalendarRepairMode];
			}
			set
			{
				this[MailboxServerSchema.CalendarRepairMode] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ScheduleInterval[] ManagedFolderAssistantSchedule
		{
			get
			{
				return (ScheduleInterval[])this[MailboxServerSchema.ElcSchedule];
			}
			set
			{
				this[MailboxServerSchema.ElcSchedule] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath LogPathForManagedFolders
		{
			get
			{
				return (LocalLongFullPath)this[MailboxServerSchema.ElcAuditLogPath];
			}
			set
			{
				this[MailboxServerSchema.ElcAuditLogPath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan LogFileAgeLimitForManagedFolders
		{
			get
			{
				return (EnhancedTimeSpan)this[MailboxServerSchema.ElcAuditLogFileAgeLimit];
			}
			set
			{
				this[MailboxServerSchema.ElcAuditLogFileAgeLimit] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> LogDirectorySizeLimitForManagedFolders
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxServerSchema.ElcAuditLogDirectorySizeLimit];
			}
			set
			{
				this[MailboxServerSchema.ElcAuditLogDirectorySizeLimit] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<ByteQuantifiedSize> LogFileSizeLimitForManagedFolders
		{
			get
			{
				return (Unlimited<ByteQuantifiedSize>)this[MailboxServerSchema.ElcAuditLogFileSizeLimit];
			}
			set
			{
				this[MailboxServerSchema.ElcAuditLogFileSizeLimit] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MigrationEventType MigrationLogLoggingLevel
		{
			get
			{
				return (MigrationEventType)this[MailboxServerSchema.MigrationLogLoggingLevel];
			}
			set
			{
				this[MailboxServerSchema.MigrationLogLoggingLevel] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath MigrationLogFilePath
		{
			get
			{
				return (LocalLongFullPath)this[MailboxServerSchema.MigrationLogFilePath];
			}
			set
			{
				this[MailboxServerSchema.MigrationLogFilePath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan MigrationLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[MailboxServerSchema.MigrationLogMaxAge];
			}
			set
			{
				this[MailboxServerSchema.MigrationLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize MigrationLogMaxDirectorySize
		{
			get
			{
				return (ByteQuantifiedSize)this[MailboxServerSchema.MigrationLogMaxDirectorySize];
			}
			set
			{
				this[MailboxServerSchema.MigrationLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize MigrationLogMaxFileSize
		{
			get
			{
				return (ByteQuantifiedSize)this[MailboxServerSchema.MigrationLogMaxFileSize];
			}
			set
			{
				this[MailboxServerSchema.MigrationLogMaxFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MAPIEncryptionRequired
		{
			get
			{
				return (bool)this[MailboxServerSchema.MAPIEncryptionRequired];
			}
			set
			{
				this[MailboxServerSchema.MAPIEncryptionRequired] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool RetentionLogForManagedFoldersEnabled
		{
			get
			{
				return (bool)this[MailboxServerSchema.ExpirationAuditLogEnabled];
			}
			set
			{
				this[MailboxServerSchema.ExpirationAuditLogEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool JournalingLogForManagedFoldersEnabled
		{
			get
			{
				return (bool)this[MailboxServerSchema.AutocopyAuditLogEnabled];
			}
			set
			{
				this[MailboxServerSchema.AutocopyAuditLogEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool FolderLogForManagedFoldersEnabled
		{
			get
			{
				return (bool)this[MailboxServerSchema.FolderAuditLogEnabled];
			}
			set
			{
				this[MailboxServerSchema.FolderAuditLogEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool SubjectLogForManagedFoldersEnabled
		{
			get
			{
				return (bool)this[MailboxServerSchema.ElcSubjectLoggingEnabled];
			}
			set
			{
				this[MailboxServerSchema.ElcSubjectLoggingEnabled] = value;
			}
		}

		public MultiValuedProperty<ADObjectId> SubmissionServerOverrideList
		{
			get
			{
				return (MultiValuedProperty<ADObjectId>)this[ServerSchema.SubmissionServerOverrideList];
			}
			set
			{
				this[ServerSchema.SubmissionServerOverrideList] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public AutoDatabaseMountDial AutoDatabaseMountDial
		{
			get
			{
				return (AutoDatabaseMountDial)this[MailboxServerSchema.AutoDatabaseMountDialType];
			}
			set
			{
				this[MailboxServerSchema.AutoDatabaseMountDialType] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool ForceGroupMetricsGeneration
		{
			get
			{
				return (bool)this[MailboxServerSchema.ForceGroupMetricsGeneration];
			}
			set
			{
				this[MailboxServerSchema.ForceGroupMetricsGeneration] = value;
			}
		}

		public bool IsPhoneticSupportEnabled
		{
			get
			{
				return (bool)this[MailboxServerSchema.IsPhoneticSupportEnabled];
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<CultureInfo> Locale
		{
			get
			{
				return (MultiValuedProperty<CultureInfo>)this[MailboxServerSchema.Locale];
			}
			set
			{
				this[MailboxServerSchema.Locale] = value;
			}
		}

		public ADObjectId DatabaseAvailabilityGroup
		{
			get
			{
				return (ADObjectId)this[MailboxServerSchema.DatabaseAvailabilityGroup];
			}
		}

		[Parameter(Mandatory = false)]
		public DatabaseCopyAutoActivationPolicyType DatabaseCopyAutoActivationPolicy
		{
			get
			{
				return (DatabaseCopyAutoActivationPolicyType)this[MailboxServerSchema.DatabaseCopyAutoActivationPolicy];
			}
			set
			{
				this[MailboxServerSchema.DatabaseCopyAutoActivationPolicy] = (int)value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool DatabaseCopyActivationDisabledAndMoveNow
		{
			get
			{
				return (bool)this[MailboxServerSchema.DatabaseCopyActivationDisabledAndMoveNow];
			}
			set
			{
				this[MailboxServerSchema.DatabaseCopyActivationDisabledAndMoveNow] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string FaultZone
		{
			get
			{
				return (string)this[MailboxServerSchema.FaultZone];
			}
			set
			{
				this[MailboxServerSchema.FaultZone] = value;
			}
		}

		internal ServerAutoDagFlags AutoDagFlags
		{
			get
			{
				return (ServerAutoDagFlags)this[ActiveDirectoryServerSchema.AutoDagFlags];
			}
			set
			{
				this[ActiveDirectoryServerSchema.AutoDagFlags] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool AutoDagServerConfigured
		{
			get
			{
				return (bool)this[MailboxServerSchema.AutoDagServerConfigured];
			}
			set
			{
				this[MailboxServerSchema.AutoDagServerConfigured] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TransportSyncDispatchEnabled
		{
			get
			{
				return (bool)this[MailboxServerSchema.TransportSyncDispatchEnabled];
			}
			set
			{
				this[MailboxServerSchema.TransportSyncDispatchEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int MaxTransportSyncDispatchers
		{
			get
			{
				return (int)this[MailboxServerSchema.MaxTransportSyncDispatchers];
			}
			set
			{
				this[MailboxServerSchema.MaxTransportSyncDispatchers] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TransportSyncLogEnabled
		{
			get
			{
				return (bool)this[MailboxServerSchema.TransportSyncLogEnabled];
			}
			set
			{
				this[MailboxServerSchema.TransportSyncLogEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SyncLoggingLevel TransportSyncLogLoggingLevel
		{
			get
			{
				return (SyncLoggingLevel)this[MailboxServerSchema.TransportSyncLogLoggingLevel];
			}
			set
			{
				this[MailboxServerSchema.TransportSyncLogLoggingLevel] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath TransportSyncLogFilePath
		{
			get
			{
				return (LocalLongFullPath)this[MailboxServerSchema.TransportSyncLogFilePath];
			}
			set
			{
				this[MailboxServerSchema.TransportSyncLogFilePath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan TransportSyncLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[MailboxServerSchema.TransportSyncLogMaxAge];
			}
			set
			{
				this[MailboxServerSchema.TransportSyncLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize TransportSyncLogMaxDirectorySize
		{
			get
			{
				return (ByteQuantifiedSize)this[MailboxServerSchema.TransportSyncLogMaxDirectorySize];
			}
			set
			{
				this[MailboxServerSchema.TransportSyncLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize TransportSyncLogMaxFileSize
		{
			get
			{
				return (ByteQuantifiedSize)this[MailboxServerSchema.TransportSyncLogMaxFileSize];
			}
			set
			{
				this[MailboxServerSchema.TransportSyncLogMaxFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool TransportSyncMailboxHealthLogEnabled
		{
			get
			{
				return (bool)this[MailboxServerSchema.TransportSyncMailboxHealthLogEnabled];
			}
			set
			{
				this[MailboxServerSchema.TransportSyncMailboxHealthLogEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public LocalLongFullPath TransportSyncMailboxHealthLogFilePath
		{
			get
			{
				return (LocalLongFullPath)this[MailboxServerSchema.TransportSyncMailboxHealthLogFilePath];
			}
			set
			{
				this[MailboxServerSchema.TransportSyncMailboxHealthLogFilePath] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan TransportSyncMailboxHealthLogMaxAge
		{
			get
			{
				return (EnhancedTimeSpan)this[MailboxServerSchema.TransportSyncMailboxHealthLogMaxAge];
			}
			set
			{
				this[MailboxServerSchema.TransportSyncMailboxHealthLogMaxAge] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize TransportSyncMailboxHealthLogMaxDirectorySize
		{
			get
			{
				return (ByteQuantifiedSize)this[MailboxServerSchema.TransportSyncMailboxHealthLogMaxDirectorySize];
			}
			set
			{
				this[MailboxServerSchema.TransportSyncMailboxHealthLogMaxDirectorySize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public ByteQuantifiedSize TransportSyncMailboxHealthLogMaxFileSize
		{
			get
			{
				return (ByteQuantifiedSize)this[MailboxServerSchema.TransportSyncMailboxHealthLogMaxFileSize];
			}
			set
			{
				this[MailboxServerSchema.TransportSyncMailboxHealthLogMaxFileSize] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsExcludedFromProvisioning
		{
			get
			{
				return (bool)this[MailboxServerSchema.IsExcludedFromProvisioning];
			}
			set
			{
				this[MailboxServerSchema.IsExcludedFromProvisioning] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? MaximumActiveDatabases
		{
			get
			{
				return (int?)this[MailboxServerSchema.MaxActiveMailboxDatabases];
			}
			set
			{
				this[MailboxServerSchema.MaxActiveMailboxDatabases] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public int? MaximumPreferredActiveDatabases
		{
			get
			{
				return (int?)this[MailboxServerSchema.MaxPreferredActiveDatabases];
			}
			set
			{
				this[MailboxServerSchema.MaxPreferredActiveDatabases] = value;
			}
		}

		public ServerVersion AdminDisplayVersion
		{
			get
			{
				return (ServerVersion)this[ExchangeServerSchema.AdminDisplayVersion];
			}
		}

		public ServerRole ServerRole
		{
			get
			{
				ServerRole serverRole = (ServerRole)this[ExchangeServerSchema.CurrentServerRole];
				if (!this.IsE15OrLater)
				{
					return serverRole;
				}
				return ExchangeServer.ConvertE15ServerRoleToOutput(serverRole);
			}
		}

		public int ExchangeLegacyServerRole
		{
			get
			{
				return (int)this[ExchangeServerSchema.ExchangeLegacyServerRole];
			}
		}

		private bool IsE15OrLater
		{
			get
			{
				return (bool)this[ExchangeServerSchema.IsE15OrLater];
			}
		}

		private static MailboxServerSchema schema = ObjectSchema.GetInstance<MailboxServerSchema>();
	}
}
