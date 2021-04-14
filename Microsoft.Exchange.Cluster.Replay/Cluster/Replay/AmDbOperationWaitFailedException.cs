using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AmDbOperationWaitFailedException : AmDbOperationException
	{
		public AmDbOperationWaitFailedException() : base(ReplayStrings.AmDbOperationWaitFailedException)
		{
		}

		public AmDbOperationWaitFailedException(Exception innerException) : base(ReplayStrings.AmDbOperationWaitFailedException, innerException)
		{
		}

		protected AmDbOperationWaitFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
