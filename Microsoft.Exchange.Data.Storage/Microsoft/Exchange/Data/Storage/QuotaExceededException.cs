using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class QuotaExceededException : StoragePermanentException
	{
		public QuotaExceededException(LocalizedString message) : base(message)
		{
		}

		public QuotaExceededException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected QuotaExceededException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
