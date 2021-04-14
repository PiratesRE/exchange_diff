using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class LoadBalanceAnchorMailboxNotFoundException : MailboxLoadBalancePermanentException
	{
		public LoadBalanceAnchorMailboxNotFoundException(string capability) : base(MigrationWorkflowServiceStrings.ErrorMissingAnchorMailbox(capability))
		{
			this.capability = capability;
		}

		public LoadBalanceAnchorMailboxNotFoundException(string capability, Exception innerException) : base(MigrationWorkflowServiceStrings.ErrorMissingAnchorMailbox(capability), innerException)
		{
			this.capability = capability;
		}

		protected LoadBalanceAnchorMailboxNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.capability = (string)info.GetValue("capability", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("capability", this.capability);
		}

		public string Capability
		{
			get
			{
				return this.capability;
			}
		}

		private readonly string capability;
	}
}
