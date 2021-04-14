using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Directory
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class UserWithoutFederatedProxyAddressException : ADOperationException
	{
		public UserWithoutFederatedProxyAddressException() : base(DirectoryStrings.UserHasNoSmtpProxyAddressWithFederatedDomain)
		{
		}

		public UserWithoutFederatedProxyAddressException(Exception innerException) : base(DirectoryStrings.UserHasNoSmtpProxyAddressWithFederatedDomain, innerException)
		{
		}

		protected UserWithoutFederatedProxyAddressException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
