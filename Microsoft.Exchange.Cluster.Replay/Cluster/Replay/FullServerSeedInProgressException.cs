using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FullServerSeedInProgressException : SeederServerException
	{
		public FullServerSeedInProgressException() : base(ReplayStrings.FullServerSeedInProgressException)
		{
		}

		public FullServerSeedInProgressException(Exception innerException) : base(ReplayStrings.FullServerSeedInProgressException, innerException)
		{
		}

		protected FullServerSeedInProgressException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
