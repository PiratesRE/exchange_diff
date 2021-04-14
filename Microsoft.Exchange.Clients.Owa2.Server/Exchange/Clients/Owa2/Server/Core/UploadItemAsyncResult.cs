using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class UploadItemAsyncResult
	{
		public AttachmentResultCode ResultCode { get; set; }

		public FileAttachmentDataProviderItem Item { get; set; }
	}
}
