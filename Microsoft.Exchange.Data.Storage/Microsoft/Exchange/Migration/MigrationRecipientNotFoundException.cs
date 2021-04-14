using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MigrationRecipientNotFoundException : MigrationPermanentException
	{
		public MigrationRecipientNotFoundException(string mailboxName) : base(Strings.MigrationRecipientNotFound(mailboxName))
		{
			this.mailboxName = mailboxName;
		}

		public MigrationRecipientNotFoundException(string mailboxName, Exception innerException) : base(Strings.MigrationRecipientNotFound(mailboxName), innerException)
		{
			this.mailboxName = mailboxName;
		}

		protected MigrationRecipientNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mailboxName = (string)info.GetValue("mailboxName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mailboxName", this.mailboxName);
		}

		public string MailboxName
		{
			get
			{
				return this.mailboxName;
			}
		}

		private readonly string mailboxName;
	}
}
