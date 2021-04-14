using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskServerMailboxServerIsInDifferentDagException : LocalizedException
	{
		public DagTaskServerMailboxServerIsInDifferentDagException(string mailboxServer, string otherDagName) : base(Strings.DagTaskServerMailboxServerIsInDifferentDagException(mailboxServer, otherDagName))
		{
			this.mailboxServer = mailboxServer;
			this.otherDagName = otherDagName;
		}

		public DagTaskServerMailboxServerIsInDifferentDagException(string mailboxServer, string otherDagName, Exception innerException) : base(Strings.DagTaskServerMailboxServerIsInDifferentDagException(mailboxServer, otherDagName), innerException)
		{
			this.mailboxServer = mailboxServer;
			this.otherDagName = otherDagName;
		}

		protected DagTaskServerMailboxServerIsInDifferentDagException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mailboxServer = (string)info.GetValue("mailboxServer", typeof(string));
			this.otherDagName = (string)info.GetValue("otherDagName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mailboxServer", this.mailboxServer);
			info.AddValue("otherDagName", this.otherDagName);
		}

		public string MailboxServer
		{
			get
			{
				return this.mailboxServer;
			}
		}

		public string OtherDagName
		{
			get
			{
				return this.otherDagName;
			}
		}

		private readonly string mailboxServer;

		private readonly string otherDagName;
	}
}
