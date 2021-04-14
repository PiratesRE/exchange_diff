using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InvalidIdMalformedException : StoragePermanentException
	{
		internal InvalidIdMalformedException() : base(LocalizedString.Empty)
		{
		}

		internal InvalidIdMalformedException(Exception innerException) : base(LocalizedString.Empty, innerException)
		{
		}

		private InvalidIdMalformedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
