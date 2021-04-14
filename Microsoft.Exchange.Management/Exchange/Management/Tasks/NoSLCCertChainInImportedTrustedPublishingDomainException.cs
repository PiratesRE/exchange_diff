using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NoSLCCertChainInImportedTrustedPublishingDomainException : LocalizedException
	{
		public NoSLCCertChainInImportedTrustedPublishingDomainException() : base(Strings.NoSLCCertChainInImportedTrustedPublishingDomain)
		{
		}

		public NoSLCCertChainInImportedTrustedPublishingDomainException(Exception innerException) : base(Strings.NoSLCCertChainInImportedTrustedPublishingDomain, innerException)
		{
		}

		protected NoSLCCertChainInImportedTrustedPublishingDomainException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
