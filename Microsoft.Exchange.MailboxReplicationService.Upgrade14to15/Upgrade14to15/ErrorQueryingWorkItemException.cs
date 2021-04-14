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
	internal class ErrorQueryingWorkItemException : MigrationTransientException
	{
		public ErrorQueryingWorkItemException() : base(UpgradeHandlerStrings.ErrorQueryingWorkItem)
		{
		}

		public ErrorQueryingWorkItemException(Exception innerException) : base(UpgradeHandlerStrings.ErrorQueryingWorkItem, innerException)
		{
		}

		protected ErrorQueryingWorkItemException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
