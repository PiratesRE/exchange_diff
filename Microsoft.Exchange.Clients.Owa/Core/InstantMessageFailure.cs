using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public enum InstantMessageFailure
	{
		None,
		SipEndpointConnectionFailure,
		SipEndpointOperationTimeout,
		SipEndpointRegister,
		SipEndpointFailureResponse,
		AddressNotAvailable = 2000,
		ExternalAuthenticationDisabled,
		ExternalIdentityUnknown,
		TermsOfUseNotSigned,
		OverMaxPayloadSize,
		CreateEndpointFailure,
		ServerTimeout = 3000,
		SignInFailure,
		SignInDisconnected,
		SessionDisconnected,
		SessionSignOut,
		ServiceShutdown,
		ReInitializeOwa,
		PrivacyMigrationInProgress = 3008,
		PrivacyMigrationNeeded,
		PrivacyPolicyChanged,
		ClientSignOut = 4000,
		ClientLimit,
		BeginSignInError = 5000,
		SignInError
	}
}
