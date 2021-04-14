using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetMailboxServerCommand : SyntheticCommandWithPipelineInputNoOutput<MailboxServer>
	{
		private SetMailboxServerCommand() : base("Set-MailboxServer")
		{
		}

		public SetMailboxServerCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetMailboxServerCommand SetParameters(SetMailboxServerCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetMailboxServerCommand SetParameters(SetMailboxServerCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual MultiValuedProperty<ServerIdParameter> SubmissionServerOverrideList
			{
				set
				{
					base.PowerSharpParameters["SubmissionServerOverrideList"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual EnhancedTimeSpan? CalendarRepairWorkCycle
			{
				set
				{
					base.PowerSharpParameters["CalendarRepairWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? CalendarRepairWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["CalendarRepairWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? SharingPolicyWorkCycle
			{
				set
				{
					base.PowerSharpParameters["SharingPolicyWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? SharingPolicyWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["SharingPolicyWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? SharingSyncWorkCycle
			{
				set
				{
					base.PowerSharpParameters["SharingSyncWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? SharingSyncWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["SharingSyncWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? PublicFolderWorkCycle
			{
				set
				{
					base.PowerSharpParameters["PublicFolderWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? PublicFolderWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["PublicFolderWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? SiteMailboxWorkCycle
			{
				set
				{
					base.PowerSharpParameters["SiteMailboxWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? SiteMailboxWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["SiteMailboxWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? ManagedFolderWorkCycle
			{
				set
				{
					base.PowerSharpParameters["ManagedFolderWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? ManagedFolderWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["ManagedFolderWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? MailboxAssociationReplicationWorkCycle
			{
				set
				{
					base.PowerSharpParameters["MailboxAssociationReplicationWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? MailboxAssociationReplicationWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["MailboxAssociationReplicationWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? GroupMailboxWorkCycle
			{
				set
				{
					base.PowerSharpParameters["GroupMailboxWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? GroupMailboxWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["GroupMailboxWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? TopNWorkCycle
			{
				set
				{
					base.PowerSharpParameters["TopNWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? TopNWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["TopNWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? UMReportingWorkCycle
			{
				set
				{
					base.PowerSharpParameters["UMReportingWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? UMReportingWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["UMReportingWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? InferenceTrainingWorkCycle
			{
				set
				{
					base.PowerSharpParameters["InferenceTrainingWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? InferenceTrainingWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["InferenceTrainingWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? DirectoryProcessorWorkCycle
			{
				set
				{
					base.PowerSharpParameters["DirectoryProcessorWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? DirectoryProcessorWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["DirectoryProcessorWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? OABGeneratorWorkCycle
			{
				set
				{
					base.PowerSharpParameters["OABGeneratorWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? OABGeneratorWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["OABGeneratorWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? InferenceDataCollectionWorkCycle
			{
				set
				{
					base.PowerSharpParameters["InferenceDataCollectionWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? InferenceDataCollectionWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["InferenceDataCollectionWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? PeopleRelevanceWorkCycle
			{
				set
				{
					base.PowerSharpParameters["PeopleRelevanceWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? PeopleRelevanceWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["PeopleRelevanceWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? SharePointSignalStoreWorkCycle
			{
				set
				{
					base.PowerSharpParameters["SharePointSignalStoreWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? SharePointSignalStoreWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["SharePointSignalStoreWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? PeopleCentricTriageWorkCycle
			{
				set
				{
					base.PowerSharpParameters["PeopleCentricTriageWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? PeopleCentricTriageWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["PeopleCentricTriageWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? MailboxProcessorWorkCycle
			{
				set
				{
					base.PowerSharpParameters["MailboxProcessorWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? StoreDsMaintenanceWorkCycle
			{
				set
				{
					base.PowerSharpParameters["StoreDsMaintenanceWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? StoreDsMaintenanceWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["StoreDsMaintenanceWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? StoreIntegrityCheckWorkCycle
			{
				set
				{
					base.PowerSharpParameters["StoreIntegrityCheckWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? StoreIntegrityCheckWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["StoreIntegrityCheckWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? StoreMaintenanceWorkCycle
			{
				set
				{
					base.PowerSharpParameters["StoreMaintenanceWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? StoreMaintenanceWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["StoreMaintenanceWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? StoreScheduledIntegrityCheckWorkCycle
			{
				set
				{
					base.PowerSharpParameters["StoreScheduledIntegrityCheckWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? StoreScheduledIntegrityCheckWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["StoreScheduledIntegrityCheckWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? StoreUrgentMaintenanceWorkCycle
			{
				set
				{
					base.PowerSharpParameters["StoreUrgentMaintenanceWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? StoreUrgentMaintenanceWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["StoreUrgentMaintenanceWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? JunkEmailOptionsCommitterWorkCycle
			{
				set
				{
					base.PowerSharpParameters["JunkEmailOptionsCommitterWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? ProbeTimeBasedAssistantWorkCycle
			{
				set
				{
					base.PowerSharpParameters["ProbeTimeBasedAssistantWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? ProbeTimeBasedAssistantWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["ProbeTimeBasedAssistantWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? SearchIndexRepairTimeBasedAssistantWorkCycle
			{
				set
				{
					base.PowerSharpParameters["SearchIndexRepairTimeBasedAssistantWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? SearchIndexRepairTimeBasedAssistantWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["SearchIndexRepairTimeBasedAssistantWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? DarTaskStoreTimeBasedAssistantWorkCycle
			{
				set
				{
					base.PowerSharpParameters["DarTaskStoreTimeBasedAssistantWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? DarTaskStoreTimeBasedAssistantWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["DarTaskStoreTimeBasedAssistantWorkCycleCheckpoint"] = value;
				}
			}

			public virtual ScheduleInterval SharingPolicySchedule
			{
				set
				{
					base.PowerSharpParameters["SharingPolicySchedule"] = value;
				}
			}

			public virtual bool CalendarRepairMissingItemFixDisabled
			{
				set
				{
					base.PowerSharpParameters["CalendarRepairMissingItemFixDisabled"] = value;
				}
			}

			public virtual bool CalendarRepairLogEnabled
			{
				set
				{
					base.PowerSharpParameters["CalendarRepairLogEnabled"] = value;
				}
			}

			public virtual bool CalendarRepairLogSubjectLoggingEnabled
			{
				set
				{
					base.PowerSharpParameters["CalendarRepairLogSubjectLoggingEnabled"] = value;
				}
			}

			public virtual LocalLongFullPath CalendarRepairLogPath
			{
				set
				{
					base.PowerSharpParameters["CalendarRepairLogPath"] = value;
				}
			}

			public virtual int CalendarRepairIntervalEndWindow
			{
				set
				{
					base.PowerSharpParameters["CalendarRepairIntervalEndWindow"] = value;
				}
			}

			public virtual EnhancedTimeSpan CalendarRepairLogFileAgeLimit
			{
				set
				{
					base.PowerSharpParameters["CalendarRepairLogFileAgeLimit"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> CalendarRepairLogDirectorySizeLimit
			{
				set
				{
					base.PowerSharpParameters["CalendarRepairLogDirectorySizeLimit"] = value;
				}
			}

			public virtual CalendarRepairType CalendarRepairMode
			{
				set
				{
					base.PowerSharpParameters["CalendarRepairMode"] = value;
				}
			}

			public virtual ScheduleInterval ManagedFolderAssistantSchedule
			{
				set
				{
					base.PowerSharpParameters["ManagedFolderAssistantSchedule"] = value;
				}
			}

			public virtual LocalLongFullPath LogPathForManagedFolders
			{
				set
				{
					base.PowerSharpParameters["LogPathForManagedFolders"] = value;
				}
			}

			public virtual EnhancedTimeSpan LogFileAgeLimitForManagedFolders
			{
				set
				{
					base.PowerSharpParameters["LogFileAgeLimitForManagedFolders"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> LogDirectorySizeLimitForManagedFolders
			{
				set
				{
					base.PowerSharpParameters["LogDirectorySizeLimitForManagedFolders"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> LogFileSizeLimitForManagedFolders
			{
				set
				{
					base.PowerSharpParameters["LogFileSizeLimitForManagedFolders"] = value;
				}
			}

			public virtual MigrationEventType MigrationLogLoggingLevel
			{
				set
				{
					base.PowerSharpParameters["MigrationLogLoggingLevel"] = value;
				}
			}

			public virtual LocalLongFullPath MigrationLogFilePath
			{
				set
				{
					base.PowerSharpParameters["MigrationLogFilePath"] = value;
				}
			}

			public virtual EnhancedTimeSpan MigrationLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["MigrationLogMaxAge"] = value;
				}
			}

			public virtual ByteQuantifiedSize MigrationLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["MigrationLogMaxDirectorySize"] = value;
				}
			}

			public virtual ByteQuantifiedSize MigrationLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["MigrationLogMaxFileSize"] = value;
				}
			}

			public virtual bool MAPIEncryptionRequired
			{
				set
				{
					base.PowerSharpParameters["MAPIEncryptionRequired"] = value;
				}
			}

			public virtual bool RetentionLogForManagedFoldersEnabled
			{
				set
				{
					base.PowerSharpParameters["RetentionLogForManagedFoldersEnabled"] = value;
				}
			}

			public virtual bool JournalingLogForManagedFoldersEnabled
			{
				set
				{
					base.PowerSharpParameters["JournalingLogForManagedFoldersEnabled"] = value;
				}
			}

			public virtual bool FolderLogForManagedFoldersEnabled
			{
				set
				{
					base.PowerSharpParameters["FolderLogForManagedFoldersEnabled"] = value;
				}
			}

			public virtual bool SubjectLogForManagedFoldersEnabled
			{
				set
				{
					base.PowerSharpParameters["SubjectLogForManagedFoldersEnabled"] = value;
				}
			}

			public virtual AutoDatabaseMountDial AutoDatabaseMountDial
			{
				set
				{
					base.PowerSharpParameters["AutoDatabaseMountDial"] = value;
				}
			}

			public virtual bool ForceGroupMetricsGeneration
			{
				set
				{
					base.PowerSharpParameters["ForceGroupMetricsGeneration"] = value;
				}
			}

			public virtual MultiValuedProperty<CultureInfo> Locale
			{
				set
				{
					base.PowerSharpParameters["Locale"] = value;
				}
			}

			public virtual DatabaseCopyAutoActivationPolicyType DatabaseCopyAutoActivationPolicy
			{
				set
				{
					base.PowerSharpParameters["DatabaseCopyAutoActivationPolicy"] = value;
				}
			}

			public virtual bool DatabaseCopyActivationDisabledAndMoveNow
			{
				set
				{
					base.PowerSharpParameters["DatabaseCopyActivationDisabledAndMoveNow"] = value;
				}
			}

			public virtual string FaultZone
			{
				set
				{
					base.PowerSharpParameters["FaultZone"] = value;
				}
			}

			public virtual bool AutoDagServerConfigured
			{
				set
				{
					base.PowerSharpParameters["AutoDagServerConfigured"] = value;
				}
			}

			public virtual bool TransportSyncDispatchEnabled
			{
				set
				{
					base.PowerSharpParameters["TransportSyncDispatchEnabled"] = value;
				}
			}

			public virtual int MaxTransportSyncDispatchers
			{
				set
				{
					base.PowerSharpParameters["MaxTransportSyncDispatchers"] = value;
				}
			}

			public virtual bool TransportSyncLogEnabled
			{
				set
				{
					base.PowerSharpParameters["TransportSyncLogEnabled"] = value;
				}
			}

			public virtual SyncLoggingLevel TransportSyncLogLoggingLevel
			{
				set
				{
					base.PowerSharpParameters["TransportSyncLogLoggingLevel"] = value;
				}
			}

			public virtual LocalLongFullPath TransportSyncLogFilePath
			{
				set
				{
					base.PowerSharpParameters["TransportSyncLogFilePath"] = value;
				}
			}

			public virtual EnhancedTimeSpan TransportSyncLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["TransportSyncLogMaxAge"] = value;
				}
			}

			public virtual ByteQuantifiedSize TransportSyncLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["TransportSyncLogMaxDirectorySize"] = value;
				}
			}

			public virtual ByteQuantifiedSize TransportSyncLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["TransportSyncLogMaxFileSize"] = value;
				}
			}

			public virtual bool TransportSyncMailboxHealthLogEnabled
			{
				set
				{
					base.PowerSharpParameters["TransportSyncMailboxHealthLogEnabled"] = value;
				}
			}

			public virtual LocalLongFullPath TransportSyncMailboxHealthLogFilePath
			{
				set
				{
					base.PowerSharpParameters["TransportSyncMailboxHealthLogFilePath"] = value;
				}
			}

			public virtual EnhancedTimeSpan TransportSyncMailboxHealthLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["TransportSyncMailboxHealthLogMaxAge"] = value;
				}
			}

			public virtual ByteQuantifiedSize TransportSyncMailboxHealthLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["TransportSyncMailboxHealthLogMaxDirectorySize"] = value;
				}
			}

			public virtual ByteQuantifiedSize TransportSyncMailboxHealthLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["TransportSyncMailboxHealthLogMaxFileSize"] = value;
				}
			}

			public virtual bool IsExcludedFromProvisioning
			{
				set
				{
					base.PowerSharpParameters["IsExcludedFromProvisioning"] = value;
				}
			}

			public virtual int? MaximumActiveDatabases
			{
				set
				{
					base.PowerSharpParameters["MaximumActiveDatabases"] = value;
				}
			}

			public virtual int? MaximumPreferredActiveDatabases
			{
				set
				{
					base.PowerSharpParameters["MaximumPreferredActiveDatabases"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}

		public class IdentityParameters : ParametersBase
		{
			public virtual MailboxServerIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual MultiValuedProperty<ServerIdParameter> SubmissionServerOverrideList
			{
				set
				{
					base.PowerSharpParameters["SubmissionServerOverrideList"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual EnhancedTimeSpan? CalendarRepairWorkCycle
			{
				set
				{
					base.PowerSharpParameters["CalendarRepairWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? CalendarRepairWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["CalendarRepairWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? SharingPolicyWorkCycle
			{
				set
				{
					base.PowerSharpParameters["SharingPolicyWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? SharingPolicyWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["SharingPolicyWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? SharingSyncWorkCycle
			{
				set
				{
					base.PowerSharpParameters["SharingSyncWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? SharingSyncWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["SharingSyncWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? PublicFolderWorkCycle
			{
				set
				{
					base.PowerSharpParameters["PublicFolderWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? PublicFolderWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["PublicFolderWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? SiteMailboxWorkCycle
			{
				set
				{
					base.PowerSharpParameters["SiteMailboxWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? SiteMailboxWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["SiteMailboxWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? ManagedFolderWorkCycle
			{
				set
				{
					base.PowerSharpParameters["ManagedFolderWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? ManagedFolderWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["ManagedFolderWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? MailboxAssociationReplicationWorkCycle
			{
				set
				{
					base.PowerSharpParameters["MailboxAssociationReplicationWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? MailboxAssociationReplicationWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["MailboxAssociationReplicationWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? GroupMailboxWorkCycle
			{
				set
				{
					base.PowerSharpParameters["GroupMailboxWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? GroupMailboxWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["GroupMailboxWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? TopNWorkCycle
			{
				set
				{
					base.PowerSharpParameters["TopNWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? TopNWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["TopNWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? UMReportingWorkCycle
			{
				set
				{
					base.PowerSharpParameters["UMReportingWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? UMReportingWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["UMReportingWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? InferenceTrainingWorkCycle
			{
				set
				{
					base.PowerSharpParameters["InferenceTrainingWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? InferenceTrainingWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["InferenceTrainingWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? DirectoryProcessorWorkCycle
			{
				set
				{
					base.PowerSharpParameters["DirectoryProcessorWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? DirectoryProcessorWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["DirectoryProcessorWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? OABGeneratorWorkCycle
			{
				set
				{
					base.PowerSharpParameters["OABGeneratorWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? OABGeneratorWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["OABGeneratorWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? InferenceDataCollectionWorkCycle
			{
				set
				{
					base.PowerSharpParameters["InferenceDataCollectionWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? InferenceDataCollectionWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["InferenceDataCollectionWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? PeopleRelevanceWorkCycle
			{
				set
				{
					base.PowerSharpParameters["PeopleRelevanceWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? PeopleRelevanceWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["PeopleRelevanceWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? SharePointSignalStoreWorkCycle
			{
				set
				{
					base.PowerSharpParameters["SharePointSignalStoreWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? SharePointSignalStoreWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["SharePointSignalStoreWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? PeopleCentricTriageWorkCycle
			{
				set
				{
					base.PowerSharpParameters["PeopleCentricTriageWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? PeopleCentricTriageWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["PeopleCentricTriageWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? MailboxProcessorWorkCycle
			{
				set
				{
					base.PowerSharpParameters["MailboxProcessorWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? StoreDsMaintenanceWorkCycle
			{
				set
				{
					base.PowerSharpParameters["StoreDsMaintenanceWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? StoreDsMaintenanceWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["StoreDsMaintenanceWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? StoreIntegrityCheckWorkCycle
			{
				set
				{
					base.PowerSharpParameters["StoreIntegrityCheckWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? StoreIntegrityCheckWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["StoreIntegrityCheckWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? StoreMaintenanceWorkCycle
			{
				set
				{
					base.PowerSharpParameters["StoreMaintenanceWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? StoreMaintenanceWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["StoreMaintenanceWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? StoreScheduledIntegrityCheckWorkCycle
			{
				set
				{
					base.PowerSharpParameters["StoreScheduledIntegrityCheckWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? StoreScheduledIntegrityCheckWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["StoreScheduledIntegrityCheckWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? StoreUrgentMaintenanceWorkCycle
			{
				set
				{
					base.PowerSharpParameters["StoreUrgentMaintenanceWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? StoreUrgentMaintenanceWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["StoreUrgentMaintenanceWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? JunkEmailOptionsCommitterWorkCycle
			{
				set
				{
					base.PowerSharpParameters["JunkEmailOptionsCommitterWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? ProbeTimeBasedAssistantWorkCycle
			{
				set
				{
					base.PowerSharpParameters["ProbeTimeBasedAssistantWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? ProbeTimeBasedAssistantWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["ProbeTimeBasedAssistantWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? SearchIndexRepairTimeBasedAssistantWorkCycle
			{
				set
				{
					base.PowerSharpParameters["SearchIndexRepairTimeBasedAssistantWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? SearchIndexRepairTimeBasedAssistantWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["SearchIndexRepairTimeBasedAssistantWorkCycleCheckpoint"] = value;
				}
			}

			public virtual EnhancedTimeSpan? DarTaskStoreTimeBasedAssistantWorkCycle
			{
				set
				{
					base.PowerSharpParameters["DarTaskStoreTimeBasedAssistantWorkCycle"] = value;
				}
			}

			public virtual EnhancedTimeSpan? DarTaskStoreTimeBasedAssistantWorkCycleCheckpoint
			{
				set
				{
					base.PowerSharpParameters["DarTaskStoreTimeBasedAssistantWorkCycleCheckpoint"] = value;
				}
			}

			public virtual ScheduleInterval SharingPolicySchedule
			{
				set
				{
					base.PowerSharpParameters["SharingPolicySchedule"] = value;
				}
			}

			public virtual bool CalendarRepairMissingItemFixDisabled
			{
				set
				{
					base.PowerSharpParameters["CalendarRepairMissingItemFixDisabled"] = value;
				}
			}

			public virtual bool CalendarRepairLogEnabled
			{
				set
				{
					base.PowerSharpParameters["CalendarRepairLogEnabled"] = value;
				}
			}

			public virtual bool CalendarRepairLogSubjectLoggingEnabled
			{
				set
				{
					base.PowerSharpParameters["CalendarRepairLogSubjectLoggingEnabled"] = value;
				}
			}

			public virtual LocalLongFullPath CalendarRepairLogPath
			{
				set
				{
					base.PowerSharpParameters["CalendarRepairLogPath"] = value;
				}
			}

			public virtual int CalendarRepairIntervalEndWindow
			{
				set
				{
					base.PowerSharpParameters["CalendarRepairIntervalEndWindow"] = value;
				}
			}

			public virtual EnhancedTimeSpan CalendarRepairLogFileAgeLimit
			{
				set
				{
					base.PowerSharpParameters["CalendarRepairLogFileAgeLimit"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> CalendarRepairLogDirectorySizeLimit
			{
				set
				{
					base.PowerSharpParameters["CalendarRepairLogDirectorySizeLimit"] = value;
				}
			}

			public virtual CalendarRepairType CalendarRepairMode
			{
				set
				{
					base.PowerSharpParameters["CalendarRepairMode"] = value;
				}
			}

			public virtual ScheduleInterval ManagedFolderAssistantSchedule
			{
				set
				{
					base.PowerSharpParameters["ManagedFolderAssistantSchedule"] = value;
				}
			}

			public virtual LocalLongFullPath LogPathForManagedFolders
			{
				set
				{
					base.PowerSharpParameters["LogPathForManagedFolders"] = value;
				}
			}

			public virtual EnhancedTimeSpan LogFileAgeLimitForManagedFolders
			{
				set
				{
					base.PowerSharpParameters["LogFileAgeLimitForManagedFolders"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> LogDirectorySizeLimitForManagedFolders
			{
				set
				{
					base.PowerSharpParameters["LogDirectorySizeLimitForManagedFolders"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize> LogFileSizeLimitForManagedFolders
			{
				set
				{
					base.PowerSharpParameters["LogFileSizeLimitForManagedFolders"] = value;
				}
			}

			public virtual MigrationEventType MigrationLogLoggingLevel
			{
				set
				{
					base.PowerSharpParameters["MigrationLogLoggingLevel"] = value;
				}
			}

			public virtual LocalLongFullPath MigrationLogFilePath
			{
				set
				{
					base.PowerSharpParameters["MigrationLogFilePath"] = value;
				}
			}

			public virtual EnhancedTimeSpan MigrationLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["MigrationLogMaxAge"] = value;
				}
			}

			public virtual ByteQuantifiedSize MigrationLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["MigrationLogMaxDirectorySize"] = value;
				}
			}

			public virtual ByteQuantifiedSize MigrationLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["MigrationLogMaxFileSize"] = value;
				}
			}

			public virtual bool MAPIEncryptionRequired
			{
				set
				{
					base.PowerSharpParameters["MAPIEncryptionRequired"] = value;
				}
			}

			public virtual bool RetentionLogForManagedFoldersEnabled
			{
				set
				{
					base.PowerSharpParameters["RetentionLogForManagedFoldersEnabled"] = value;
				}
			}

			public virtual bool JournalingLogForManagedFoldersEnabled
			{
				set
				{
					base.PowerSharpParameters["JournalingLogForManagedFoldersEnabled"] = value;
				}
			}

			public virtual bool FolderLogForManagedFoldersEnabled
			{
				set
				{
					base.PowerSharpParameters["FolderLogForManagedFoldersEnabled"] = value;
				}
			}

			public virtual bool SubjectLogForManagedFoldersEnabled
			{
				set
				{
					base.PowerSharpParameters["SubjectLogForManagedFoldersEnabled"] = value;
				}
			}

			public virtual AutoDatabaseMountDial AutoDatabaseMountDial
			{
				set
				{
					base.PowerSharpParameters["AutoDatabaseMountDial"] = value;
				}
			}

			public virtual bool ForceGroupMetricsGeneration
			{
				set
				{
					base.PowerSharpParameters["ForceGroupMetricsGeneration"] = value;
				}
			}

			public virtual MultiValuedProperty<CultureInfo> Locale
			{
				set
				{
					base.PowerSharpParameters["Locale"] = value;
				}
			}

			public virtual DatabaseCopyAutoActivationPolicyType DatabaseCopyAutoActivationPolicy
			{
				set
				{
					base.PowerSharpParameters["DatabaseCopyAutoActivationPolicy"] = value;
				}
			}

			public virtual bool DatabaseCopyActivationDisabledAndMoveNow
			{
				set
				{
					base.PowerSharpParameters["DatabaseCopyActivationDisabledAndMoveNow"] = value;
				}
			}

			public virtual string FaultZone
			{
				set
				{
					base.PowerSharpParameters["FaultZone"] = value;
				}
			}

			public virtual bool AutoDagServerConfigured
			{
				set
				{
					base.PowerSharpParameters["AutoDagServerConfigured"] = value;
				}
			}

			public virtual bool TransportSyncDispatchEnabled
			{
				set
				{
					base.PowerSharpParameters["TransportSyncDispatchEnabled"] = value;
				}
			}

			public virtual int MaxTransportSyncDispatchers
			{
				set
				{
					base.PowerSharpParameters["MaxTransportSyncDispatchers"] = value;
				}
			}

			public virtual bool TransportSyncLogEnabled
			{
				set
				{
					base.PowerSharpParameters["TransportSyncLogEnabled"] = value;
				}
			}

			public virtual SyncLoggingLevel TransportSyncLogLoggingLevel
			{
				set
				{
					base.PowerSharpParameters["TransportSyncLogLoggingLevel"] = value;
				}
			}

			public virtual LocalLongFullPath TransportSyncLogFilePath
			{
				set
				{
					base.PowerSharpParameters["TransportSyncLogFilePath"] = value;
				}
			}

			public virtual EnhancedTimeSpan TransportSyncLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["TransportSyncLogMaxAge"] = value;
				}
			}

			public virtual ByteQuantifiedSize TransportSyncLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["TransportSyncLogMaxDirectorySize"] = value;
				}
			}

			public virtual ByteQuantifiedSize TransportSyncLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["TransportSyncLogMaxFileSize"] = value;
				}
			}

			public virtual bool TransportSyncMailboxHealthLogEnabled
			{
				set
				{
					base.PowerSharpParameters["TransportSyncMailboxHealthLogEnabled"] = value;
				}
			}

			public virtual LocalLongFullPath TransportSyncMailboxHealthLogFilePath
			{
				set
				{
					base.PowerSharpParameters["TransportSyncMailboxHealthLogFilePath"] = value;
				}
			}

			public virtual EnhancedTimeSpan TransportSyncMailboxHealthLogMaxAge
			{
				set
				{
					base.PowerSharpParameters["TransportSyncMailboxHealthLogMaxAge"] = value;
				}
			}

			public virtual ByteQuantifiedSize TransportSyncMailboxHealthLogMaxDirectorySize
			{
				set
				{
					base.PowerSharpParameters["TransportSyncMailboxHealthLogMaxDirectorySize"] = value;
				}
			}

			public virtual ByteQuantifiedSize TransportSyncMailboxHealthLogMaxFileSize
			{
				set
				{
					base.PowerSharpParameters["TransportSyncMailboxHealthLogMaxFileSize"] = value;
				}
			}

			public virtual bool IsExcludedFromProvisioning
			{
				set
				{
					base.PowerSharpParameters["IsExcludedFromProvisioning"] = value;
				}
			}

			public virtual int? MaximumActiveDatabases
			{
				set
				{
					base.PowerSharpParameters["MaximumActiveDatabases"] = value;
				}
			}

			public virtual int? MaximumPreferredActiveDatabases
			{
				set
				{
					base.PowerSharpParameters["MaximumPreferredActiveDatabases"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}
	}
}
