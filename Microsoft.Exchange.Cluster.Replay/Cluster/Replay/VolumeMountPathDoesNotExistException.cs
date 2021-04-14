using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class VolumeMountPathDoesNotExistException : DatabaseVolumeInfoException
	{
		public VolumeMountPathDoesNotExistException() : base(ReplayStrings.VolumeMountPathDoesNotExistException)
		{
		}

		public VolumeMountPathDoesNotExistException(Exception innerException) : base(ReplayStrings.VolumeMountPathDoesNotExistException, innerException)
		{
		}

		protected VolumeMountPathDoesNotExistException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
