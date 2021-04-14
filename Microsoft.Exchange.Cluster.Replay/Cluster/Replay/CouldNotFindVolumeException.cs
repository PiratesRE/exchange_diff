using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CouldNotFindVolumeException : DatabaseVolumeInfoException
	{
		public CouldNotFindVolumeException(string volumeName) : base(ReplayStrings.CouldNotFindVolumeException(volumeName))
		{
			this.volumeName = volumeName;
		}

		public CouldNotFindVolumeException(string volumeName, Exception innerException) : base(ReplayStrings.CouldNotFindVolumeException(volumeName), innerException)
		{
			this.volumeName = volumeName;
		}

		protected CouldNotFindVolumeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.volumeName = (string)info.GetValue("volumeName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("volumeName", this.volumeName);
		}

		public string VolumeName
		{
			get
			{
				return this.volumeName;
			}
		}

		private readonly string volumeName;
	}
}
