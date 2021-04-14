using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal enum AddAggregatedAccountMetadata
	{
		[DisplayName("AAA", "TT")]
		TotalTime,
		[DisplayName("AAA", "VET")]
		VerifyEnvironmentTime,
		[DisplayName("AAA", "VUIT")]
		VerifyUserIdentityTypeTime,
		[DisplayName("AAA", "NSRG")]
		NewSyncRequestGuid,
		[DisplayName("AAA", "CSPR")]
		CheckShouldProceedWithRequest,
		[DisplayName("AAA", "AGAT")]
		AddAggregatedMailboxGuidToADUserTime,
		[DisplayName("AAA", "MCT")]
		MailboxCleanupTime,
		[DisplayName("AAA", "NSCT")]
		NewSyncRequestCmdletTime,
		[DisplayName("AAA", "TUCL")]
		TestUserCanLogonTime,
		[DisplayName("AAA", "CVSE")]
		CacheValidatedSettings,
		[DisplayName("AAA", "CNVS")]
		CacheNotValidatedSettings,
		[DisplayName("AAA", "SMBE")]
		SetMailboxCmdletError,
		[DisplayName("AAA", "NOAA")]
		NumberOfAggregatedAccounts
	}
}
