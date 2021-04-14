using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RemoveDagServerDatabaseIsReplicatedException : LocalizedException
	{
		public RemoveDagServerDatabaseIsReplicatedException(string mailboxServer, string databaseName) : base(Strings.RemoveDagServerDatabaseIsReplicatedException(mailboxServer, databaseName))
		{
			this.mailboxServer = mailboxServer;
			this.databaseName = databaseName;
		}

		public RemoveDagServerDatabaseIsReplicatedException(string mailboxServer, string databaseName, Exception innerException) : base(Strings.RemoveDagServerDatabaseIsReplicatedException(mailboxServer, databaseName), innerException)
		{
			this.mailboxServer = mailboxServer;
			this.databaseName = databaseName;
		}

		protected RemoveDagServerDatabaseIsReplicatedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mailboxServer = (string)info.GetValue("mailboxServer", typeof(string));
			this.databaseName = (string)info.GetValue("databaseName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mailboxServer", this.mailboxServer);
			info.AddValue("databaseName", this.databaseName);
		}

		public string MailboxServer
		{
			get
			{
				return this.mailboxServer;
			}
		}

		public string DatabaseName
		{
			get
			{
				return this.databaseName;
			}
		}

		private readonly string mailboxServer;

		private readonly string databaseName;
	}
}
