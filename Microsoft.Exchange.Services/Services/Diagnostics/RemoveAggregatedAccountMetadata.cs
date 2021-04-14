using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal enum RemoveAggregatedAccountMetadata
	{
		[DisplayName("RAA", "TT")]
		TotalTime,
		[DisplayName("RAA", "VET")]
		VerifyEnvironmentTime,
		[DisplayName("RAA", "VUIT")]
		VerifyUserIdentityTypeTime,
		[DisplayName("RAA", "RSCT")]
		RemoveSyncRequestCmdletTime,
		[DisplayName("RAA", "RGAT")]
		RemoveAggregatedMailboxGuidFromADUserTime,
		[DisplayName("RAA", "GMGT")]
		GetAggregatedMailboxGuidFromSyncRequestStatisticsTime,
		[DisplayName("RAA", "GSNE")]
		GetSyncRequestStatisticsCmdletNonAggregatedAccountError
	}
}
