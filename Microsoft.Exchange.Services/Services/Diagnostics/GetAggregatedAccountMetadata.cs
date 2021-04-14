using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal enum GetAggregatedAccountMetadata
	{
		[DisplayName("GAA", "TT")]
		TotalTime,
		[DisplayName("GAA", "VET")]
		VerifyEnvironmentTime,
		[DisplayName("GAA", "VUIT")]
		VerifyUserIdentityTypeTime,
		[DisplayName("GAA", "GSCT")]
		GetSyncRequestCmdletTime,
		[DisplayName("GAA", "GSST")]
		GetSyncRequestStatisticsCmdletTime,
		[DisplayName("GAA", "GSSE")]
		GetSyncRequestStatisticsCmdletError,
		[DisplayName("GAA", "GSNE")]
		GetSyncRequestStatisticsCmdletNonAggregatedAccountError
	}
}
