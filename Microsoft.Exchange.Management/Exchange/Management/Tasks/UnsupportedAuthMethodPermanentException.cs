using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class UnsupportedAuthMethodPermanentException : MailboxReplicationPermanentException
	{
		public UnsupportedAuthMethodPermanentException(string authMethod) : base(Strings.ErrorUnsupportedAuthMethodForMerges(authMethod))
		{
			this.authMethod = authMethod;
		}

		public UnsupportedAuthMethodPermanentException(string authMethod, Exception innerException) : base(Strings.ErrorUnsupportedAuthMethodForMerges(authMethod), innerException)
		{
			this.authMethod = authMethod;
		}

		protected UnsupportedAuthMethodPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.authMethod = (string)info.GetValue("authMethod", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("authMethod", this.authMethod);
		}

		public string AuthMethod
		{
			get
			{
				return this.authMethod;
			}
		}

		private readonly string authMethod;
	}
}
