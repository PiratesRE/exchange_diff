using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class CheckpointTooDeepException : StorageTransientException
	{
		public CheckpointTooDeepException(LocalizedString message) : base(message)
		{
		}

		public CheckpointTooDeepException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected CheckpointTooDeepException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
