using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskClusteringRequiresEnterpriseSkuException : DagTaskServerException
	{
		public DagTaskClusteringRequiresEnterpriseSkuException() : base(ReplayStrings.DagTaskClusteringRequiresEnterpriseSkuException)
		{
		}

		public DagTaskClusteringRequiresEnterpriseSkuException(Exception innerException) : base(ReplayStrings.DagTaskClusteringRequiresEnterpriseSkuException, innerException)
		{
		}

		protected DagTaskClusteringRequiresEnterpriseSkuException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
