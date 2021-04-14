using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Connections.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ConnectionAlreadyOpenException : TransientException
	{
		public ConnectionAlreadyOpenException() : base(CXStrings.ConnectionAlreadyOpenError)
		{
		}

		public ConnectionAlreadyOpenException(Exception innerException) : base(CXStrings.ConnectionAlreadyOpenError, innerException)
		{
		}

		protected ConnectionAlreadyOpenException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
