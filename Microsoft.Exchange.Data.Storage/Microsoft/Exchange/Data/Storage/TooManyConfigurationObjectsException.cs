using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class TooManyConfigurationObjectsException : StoragePermanentException
	{
		public TooManyConfigurationObjectsException(LocalizedString message) : base(message)
		{
		}

		public TooManyConfigurationObjectsException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected TooManyConfigurationObjectsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
