using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Mapi.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MapiNetworkErrorException : MapiTransientException
	{
		public MapiNetworkErrorException(LocalizedString message) : base(message)
		{
		}

		public MapiNetworkErrorException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected MapiNetworkErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
