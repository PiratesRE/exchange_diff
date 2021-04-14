using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.AirSync
{
	[Serializable]
	internal class ChangeTrackingItemRejectedException : AirSyncPermanentException
	{
		internal ChangeTrackingItemRejectedException() : base(false)
		{
		}

		protected ChangeTrackingItemRejectedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
