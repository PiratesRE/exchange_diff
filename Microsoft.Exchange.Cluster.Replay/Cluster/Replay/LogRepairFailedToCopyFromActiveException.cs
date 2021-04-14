using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LogRepairFailedToCopyFromActiveException : LocalizedException
	{
		public LogRepairFailedToCopyFromActiveException(string tempLogName, string exceptionText) : base(ReplayStrings.LogRepairFailedToCopyFromActive(tempLogName, exceptionText))
		{
			this.tempLogName = tempLogName;
			this.exceptionText = exceptionText;
		}

		public LogRepairFailedToCopyFromActiveException(string tempLogName, string exceptionText, Exception innerException) : base(ReplayStrings.LogRepairFailedToCopyFromActive(tempLogName, exceptionText), innerException)
		{
			this.tempLogName = tempLogName;
			this.exceptionText = exceptionText;
		}

		protected LogRepairFailedToCopyFromActiveException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.tempLogName = (string)info.GetValue("tempLogName", typeof(string));
			this.exceptionText = (string)info.GetValue("exceptionText", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("tempLogName", this.tempLogName);
			info.AddValue("exceptionText", this.exceptionText);
		}

		public string TempLogName
		{
			get
			{
				return this.tempLogName;
			}
		}

		public string ExceptionText
		{
			get
			{
				return this.exceptionText;
			}
		}

		private readonly string tempLogName;

		private readonly string exceptionText;
	}
}
