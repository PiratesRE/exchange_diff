using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public abstract class RecurrenceException : StoragePermanentException
	{
		public RecurrenceException(LocalizedString message) : base(message)
		{
		}

		public RecurrenceException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected RecurrenceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
