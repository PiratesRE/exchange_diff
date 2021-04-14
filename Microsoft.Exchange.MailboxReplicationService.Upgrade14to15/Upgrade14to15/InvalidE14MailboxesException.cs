using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InvalidE14MailboxesException : MigrationPermanentException
	{
		public InvalidE14MailboxesException() : base(UpgradeHandlerStrings.InvalidE14Mailboxes)
		{
		}

		public InvalidE14MailboxesException(Exception innerException) : base(UpgradeHandlerStrings.InvalidE14Mailboxes, innerException)
		{
		}

		protected InvalidE14MailboxesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
