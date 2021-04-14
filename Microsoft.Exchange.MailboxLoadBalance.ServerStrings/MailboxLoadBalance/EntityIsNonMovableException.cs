using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class EntityIsNonMovableException : MailboxLoadBalancePermanentException
	{
		public EntityIsNonMovableException(string orgId, string userId) : base(MigrationWorkflowServiceStrings.ErrorEntityNotMovable(orgId, userId))
		{
			this.orgId = orgId;
			this.userId = userId;
		}

		public EntityIsNonMovableException(string orgId, string userId, Exception innerException) : base(MigrationWorkflowServiceStrings.ErrorEntityNotMovable(orgId, userId), innerException)
		{
			this.orgId = orgId;
			this.userId = userId;
		}

		protected EntityIsNonMovableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.orgId = (string)info.GetValue("orgId", typeof(string));
			this.userId = (string)info.GetValue("userId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("orgId", this.orgId);
			info.AddValue("userId", this.userId);
		}

		public string OrgId
		{
			get
			{
				return this.orgId;
			}
		}

		public string UserId
		{
			get
			{
				return this.userId;
			}
		}

		private readonly string orgId;

		private readonly string userId;
	}
}
