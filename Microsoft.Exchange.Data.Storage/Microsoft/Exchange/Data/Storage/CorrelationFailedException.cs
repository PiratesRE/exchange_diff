using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class CorrelationFailedException : StoragePermanentException
	{
		public CorrelationFailedException(LocalizedString message) : base(message)
		{
		}

		public CorrelationFailedException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected CorrelationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
