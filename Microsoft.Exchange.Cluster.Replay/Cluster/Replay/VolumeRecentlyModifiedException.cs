using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class VolumeRecentlyModifiedException : DatabaseVolumeInfoException
	{
		public VolumeRecentlyModifiedException(string volumeName, TimeSpan threshold, string dateTimeUtc, string mountPoint, string lastUpdatePath) : base(ReplayStrings.VolumeRecentlyModifiedException(volumeName, threshold, dateTimeUtc, mountPoint, lastUpdatePath))
		{
			this.volumeName = volumeName;
			this.threshold = threshold;
			this.dateTimeUtc = dateTimeUtc;
			this.mountPoint = mountPoint;
			this.lastUpdatePath = lastUpdatePath;
		}

		public VolumeRecentlyModifiedException(string volumeName, TimeSpan threshold, string dateTimeUtc, string mountPoint, string lastUpdatePath, Exception innerException) : base(ReplayStrings.VolumeRecentlyModifiedException(volumeName, threshold, dateTimeUtc, mountPoint, lastUpdatePath), innerException)
		{
			this.volumeName = volumeName;
			this.threshold = threshold;
			this.dateTimeUtc = dateTimeUtc;
			this.mountPoint = mountPoint;
			this.lastUpdatePath = lastUpdatePath;
		}

		protected VolumeRecentlyModifiedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.volumeName = (string)info.GetValue("volumeName", typeof(string));
			this.threshold = (TimeSpan)info.GetValue("threshold", typeof(TimeSpan));
			this.dateTimeUtc = (string)info.GetValue("dateTimeUtc", typeof(string));
			this.mountPoint = (string)info.GetValue("mountPoint", typeof(string));
			this.lastUpdatePath = (string)info.GetValue("lastUpdatePath", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("volumeName", this.volumeName);
			info.AddValue("threshold", this.threshold);
			info.AddValue("dateTimeUtc", this.dateTimeUtc);
			info.AddValue("mountPoint", this.mountPoint);
			info.AddValue("lastUpdatePath", this.lastUpdatePath);
		}

		public string VolumeName
		{
			get
			{
				return this.volumeName;
			}
		}

		public TimeSpan Threshold
		{
			get
			{
				return this.threshold;
			}
		}

		public string DateTimeUtc
		{
			get
			{
				return this.dateTimeUtc;
			}
		}

		public string MountPoint
		{
			get
			{
				return this.mountPoint;
			}
		}

		public string LastUpdatePath
		{
			get
			{
				return this.lastUpdatePath;
			}
		}

		private readonly string volumeName;

		private readonly TimeSpan threshold;

		private readonly string dateTimeUtc;

		private readonly string mountPoint;

		private readonly string lastUpdatePath;
	}
}
