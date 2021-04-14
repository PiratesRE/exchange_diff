using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DbHTServiceShuttingDownException : DatabaseHealthTrackerException
	{
		public DbHTServiceShuttingDownException() : base(ReplayStrings.DbHTServiceShuttingDownException)
		{
		}

		public DbHTServiceShuttingDownException(Exception innerException) : base(ReplayStrings.DbHTServiceShuttingDownException, innerException)
		{
		}

		protected DbHTServiceShuttingDownException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
