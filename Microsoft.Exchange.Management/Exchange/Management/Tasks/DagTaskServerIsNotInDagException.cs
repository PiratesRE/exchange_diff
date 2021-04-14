using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class DagTaskServerIsNotInDagException : LocalizedException
	{
		public DagTaskServerIsNotInDagException(string mailboxServer, string dagName) : base(Strings.DagTaskServerIsNotInDagException(mailboxServer, dagName))
		{
			this.mailboxServer = mailboxServer;
			this.dagName = dagName;
		}

		public DagTaskServerIsNotInDagException(string mailboxServer, string dagName, Exception innerException) : base(Strings.DagTaskServerIsNotInDagException(mailboxServer, dagName), innerException)
		{
			this.mailboxServer = mailboxServer;
			this.dagName = dagName;
		}

		protected DagTaskServerIsNotInDagException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mailboxServer = (string)info.GetValue("mailboxServer", typeof(string));
			this.dagName = (string)info.GetValue("dagName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mailboxServer", this.mailboxServer);
			info.AddValue("dagName", this.dagName);
		}

		public string MailboxServer
		{
			get
			{
				return this.mailboxServer;
			}
		}

		public string DagName
		{
			get
			{
				return this.dagName;
			}
		}

		private readonly string mailboxServer;

		private readonly string dagName;
	}
}
