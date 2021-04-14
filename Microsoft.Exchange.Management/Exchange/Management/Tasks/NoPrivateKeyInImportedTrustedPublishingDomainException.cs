using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NoPrivateKeyInImportedTrustedPublishingDomainException : LocalizedException
	{
		public NoPrivateKeyInImportedTrustedPublishingDomainException() : base(Strings.NoPrivateKeyInImportedTrustedPublishingDomain)
		{
		}

		public NoPrivateKeyInImportedTrustedPublishingDomainException(Exception innerException) : base(Strings.NoPrivateKeyInImportedTrustedPublishingDomain, innerException)
		{
		}

		protected NoPrivateKeyInImportedTrustedPublishingDomainException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
