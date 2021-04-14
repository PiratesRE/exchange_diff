using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskSetDagNeedsAllNodesUpToChangeQuorumException : DagTaskServerException
	{
		public DagTaskSetDagNeedsAllNodesUpToChangeQuorumException(string machineNames) : base(ReplayStrings.DagTaskSetDagNeedsAllNodesUpToChangeQuorumException(machineNames))
		{
			this.machineNames = machineNames;
		}

		public DagTaskSetDagNeedsAllNodesUpToChangeQuorumException(string machineNames, Exception innerException) : base(ReplayStrings.DagTaskSetDagNeedsAllNodesUpToChangeQuorumException(machineNames), innerException)
		{
			this.machineNames = machineNames;
		}

		protected DagTaskSetDagNeedsAllNodesUpToChangeQuorumException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.machineNames = (string)info.GetValue("machineNames", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("machineNames", this.machineNames);
		}

		public string MachineNames
		{
			get
			{
				return this.machineNames;
			}
		}

		private readonly string machineNames;
	}
}
