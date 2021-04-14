using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Connections.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ConnectionsPermanentException : LocalizedException
	{
		public ConnectionsPermanentException(LocalizedString message) : base(message)
		{
		}

		public ConnectionsPermanentException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected ConnectionsPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
