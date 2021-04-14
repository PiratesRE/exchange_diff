using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmDbActionMoveFailedException : AmDbActionException
	{
		public AmDbActionMoveFailedException() : base(ReplayStrings.AmDbActionMoveFailedException)
		{
		}

		public AmDbActionMoveFailedException(Exception innerException) : base(ReplayStrings.AmDbActionMoveFailedException, innerException)
		{
		}

		protected AmDbActionMoveFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
