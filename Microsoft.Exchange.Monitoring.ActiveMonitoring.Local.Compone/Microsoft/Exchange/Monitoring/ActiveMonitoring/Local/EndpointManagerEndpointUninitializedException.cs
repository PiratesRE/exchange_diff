using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Local
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class EndpointManagerEndpointUninitializedException : LocalizedException
	{
		public EndpointManagerEndpointUninitializedException(LocalizedString message) : base(message)
		{
		}

		public EndpointManagerEndpointUninitializedException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected EndpointManagerEndpointUninitializedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
