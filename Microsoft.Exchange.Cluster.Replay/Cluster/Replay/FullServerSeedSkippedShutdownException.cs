using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FullServerSeedSkippedShutdownException : SeederServerException
	{
		public FullServerSeedSkippedShutdownException() : base(ReplayStrings.FullServerSeedSkippedShutdownException)
		{
		}

		public FullServerSeedSkippedShutdownException(Exception innerException) : base(ReplayStrings.FullServerSeedSkippedShutdownException, innerException)
		{
		}

		protected FullServerSeedSkippedShutdownException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
