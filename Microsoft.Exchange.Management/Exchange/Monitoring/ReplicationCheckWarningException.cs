using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ReplicationCheckWarningException : ReplicationCheckException
	{
		public ReplicationCheckWarningException(string checkTitle, string warningMessage) : base(Strings.ReplicationCheckWarningException(checkTitle, warningMessage))
		{
			this.checkTitle = checkTitle;
			this.warningMessage = warningMessage;
		}

		public ReplicationCheckWarningException(string checkTitle, string warningMessage, Exception innerException) : base(Strings.ReplicationCheckWarningException(checkTitle, warningMessage), innerException)
		{
			this.checkTitle = checkTitle;
			this.warningMessage = warningMessage;
		}

		protected ReplicationCheckWarningException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.checkTitle = (string)info.GetValue("checkTitle", typeof(string));
			this.warningMessage = (string)info.GetValue("warningMessage", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("checkTitle", this.checkTitle);
			info.AddValue("warningMessage", this.warningMessage);
		}

		public string CheckTitle
		{
			get
			{
				return this.checkTitle;
			}
		}

		public string WarningMessage
		{
			get
			{
				return this.warningMessage;
			}
		}

		private readonly string checkTitle;

		private readonly string warningMessage;
	}
}
