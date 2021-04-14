using System;

namespace Microsoft.Exchange.Net
{
	internal sealed class DownloadCompleteEventArgs : EventArgs
	{
		public DownloadCompleteEventArgs(long bytesDownloaded) : this(bytesDownloaded, 0L)
		{
		}

		public DownloadCompleteEventArgs(long bytesDownloaded, long bytesUploaded)
		{
			this.bytesDownloaded = bytesDownloaded;
			this.bytesUploaded = bytesUploaded;
		}

		public long BytesDownloaded
		{
			get
			{
				return this.bytesDownloaded;
			}
		}

		public long BytesUploaded
		{
			get
			{
				return this.bytesUploaded;
			}
		}

		private long bytesDownloaded;

		private long bytesUploaded;
	}
}
