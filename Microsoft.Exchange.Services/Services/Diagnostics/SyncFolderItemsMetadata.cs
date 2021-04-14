using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal enum SyncFolderItemsMetadata
	{
		[DisplayName("SFi", "TotalT")]
		TotalTime,
		[DisplayName("SFi", "IcsT")]
		IcsTime,
		[DisplayName("SFi", "QueryT")]
		QueryTime,
		[DisplayName("SFi", "CatchUpT")]
		CatchUpTime,
		[DisplayName("SFi", "ChangesT")]
		ChangesTime,
		[DisplayName("SFi", "SSS")]
		SyncStateSize,
		[DisplayName("SFi", "SSH")]
		SyncStateHash,
		[DisplayName("SFi", "CC")]
		ItemCount,
		[DisplayName("SFi", "Last")]
		IncludesLastItemInRange,
		[DisplayName("SFi", "XcptId")]
		ExceptionItemId
	}
}
