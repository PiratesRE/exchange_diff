using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal enum GetModernGroupsMetadata
	{
		[DisplayName("GMGS", "AADT")]
		AADLatency,
		[DisplayName("GMGS", "AADGC")]
		AADOnlyGroupCount
	}
}
