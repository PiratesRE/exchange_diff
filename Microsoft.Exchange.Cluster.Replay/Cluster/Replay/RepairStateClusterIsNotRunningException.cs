using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RepairStateClusterIsNotRunningException : RepairStateException
	{
		public RepairStateClusterIsNotRunningException() : base(ReplayStrings.RepairStateClusterIsNotRunning)
		{
		}

		public RepairStateClusterIsNotRunningException(Exception innerException) : base(ReplayStrings.RepairStateClusterIsNotRunning, innerException)
		{
		}

		protected RepairStateClusterIsNotRunningException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
