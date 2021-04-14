using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class DatabaseFailedOverException : MailboxLoadBalanceTransientException
	{
		public DatabaseFailedOverException(string guid) : base(MigrationWorkflowServiceStrings.ErrorDatabaseFailedOver(guid))
		{
			this.guid = guid;
		}

		public DatabaseFailedOverException(string guid, Exception innerException) : base(MigrationWorkflowServiceStrings.ErrorDatabaseFailedOver(guid), innerException)
		{
			this.guid = guid;
		}

		protected DatabaseFailedOverException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.guid = (string)info.GetValue("guid", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("guid", this.guid);
		}

		public string Guid
		{
			get
			{
				return this.guid;
			}
		}

		private readonly string guid;
	}
}
