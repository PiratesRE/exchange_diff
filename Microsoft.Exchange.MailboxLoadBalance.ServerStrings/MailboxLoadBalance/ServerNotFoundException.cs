using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ServerNotFoundException : MailboxLoadBalanceTransientException
	{
		public ServerNotFoundException(string guid) : base(MigrationWorkflowServiceStrings.ErrorServerNotFound(guid))
		{
			this.guid = guid;
		}

		public ServerNotFoundException(string guid, Exception innerException) : base(MigrationWorkflowServiceStrings.ErrorServerNotFound(guid), innerException)
		{
			this.guid = guid;
		}

		protected ServerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
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
