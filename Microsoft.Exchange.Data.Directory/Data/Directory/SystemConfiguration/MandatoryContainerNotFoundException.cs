using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MandatoryContainerNotFoundException : ADExternalException
	{
		public MandatoryContainerNotFoundException(LocalizedString message) : base(message)
		{
		}

		public MandatoryContainerNotFoundException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected MandatoryContainerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
