using System;
using System.Net;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.AirSync
{
	[Serializable]
	internal class DataTooLargeException : AirSyncPermanentException
	{
		internal DataTooLargeException(StatusCode airSyncStatusCode) : base(HttpStatusCode.InternalServerError, airSyncStatusCode, null, false)
		{
		}

		protected DataTooLargeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
