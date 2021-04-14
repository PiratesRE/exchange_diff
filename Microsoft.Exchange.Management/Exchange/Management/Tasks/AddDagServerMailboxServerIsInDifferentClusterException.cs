using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AddDagServerMailboxServerIsInDifferentClusterException : LocalizedException
	{
		public AddDagServerMailboxServerIsInDifferentClusterException(string mailboxServer, string thisClusterName, string otherClusterName) : base(Strings.AddDagServerMailboxServerIsInDifferentClusterException(mailboxServer, thisClusterName, otherClusterName))
		{
			this.mailboxServer = mailboxServer;
			this.thisClusterName = thisClusterName;
			this.otherClusterName = otherClusterName;
		}

		public AddDagServerMailboxServerIsInDifferentClusterException(string mailboxServer, string thisClusterName, string otherClusterName, Exception innerException) : base(Strings.AddDagServerMailboxServerIsInDifferentClusterException(mailboxServer, thisClusterName, otherClusterName), innerException)
		{
			this.mailboxServer = mailboxServer;
			this.thisClusterName = thisClusterName;
			this.otherClusterName = otherClusterName;
		}

		protected AddDagServerMailboxServerIsInDifferentClusterException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mailboxServer = (string)info.GetValue("mailboxServer", typeof(string));
			this.thisClusterName = (string)info.GetValue("thisClusterName", typeof(string));
			this.otherClusterName = (string)info.GetValue("otherClusterName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mailboxServer", this.mailboxServer);
			info.AddValue("thisClusterName", this.thisClusterName);
			info.AddValue("otherClusterName", this.otherClusterName);
		}

		public string MailboxServer
		{
			get
			{
				return this.mailboxServer;
			}
		}

		public string ThisClusterName
		{
			get
			{
				return this.thisClusterName;
			}
		}

		public string OtherClusterName
		{
			get
			{
				return this.otherClusterName;
			}
		}

		private readonly string mailboxServer;

		private readonly string thisClusterName;

		private readonly string otherClusterName;
	}
}
