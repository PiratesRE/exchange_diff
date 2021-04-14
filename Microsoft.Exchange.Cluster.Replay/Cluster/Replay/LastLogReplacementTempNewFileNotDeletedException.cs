using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LastLogReplacementTempNewFileNotDeletedException : LastLogReplacementException
	{
		public LastLogReplacementTempNewFileNotDeletedException(string dbCopy, string tempNewFile, string error) : base(ReplayStrings.LastLogReplacementTempNewFileNotDeletedException(dbCopy, tempNewFile, error))
		{
			this.dbCopy = dbCopy;
			this.tempNewFile = tempNewFile;
			this.error = error;
		}

		public LastLogReplacementTempNewFileNotDeletedException(string dbCopy, string tempNewFile, string error, Exception innerException) : base(ReplayStrings.LastLogReplacementTempNewFileNotDeletedException(dbCopy, tempNewFile, error), innerException)
		{
			this.dbCopy = dbCopy;
			this.tempNewFile = tempNewFile;
			this.error = error;
		}

		protected LastLogReplacementTempNewFileNotDeletedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbCopy = (string)info.GetValue("dbCopy", typeof(string));
			this.tempNewFile = (string)info.GetValue("tempNewFile", typeof(string));
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbCopy", this.dbCopy);
			info.AddValue("tempNewFile", this.tempNewFile);
			info.AddValue("error", this.error);
		}

		public string DbCopy
		{
			get
			{
				return this.dbCopy;
			}
		}

		public string TempNewFile
		{
			get
			{
				return this.tempNewFile;
			}
		}

		public string Error
		{
			get
			{
				return this.error;
			}
		}

		private readonly string dbCopy;

		private readonly string tempNewFile;

		private readonly string error;
	}
}
