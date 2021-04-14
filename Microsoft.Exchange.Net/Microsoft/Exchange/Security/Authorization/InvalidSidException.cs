using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Security.Authorization
{
	[Serializable]
	internal class InvalidSidException : LocalizedException
	{
		public InvalidSidException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
		{
		}

		public InvalidSidException(string invalidSid) : base(new LocalizedString(NetException.InvalidSid((invalidSid == null) ? string.Empty : invalidSid)))
		{
			this.invalidSid = invalidSid;
		}

		public InvalidSidException(string invalidSid, Exception innerException) : base(new LocalizedString(NetException.InvalidSid((invalidSid == null) ? string.Empty : invalidSid)), innerException)
		{
			this.invalidSid = invalidSid;
		}

		public string InvalidSid
		{
			get
			{
				return this.invalidSid;
			}
		}

		private string invalidSid;
	}
}
