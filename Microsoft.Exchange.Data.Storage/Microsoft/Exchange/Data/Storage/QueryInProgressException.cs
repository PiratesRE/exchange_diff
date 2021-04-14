using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class QueryInProgressException : StorageTransientException
	{
		public QueryInProgressException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected QueryInProgressException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
