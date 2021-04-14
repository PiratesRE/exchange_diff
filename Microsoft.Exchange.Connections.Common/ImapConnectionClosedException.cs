using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Connections.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ImapConnectionClosedException : OperationLevelTransientException
	{
		public ImapConnectionClosedException(string imapConnectionClosedErrMsg) : base(CXStrings.ImapConnectionClosedErrorMsg(imapConnectionClosedErrMsg))
		{
			this.imapConnectionClosedErrMsg = imapConnectionClosedErrMsg;
		}

		public ImapConnectionClosedException(string imapConnectionClosedErrMsg, Exception innerException) : base(CXStrings.ImapConnectionClosedErrorMsg(imapConnectionClosedErrMsg), innerException)
		{
			this.imapConnectionClosedErrMsg = imapConnectionClosedErrMsg;
		}

		protected ImapConnectionClosedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.imapConnectionClosedErrMsg = (string)info.GetValue("imapConnectionClosedErrMsg", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("imapConnectionClosedErrMsg", this.imapConnectionClosedErrMsg);
		}

		public string ImapConnectionClosedErrMsg
		{
			get
			{
				return this.imapConnectionClosedErrMsg;
			}
		}

		private readonly string imapConnectionClosedErrMsg;
	}
}
