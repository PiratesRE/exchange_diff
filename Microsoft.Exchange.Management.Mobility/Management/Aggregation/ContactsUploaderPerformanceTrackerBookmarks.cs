using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Management.Aggregation
{
	internal enum ContactsUploaderPerformanceTrackerBookmarks
	{
		[DisplayName("CU", "ExportT")]
		ExportTime,
		[DisplayName("CU", "FormatT")]
		FormatTime,
		[DisplayName("CU", "UploadT")]
		UploadTime
	}
}
