using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class CannotCompleteOperationException : StorageTransientException
	{
		public CannotCompleteOperationException(LocalizedString message) : base(message)
		{
		}

		public CannotCompleteOperationException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected CannotCompleteOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
