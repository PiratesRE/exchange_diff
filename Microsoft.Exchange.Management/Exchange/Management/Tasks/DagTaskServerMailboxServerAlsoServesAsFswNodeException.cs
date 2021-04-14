using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskServerMailboxServerAlsoServesAsFswNodeException : LocalizedException
	{
		public DagTaskServerMailboxServerAlsoServesAsFswNodeException(string mailboxServer) : base(Strings.DagTaskServerMailboxServerAlsoServesAsFswNodeException(mailboxServer))
		{
			this.mailboxServer = mailboxServer;
		}

		public DagTaskServerMailboxServerAlsoServesAsFswNodeException(string mailboxServer, Exception innerException) : base(Strings.DagTaskServerMailboxServerAlsoServesAsFswNodeException(mailboxServer), innerException)
		{
			this.mailboxServer = mailboxServer;
		}

		protected DagTaskServerMailboxServerAlsoServesAsFswNodeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mailboxServer = (string)info.GetValue("mailboxServer", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mailboxServer", this.mailboxServer);
		}

		public string MailboxServer
		{
			get
			{
				return this.mailboxServer;
			}
		}

		private readonly string mailboxServer;
	}
}
