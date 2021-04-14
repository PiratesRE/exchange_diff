using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class CannotDeleteAssociatedMailboxPolicyException : LocalizedException
	{
		public CannotDeleteAssociatedMailboxPolicyException(string dn) : base(Strings.CannotDeleteAssociatedMailboxPolicyException(dn))
		{
			this.dn = dn;
		}

		public CannotDeleteAssociatedMailboxPolicyException(string dn, Exception innerException) : base(Strings.CannotDeleteAssociatedMailboxPolicyException(dn), innerException)
		{
			this.dn = dn;
		}

		protected CannotDeleteAssociatedMailboxPolicyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.dn = (string)info.GetValue("dn", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("dn", this.dn);
		}

		public string Dn
		{
			get
			{
				return this.dn;
			}
		}

		private readonly string dn;
	}
}
