using System;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.Services.Diagnostics
{
	internal enum SetAggregatedAccountMetadata
	{
		[DisplayName("SAA", "TT")]
		TotalTime,
		[DisplayName("SAA", "VET")]
		VerifyEnvironmentTime,
		[DisplayName("SAA", "VUIT")]
		VerifyUserIdentityTypeTime,
		[DisplayName("SAA", "SSCT")]
		SetSyncRequestCmdletTime,
		[DisplayName("SAA", "GSCT")]
		GetSyncRequestStatisticsCmdletTime,
		[DisplayName("SAA", "TUCL")]
		TestUserCanLogonTime,
		[DisplayName("SAA", "CVSE")]
		CacheValidatedSettings,
		[DisplayName("SAA", "CNVS")]
		CacheNotValidatedSettings,
		[DisplayName("SAA", "GSNE")]
		GetSyncRequestStatisticsCmdletNonAggregatedAccountError,
		[DisplayName("SAA", "GUST")]
		GetUpdatedConnectionSettingsTime,
		[DisplayName("SAA", "TLST")]
		TestLogonWithCurrentSettingsTime
	}
}
