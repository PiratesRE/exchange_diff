using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class ResourcesException : StorageTransientException
	{
		public ResourcesException(LocalizedString message) : base(message)
		{
		}

		public ResourcesException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected ResourcesException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
