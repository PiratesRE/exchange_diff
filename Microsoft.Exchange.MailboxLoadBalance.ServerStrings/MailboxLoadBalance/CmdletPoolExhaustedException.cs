using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CmdletPoolExhaustedException : MailboxLoadBalancePermanentException
	{
		public CmdletPoolExhaustedException() : base(MigrationWorkflowServiceStrings.ErrorCmdletPoolExhausted)
		{
		}

		public CmdletPoolExhaustedException(Exception innerException) : base(MigrationWorkflowServiceStrings.ErrorCmdletPoolExhausted, innerException)
		{
		}

		protected CmdletPoolExhaustedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
