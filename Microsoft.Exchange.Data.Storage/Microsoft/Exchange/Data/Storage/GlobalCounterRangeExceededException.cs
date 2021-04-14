using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class GlobalCounterRangeExceededException : StorageTransientException
	{
		public GlobalCounterRangeExceededException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected GlobalCounterRangeExceededException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
