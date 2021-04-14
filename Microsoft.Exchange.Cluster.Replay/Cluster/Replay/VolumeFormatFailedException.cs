using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class VolumeFormatFailedException : DatabaseVolumeInfoException
	{
		public VolumeFormatFailedException(string volumeName, string mountPoint, string err) : base(ReplayStrings.VolumeFormatFailedException(volumeName, mountPoint, err))
		{
			this.volumeName = volumeName;
			this.mountPoint = mountPoint;
			this.err = err;
		}

		public VolumeFormatFailedException(string volumeName, string mountPoint, string err, Exception innerException) : base(ReplayStrings.VolumeFormatFailedException(volumeName, mountPoint, err), innerException)
		{
			this.volumeName = volumeName;
			this.mountPoint = mountPoint;
			this.err = err;
		}

		protected VolumeFormatFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.volumeName = (string)info.GetValue("volumeName", typeof(string));
			this.mountPoint = (string)info.GetValue("mountPoint", typeof(string));
			this.err = (string)info.GetValue("err", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("volumeName", this.volumeName);
			info.AddValue("mountPoint", this.mountPoint);
			info.AddValue("err", this.err);
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

		public string Err
		{
			get
			{
				return this.err;
			}
		}

		private readonly string volumeName;

		private readonly string mountPoint;

		private readonly string err;
	}
}
