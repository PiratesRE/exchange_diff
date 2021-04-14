using System;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	internal enum CredFailure
	{
		None,
		Invalid,
		Expired,
		LockedOut,
		LiveIdFailure,
		STSFailure,
		AppPasswordRequired,
		ADFSRulesDeny,
		AccountNotProvisioned,
		Forbidden,
		UnfamiliarLocation,
		NotSupportedProtocolForOutlookCom
	}
}
