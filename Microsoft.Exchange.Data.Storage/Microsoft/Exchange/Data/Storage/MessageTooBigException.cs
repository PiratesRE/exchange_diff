using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class MessageTooBigException : StoragePermanentException
	{
		public MessageTooBigException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected MessageTooBigException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
