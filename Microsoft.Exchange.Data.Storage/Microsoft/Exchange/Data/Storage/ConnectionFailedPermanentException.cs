using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class ConnectionFailedPermanentException : StoragePermanentException
	{
		public ConnectionFailedPermanentException(LocalizedString message) : base(message)
		{
		}

		public ConnectionFailedPermanentException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected ConnectionFailedPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
