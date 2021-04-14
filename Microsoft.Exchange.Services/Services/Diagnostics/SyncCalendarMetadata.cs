using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal enum SyncCalendarMetadata
	{
		[DisplayName("SCa", "TT")]
		TotalTime,
		[DisplayName("SCa", "SSS")]
		SyncStateSize,
		[DisplayName("SCa", "SSH")]
		SyncStateHash,
		[DisplayName("SCa", "DIC")]
		DeletedItemsCount,
		[DisplayName("SCa", "UIC")]
		UpdatedItemsCount,
		[DisplayName("SCa", "RMwIC")]
		RecurrenceMastersWithInstancesCount,
		[DisplayName("SCa", "URMwIC")]
		UnchangedRecurrenceMastersWithInstancesCount,
		[DisplayName("SCa", "RMwoIC")]
		RecurrenceMastersWithoutInstancesCount,
		[DisplayName("SCa", "Last")]
		IncludesLastItemInRange,
		[DisplayName("SCa", "XcptId")]
		ExceptionStoreId
	}
}
