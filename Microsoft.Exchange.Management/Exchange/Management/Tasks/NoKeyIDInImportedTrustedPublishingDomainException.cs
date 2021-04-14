using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NoKeyIDInImportedTrustedPublishingDomainException : LocalizedException
	{
		public NoKeyIDInImportedTrustedPublishingDomainException() : base(Strings.NoKeyIDInImportedTrustedPublishingDomain)
		{
		}

		public NoKeyIDInImportedTrustedPublishingDomainException(Exception innerException) : base(Strings.NoKeyIDInImportedTrustedPublishingDomain, innerException)
		{
		}

		protected NoKeyIDInImportedTrustedPublishingDomainException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
