using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnknownSecurityPropException : MailboxReplicationPermanentException
	{
		public UnknownSecurityPropException(int securityProp) : base(MrsStrings.UnknownSecurityProp(securityProp))
		{
			this.securityProp = securityProp;
		}

		public UnknownSecurityPropException(int securityProp, Exception innerException) : base(MrsStrings.UnknownSecurityProp(securityProp), innerException)
		{
			this.securityProp = securityProp;
		}

		protected UnknownSecurityPropException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.securityProp = (int)info.GetValue("securityProp", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("securityProp", this.securityProp);
		}

		public int SecurityProp
		{
			get
			{
				return this.securityProp;
			}
		}

		private readonly int securityProp;
	}
}
