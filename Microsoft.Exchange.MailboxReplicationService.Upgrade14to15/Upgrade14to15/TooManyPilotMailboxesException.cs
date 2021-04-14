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
	internal class TooManyPilotMailboxesException : MigrationPermanentException
	{
		public TooManyPilotMailboxesException() : base(UpgradeHandlerStrings.TooManyPilotMailboxes)
		{
		}

		public TooManyPilotMailboxesException(Exception innerException) : base(UpgradeHandlerStrings.TooManyPilotMailboxes, innerException)
		{
		}

		protected TooManyPilotMailboxesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
