using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxLoadBalance
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CannotRetrieveCapacityDataException : MailboxLoadBalancePermanentException
	{
		public CannotRetrieveCapacityDataException(string objectIdentity) : base(MigrationWorkflowServiceStrings.ErrorCannotRetrieveCapacityData(objectIdentity))
		{
			this.objectIdentity = objectIdentity;
		}

		public CannotRetrieveCapacityDataException(string objectIdentity, Exception innerException) : base(MigrationWorkflowServiceStrings.ErrorCannotRetrieveCapacityData(objectIdentity), innerException)
		{
			this.objectIdentity = objectIdentity;
		}

		protected CannotRetrieveCapacityDataException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.objectIdentity = (string)info.GetValue("objectIdentity", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("objectIdentity", this.objectIdentity);
		}

		public string ObjectIdentity
		{
			get
			{
				return this.objectIdentity;
			}
		}

		private readonly string objectIdentity;
	}
}
