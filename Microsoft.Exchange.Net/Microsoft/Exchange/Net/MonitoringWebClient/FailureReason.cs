using System;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal enum FailureReason
	{
		Unknown,
		CancelationRequested,
		NameResolution,
		NetworkConnection,
		ServerUnreachable,
		RequestTimeout,
		ConnectionTimeout,
		UnexpectedHttpResponseCode,
		MissingKeyword,
		ScenarioTimeout,
		SslNegotiation,
		BrokenAffinity,
		PassiveDatabase,
		BadUserNameOrPassword,
		AccountLocked,
		LogonError,
		CafeFailure,
		CafeActiveDirectoryFailure,
		CafeHighAvailabilityFailure,
		CafeToMailboxNetworkingFailure,
		CafeTimeoutContactingBackend,
		OwaMailboxErrorPage,
		OwaActiveDirectoryErrorPage,
		OwaMServErrorPage,
		OwaErrorPage,
		OwaFindPlacesError,
		EcpMailboxErrorResponse,
		EcpActiveDirectoryErrorResponse,
		EcpErrorPage,
		EcpJsonErrorResponse,
		EcpJsonResultErrorResponse,
		RwsDataMartErrorResponse,
		RwsActiveDirectoryErrorResponse,
		RwsError
	}
}
