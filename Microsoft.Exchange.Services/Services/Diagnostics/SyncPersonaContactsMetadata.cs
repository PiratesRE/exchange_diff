using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal enum SyncPersonaContactsMetadata
	{
		[DisplayName("SPe", "TT")]
		TotalTime,
		[DisplayName("SPe", "IT")]
		IcsTime,
		[DisplayName("SPe", "CT")]
		CatchUpTime,
		[DisplayName("SPe", "QS")]
		QuerySync,
		[DisplayName("SPe", "IS")]
		IcsSync,
		[DisplayName("SPe", "FC")]
		FolderCount,
		[DisplayName("SPe", "ENUM")]
		ContactsEnumerated,
		[DisplayName("SPe", "ICS")]
		IcsChangesProcessed,
		[DisplayName("SPe", "SSS")]
		SyncStateSize,
		[DisplayName("SPe", "SSH")]
		SyncStateHash,
		[DisplayName("SPe", "PC")]
		PeopleCount,
		[DisplayName("SPe", "DPC")]
		DeletedPeopleCount,
		[DisplayName("SPe", "Last")]
		IncludesLastItemInRange,
		[DisplayName("SPe", "Bad")]
		InvalidContacts,
		[DisplayName("SPe", "XcptId")]
		ExceptionPersonId,
		[DisplayName("SPe", "Spct")]
		SyncPersonaContactsType
	}
}
