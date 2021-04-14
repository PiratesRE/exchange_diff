using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LogRepairUnexpectedVerifyException : LocalizedException
	{
		public LogRepairUnexpectedVerifyException(string logName, string exceptionText) : base(ReplayStrings.LogRepairUnexpectedVerifyError(logName, exceptionText))
		{
			this.logName = logName;
			this.exceptionText = exceptionText;
		}

		public LogRepairUnexpectedVerifyException(string logName, string exceptionText, Exception innerException) : base(ReplayStrings.LogRepairUnexpectedVerifyError(logName, exceptionText), innerException)
		{
			this.logName = logName;
			this.exceptionText = exceptionText;
		}

		protected LogRepairUnexpectedVerifyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.logName = (string)info.GetValue("logName", typeof(string));
			this.exceptionText = (string)info.GetValue("exceptionText", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("logName", this.logName);
			info.AddValue("exceptionText", this.exceptionText);
		}

		public string LogName
		{
			get
			{
				return this.logName;
			}
		}

		public string ExceptionText
		{
			get
			{
				return this.exceptionText;
			}
		}

		private readonly string logName;

		private readonly string exceptionText;
	}
}
