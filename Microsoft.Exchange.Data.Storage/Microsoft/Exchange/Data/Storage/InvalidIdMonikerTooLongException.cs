using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class InvalidIdMonikerTooLongException : StoragePermanentException
	{
		internal InvalidIdMonikerTooLongException() : base(LocalizedString.Empty)
		{
		}

		private InvalidIdMonikerTooLongException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
