using System;
using Microsoft.Exchange.Assistants;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationMailboxAssistantsComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationMailboxAssistantsComponent() : base("MailboxAssistants")
		{
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "FlagPlus", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "ApprovalAssistantCheckRateLimit", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "StoreUrgentMaintenanceAssistant", typeof(IMailboxAssistantSettings), true));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "SharePointSignalStoreAssistant", typeof(IMailboxAssistantSettings), true));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "StoreOnlineIntegrityCheckAssistant", typeof(IMailboxAssistantSettings), true));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "DirectoryProcessorTenantLogging", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "CalendarRepairAssistantLogging", typeof(ICalendarRepairLoggerSettings), true));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "CalendarNotificationAssistantSkipUserSettingsUpdate", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "ElcAssistantTryProcessEhaMigratedMessages", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "InferenceDataCollectionAssistant", typeof(IMailboxAssistantSettings), false));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "SearchIndexRepairAssistant", typeof(IMailboxAssistantSettings), true));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "TimeBasedAssistantsMonitoring", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "TestTBA", typeof(IMailboxAssistantSettings), true));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "OrgMailboxCheckScaleRequirements", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "PeopleRelevanceAssistant", typeof(IMailboxAssistantSettings), true));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "PublicFolderAssistant", typeof(IMailboxAssistantSettings), true));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "ElcAssistant", typeof(IMailboxAssistantSettings), true));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "ElcRemoteArchive", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "PublicFolderSplit", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "InferenceTrainingAssistant", typeof(IMailboxAssistantSettings), false));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "SharePointSignalStore", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "ProbeTimeBasedAssistant", typeof(IMailboxAssistantSettings), true));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "SharePointSignalStoreInDatacenter", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "GenerateGroupPhoto", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "StoreMaintenanceAssistant", typeof(IMailboxAssistantSettings), true));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "CalendarSyncAssistant", typeof(IMailboxAssistantSettings), true));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "EmailReminders", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "DelegateRulesLogger", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "OABGeneratorAssistant", typeof(IMailboxAssistantSettings), true));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "StoreScheduledIntegrityCheckAssistant", typeof(IMailboxAssistantSettings), true));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "MwiAssistantGetUMEnabledUsersFromDatacenter", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "MailboxProcessorAssistant", typeof(IMailboxAssistantSettings), true));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "PeopleCentricTriageAssistant", typeof(IMailboxAssistantSettings), true));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "MailboxAssistantService", typeof(IMailboxAssistantServiceSettings), true));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "ElcAssistantApplyLitigationHoldDuration", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "TopNWordsAssistant", typeof(IMailboxAssistantSettings), true));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "SiteMailboxAssistant", typeof(IMailboxAssistantSettings), true));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "UMReportingAssistant", typeof(IMailboxAssistantSettings), true));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "DarTaskStoreAssistant", typeof(IMailboxAssistantSettings), false));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "GroupMailboxAssistant", typeof(IMailboxAssistantSettings), true));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "QuickCapture", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "JunkEmailOptionsCommitterAssistant", typeof(IMailboxAssistantSettings), true));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "DirectoryProcessorAssistant", typeof(IMailboxAssistantSettings), true));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "CalendarRepairAssistant", typeof(IMailboxAssistantSettings), true));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "MailboxAssociationReplicationAssistant", typeof(IMailboxAssistantSettings), true));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "StoreDSMaintenanceAssistant", typeof(IMailboxAssistantSettings), true));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "ElcAssistantDiscoveryHoldSynchronizer", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "SharingPolicyAssistant", typeof(IMailboxAssistantSettings), true));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "ElcAssistantAlwaysProcessMailbox", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "CalendarRepairAssistantReliabilityLogger", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "UnifiedPolicyHold", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MailboxAssistants.settings.ini", "PerformRecipientDLExpansion", typeof(IFeature), false));
		}

		public VariantConfigurationSection FlagPlus
		{
			get
			{
				return base["FlagPlus"];
			}
		}

		public VariantConfigurationSection ApprovalAssistantCheckRateLimit
		{
			get
			{
				return base["ApprovalAssistantCheckRateLimit"];
			}
		}

		public VariantConfigurationSection StoreUrgentMaintenanceAssistant
		{
			get
			{
				return base["StoreUrgentMaintenanceAssistant"];
			}
		}

		public VariantConfigurationSection SharePointSignalStoreAssistant
		{
			get
			{
				return base["SharePointSignalStoreAssistant"];
			}
		}

		public VariantConfigurationSection StoreOnlineIntegrityCheckAssistant
		{
			get
			{
				return base["StoreOnlineIntegrityCheckAssistant"];
			}
		}

		public VariantConfigurationSection DirectoryProcessorTenantLogging
		{
			get
			{
				return base["DirectoryProcessorTenantLogging"];
			}
		}

		public VariantConfigurationSection CalendarRepairAssistantLogging
		{
			get
			{
				return base["CalendarRepairAssistantLogging"];
			}
		}

		public VariantConfigurationSection CalendarNotificationAssistantSkipUserSettingsUpdate
		{
			get
			{
				return base["CalendarNotificationAssistantSkipUserSettingsUpdate"];
			}
		}

		public VariantConfigurationSection ElcAssistantTryProcessEhaMigratedMessages
		{
			get
			{
				return base["ElcAssistantTryProcessEhaMigratedMessages"];
			}
		}

		public VariantConfigurationSection InferenceDataCollectionAssistant
		{
			get
			{
				return base["InferenceDataCollectionAssistant"];
			}
		}

		public VariantConfigurationSection SearchIndexRepairAssistant
		{
			get
			{
				return base["SearchIndexRepairAssistant"];
			}
		}

		public VariantConfigurationSection TimeBasedAssistantsMonitoring
		{
			get
			{
				return base["TimeBasedAssistantsMonitoring"];
			}
		}

		public VariantConfigurationSection TestTBA
		{
			get
			{
				return base["TestTBA"];
			}
		}

		public VariantConfigurationSection OrgMailboxCheckScaleRequirements
		{
			get
			{
				return base["OrgMailboxCheckScaleRequirements"];
			}
		}

		public VariantConfigurationSection PeopleRelevanceAssistant
		{
			get
			{
				return base["PeopleRelevanceAssistant"];
			}
		}

		public VariantConfigurationSection PublicFolderAssistant
		{
			get
			{
				return base["PublicFolderAssistant"];
			}
		}

		public VariantConfigurationSection ElcAssistant
		{
			get
			{
				return base["ElcAssistant"];
			}
		}

		public VariantConfigurationSection ElcRemoteArchive
		{
			get
			{
				return base["ElcRemoteArchive"];
			}
		}

		public VariantConfigurationSection PublicFolderSplit
		{
			get
			{
				return base["PublicFolderSplit"];
			}
		}

		public VariantConfigurationSection InferenceTrainingAssistant
		{
			get
			{
				return base["InferenceTrainingAssistant"];
			}
		}

		public VariantConfigurationSection SharePointSignalStore
		{
			get
			{
				return base["SharePointSignalStore"];
			}
		}

		public VariantConfigurationSection ProbeTimeBasedAssistant
		{
			get
			{
				return base["ProbeTimeBasedAssistant"];
			}
		}

		public VariantConfigurationSection SharePointSignalStoreInDatacenter
		{
			get
			{
				return base["SharePointSignalStoreInDatacenter"];
			}
		}

		public VariantConfigurationSection GenerateGroupPhoto
		{
			get
			{
				return base["GenerateGroupPhoto"];
			}
		}

		public VariantConfigurationSection StoreMaintenanceAssistant
		{
			get
			{
				return base["StoreMaintenanceAssistant"];
			}
		}

		public VariantConfigurationSection CalendarSyncAssistant
		{
			get
			{
				return base["CalendarSyncAssistant"];
			}
		}

		public VariantConfigurationSection EmailReminders
		{
			get
			{
				return base["EmailReminders"];
			}
		}

		public VariantConfigurationSection DelegateRulesLogger
		{
			get
			{
				return base["DelegateRulesLogger"];
			}
		}

		public VariantConfigurationSection OABGeneratorAssistant
		{
			get
			{
				return base["OABGeneratorAssistant"];
			}
		}

		public VariantConfigurationSection StoreScheduledIntegrityCheckAssistant
		{
			get
			{
				return base["StoreScheduledIntegrityCheckAssistant"];
			}
		}

		public VariantConfigurationSection MwiAssistantGetUMEnabledUsersFromDatacenter
		{
			get
			{
				return base["MwiAssistantGetUMEnabledUsersFromDatacenter"];
			}
		}

		public VariantConfigurationSection MailboxProcessorAssistant
		{
			get
			{
				return base["MailboxProcessorAssistant"];
			}
		}

		public VariantConfigurationSection PeopleCentricTriageAssistant
		{
			get
			{
				return base["PeopleCentricTriageAssistant"];
			}
		}

		public VariantConfigurationSection MailboxAssistantService
		{
			get
			{
				return base["MailboxAssistantService"];
			}
		}

		public VariantConfigurationSection ElcAssistantApplyLitigationHoldDuration
		{
			get
			{
				return base["ElcAssistantApplyLitigationHoldDuration"];
			}
		}

		public VariantConfigurationSection TopNWordsAssistant
		{
			get
			{
				return base["TopNWordsAssistant"];
			}
		}

		public VariantConfigurationSection SiteMailboxAssistant
		{
			get
			{
				return base["SiteMailboxAssistant"];
			}
		}

		public VariantConfigurationSection UMReportingAssistant
		{
			get
			{
				return base["UMReportingAssistant"];
			}
		}

		public VariantConfigurationSection DarTaskStoreAssistant
		{
			get
			{
				return base["DarTaskStoreAssistant"];
			}
		}

		public VariantConfigurationSection GroupMailboxAssistant
		{
			get
			{
				return base["GroupMailboxAssistant"];
			}
		}

		public VariantConfigurationSection QuickCapture
		{
			get
			{
				return base["QuickCapture"];
			}
		}

		public VariantConfigurationSection JunkEmailOptionsCommitterAssistant
		{
			get
			{
				return base["JunkEmailOptionsCommitterAssistant"];
			}
		}

		public VariantConfigurationSection DirectoryProcessorAssistant
		{
			get
			{
				return base["DirectoryProcessorAssistant"];
			}
		}

		public VariantConfigurationSection CalendarRepairAssistant
		{
			get
			{
				return base["CalendarRepairAssistant"];
			}
		}

		public VariantConfigurationSection MailboxAssociationReplicationAssistant
		{
			get
			{
				return base["MailboxAssociationReplicationAssistant"];
			}
		}

		public VariantConfigurationSection StoreDSMaintenanceAssistant
		{
			get
			{
				return base["StoreDSMaintenanceAssistant"];
			}
		}

		public VariantConfigurationSection ElcAssistantDiscoveryHoldSynchronizer
		{
			get
			{
				return base["ElcAssistantDiscoveryHoldSynchronizer"];
			}
		}

		public VariantConfigurationSection SharingPolicyAssistant
		{
			get
			{
				return base["SharingPolicyAssistant"];
			}
		}

		public VariantConfigurationSection ElcAssistantAlwaysProcessMailbox
		{
			get
			{
				return base["ElcAssistantAlwaysProcessMailbox"];
			}
		}

		public VariantConfigurationSection CalendarRepairAssistantReliabilityLogger
		{
			get
			{
				return base["CalendarRepairAssistantReliabilityLogger"];
			}
		}

		public VariantConfigurationSection UnifiedPolicyHold
		{
			get
			{
				return base["UnifiedPolicyHold"];
			}
		}

		public VariantConfigurationSection PerformRecipientDLExpansion
		{
			get
			{
				return base["PerformRecipientDLExpansion"];
			}
		}
	}
}
