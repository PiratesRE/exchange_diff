using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.EseRepl
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NetworkCorruptDataGenericException : NetworkTransportException
	{
		public NetworkCorruptDataGenericException() : base(Strings.NetworkCorruptDataGeneric)
		{
		}

		public NetworkCorruptDataGenericException(Exception innerException) : base(Strings.NetworkCorruptDataGeneric, innerException)
		{
		}

		protected NetworkCorruptDataGenericException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
