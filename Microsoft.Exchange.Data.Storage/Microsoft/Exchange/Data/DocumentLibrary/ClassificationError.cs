using System;

namespace Microsoft.Exchange.Data.DocumentLibrary
{
	internal enum ClassificationError
	{
		None,
		ConnectionFailed,
		AccessDenied,
		ObjectNotFound,
		UriTypeNotSupported,
		UnknownError,
		InvalidUri,
		ProxyError
	}
}
