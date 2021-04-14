using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.AirSync
{
	[Serializable]
	internal class SearchNeedToFolderSyncException : AirSyncPermanentException
	{
		internal SearchNeedToFolderSyncException() : base(StatusCode.Sync_NotificationsNotProvisioned, false)
		{
			base.ErrorStringForProtocolLogger = "FolderSyncFirst";
		}

		protected SearchNeedToFolderSyncException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
