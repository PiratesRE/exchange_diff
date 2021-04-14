using System;

namespace Microsoft.Exchange.Services.OnlineMeetings
{
	internal enum ErrorCode
	{
		Unknown,
		BadRequest = 400,
		Forbidden = 403,
		NotFound,
		MethodNotAllowed,
		ClientTimeout = 408,
		Conflict,
		Gone,
		PreconditionFailed = 412,
		EntityTooLarge,
		UnsupportedMediaType = 415,
		PreconditionRequired = 428,
		TooManyRequests,
		ServiceFailure = 500,
		BadGateway = 502,
		ServiceUnavailable,
		Timeout,
		ExpectedFailure = 700,
		LocalFailure = 710,
		RemoteFailure = 720,
		Informational = 730
	}
}
