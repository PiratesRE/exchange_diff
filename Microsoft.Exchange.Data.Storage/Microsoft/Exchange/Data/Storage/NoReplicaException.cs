using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class NoReplicaException : StoragePermanentException
	{
		public NoReplicaException(LocalizedString message) : base(message)
		{
		}

		public NoReplicaException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected NoReplicaException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
