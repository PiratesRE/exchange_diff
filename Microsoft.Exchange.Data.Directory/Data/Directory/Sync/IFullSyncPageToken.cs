using System;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal interface IFullSyncPageToken : ISyncCookie
	{
		bool MoreData { get; }

		BackSyncOptions SyncOptions { get; }

		byte[] ToByteArray();

		DateTime Timestamp { get; set; }

		DateTime LastReadFailureStartTime { get; set; }

		void PrepareForFailover();
	}
}
