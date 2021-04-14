using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CouldNotFindVolumeForFormatException : DatabaseVolumeInfoException
	{
		public CouldNotFindVolumeForFormatException() : base(ReplayStrings.CouldNotFindVolumeForFormatException)
		{
		}

		public CouldNotFindVolumeForFormatException(Exception innerException) : base(ReplayStrings.CouldNotFindVolumeForFormatException, innerException)
		{
		}

		protected CouldNotFindVolumeForFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
