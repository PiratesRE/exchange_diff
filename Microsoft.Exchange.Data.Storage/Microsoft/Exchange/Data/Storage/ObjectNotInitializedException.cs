using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class ObjectNotInitializedException : StorageTransientException
	{
		public ObjectNotInitializedException(LocalizedString message) : base(message)
		{
		}

		public ObjectNotInitializedException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected ObjectNotInitializedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
