using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Transport.Sync.Common.Exceptions;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ConnectionClosedException : NonPromotableTransientException
	{
		public ConnectionClosedException() : base(Strings.ConnectionClosedException)
		{
		}

		public ConnectionClosedException(Exception innerException) : base(Strings.ConnectionClosedException, innerException)
		{
		}

		protected ConnectionClosedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
