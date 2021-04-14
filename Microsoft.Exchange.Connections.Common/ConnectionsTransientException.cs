using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Connections.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ConnectionsTransientException : TransientException
	{
		public ConnectionsTransientException(LocalizedString message) : base(message)
		{
		}

		public ConnectionsTransientException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected ConnectionsTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
