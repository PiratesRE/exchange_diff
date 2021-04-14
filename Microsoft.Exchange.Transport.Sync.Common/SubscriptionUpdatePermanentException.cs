using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class SubscriptionUpdatePermanentException : LocalizedException
	{
		public SubscriptionUpdatePermanentException() : base(Strings.SubscriptionUpdatePermanentException)
		{
		}

		public SubscriptionUpdatePermanentException(Exception innerException) : base(Strings.SubscriptionUpdatePermanentException, innerException)
		{
		}

		protected SubscriptionUpdatePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
