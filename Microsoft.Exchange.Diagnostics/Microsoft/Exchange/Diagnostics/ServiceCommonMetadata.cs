using System;

namespace Microsoft.Exchange.Diagnostics
{
	public enum ServiceCommonMetadata
	{
		ServerHostName,
		ClientIpAddress,
		RequestSize,
		ResponseSize,
		Cookie,
		HttpStatus,
		HttpMethod,
		IsAuthenticated,
		AuthenticatedUser,
		ErrorCode,
		LiveIdBasicLog,
		LiveIdBasicError,
		LiveIdNegotiateError,
		OAuthError,
		OAuthErrorCategory,
		OAuthExtraInfo,
		OAuthToken,
		BackEndGenericInfo,
		GenericInfo,
		AuthenticationErrors,
		GenericErrors,
		ExceptionName,
		CorrectBEServerToUse,
		IsDuplicatedAction,
		ServerVersionMajor,
		ServerVersionMinor,
		ServerVersionBuild,
		ServerVersionRevision
	}
}
