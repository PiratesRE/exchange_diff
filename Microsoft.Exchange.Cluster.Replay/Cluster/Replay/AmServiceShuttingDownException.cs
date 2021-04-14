using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmServiceShuttingDownException : AmCommonException
	{
		public AmServiceShuttingDownException() : base(ReplayStrings.AmServiceShuttingDown)
		{
		}

		public AmServiceShuttingDownException(Exception innerException) : base(ReplayStrings.AmServiceShuttingDown, innerException)
		{
		}

		protected AmServiceShuttingDownException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
