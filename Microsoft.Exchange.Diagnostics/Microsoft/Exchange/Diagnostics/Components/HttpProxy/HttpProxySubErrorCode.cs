using System;

namespace Microsoft.Exchange.Diagnostics.Components.HttpProxy
{
	internal enum HttpProxySubErrorCode
	{
		DirectoryOperationError = 2001,
		MServOperationError,
		ServerDiscoveryError,
		ServerLocatorError,
		TooManyOutstandingProxyRequests = 2011,
		TooManyOutstandingAuthenticationRequests,
		RpcHttpConnectionEstablishmentTimeout,
		BackEndRequestTimedOut,
		CacheDeserializationError,
		ServerKerberosAuthenticationFailure,
		TooManyOutstandingProxyRequestsToForest = 2016,
		TooManyOutstandingProxyRequestsToDag,
		EndpointNotFound = 3001,
		UserNotFound,
		MailboxGuidWithDomainNotFound,
		DatabaseNameNotFound,
		DatabaseGuidNotFound,
		OrganizationMailboxNotFound,
		ServerNotFound,
		ServerVersionNotFound,
		DomainNotFound,
		MailboxExternalDirectoryObjectIdNotFound,
		UnauthenticatedRequest = 4001,
		BadSamlToken,
		ClientDisconnect,
		InvalidOAuthToken,
		CannotReplayRequest = 5001
	}
}
