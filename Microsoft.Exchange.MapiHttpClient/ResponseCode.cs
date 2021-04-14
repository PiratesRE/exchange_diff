using System;

namespace Microsoft.Exchange.MapiHttp
{
	public enum ResponseCode : uint
	{
		Success,
		UnknownFailure,
		InvalidVerb,
		InvalidPath,
		InvalidHeader,
		InvalidRequestType,
		InvalidContextCookie,
		MissingHeader,
		AnonymousNotAllowed,
		TooLarge,
		ContextNotFound,
		NotContextOwner,
		InvalidPayload,
		MissingCookie,
		NoClient,
		InvalidSequence,
		EndpointDisabled,
		InvalidResponse,
		EndpointShuttingDown
	}
}
