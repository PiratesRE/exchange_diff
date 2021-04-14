using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SeederEcNotEnoughDiskException : SeederServerException
	{
		public SeederEcNotEnoughDiskException() : base(ReplayStrings.SeederEcNotEnoughDiskException)
		{
		}

		public SeederEcNotEnoughDiskException(Exception innerException) : base(ReplayStrings.SeederEcNotEnoughDiskException, innerException)
		{
		}

		protected SeederEcNotEnoughDiskException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
