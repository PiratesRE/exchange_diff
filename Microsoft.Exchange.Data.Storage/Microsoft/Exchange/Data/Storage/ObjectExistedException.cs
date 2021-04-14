using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class ObjectExistedException : StoragePermanentException
	{
		public ObjectExistedException(LocalizedString message) : base(message)
		{
		}

		public ObjectExistedException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected ObjectExistedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
