using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MailboxServerNotHostingMdbException : LocalizedException
	{
		public MailboxServerNotHostingMdbException(string mailboxServer) : base(Strings.MailboxServerNotHostingMdbException(mailboxServer))
		{
			this.mailboxServer = mailboxServer;
		}

		public MailboxServerNotHostingMdbException(string mailboxServer, Exception innerException) : base(Strings.MailboxServerNotHostingMdbException(mailboxServer), innerException)
		{
			this.mailboxServer = mailboxServer;
		}

		protected MailboxServerNotHostingMdbException(SerializationInfo info, StreamingContext context) : base(info, context)
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
