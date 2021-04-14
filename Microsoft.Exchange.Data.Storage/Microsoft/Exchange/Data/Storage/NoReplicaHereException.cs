using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class NoReplicaHereException : StoragePermanentException
	{
		public NoReplicaHereException(LocalizedString message) : base(message)
		{
		}

		public NoReplicaHereException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected NoReplicaHereException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
