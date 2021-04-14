using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class StorageTransientException : TransientException
	{
		public StorageTransientException(LocalizedString message) : base(message)
		{
		}

		public StorageTransientException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected StorageTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
