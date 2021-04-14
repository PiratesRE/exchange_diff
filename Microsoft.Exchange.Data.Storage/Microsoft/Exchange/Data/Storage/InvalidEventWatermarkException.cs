using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class InvalidEventWatermarkException : StoragePermanentException
	{
		public InvalidEventWatermarkException(LocalizedString message) : base(message)
		{
		}

		protected InvalidEventWatermarkException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
