using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class NoMoreConnectionsException : StoragePermanentException
	{
		public NoMoreConnectionsException(LocalizedString message) : base(message)
		{
		}

		public NoMoreConnectionsException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected NoMoreConnectionsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
