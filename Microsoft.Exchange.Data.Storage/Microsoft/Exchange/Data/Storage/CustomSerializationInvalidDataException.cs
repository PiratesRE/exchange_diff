using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CustomSerializationInvalidDataException : CustomSerializationException
	{
		public CustomSerializationInvalidDataException() : base(ServerStrings.ExInvalidCustomSerializationData)
		{
		}

		public CustomSerializationInvalidDataException(Exception innerException) : base(ServerStrings.ExInvalidCustomSerializationData, innerException)
		{
		}
	}
}
