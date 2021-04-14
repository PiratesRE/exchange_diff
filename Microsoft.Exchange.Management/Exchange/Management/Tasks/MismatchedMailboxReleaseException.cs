using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MismatchedMailboxReleaseException : MailboxReplicationPermanentException
	{
		public MismatchedMailboxReleaseException(string mailbox, string database, string serverMailboxRelease, string desiredMailboxRelease) : base(Strings.ErrorMismatchedMailboxRelease(mailbox, database, serverMailboxRelease, desiredMailboxRelease))
		{
			this.mailbox = mailbox;
			this.database = database;
			this.serverMailboxRelease = serverMailboxRelease;
			this.desiredMailboxRelease = desiredMailboxRelease;
		}

		public MismatchedMailboxReleaseException(string mailbox, string database, string serverMailboxRelease, string desiredMailboxRelease, Exception innerException) : base(Strings.ErrorMismatchedMailboxRelease(mailbox, database, serverMailboxRelease, desiredMailboxRelease), innerException)
		{
			this.mailbox = mailbox;
			this.database = database;
			this.serverMailboxRelease = serverMailboxRelease;
			this.desiredMailboxRelease = desiredMailboxRelease;
		}

		protected MismatchedMailboxReleaseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mailbox = (string)info.GetValue("mailbox", typeof(string));
			this.database = (string)info.GetValue("database", typeof(string));
			this.serverMailboxRelease = (string)info.GetValue("serverMailboxRelease", typeof(string));
			this.desiredMailboxRelease = (string)info.GetValue("desiredMailboxRelease", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mailbox", this.mailbox);
			info.AddValue("database", this.database);
			info.AddValue("serverMailboxRelease", this.serverMailboxRelease);
			info.AddValue("desiredMailboxRelease", this.desiredMailboxRelease);
		}

		public string Mailbox
		{
			get
			{
				return this.mailbox;
			}
		}

		public string Database
		{
			get
			{
				return this.database;
			}
		}

		public string ServerMailboxRelease
		{
			get
			{
				return this.serverMailboxRelease;
			}
		}

		public string DesiredMailboxRelease
		{
			get
			{
				return this.desiredMailboxRelease;
			}
		}

		private readonly string mailbox;

		private readonly string database;

		private readonly string serverMailboxRelease;

		private readonly string desiredMailboxRelease;
	}
}
