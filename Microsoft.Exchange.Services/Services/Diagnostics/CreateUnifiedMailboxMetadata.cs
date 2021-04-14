using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal enum CreateUnifiedMailboxMetadata
	{
		[DisplayName("CRUM", "TT")]
		TotalTime,
		[DisplayName("CRUM", "VET")]
		VerifyEnvironmentTime,
		[DisplayName("CRUM", "VUIT")]
		VerifyUserIdentityTypeTime,
		[DisplayName("CRUM", "NMCT")]
		NewMailboxCmdletTime,
		[DisplayName("CRUM", "GAUT")]
		GlsAddUserTime,
		[DisplayName("CRUM", "GAUS")]
		GlsAddUserOperationStatus
	}
}
