using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class AutoDAccessException : StoragePermanentException
	{
		public AutoDAccessException(LocalizedString message) : base(message)
		{
		}

		public AutoDAccessException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected AutoDAccessException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
