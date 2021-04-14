using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class InvalidSyncStateVersionException : StoragePermanentException
	{
		public InvalidSyncStateVersionException(LocalizedString message) : base(message)
		{
		}

		public InvalidSyncStateVersionException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidSyncStateVersionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
