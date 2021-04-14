using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CustomSerializationException : StoragePermanentException
	{
		public CustomSerializationException(LocalizedString message) : base(message)
		{
		}

		public CustomSerializationException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
