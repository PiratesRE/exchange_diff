using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LastLogReplacementUnexpectedTempFilesException : LastLogReplacementException
	{
		public LastLogReplacementUnexpectedTempFilesException(string dbCopy, string logPath) : base(ReplayStrings.LastLogReplacementUnexpectedTempFilesException(dbCopy, logPath))
		{
			this.dbCopy = dbCopy;
			this.logPath = logPath;
		}

		public LastLogReplacementUnexpectedTempFilesException(string dbCopy, string logPath, Exception innerException) : base(ReplayStrings.LastLogReplacementUnexpectedTempFilesException(dbCopy, logPath), innerException)
		{
			this.dbCopy = dbCopy;
			this.logPath = logPath;
		}

		protected LastLogReplacementUnexpectedTempFilesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbCopy = (string)info.GetValue("dbCopy", typeof(string));
			this.logPath = (string)info.GetValue("logPath", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbCopy", this.dbCopy);
			info.AddValue("logPath", this.logPath);
		}

		public string DbCopy
		{
			get
			{
				return this.dbCopy;
			}
		}

		public string LogPath
		{
			get
			{
				return this.logPath;
			}
		}

		private readonly string dbCopy;

		private readonly string logPath;
	}
}
