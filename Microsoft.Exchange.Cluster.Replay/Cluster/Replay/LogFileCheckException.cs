using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LogFileCheckException : FileCheckException
	{
		public LogFileCheckException(string logFileName, string errMsg) : base(ReplayStrings.LogFileCheckError(logFileName, errMsg))
		{
			this.logFileName = logFileName;
			this.errMsg = errMsg;
		}

		public LogFileCheckException(string logFileName, string errMsg, Exception innerException) : base(ReplayStrings.LogFileCheckError(logFileName, errMsg), innerException)
		{
			this.logFileName = logFileName;
			this.errMsg = errMsg;
		}

		protected LogFileCheckException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.logFileName = (string)info.GetValue("logFileName", typeof(string));
			this.errMsg = (string)info.GetValue("errMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("logFileName", this.logFileName);
			info.AddValue("errMsg", this.errMsg);
		}

		public string LogFileName
		{
			get
			{
				return this.logFileName;
			}
		}

		public string ErrMsg
		{
			get
			{
				return this.errMsg;
			}
		}

		private readonly string logFileName;

		private readonly string errMsg;
	}
}
