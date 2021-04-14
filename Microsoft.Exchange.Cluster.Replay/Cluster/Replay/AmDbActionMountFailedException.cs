using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmDbActionMountFailedException : AmDbActionException
	{
		public AmDbActionMountFailedException() : base(ReplayStrings.AmDbActionMountFailedException)
		{
		}

		public AmDbActionMountFailedException(Exception innerException) : base(ReplayStrings.AmDbActionMountFailedException, innerException)
		{
		}

		protected AmDbActionMountFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
