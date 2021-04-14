using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal enum GetUserUnifiedGroupsMetadata
	{
		[DisplayName("GUUGS", "AADT")]
		AADLatency,
		[DisplayName("GUUGS", "AADGC")]
		AADOnlyGroupCount
	}
}
