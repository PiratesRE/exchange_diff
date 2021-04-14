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
	internal class InvalidE15MailboxesException : MigrationPermanentException
	{
		public InvalidE15MailboxesException() : base(UpgradeHandlerStrings.InvalidE15Mailboxes)
		{
		}

		public InvalidE15MailboxesException(Exception innerException) : base(UpgradeHandlerStrings.InvalidE15Mailboxes, innerException)
		{
		}

		protected InvalidE15MailboxesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
