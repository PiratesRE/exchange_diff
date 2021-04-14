using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidDbForSeedSpecifiedException : SeedPrepareException
	{
		public InvalidDbForSeedSpecifiedException() : base(ReplayStrings.InvalidDbForSeedSpecifiedException)
		{
		}

		public InvalidDbForSeedSpecifiedException(Exception innerException) : base(ReplayStrings.InvalidDbForSeedSpecifiedException, innerException)
		{
		}

		protected InvalidDbForSeedSpecifiedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
