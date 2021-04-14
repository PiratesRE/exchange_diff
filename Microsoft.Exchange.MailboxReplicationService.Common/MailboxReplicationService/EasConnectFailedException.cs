using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class EasConnectFailedException : MailboxReplicationTransientException
	{
		public EasConnectFailedException(string connectStatus, string httpStatus, string smtpAddress) : base(MrsStrings.EasConnectFailed(connectStatus, httpStatus, smtpAddress))
		{
			this.connectStatus = connectStatus;
			this.httpStatus = httpStatus;
			this.smtpAddress = smtpAddress;
		}

		public EasConnectFailedException(string connectStatus, string httpStatus, string smtpAddress, Exception innerException) : base(MrsStrings.EasConnectFailed(connectStatus, httpStatus, smtpAddress), innerException)
		{
			this.connectStatus = connectStatus;
			this.httpStatus = httpStatus;
			this.smtpAddress = smtpAddress;
		}

		protected EasConnectFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.connectStatus = (string)info.GetValue("connectStatus", typeof(string));
			this.httpStatus = (string)info.GetValue("httpStatus", typeof(string));
			this.smtpAddress = (string)info.GetValue("smtpAddress", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("connectStatus", this.connectStatus);
			info.AddValue("httpStatus", this.httpStatus);
			info.AddValue("smtpAddress", this.smtpAddress);
		}

		public string ConnectStatus
		{
			get
			{
				return this.connectStatus;
			}
		}

		public string HttpStatus
		{
			get
			{
				return this.httpStatus;
			}
		}

		public string SmtpAddress
		{
			get
			{
				return this.smtpAddress;
			}
		}

		private readonly string connectStatus;

		private readonly string httpStatus;

		private readonly string smtpAddress;
	}
}
