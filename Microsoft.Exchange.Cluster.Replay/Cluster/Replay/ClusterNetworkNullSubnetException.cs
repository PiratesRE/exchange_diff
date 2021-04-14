using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ClusterNetworkNullSubnetException : TransientException
	{
		public ClusterNetworkNullSubnetException(string clusterNetworkName) : base(ReplayStrings.ClusterNetworkNullSubnetError(clusterNetworkName))
		{
			this.clusterNetworkName = clusterNetworkName;
		}

		public ClusterNetworkNullSubnetException(string clusterNetworkName, Exception innerException) : base(ReplayStrings.ClusterNetworkNullSubnetError(clusterNetworkName), innerException)
		{
			this.clusterNetworkName = clusterNetworkName;
		}

		protected ClusterNetworkNullSubnetException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.clusterNetworkName = (string)info.GetValue("clusterNetworkName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("clusterNetworkName", this.clusterNetworkName);
		}

		public string ClusterNetworkName
		{
			get
			{
				return this.clusterNetworkName;
			}
		}

		private readonly string clusterNetworkName;
	}
}
