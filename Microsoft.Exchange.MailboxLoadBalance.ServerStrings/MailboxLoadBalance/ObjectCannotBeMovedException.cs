using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ObjectCannotBeMovedException : MailboxLoadBalancePermanentException
	{
		public ObjectCannotBeMovedException(string objectType, string objectIdentity) : base(MigrationWorkflowServiceStrings.ErrorObjectCannotBeMoved(objectType, objectIdentity))
		{
			this.objectType = objectType;
			this.objectIdentity = objectIdentity;
		}

		public ObjectCannotBeMovedException(string objectType, string objectIdentity, Exception innerException) : base(MigrationWorkflowServiceStrings.ErrorObjectCannotBeMoved(objectType, objectIdentity), innerException)
		{
			this.objectType = objectType;
			this.objectIdentity = objectIdentity;
		}

		protected ObjectCannotBeMovedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.objectType = (string)info.GetValue("objectType", typeof(string));
			this.objectIdentity = (string)info.GetValue("objectIdentity", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("objectType", this.objectType);
			info.AddValue("objectIdentity", this.objectIdentity);
		}

		public string ObjectType
		{
			get
			{
				return this.objectType;
			}
		}

		public string ObjectIdentity
		{
			get
			{
				return this.objectIdentity;
			}
		}

		private readonly string objectType;

		private readonly string objectIdentity;
	}
}
