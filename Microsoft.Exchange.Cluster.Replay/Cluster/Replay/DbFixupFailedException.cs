using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DbFixupFailedException : DatabaseVolumeInfoException
	{
		public DbFixupFailedException(string dbName, string volumeName, string reason) : base(ReplayStrings.DbFixupFailedException(dbName, volumeName, reason))
		{
			this.dbName = dbName;
			this.volumeName = volumeName;
			this.reason = reason;
		}

		public DbFixupFailedException(string dbName, string volumeName, string reason, Exception innerException) : base(ReplayStrings.DbFixupFailedException(dbName, volumeName, reason), innerException)
		{
			this.dbName = dbName;
			this.volumeName = volumeName;
			this.reason = reason;
		}

		protected DbFixupFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbName = (string)info.GetValue("dbName", typeof(string));
			this.volumeName = (string)info.GetValue("volumeName", typeof(string));
			this.reason = (string)info.GetValue("reason", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbName", this.dbName);
			info.AddValue("volumeName", this.volumeName);
			info.AddValue("reason", this.reason);
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

		public string Reason
		{
			get
			{
				return this.reason;
			}
		}

		private readonly string dbName;

		private readonly string volumeName;

		private readonly string reason;
	}
}
