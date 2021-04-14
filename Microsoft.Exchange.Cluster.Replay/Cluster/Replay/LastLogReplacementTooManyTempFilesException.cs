using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LastLogReplacementTooManyTempFilesException : LastLogReplacementException
	{
		public LastLogReplacementTooManyTempFilesException(string dbCopy, string filter, int count, string logPath) : base(ReplayStrings.LastLogReplacementTooManyTempFilesException(dbCopy, filter, count, logPath))
		{
			this.dbCopy = dbCopy;
			this.filter = filter;
			this.count = count;
			this.logPath = logPath;
		}

		public LastLogReplacementTooManyTempFilesException(string dbCopy, string filter, int count, string logPath, Exception innerException) : base(ReplayStrings.LastLogReplacementTooManyTempFilesException(dbCopy, filter, count, logPath), innerException)
		{
			this.dbCopy = dbCopy;
			this.filter = filter;
			this.count = count;
			this.logPath = logPath;
		}

		protected LastLogReplacementTooManyTempFilesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbCopy = (string)info.GetValue("dbCopy", typeof(string));
			this.filter = (string)info.GetValue("filter", typeof(string));
			this.count = (int)info.GetValue("count", typeof(int));
			this.logPath = (string)info.GetValue("logPath", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbCopy", this.dbCopy);
			info.AddValue("filter", this.filter);
			info.AddValue("count", this.count);
			info.AddValue("logPath", this.logPath);
		}

		public string DbCopy
		{
			get
			{
				return this.dbCopy;
			}
		}

		public string Filter
		{
			get
			{
				return this.filter;
			}
		}

		public int Count
		{
			get
			{
				return this.count;
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

		private readonly string filter;

		private readonly int count;

		private readonly string logPath;
	}
}
