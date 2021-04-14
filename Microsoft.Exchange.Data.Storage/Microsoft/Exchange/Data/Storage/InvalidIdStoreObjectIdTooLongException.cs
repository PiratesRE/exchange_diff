using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InvalidIdStoreObjectIdTooLongException : StoragePermanentException
	{
		internal InvalidIdStoreObjectIdTooLongException() : base(LocalizedString.Empty)
		{
		}

		private InvalidIdStoreObjectIdTooLongException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
