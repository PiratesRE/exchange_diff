using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct ManagedStore_FastTransferTags
	{
		public const int SourceSend = 0;

		public const int IcsDownload = 1;

		public const int IcsDownloadState = 2;

		public const int IcsUploadState = 3;

		public const int FaultInjection = 20;

		public static Guid guid = new Guid("e8d090ac-ab71-4752-b432-0b86b6e380e6");
	}
}
