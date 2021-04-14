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
	internal class NoE14ServersFoundException : MigrationTransientException
	{
		public NoE14ServersFoundException() : base(UpgradeHandlerStrings.ErrorNoE14ServersFound)
		{
		}

		public NoE14ServersFoundException(Exception innerException) : base(UpgradeHandlerStrings.ErrorNoE14ServersFound, innerException)
		{
		}

		protected NoE14ServersFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
