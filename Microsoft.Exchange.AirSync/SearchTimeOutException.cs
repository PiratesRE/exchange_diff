using System;
using System.Net;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.AirSync
{
	[Serializable]
	internal class SearchTimeOutException : AirSyncPermanentException
	{
		internal SearchTimeOutException() : base(HttpStatusCode.ServiceUnavailable, StatusCode.Sync_NotificationGUID, null, false)
		{
		}

		protected SearchTimeOutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
