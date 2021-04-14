using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Autodiscover
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class AuthMetadataInternalException : AuthMetadataBuilderException
	{
		public AuthMetadataInternalException(LocalizedString message) : base(message)
		{
		}

		public AuthMetadataInternalException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected AuthMetadataInternalException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
