using System;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal enum OWAEnabledFlags
	{
		GlobalAddressListEnabledMask = 1,
		CalendarEnabledMask,
		ContactsEnabledMask = 4,
		TasksEnabledMask = 8,
		JournalEnabledMask = 16,
		NotesEnabledMask = 32,
		PublicFoldersEnabledMask = 64,
		OrganizationEnabledMask = 128,
		RemindersAndNotificationsEnabledMask = 256,
		PremiumClientEnabledMask = 512,
		SpellCheckerEnabledMask = 1024,
		SMimeEnabledMask = 2048,
		SearchFoldersEnabledMask = 4096,
		SignaturesEnabledMask = 8192,
		RulesEnabledMask = 16384,
		ThemeSelectionEnabledMask = 32768,
		JunkEmailEnabledMask = 65536,
		UMIntegrationEnabledMask = 131072,
		WSSAccessOnPublicComputersEnabledMask = 262144,
		WSSAccessOnPrivateComputersEnabledMask = 524288,
		UNCAccessOnPublicComputersEnabledMask = 1048576,
		UNCAccessOnPrivateComputersEnabledMask = 2097152,
		ActiveSyncIntegrationEnabledMask = 4194304,
		ExplicitLogonEnabledMask = 8388608,
		AllAddressListsEnabledMask = 16777216,
		RecoverDeletedItemsEnabledMask = 33554432,
		ChangePasswordEnabledMask = 67108864,
		InstantMessagingEnabledMask = 134217728,
		TextMessagingEnabledMask = 268435456,
		OWALightEnabledMask = 536870912,
		DelegateAccessEnabledMask = 1073741824,
		IRMEnabledMask = -2147483648
	}
}
