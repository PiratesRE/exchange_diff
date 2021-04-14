using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DbFixupFailedVolumeHasMaxDbMountPointsException : DatabaseVolumeInfoException
	{
		public DbFixupFailedVolumeHasMaxDbMountPointsException(string dbName, string volumeName) : base(ReplayStrings.DbFixupFailedVolumeHasMaxDbMountPointsException(dbName, volumeName))
		{
			this.dbName = dbName;
			this.volumeName = volumeName;
		}

		public DbFixupFailedVolumeHasMaxDbMountPointsException(string dbName, string volumeName, Exception innerException) : base(ReplayStrings.DbFixupFailedVolumeHasMaxDbMountPointsException(dbName, volumeName), innerException)
		{
			this.dbName = dbName;
			this.volumeName = volumeName;
		}

		protected DbFixupFailedVolumeHasMaxDbMountPointsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbName = (string)info.GetValue("dbName", typeof(string));
			this.volumeName = (string)info.GetValue("volumeName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbName", this.dbName);
			info.AddValue("volumeName", this.volumeName);
		}

		public string DbName
		{
			get
			{
				return this.dbName;
			}
		}

		public string VolumeName
		{
			get
			{
				return this.volumeName;
			}
		}

		private readonly string dbName;

		private readonly string volumeName;
	}
}
