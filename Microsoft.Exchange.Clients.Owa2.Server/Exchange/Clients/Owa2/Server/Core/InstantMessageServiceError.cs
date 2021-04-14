using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public enum InstantMessageServiceError
	{
		None,
		SipEndpointConnectionFailure,
		SipEndpointOperationTimeout,
		SipEndpointRegister,
		SipEndpointFailureResponse,
		AddressNotAvailable = 2000,
		ExternalAuthenticationDisabled,
		ExternalIdentityUnknown,
		OverMaxPayloadSize = 2004,
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
