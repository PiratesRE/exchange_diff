using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CouldNotCreateDbMountPointFolderException : DatabaseVolumeInfoException
	{
		public CouldNotCreateDbMountPointFolderException(string database, string dbMountPoint, string errMsg) : base(ReplayStrings.CouldNotCreateDbMountPointFolderException(database, dbMountPoint, errMsg))
		{
			this.database = database;
			this.dbMountPoint = dbMountPoint;
			this.errMsg = errMsg;
		}

		public CouldNotCreateDbMountPointFolderException(string database, string dbMountPoint, string errMsg, Exception innerException) : base(ReplayStrings.CouldNotCreateDbMountPointFolderException(database, dbMountPoint, errMsg), innerException)
		{
			this.database = database;
			this.dbMountPoint = dbMountPoint;
			this.errMsg = errMsg;
		}

		protected CouldNotCreateDbMountPointFolderException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.database = (string)info.GetValue("database", typeof(string));
			this.dbMountPoint = (string)info.GetValue("dbMountPoint", typeof(string));
			this.errMsg = (string)info.GetValue("errMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("database", this.database);
			info.AddValue("dbMountPoint", this.dbMountPoint);
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

		public string ErrMsg
		{
			get
			{
				return this.errMsg;
			}
		}

		private readonly string database;

		private readonly string dbMountPoint;

		private readonly string errMsg;
	}
}
