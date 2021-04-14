using System;

namespace Microsoft.Exchange.Net.Mserve
{
	internal enum TopLevelStatusCode
	{
		Success = 1,
		RequestIPNotFoundInACL = 3101,
		HttpsRequired,
		NotAuthorized,
		UnknownCertificate,
		RequestSyntaxError = 4102,
		ConcurrentOperation = 4108,
		TooManyCommands = 4301
	}
}
