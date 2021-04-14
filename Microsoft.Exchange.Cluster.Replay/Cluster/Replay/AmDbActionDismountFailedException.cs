using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmDbActionDismountFailedException : AmDbActionException
	{
		public AmDbActionDismountFailedException() : base(ReplayStrings.AmDbActionDismountFailedException)
		{
		}

		public AmDbActionDismountFailedException(Exception innerException) : base(ReplayStrings.AmDbActionDismountFailedException, innerException)
		{
		}

		protected AmDbActionDismountFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
