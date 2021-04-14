using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal enum GetPersonaModernGroupMembershipMetadata
	{
		[DisplayName("GPMG", "GC")]
		GroupCount,
		[DisplayName("GPMG", "RT")]
		IsValidReplicationTarget
	}
}
