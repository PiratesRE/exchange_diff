using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class SubscriptionInvalidPasswordException : TransientException
	{
		public SubscriptionInvalidPasswordException() : base(Strings.SubscriptionInvalidPasswordException)
		{
		}

		public SubscriptionInvalidPasswordException(Exception innerException) : base(Strings.SubscriptionInvalidPasswordException, innerException)
		{
		}

		protected SubscriptionInvalidPasswordException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
