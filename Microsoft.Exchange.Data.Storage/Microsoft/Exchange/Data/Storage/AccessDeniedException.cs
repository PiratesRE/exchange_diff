using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class AccessDeniedException : StoragePermanentException
	{
		public AccessDeniedException(LocalizedString message) : base(message)
		{
		}

		public AccessDeniedException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected AccessDeniedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
