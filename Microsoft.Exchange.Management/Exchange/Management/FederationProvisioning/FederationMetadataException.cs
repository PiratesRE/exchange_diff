using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.FederationProvisioning
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FederationMetadataException : FederationException
	{
		public FederationMetadataException(LocalizedString message) : base(message)
		{
		}

		public FederationMetadataException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected FederationMetadataException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
