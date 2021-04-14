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
	internal class SubscriptionMailboxNotFoundException : MigrationPermanentException
	{
		public SubscriptionMailboxNotFoundException() : base(Strings.MailboxNotFoundSubscriptionStatus)
		{
		}

		public SubscriptionMailboxNotFoundException(Exception innerException) : base(Strings.MailboxNotFoundSubscriptionStatus, innerException)
		{
		}

		protected SubscriptionMailboxNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
