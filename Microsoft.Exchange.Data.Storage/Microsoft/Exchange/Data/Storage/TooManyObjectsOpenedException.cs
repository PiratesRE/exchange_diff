using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class TooManyObjectsOpenedException : StorageTransientException
	{
		public TooManyObjectsOpenedException(LocalizedString message) : base(message)
		{
		}

		public TooManyObjectsOpenedException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected TooManyObjectsOpenedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
