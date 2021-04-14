using System;

namespace Microsoft.Exchange.Diagnostics.Components.Security
{
	public enum LiveIdAuthResult
	{
		Success,
		UserNotFoundInAD,
		LiveServerUnreachable,
		FederatedStsUnreachable,
		OperationTimedOut,
		CommunicationFailure,
		AuthFailure,
		ExpiredCreds,
		InvalidCreds,
		RecoverableAuthFailure,
		S4ULogonFailure,
		HRDFailure,
		OfflineOrgIdAuthFailure,
		AmbigiousMailboxFoundFailure,
		UnableToOpenTicketFailure,
		PuidMismatchFailure,
		InvalidUsername,
		FaultException,
		LowPasswordConfidence,
		OfflineHrdFailed,
		AppPasswordRequired,
		FederatedStsUrlNotEncrypted,
		FederatedStsADFSRulesDenied,
		InternalServerError,
		AccountNotProvisioned,
		Forbidden,
		UnfamiliarLocation
	}
}
