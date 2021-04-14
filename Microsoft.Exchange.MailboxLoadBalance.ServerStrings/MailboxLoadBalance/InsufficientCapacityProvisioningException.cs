using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class InsufficientCapacityProvisioningException : MailboxLoadBalancePermanentException
	{
		public InsufficientCapacityProvisioningException() : base(MigrationWorkflowServiceStrings.ErrorInsufficientCapacityProvisioning)
		{
		}

		public InsufficientCapacityProvisioningException(Exception innerException) : base(MigrationWorkflowServiceStrings.ErrorInsufficientCapacityProvisioning, innerException)
		{
		}

		protected InsufficientCapacityProvisioningException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
