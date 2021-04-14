using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CouldNotCreateDbMountPointException : DatabaseVolumeInfoException
	{
		public CouldNotCreateDbMountPointException(string database, string dbMountPoint, string volumeName, string errMsg) : base(ReplayStrings.CouldNotCreateDbMountPointException(database, dbMountPoint, volumeName, errMsg))
		{
			this.database = database;
			this.dbMountPoint = dbMountPoint;
			this.volumeName = volumeName;
			this.errMsg = errMsg;
		}

		public CouldNotCreateDbMountPointException(string database, string dbMountPoint, string volumeName, string errMsg, Exception innerException) : base(ReplayStrings.CouldNotCreateDbMountPointException(database, dbMountPoint, volumeName, errMsg), innerException)
		{
			this.database = database;
			this.dbMountPoint = dbMountPoint;
			this.volumeName = volumeName;
			this.errMsg = errMsg;
		}

		protected CouldNotCreateDbMountPointException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.database = (string)info.GetValue("database", typeof(string));
			this.dbMountPoint = (string)info.GetValue("dbMountPoint", typeof(string));
			this.volumeName = (string)info.GetValue("volumeName", typeof(string));
			this.errMsg = (string)info.GetValue("errMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("database", this.database);
			info.AddValue("dbMountPoint", this.dbMountPoint);
			info.AddValue("volumeName", this.volumeName);
			info.AddValue("errMsg", this.errMsg);
		}

		public string Database
		{
			get
			{
				return this.database;
			}
		}

		public string DbMountPoint
		{
			get
			{
				return this.dbMountPoint;
			}
		}

		public string VolumeName
		{
			get
			{
				return this.volumeName;
			}
		}

		public string ErrMsg
		{
			get
			{
				return this.errMsg;
			}
		}

		private readonly string database;

		private readonly string dbMountPoint;

		private readonly string volumeName;

		private readonly string errMsg;
	}
}
