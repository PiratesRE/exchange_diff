using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public class DownloadItemAsyncResult
	{
		public AttachmentResultCode ResultCode { get; set; }

		public FileAttachmentDataProviderItem Item { get; set; }

		public byte[] Bytes { get; set; }
	}
}
