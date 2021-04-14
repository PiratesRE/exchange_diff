using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NetworkDataOverflowGenericException : NetworkTransportException
	{
		public NetworkDataOverflowGenericException() : base(ReplayStrings.NetworkDataOverflowGeneric)
		{
		}

		public NetworkDataOverflowGenericException(Exception innerException) : base(ReplayStrings.NetworkDataOverflowGeneric, innerException)
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
