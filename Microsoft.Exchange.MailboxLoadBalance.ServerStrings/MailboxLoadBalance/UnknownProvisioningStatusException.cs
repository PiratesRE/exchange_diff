using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UnknownProvisioningStatusException : MailboxLoadBalancePermanentException
	{
		public UnknownProvisioningStatusException() : base(MigrationWorkflowServiceStrings.ErrorUnknownProvisioningStatus)
		{
		}

		public UnknownProvisioningStatusException(Exception innerException) : base(MigrationWorkflowServiceStrings.ErrorUnknownProvisioningStatus, innerException)
		{
		}

		protected UnknownProvisioningStatusException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
