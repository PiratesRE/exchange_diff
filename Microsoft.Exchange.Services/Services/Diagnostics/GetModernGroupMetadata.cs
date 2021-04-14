using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal enum GetModernGroupMetadata
	{
		[DisplayName("GMG", "MC")]
		MemberCount,
		[DisplayName("GMG", "MADTime")]
		MemberADLatency,
		[DisplayName("GMG", "MMBTime")]
		MemberMBLatency,
		[DisplayName("GMG", "MMBCount")]
		MemberMBCount,
		[DisplayName("GMG", "SO")]
		SortOrder,
		[DisplayName("GMG", "GI")]
		GeneralInfo,
		[DisplayName("GMG", "OL")]
		OwnerList
	}
}
