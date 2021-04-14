using System;

namespace Microsoft.Exchange.Data.Directory.Sync
{
	internal interface ISyncEventLogger
	{
		void LogSerializationFailedEvent(string objectId, int errorCount);

		void LogTooManyObjectReadRestartsEvent(string objectId, int pageLinkReadRestartsLimit);

		void LogFullSyncFallbackDetectedEvent(BackSyncCookie previousCookie, BackSyncCookie currentCookie);
	}
}
