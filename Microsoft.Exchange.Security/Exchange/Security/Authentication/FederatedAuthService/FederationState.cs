using System;

namespace Microsoft.Exchange.Security.Authentication.FederatedAuthService
{
	internal enum FederationState
	{
		NoCache_PwdOk,
		NoCache_Fed,
		Email_PwdOk,
		Email_Fed,
		Domain_PwdOk
	}
}
