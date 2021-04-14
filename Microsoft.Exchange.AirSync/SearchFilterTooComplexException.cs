using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.AirSync
{
	[Serializable]
	internal class SearchFilterTooComplexException : AirSyncPermanentException
	{
		internal SearchFilterTooComplexException() : base(StatusCode.Sync_ObjectNotFound, false)
		{
		}

		protected SearchFilterTooComplexException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
