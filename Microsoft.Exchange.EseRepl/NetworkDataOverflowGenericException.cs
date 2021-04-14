using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.EseRepl
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NetworkDataOverflowGenericException : NetworkTransportException
	{
		public NetworkDataOverflowGenericException() : base(Strings.NetworkDataOverflowGeneric)
		{
		}

		public NetworkDataOverflowGenericException(Exception innerException) : base(Strings.NetworkDataOverflowGeneric, innerException)
		{
		}

		protected NetworkDataOverflowGenericException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
