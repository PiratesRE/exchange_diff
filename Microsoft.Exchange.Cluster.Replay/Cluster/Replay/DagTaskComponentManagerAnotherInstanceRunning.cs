using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskComponentManagerAnotherInstanceRunning : DagTaskServerException
	{
		public DagTaskComponentManagerAnotherInstanceRunning() : base(ReplayStrings.DagTaskComponentManagerAnotherInstanceRunning)
		{
		}

		public DagTaskComponentManagerAnotherInstanceRunning(Exception innerException) : base(ReplayStrings.DagTaskComponentManagerAnotherInstanceRunning, innerException)
		{
		}

		protected DagTaskComponentManagerAnotherInstanceRunning(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
