using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class AutomaticMailboxLoadBalancingNotAllowedException : MailboxLoadBalancePermanentException
	{
		public AutomaticMailboxLoadBalancingNotAllowedException() : base(MigrationWorkflowServiceStrings.ErrorAutomaticMailboxLoadBalancingNotAllowed)
		{
		}

		public AutomaticMailboxLoadBalancingNotAllowedException(Exception innerException) : base(MigrationWorkflowServiceStrings.ErrorAutomaticMailboxLoadBalancingNotAllowed, innerException)
		{
		}

		protected AutomaticMailboxLoadBalancingNotAllowedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
