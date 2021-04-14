using System;
using System.Net;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.AirSync
{
	[Serializable]
	internal class DiscoveryInfoMissingException : AirSyncPermanentException
	{
		internal DiscoveryInfoMissingException(HttpStatusCode httpStatusCode, StatusCode airSyncStatusCode, LocalizedString message) : base(httpStatusCode, airSyncStatusCode, message, true)
		{
		}

		protected DiscoveryInfoMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			base.LogExceptionToEventLog = true;
		}
	}
}
