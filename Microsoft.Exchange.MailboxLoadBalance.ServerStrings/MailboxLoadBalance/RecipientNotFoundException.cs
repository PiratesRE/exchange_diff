using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class RecipientNotFoundException : MailboxLoadBalancePermanentException
	{
		public RecipientNotFoundException(string userId) : base(MigrationWorkflowServiceStrings.ErrorRecipientNotFound(userId))
		{
			this.userId = userId;
		}

		public RecipientNotFoundException(string userId, Exception innerException) : base(MigrationWorkflowServiceStrings.ErrorRecipientNotFound(userId), innerException)
		{
			this.userId = userId;
		}

		protected RecipientNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.userId = (string)info.GetValue("userId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("userId", this.userId);
		}

		public string UserId
		{
			get
			{
				return this.userId;
			}
		}

		private readonly string userId;
	}
}
