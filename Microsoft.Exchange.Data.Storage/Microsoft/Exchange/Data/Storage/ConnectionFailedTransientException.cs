using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class ConnectionFailedTransientException : StorageTransientException
	{
		public ConnectionFailedTransientException(LocalizedString message) : base(message)
		{
		}

		public ConnectionFailedTransientException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected ConnectionFailedTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
