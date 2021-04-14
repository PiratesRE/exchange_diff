using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LastLogReplacementFailedUnexpectedFileFoundException : LastLogReplacementException
	{
		public LastLogReplacementFailedUnexpectedFileFoundException(string dbCopy, string unexpectedFile, string e00log) : base(ReplayStrings.LastLogReplacementFailedUnexpectedFileFoundException(dbCopy, unexpectedFile, e00log))
		{
			this.dbCopy = dbCopy;
			this.unexpectedFile = unexpectedFile;
			this.e00log = e00log;
		}

		public LastLogReplacementFailedUnexpectedFileFoundException(string dbCopy, string unexpectedFile, string e00log, Exception innerException) : base(ReplayStrings.LastLogReplacementFailedUnexpectedFileFoundException(dbCopy, unexpectedFile, e00log), innerException)
		{
			this.dbCopy = dbCopy;
			this.unexpectedFile = unexpectedFile;
			this.e00log = e00log;
		}

		protected LastLogReplacementFailedUnexpectedFileFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbCopy = (string)info.GetValue("dbCopy", typeof(string));
			this.unexpectedFile = (string)info.GetValue("unexpectedFile", typeof(string));
			this.e00log = (string)info.GetValue("e00log", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbCopy", this.dbCopy);
			info.AddValue("unexpectedFile", this.unexpectedFile);
			info.AddValue("e00log", this.e00log);
		}

		public string DbCopy
		{
			get
			{
				return this.dbCopy;
			}
		}

		public string UnexpectedFile
		{
			get
			{
				return this.unexpectedFile;
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

		private readonly string unexpectedFile;

		private readonly string e00log;
	}
}
