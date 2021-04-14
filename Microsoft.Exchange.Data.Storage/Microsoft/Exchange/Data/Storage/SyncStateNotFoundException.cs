using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class SyncStateNotFoundException : ObjectNotFoundException
	{
		public SyncStateNotFoundException(LocalizedString message) : base(message)
		{
		}

		public SyncStateNotFoundException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected SyncStateNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
