using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal enum SyncConversationMetadata
	{
		[DisplayName("SCo", "TotalT")]
		TotalTime,
		[DisplayName("SCo", "IcsT")]
		IcsTime,
		[DisplayName("SCo", "QueryT")]
		QueryTime,
		[DisplayName("SCo", "DraftT")]
		DraftTime,
		[DisplayName("SCo", "ReadFlagT")]
		ReadFlagTime,
		[DisplayName("SCo", "IcsBindT")]
		IcsBindTime,
		[DisplayName("SCo", "CatchUpT")]
		CatchUpTime,
		[DisplayName("SCo", "ChangesT")]
		ChangesTime,
		[DisplayName("SCo", "ChangesCallC")]
		ChangesCallCount,
		[DisplayName("SCo", "FetchT")]
		FetchTime,
		[DisplayName("SCo", "FetchC")]
		FetchCount,
		[DisplayName("SCo", "FetchQT")]
		FetchQueryTime,
		[DisplayName("SCo", "FetchUnC")]
		FetchUnneededCount,
		[DisplayName("SCo", "LeftOverC")]
		LeftOverCount,
		[DisplayName("SCo", "LeftOverQT")]
		LeftOverQueryTime,
		[DisplayName("SCo", "QueryBindT")]
		QueryBindTime,
		[DisplayName("SCo", "FC")]
		FolderCount,
		[DisplayName("SCo", "SSS")]
		SyncStateSize,
		[DisplayName("SCo", "SSH")]
		SyncStateHash,
		[DisplayName("SCo", "CC")]
		ConversationCount,
		[DisplayName("SCo", "DCC")]
		DeletedConversationCount,
		[DisplayName("SCo", "Last")]
		IncludesLastItemInRange,
		[DisplayName("SCo", "XcptId")]
		ExceptionConversationId
	}
}
