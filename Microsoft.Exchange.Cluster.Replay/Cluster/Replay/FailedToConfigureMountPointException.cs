using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToConfigureMountPointException : DatabaseVolumeInfoException
	{
		public FailedToConfigureMountPointException(string volumeName, string reason) : base(ReplayStrings.FailedToConfigureMountPointException(volumeName, reason))
		{
			this.volumeName = volumeName;
			this.reason = reason;
		}

		public FailedToConfigureMountPointException(string volumeName, string reason, Exception innerException) : base(ReplayStrings.FailedToConfigureMountPointException(volumeName, reason), innerException)
		{
			this.volumeName = volumeName;
			this.reason = reason;
		}

		protected FailedToConfigureMountPointException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.volumeName = (string)info.GetValue("volumeName", typeof(string));
			this.reason = (string)info.GetValue("reason", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("volumeName", this.volumeName);
			info.AddValue("reason", this.reason);
		}

		public string VolumeName
		{
			get
			{
				return this.volumeName;
			}
		}

		public string Reason
		{
			get
			{
				return this.reason;
			}
		}

		private readonly string volumeName;

		private readonly string reason;
	}
}
