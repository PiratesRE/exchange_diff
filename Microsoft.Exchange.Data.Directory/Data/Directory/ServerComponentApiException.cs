using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ServerComponentApiException : TransientException
	{
		public ServerComponentApiException(LocalizedString message) : base(message)
		{
		}

		public ServerComponentApiException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected ServerComponentApiException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
