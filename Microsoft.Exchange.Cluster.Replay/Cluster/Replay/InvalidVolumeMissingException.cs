using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidVolumeMissingException : DatabaseVolumeInfoException
	{
		public InvalidVolumeMissingException(string volumeName) : base(ReplayStrings.InvalidVolumeMissingException(volumeName))
		{
			this.volumeName = volumeName;
		}

		public InvalidVolumeMissingException(string volumeName, Exception innerException) : base(ReplayStrings.InvalidVolumeMissingException(volumeName), innerException)
		{
			this.volumeName = volumeName;
		}

		protected InvalidVolumeMissingException(SerializationInfo info, StreamingContext context) : base(info, context)
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
