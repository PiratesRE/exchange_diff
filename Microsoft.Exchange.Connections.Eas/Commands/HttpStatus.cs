using System;

namespace Microsoft.Exchange.Connections.Eas.Commands
{
	public enum HttpStatus
	{
		OK = 200,
		BadRequest = 400,
		Unauthorized,
		Forbidden = 403,
		NotFound,
		NeedProvisioning = 449,
		ActiveSyncRedirect = 451,
		InternalServerError = 500,
		NotImplemented,
		ProxyError,
		ServiceUnavailable,
		VersionNotSupported = 505,
		InsufficientDiskSapce = 507
	}
}
