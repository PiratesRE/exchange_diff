using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class SyncStateExistedException : ObjectExistedException
	{
		public SyncStateExistedException(LocalizedString message) : base(message)
		{
		}

		public SyncStateExistedException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected SyncStateExistedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
