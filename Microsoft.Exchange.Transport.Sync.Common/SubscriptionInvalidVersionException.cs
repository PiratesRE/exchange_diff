using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class SubscriptionInvalidVersionException : TransientException
	{
		public SubscriptionInvalidVersionException() : base(Strings.SubscriptionInvalidVersionException)
		{
		}

		public SubscriptionInvalidVersionException(Exception innerException) : base(Strings.SubscriptionInvalidVersionException, innerException)
		{
		}

		protected SubscriptionInvalidVersionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
