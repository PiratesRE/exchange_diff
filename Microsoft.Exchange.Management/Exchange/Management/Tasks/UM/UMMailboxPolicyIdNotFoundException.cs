using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UMMailboxPolicyIdNotFoundException : UMMailboxPolicyNotFoundException
	{
		public UMMailboxPolicyIdNotFoundException(string id) : base(Strings.UMMailboxPolicyIdNotFound(id))
		{
			this.id = id;
		}

		public UMMailboxPolicyIdNotFoundException(string id, Exception innerException) : base(Strings.UMMailboxPolicyIdNotFound(id), innerException)
		{
			this.id = id;
		}

		protected UMMailboxPolicyIdNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.id = (string)info.GetValue("id", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("id", this.id);
		}

		public string Id
		{
			get
			{
				return this.id;
			}
		}

		private readonly string id;
	}
}
