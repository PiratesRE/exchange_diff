using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LastLogReplacementFailedFileNotFoundException : LastLogReplacementException
	{
		public LastLogReplacementFailedFileNotFoundException(string dbCopy, string missingFile, string e00log) : base(ReplayStrings.LastLogReplacementFailedFileNotFoundException(dbCopy, missingFile, e00log))
		{
			this.dbCopy = dbCopy;
			this.missingFile = missingFile;
			this.e00log = e00log;
		}

		public LastLogReplacementFailedFileNotFoundException(string dbCopy, string missingFile, string e00log, Exception innerException) : base(ReplayStrings.LastLogReplacementFailedFileNotFoundException(dbCopy, missingFile, e00log), innerException)
		{
			this.dbCopy = dbCopy;
			this.missingFile = missingFile;
			this.e00log = e00log;
		}

		protected LastLogReplacementFailedFileNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbCopy = (string)info.GetValue("dbCopy", typeof(string));
			this.missingFile = (string)info.GetValue("missingFile", typeof(string));
			this.e00log = (string)info.GetValue("e00log", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbCopy", this.dbCopy);
			info.AddValue("missingFile", this.missingFile);
			info.AddValue("e00log", this.e00log);
		}

		public string DbCopy
		{
			get
			{
				return this.dbCopy;
			}
		}

		public string MissingFile
		{
			get
			{
				return this.missingFile;
			}
		}

		public string E00log
		{
			get
			{
				return this.e00log;
			}
		}

		private readonly string dbCopy;

		private readonly string missingFile;

		private readonly string e00log;
	}
}
