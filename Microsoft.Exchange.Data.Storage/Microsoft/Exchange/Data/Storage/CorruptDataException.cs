using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class CorruptDataException : StoragePermanentException
	{
		public CorruptDataException(LocalizedString message) : base(message)
		{
		}

		public CorruptDataException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected CorruptDataException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
