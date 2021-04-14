using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public enum WacAttachmentStatus
	{
		Success,
		UnknownError,
		NotFound,
		ProtectedByUnsupportedIrm,
		UnsupportedObjectType,
		InvalidRequest,
		AttachmentDataProviderError,
		UploadFailed,
		WacDiscoveryFailed
	}
}
