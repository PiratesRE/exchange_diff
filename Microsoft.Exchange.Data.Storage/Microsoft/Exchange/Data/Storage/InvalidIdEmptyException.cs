using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InvalidIdEmptyException : StoragePermanentException
	{
		internal InvalidIdEmptyException() : base(LocalizedString.Empty)
		{
		}

		private InvalidIdEmptyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
