using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Connections.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ConnectionClosedException : TransientException
	{
		public ConnectionClosedException() : base(CXStrings.ConnectionClosedError)
		{
		}

		public ConnectionClosedException(Exception innerException) : base(CXStrings.ConnectionClosedError, innerException)
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
