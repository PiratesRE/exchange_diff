using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LastLogReplacementTempOldFileNotDeletedException : LastLogReplacementException
	{
		public LastLogReplacementTempOldFileNotDeletedException(string dbCopy, string tempOldFile, string error) : base(ReplayStrings.LastLogReplacementTempOldFileNotDeletedException(dbCopy, tempOldFile, error))
		{
			this.dbCopy = dbCopy;
			this.tempOldFile = tempOldFile;
			this.error = error;
		}

		public LastLogReplacementTempOldFileNotDeletedException(string dbCopy, string tempOldFile, string error, Exception innerException) : base(ReplayStrings.LastLogReplacementTempOldFileNotDeletedException(dbCopy, tempOldFile, error), innerException)
		{
			this.dbCopy = dbCopy;
			this.tempOldFile = tempOldFile;
			this.error = error;
		}

		protected LastLogReplacementTempOldFileNotDeletedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbCopy = (string)info.GetValue("dbCopy", typeof(string));
			this.tempOldFile = (string)info.GetValue("tempOldFile", typeof(string));
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbCopy", this.dbCopy);
			info.AddValue("tempOldFile", this.tempOldFile);
			info.AddValue("error", this.error);
		}

		public string DbCopy
		{
			get
			{
				return this.dbCopy;
			}
		}

		public string TempOldFile
		{
			get
			{
				return this.tempOldFile;
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

		private readonly string tempOldFile;

		private readonly string error;
	}
}
