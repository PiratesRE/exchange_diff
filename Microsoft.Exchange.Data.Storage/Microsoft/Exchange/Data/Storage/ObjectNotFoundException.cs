using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class ObjectNotFoundException : StoragePermanentException
	{
		public ObjectNotFoundException(LocalizedString message) : base(message)
		{
		}

		public ObjectNotFoundException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected ObjectNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
