using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class GranularReplicationOverflowException : TransientException
	{
		public GranularReplicationOverflowException() : base(ReplayStrings.GranularReplicationOverflow)
		{
		}

		public GranularReplicationOverflowException(Exception innerException) : base(ReplayStrings.GranularReplicationOverflow, innerException)
		{
		}

		protected GranularReplicationOverflowException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
