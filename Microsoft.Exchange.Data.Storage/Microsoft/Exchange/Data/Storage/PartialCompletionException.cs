using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class PartialCompletionException : StoragePermanentException
	{
		public PartialCompletionException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected PartialCompletionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
