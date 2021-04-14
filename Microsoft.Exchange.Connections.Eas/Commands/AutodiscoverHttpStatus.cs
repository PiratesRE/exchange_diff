using System;

namespace Microsoft.Exchange.Connections.Eas.Commands
{
	internal enum AutodiscoverHttpStatus
	{
		OK = 200,
		MovedPermanently = 301,
		Redirect,
		TemporaryRedirect = 307,
		Unauthorized = 401,
		NotFound = 404,
		InternalServerError = 500,
		ProxyError = 502,
		ServiceUnavailable
	}
}
