using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class EventNotFoundException : StoragePermanentException
	{
		public EventNotFoundException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		public EventNotFoundException(LocalizedString message) : base(message)
		{
		}

		protected EventNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
