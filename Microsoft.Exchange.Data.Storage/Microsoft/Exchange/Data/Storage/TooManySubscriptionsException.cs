using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class TooManySubscriptionsException : StoragePermanentException
	{
		public TooManySubscriptionsException(LocalizedString message) : base(message)
		{
		}

		public TooManySubscriptionsException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected TooManySubscriptionsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
