using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class MultipleRecipientFoundException : MailboxLoadBalancePermanentException
	{
		public MultipleRecipientFoundException(string userId) : base(MigrationWorkflowServiceStrings.ErrorMultipleRecipientFound(userId))
		{
			this.userId = userId;
		}

		public MultipleRecipientFoundException(string userId, Exception innerException) : base(MigrationWorkflowServiceStrings.ErrorMultipleRecipientFound(userId), innerException)
		{
			this.userId = userId;
		}

		protected MultipleRecipientFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
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
