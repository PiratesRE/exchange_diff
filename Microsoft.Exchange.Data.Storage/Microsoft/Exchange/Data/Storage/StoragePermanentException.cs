using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class StoragePermanentException : LocalizedException
	{
		public StoragePermanentException(LocalizedString message) : base(message)
		{
		}

		public StoragePermanentException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected StoragePermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
