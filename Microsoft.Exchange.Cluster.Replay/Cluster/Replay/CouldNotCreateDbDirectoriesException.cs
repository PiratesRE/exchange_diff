using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CouldNotCreateDbDirectoriesException : DatabaseVolumeInfoException
	{
		public CouldNotCreateDbDirectoriesException(string database, string volumeName, string errMsg) : base(ReplayStrings.CouldNotCreateDbDirectoriesException(database, volumeName, errMsg))
		{
			this.database = database;
			this.volumeName = volumeName;
			this.errMsg = errMsg;
		}

		public CouldNotCreateDbDirectoriesException(string database, string volumeName, string errMsg, Exception innerException) : base(ReplayStrings.CouldNotCreateDbDirectoriesException(database, volumeName, errMsg), innerException)
		{
			this.database = database;
			this.volumeName = volumeName;
			this.errMsg = errMsg;
		}

		protected CouldNotCreateDbDirectoriesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.database = (string)info.GetValue("database", typeof(string));
			this.volumeName = (string)info.GetValue("volumeName", typeof(string));
			this.errMsg = (string)info.GetValue("errMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("database", this.database);
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

		private readonly string volumeName;

		private readonly string errMsg;
	}
}
