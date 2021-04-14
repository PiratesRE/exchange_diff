using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.AirSync
{
	[Serializable]
	internal class SearchProtocolErrorException : AirSyncPermanentException
	{
		internal SearchProtocolErrorException() : base(StatusCode.Sync_ProtocolVersionMismatch, false)
		{
		}

		protected SearchProtocolErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
