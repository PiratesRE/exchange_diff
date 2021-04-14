using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class SubscriptionUpdateTransientException : TransientException
	{
		public SubscriptionUpdateTransientException() : base(Strings.SubscriptionUpdateTransientException)
		{
		}

		public SubscriptionUpdateTransientException(Exception innerException) : base(Strings.SubscriptionUpdateTransientException, innerException)
		{
		}

		protected SubscriptionUpdateTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
