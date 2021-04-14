using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.SoapWebClient
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class AutodiscoverClientException : LocalizedException
	{
		public AutodiscoverClientException(LocalizedString message) : base(message)
		{
		}

		public AutodiscoverClientException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected AutodiscoverClientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
