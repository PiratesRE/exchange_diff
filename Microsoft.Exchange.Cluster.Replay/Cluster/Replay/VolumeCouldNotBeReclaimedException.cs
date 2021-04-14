using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class VolumeCouldNotBeReclaimedException : DatabaseVolumeInfoException
	{
		public VolumeCouldNotBeReclaimedException(string volumeName, string mountPoint) : base(ReplayStrings.VolumeCouldNotBeReclaimedException(volumeName, mountPoint))
		{
			this.volumeName = volumeName;
			this.mountPoint = mountPoint;
		}

		public VolumeCouldNotBeReclaimedException(string volumeName, string mountPoint, Exception innerException) : base(ReplayStrings.VolumeCouldNotBeReclaimedException(volumeName, mountPoint), innerException)
		{
			this.volumeName = volumeName;
			this.mountPoint = mountPoint;
		}

		protected VolumeCouldNotBeReclaimedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.volumeName = (string)info.GetValue("volumeName", typeof(string));
			this.mountPoint = (string)info.GetValue("mountPoint", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("volumeName", this.volumeName);
			info.AddValue("mountPoint", this.mountPoint);
		}

		public string VolumeName
		{
			get
			{
				return this.volumeName;
			}
		}

		public string MountPoint
		{
			get
			{
				return this.mountPoint;
			}
		}

		private readonly string volumeName;

		private readonly string mountPoint;
	}
}
