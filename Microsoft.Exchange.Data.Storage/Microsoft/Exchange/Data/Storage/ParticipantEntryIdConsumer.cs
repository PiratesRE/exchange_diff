using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum ParticipantEntryIdConsumer
	{
		SupportsNone = 0,
		SupportsADParticipantEntryId = 1,
		SupportsStoreParticipantEntryIdForPDLs = 2,
		SupportsStoreParticipantEntryId = 6,
		SupportsWindowsAddressBookEnvelope = 8,
		CAI = 1,
		RecipientTablePrimary = 3,
		RecipientTableSecondary = 7,
		Rules = 7,
		DLMemberList = 15,
		DLOneOffList = 0,
		ContactEmailSlot = 9,
		ORAR = 0
	}
}
