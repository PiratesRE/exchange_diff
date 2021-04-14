using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal enum TimeBasedAssistantIndex
	{
		CalendarRepair,
		SharingPolicy,
		SharingSync,
		ManagedFolder,
		TopN,
		UMReporting,
		PublicFolder,
		InferenceTraining,
		DirectoryProcessor,
		OABGenerator,
		InferenceDataCollection = 11,
		PeopleRelevance,
		SiteMailbox,
		MailboxProcessor,
		MailboxAssociationReplication,
		SharePointSignalStore,
		PeopleCentricTriage,
		StoreDsMaintenance,
		StoreIntegrityCheck,
		StoreMaintenance,
		StoreScheduledIntegrityCheck,
		StoreUrgentMaintenance,
		JunkEmailOptionsCommitter,
		ProbeTimeBasedAssistant,
		SearchIndexRepairTimeBasedAssistant,
		DarTaskStoreTimeBasedAssistant,
		GroupMailbox
	}
}
