using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.SoapWebClient
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class GetFederationInformationException : LocalizedException
	{
		public GetFederationInformationException(LocalizedString message) : base(message)
		{
		}

		public GetFederationInformationException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected GetFederationInformationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
