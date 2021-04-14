using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ExchangeVolumeInfoMultipleExMountPointsException : DatabaseVolumeInfoException
	{
		public ExchangeVolumeInfoMultipleExMountPointsException(string volumeName, string exVolRootPath, string exMountPoints) : base(ReplayStrings.ExchangeVolumeInfoMultipleExMountPointsException(volumeName, exVolRootPath, exMountPoints))
		{
			this.volumeName = volumeName;
			this.exVolRootPath = exVolRootPath;
			this.exMountPoints = exMountPoints;
		}

		public ExchangeVolumeInfoMultipleExMountPointsException(string volumeName, string exVolRootPath, string exMountPoints, Exception innerException) : base(ReplayStrings.ExchangeVolumeInfoMultipleExMountPointsException(volumeName, exVolRootPath, exMountPoints), innerException)
		{
			this.volumeName = volumeName;
			this.exVolRootPath = exVolRootPath;
			this.exMountPoints = exMountPoints;
		}

		protected ExchangeVolumeInfoMultipleExMountPointsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.volumeName = (string)info.GetValue("volumeName", typeof(string));
			this.exVolRootPath = (string)info.GetValue("exVolRootPath", typeof(string));
			this.exMountPoints = (string)info.GetValue("exMountPoints", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("volumeName", this.volumeName);
			info.AddValue("exVolRootPath", this.exVolRootPath);
			info.AddValue("exMountPoints", this.exMountPoints);
		}

		public string VolumeName
		{
			get
			{
				return this.volumeName;
			}
		}

		public string ExVolRootPath
		{
			get
			{
				return this.exVolRootPath;
			}
		}

		public string ExMountPoints
		{
			get
			{
				return this.exMountPoints;
			}
		}

		private readonly string volumeName;

		private readonly string exVolRootPath;

		private readonly string exMountPoints;
	}
}
