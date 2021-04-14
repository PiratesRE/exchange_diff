using System;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	internal enum AuthStatus
	{
		LogonSuccess = 1,
		LogonFailed = 0,
		Redirect = -1,
		LiveIDFailed = -2,
		FederatedStsFailed = -3,
		RecoverableLogonFailed = -4,
		ExpiredCredentials = -5,
		RepeatedLogonFailure = -6,
		RepeatedLiveIDFailure = -7,
		RepeatedFederatedStsFailure = -8,
		RepeatedRecoverableFailure = -9,
		RepeatedExpiredCredentials = -10,
		LowConfidence = -11,
		BadPassword = -12,
		RepeatedBadPassword = -13,
		S4ULogonFailed = -14,
		HRDFailed = -15,
		OffineOrgIdAuthFailed = -16,
		AmbigiousMailboxFound = -17,
		UnableToOpenTicket = -18,
		PuidMismatch = -19,
		PuidNotFound = -20,
		OfflineHrdFailed = -21,
		AppPasswordRequired = -22,
		FederatedStsUrlNotEncrypted = -23,
		ADFSRulesDenied = -24,
		RepeatedADFSRulesDenied = -25,
		AccountNotProvisioned = -26,
		InternalServerError = -27,
		Forbidden = -28,
		UnfamiliarLocation = -29,
		MaxValue = -29
	}
}
