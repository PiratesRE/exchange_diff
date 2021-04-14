using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class LimitExceededException : StoragePermanentException
	{
		public LimitExceededException(LocalizedString message) : base(message)
		{
		}

		protected LimitExceededException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
