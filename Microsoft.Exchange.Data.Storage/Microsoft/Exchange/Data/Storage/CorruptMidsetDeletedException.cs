using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class CorruptMidsetDeletedException : StoragePermanentException
	{
		public CorruptMidsetDeletedException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected CorruptMidsetDeletedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
