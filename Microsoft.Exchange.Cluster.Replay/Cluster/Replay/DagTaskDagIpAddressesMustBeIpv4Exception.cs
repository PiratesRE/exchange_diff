using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskDagIpAddressesMustBeIpv4Exception : LocalizedException
	{
		public DagTaskDagIpAddressesMustBeIpv4Exception() : base(ReplayStrings.DagTaskDagIpAddressesMustBeIpv4Exception)
		{
		}

		public DagTaskDagIpAddressesMustBeIpv4Exception(Exception innerException) : base(ReplayStrings.DagTaskDagIpAddressesMustBeIpv4Exception, innerException)
		{
		}

		protected DagTaskDagIpAddressesMustBeIpv4Exception(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
