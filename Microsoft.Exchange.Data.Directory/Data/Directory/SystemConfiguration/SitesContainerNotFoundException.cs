using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class SitesContainerNotFoundException : ADOperationException
	{
		public SitesContainerNotFoundException(LocalizedString message) : base(message)
		{
		}

		public SitesContainerNotFoundException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected SitesContainerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
