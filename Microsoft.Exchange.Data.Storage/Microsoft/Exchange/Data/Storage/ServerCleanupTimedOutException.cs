using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class ServerCleanupTimedOutException : StorageTransientException
	{
		public ServerCleanupTimedOutException(LocalizedString message) : base(message)
		{
		}

		public ServerCleanupTimedOutException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected ServerCleanupTimedOutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
