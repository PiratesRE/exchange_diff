using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NewDagServerIsAlreadyInDifferentDagException : LocalizedException
	{
		public NewDagServerIsAlreadyInDifferentDagException(string mailboxServer, string currentDag, string desiredDag) : base(Strings.NewDagServerIsAlreadyInDifferentDagException(mailboxServer, currentDag, desiredDag))
		{
			this.mailboxServer = mailboxServer;
			this.currentDag = currentDag;
			this.desiredDag = desiredDag;
		}

		public NewDagServerIsAlreadyInDifferentDagException(string mailboxServer, string currentDag, string desiredDag, Exception innerException) : base(Strings.NewDagServerIsAlreadyInDifferentDagException(mailboxServer, currentDag, desiredDag), innerException)
		{
			this.mailboxServer = mailboxServer;
			this.currentDag = currentDag;
			this.desiredDag = desiredDag;
		}

		protected NewDagServerIsAlreadyInDifferentDagException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mailboxServer = (string)info.GetValue("mailboxServer", typeof(string));
			this.currentDag = (string)info.GetValue("currentDag", typeof(string));
			this.desiredDag = (string)info.GetValue("desiredDag", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mailboxServer", this.mailboxServer);
			info.AddValue("currentDag", this.currentDag);
			info.AddValue("desiredDag", this.desiredDag);
		}

		public string MailboxServer
		{
			get
			{
				return this.mailboxServer;
			}
		}

		public string CurrentDag
		{
			get
			{
				return this.currentDag;
			}
		}

		public string DesiredDag
		{
			get
			{
				return this.desiredDag;
			}
		}

		private readonly string mailboxServer;

		private readonly string currentDag;

		private readonly string desiredDag;
	}
}
