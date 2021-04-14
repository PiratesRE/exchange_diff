using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public enum AttachmentResultCode
	{
		Success,
		GenericFailure,
		AccessDenied,
		Timeout,
		NotFound,
		UploadError,
		UpdatePermissionsError,
		ExchangeOAuthError,
		RestResponseParseError,
		GroupNotFound,
		GroupDocumentsUrlNotFound,
		Cancelled,
		GetUploadFolderError
	}
}
