using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UMMailboxPolicyNotPresentException : UMMailboxPolicyNotFoundException
	{
		public UMMailboxPolicyNotPresentException(string user) : base(Strings.UMMailboxPolicyNotPresent(user))
		{
			this.user = user;
		}

		public UMMailboxPolicyNotPresentException(string user, Exception innerException) : base(Strings.UMMailboxPolicyNotPresent(user), innerException)
		{
			this.user = user;
		}

		protected UMMailboxPolicyNotPresentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.user = (string)info.GetValue("user", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("user", this.user);
		}

		public string User
		{
			get
			{
				return this.user;
			}
		}

		private readonly string user;
	}
}
