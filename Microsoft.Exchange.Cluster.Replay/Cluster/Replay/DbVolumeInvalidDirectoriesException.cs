using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DbVolumeInvalidDirectoriesException : DatabaseVolumeInfoException
	{
		public DbVolumeInvalidDirectoriesException(string volumeName, string mountedFolder, int numExpected, int numActual) : base(ReplayStrings.DbVolumeInvalidDirectoriesException(volumeName, mountedFolder, numExpected, numActual))
		{
			this.volumeName = volumeName;
			this.mountedFolder = mountedFolder;
			this.numExpected = numExpected;
			this.numActual = numActual;
		}

		public DbVolumeInvalidDirectoriesException(string volumeName, string mountedFolder, int numExpected, int numActual, Exception innerException) : base(ReplayStrings.DbVolumeInvalidDirectoriesException(volumeName, mountedFolder, numExpected, numActual), innerException)
		{
			this.volumeName = volumeName;
			this.mountedFolder = mountedFolder;
			this.numExpected = numExpected;
			this.numActual = numActual;
		}

		protected DbVolumeInvalidDirectoriesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.volumeName = (string)info.GetValue("volumeName", typeof(string));
			this.mountedFolder = (string)info.GetValue("mountedFolder", typeof(string));
			this.numExpected = (int)info.GetValue("numExpected", typeof(int));
			this.numActual = (int)info.GetValue("numActual", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("volumeName", this.volumeName);
			info.AddValue("mountedFolder", this.mountedFolder);
			info.AddValue("numExpected", this.numExpected);
			info.AddValue("numActual", this.numActual);
		}

		public string VolumeName
		{
			get
			{
				return this.volumeName;
			}
		}

		public string MountedFolder
		{
			get
			{
				return this.mountedFolder;
			}
		}

		public int NumExpected
		{
			get
			{
				return this.numExpected;
			}
		}

		public int NumActual
		{
			get
			{
				return this.numActual;
			}
		}

		private readonly string volumeName;

		private readonly string mountedFolder;

		private readonly int numExpected;

		private readonly int numActual;
	}
}
