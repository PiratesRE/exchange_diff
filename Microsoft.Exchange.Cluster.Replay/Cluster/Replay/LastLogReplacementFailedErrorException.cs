using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LastLogReplacementFailedErrorException : LastLogReplacementException
	{
		public LastLogReplacementFailedErrorException(string dbCopy, string e00log, string error) : base(ReplayStrings.LastLogReplacementFailedErrorException(dbCopy, e00log, error))
		{
			this.dbCopy = dbCopy;
			this.e00log = e00log;
			this.error = error;
		}

		public LastLogReplacementFailedErrorException(string dbCopy, string e00log, string error, Exception innerException) : base(ReplayStrings.LastLogReplacementFailedErrorException(dbCopy, e00log, error), innerException)
		{
			this.dbCopy = dbCopy;
			this.e00log = e00log;
			this.error = error;
		}

		protected LastLogReplacementFailedErrorException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dbCopy = (string)info.GetValue("dbCopy", typeof(string));
			this.e00log = (string)info.GetValue("e00log", typeof(string));
			this.error = (string)info.GetValue("error", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dbCopy", this.dbCopy);
			info.AddValue("e00log", this.e00log);
			info.AddValue("error", this.error);
		}

		public string DbCopy
		{
			get
			{
				return this.dbCopy;
			}
		}

		public string E00log
		{
			get
			{
				return this.e00log;
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

		private readonly string e00log;

		private readonly string error;
	}
}
