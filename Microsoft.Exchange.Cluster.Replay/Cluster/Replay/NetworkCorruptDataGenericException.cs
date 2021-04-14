using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NetworkCorruptDataGenericException : NetworkTransportException
	{
		public NetworkCorruptDataGenericException() : base(ReplayStrings.NetworkCorruptDataGeneric)
		{
		}

		public NetworkCorruptDataGenericException(Exception innerException) : base(ReplayStrings.NetworkCorruptDataGeneric, innerException)
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
