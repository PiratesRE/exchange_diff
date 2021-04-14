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
	internal class AnchorServiceInstanceNotActiveException : MigrationPermanentException
	{
		public AnchorServiceInstanceNotActiveException() : base(UpgradeHandlerStrings.AnchorServiceInstanceNotActive)
		{
		}

		public AnchorServiceInstanceNotActiveException(Exception innerException) : base(UpgradeHandlerStrings.AnchorServiceInstanceNotActive, innerException)
		{
		}

		protected AnchorServiceInstanceNotActiveException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
