using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class LogRepairFailedToVerifyFromActiveException : LocalizedException
	{
		public LogRepairFailedToVerifyFromActiveException(string tempLogName, string activeServerName, string exceptionText) : base(ReplayStrings.LogRepairFailedToVerifyFromActive(tempLogName, activeServerName, exceptionText))
		{
			this.tempLogName = tempLogName;
			this.activeServerName = activeServerName;
			this.exceptionText = exceptionText;
		}

		public LogRepairFailedToVerifyFromActiveException(string tempLogName, string activeServerName, string exceptionText, Exception innerException) : base(ReplayStrings.LogRepairFailedToVerifyFromActive(tempLogName, activeServerName, exceptionText), innerException)
		{
			this.tempLogName = tempLogName;
			this.activeServerName = activeServerName;
			this.exceptionText = exceptionText;
		}

		protected LogRepairFailedToVerifyFromActiveException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.tempLogName = (string)info.GetValue("tempLogName", typeof(string));
			this.activeServerName = (string)info.GetValue("activeServerName", typeof(string));
			this.exceptionText = (string)info.GetValue("exceptionText", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("tempLogName", this.tempLogName);
			info.AddValue("activeServerName", this.activeServerName);
			info.AddValue("exceptionText", this.exceptionText);
		}

		public string TempLogName
		{
			get
			{
				return this.tempLogName;
			}
		}

		public string ActiveServerName
		{
			get
			{
				return this.activeServerName;
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

		private readonly string activeServerName;

		private readonly string exceptionText;
	}
}
