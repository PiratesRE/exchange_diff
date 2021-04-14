using System;

namespace Microsoft.Office.CompliancePolicy
{
	internal sealed class DownloadCompleteEventArgs : EventArgs
	{
		public DownloadCompleteEventArgs(long bytesDownloaded) : this(bytesDownloaded, 0L)
		{
		}

		public DownloadCompleteEventArgs(long bytesDownloaded, long bytesUploaded)
		{
			this.BytesDownloaded = bytesDownloaded;
			this.BytesUploaded = bytesUploaded;
		}

		public long BytesDownloaded { get; private set; }

		public long BytesUploaded { get; private set; }
	}
}
