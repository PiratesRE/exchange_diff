using System;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationWorkloadManagementComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationWorkloadManagementComponent() : base("WorkloadManagement")
		{
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "PowerShell", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "PowerShellForwardSync", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "Ews", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "Processor", typeof(IResourceSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "StoreUrgentMaintenanceAssistant", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "SharePointSignalStoreAssistant", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "StoreOnlineIntegrityCheckAssistant", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "PowerShellLowPriorityWorkFlow", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "E4eRecipient", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "Eas", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "Transport", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "InferenceDataCollectionAssistant", typeof(IWorkloadSettings), false));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "Owa", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "PeopleRelevanceAssistant", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "PublicFolderAssistant", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "TransportSync", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "OrgContactsSyncAssistant", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "InferenceTrainingAssistant", typeof(IWorkloadSettings), false));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "ProbeTimeBasedAssistant", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "DarRuntime", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "Blackout", typeof(IBlackoutSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "MailboxReplicationServiceHighPriority", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "MailboxReplicationServiceInteractive", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "StoreMaintenanceAssistant", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "CalendarSyncAssistant", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "DarTaskStoreTimeBasedAssistant", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "ELCAssistant", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "SystemWorkloadManager", typeof(ISystemWorkloadManagerSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "DiskLatency", typeof(IResourceSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "O365SuiteService", typeof(IWorkloadSettings), false));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "DiskLatencySettings", typeof(IDiskLatencyMonitorSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "PublicFolderMailboxSync", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "OABGeneratorAssistant", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "TeamMailboxSync", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "StoreScheduledIntegrityCheckAssistant", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "Domt", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "MailboxReplicationServiceInternalMaintenance", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "PowerShellGalSync", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "Momt", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "MailboxProcessorAssistant", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "SearchIndexRepairTimebasedAssistant", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "PeopleCentricTriageAssistant", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "TopNAssistant", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "OwaVoice", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "E4eSender", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "Imap", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "SiteMailboxAssistant", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "UMReportingAssistant", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "ActiveDirectoryReplicationLatency", typeof(IResourceSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "OutlookService", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "GroupMailboxAssistant", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "MdbAvailability", typeof(IResourceSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "MdbLatency", typeof(IResourceSettings), false));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "JunkEmailOptionsCommitterAssistant", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "DirectoryProcessorAssistant", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "CalendarRepairAssistant", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "MailboxAssociationReplicationAssistant", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "StoreDSMaintenanceAssistant", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "CiAgeOfLastNotification", typeof(IResourceSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "SharingPolicyAssistant", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "MdbReplication", typeof(IResourceSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "PowerShellBackSync", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "Pop", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "MailboxReplicationService", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "PushNotificationService", typeof(IWorkloadSettings), true));
			base.Add(new VariantConfigurationSection("WorkloadManagement.settings.ini", "PowerShellDiscretionaryWorkFlow", typeof(IWorkloadSettings), true));
		}

		public VariantConfigurationSection PowerShell
		{
			get
			{
				return base["PowerShell"];
			}
		}

		public VariantConfigurationSection PowerShellForwardSync
		{
			get
			{
				return base["PowerShellForwardSync"];
			}
		}

		public VariantConfigurationSection Ews
		{
			get
			{
				return base["Ews"];
			}
		}

		public VariantConfigurationSection Processor
		{
			get
			{
				return base["Processor"];
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

		public VariantConfigurationSection PowerShellLowPriorityWorkFlow
		{
			get
			{
				return base["PowerShellLowPriorityWorkFlow"];
			}
		}

		public VariantConfigurationSection E4eRecipient
		{
			get
			{
				return base["E4eRecipient"];
			}
		}

		public VariantConfigurationSection Eas
		{
			get
			{
				return base["Eas"];
			}
		}

		public VariantConfigurationSection Transport
		{
			get
			{
				return base["Transport"];
			}
		}

		public VariantConfigurationSection InferenceDataCollectionAssistant
		{
			get
			{
				return base["InferenceDataCollectionAssistant"];
			}
		}

		public VariantConfigurationSection Owa
		{
			get
			{
				return base["Owa"];
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

		public VariantConfigurationSection TransportSync
		{
			get
			{
				return base["TransportSync"];
			}
		}

		public VariantConfigurationSection OrgContactsSyncAssistant
		{
			get
			{
				return base["OrgContactsSyncAssistant"];
			}
		}

		public VariantConfigurationSection InferenceTrainingAssistant
		{
			get
			{
				return base["InferenceTrainingAssistant"];
			}
		}

		public VariantConfigurationSection ProbeTimeBasedAssistant
		{
			get
			{
				return base["ProbeTimeBasedAssistant"];
			}
		}

		public VariantConfigurationSection DarRuntime
		{
			get
			{
				return base["DarRuntime"];
			}
		}

		public VariantConfigurationSection Blackout
		{
			get
			{
				return base["Blackout"];
			}
		}

		public VariantConfigurationSection MailboxReplicationServiceHighPriority
		{
			get
			{
				return base["MailboxReplicationServiceHighPriority"];
			}
		}

		public VariantConfigurationSection MailboxReplicationServiceInteractive
		{
			get
			{
				return base["MailboxReplicationServiceInteractive"];
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

		public VariantConfigurationSection DarTaskStoreTimeBasedAssistant
		{
			get
			{
				return base["DarTaskStoreTimeBasedAssistant"];
			}
		}

		public VariantConfigurationSection ELCAssistant
		{
			get
			{
				return base["ELCAssistant"];
			}
		}

		public VariantConfigurationSection SystemWorkloadManager
		{
			get
			{
				return base["SystemWorkloadManager"];
			}
		}

		public VariantConfigurationSection DiskLatency
		{
			get
			{
				return base["DiskLatency"];
			}
		}

		public VariantConfigurationSection O365SuiteService
		{
			get
			{
				return base["O365SuiteService"];
			}
		}

		public VariantConfigurationSection DiskLatencySettings
		{
			get
			{
				return base["DiskLatencySettings"];
			}
		}

		public VariantConfigurationSection PublicFolderMailboxSync
		{
			get
			{
				return base["PublicFolderMailboxSync"];
			}
		}

		public VariantConfigurationSection OABGeneratorAssistant
		{
			get
			{
				return base["OABGeneratorAssistant"];
			}
		}

		public VariantConfigurationSection TeamMailboxSync
		{
			get
			{
				return base["TeamMailboxSync"];
			}
		}

		public VariantConfigurationSection StoreScheduledIntegrityCheckAssistant
		{
			get
			{
				return base["StoreScheduledIntegrityCheckAssistant"];
			}
		}

		public VariantConfigurationSection Domt
		{
			get
			{
				return base["Domt"];
			}
		}

		public VariantConfigurationSection MailboxReplicationServiceInternalMaintenance
		{
			get
			{
				return base["MailboxReplicationServiceInternalMaintenance"];
			}
		}

		public VariantConfigurationSection PowerShellGalSync
		{
			get
			{
				return base["PowerShellGalSync"];
			}
		}

		public VariantConfigurationSection Momt
		{
			get
			{
				return base["Momt"];
			}
		}

		public VariantConfigurationSection MailboxProcessorAssistant
		{
			get
			{
				return base["MailboxProcessorAssistant"];
			}
		}

		public VariantConfigurationSection SearchIndexRepairTimebasedAssistant
		{
			get
			{
				return base["SearchIndexRepairTimebasedAssistant"];
			}
		}

		public VariantConfigurationSection PeopleCentricTriageAssistant
		{
			get
			{
				return base["PeopleCentricTriageAssistant"];
			}
		}

		public VariantConfigurationSection TopNAssistant
		{
			get
			{
				return base["TopNAssistant"];
			}
		}

		public VariantConfigurationSection OwaVoice
		{
			get
			{
				return base["OwaVoice"];
			}
		}

		public VariantConfigurationSection E4eSender
		{
			get
			{
				return base["E4eSender"];
			}
		}

		public VariantConfigurationSection Imap
		{
			get
			{
				return base["Imap"];
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

		public VariantConfigurationSection ActiveDirectoryReplicationLatency
		{
			get
			{
				return base["ActiveDirectoryReplicationLatency"];
			}
		}

		public VariantConfigurationSection OutlookService
		{
			get
			{
				return base["OutlookService"];
			}
		}

		public VariantConfigurationSection GroupMailboxAssistant
		{
			get
			{
				return base["GroupMailboxAssistant"];
			}
		}

		public VariantConfigurationSection MdbAvailability
		{
			get
			{
				return base["MdbAvailability"];
			}
		}

		public VariantConfigurationSection MdbLatency
		{
			get
			{
				return base["MdbLatency"];
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

		public VariantConfigurationSection CiAgeOfLastNotification
		{
			get
			{
				return base["CiAgeOfLastNotification"];
			}
		}

		public VariantConfigurationSection SharingPolicyAssistant
		{
			get
			{
				return base["SharingPolicyAssistant"];
			}
		}

		public VariantConfigurationSection MdbReplication
		{
			get
			{
				return base["MdbReplication"];
			}
		}

		public VariantConfigurationSection PowerShellBackSync
		{
			get
			{
				return base["PowerShellBackSync"];
			}
		}

		public VariantConfigurationSection Pop
		{
			get
			{
				return base["Pop"];
			}
		}

		public VariantConfigurationSection MailboxReplicationService
		{
			get
			{
				return base["MailboxReplicationService"];
			}
		}

		public VariantConfigurationSection PushNotificationService
		{
			get
			{
				return base["PushNotificationService"];
			}
		}

		public VariantConfigurationSection PowerShellDiscretionaryWorkFlow
		{
			get
			{
				return base["PowerShellDiscretionaryWorkFlow"];
			}
		}
	}
}
