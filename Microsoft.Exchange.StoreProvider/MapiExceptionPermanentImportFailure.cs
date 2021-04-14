using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionPermanentImportFailure : MapiPermanentException
	{
		internal MapiExceptionPermanentImportFailure(string message, Exception innerException) : base("MapiExceptionPermanentImportFailure", message, innerException)
		{
		}

		private MapiExceptionPermanentImportFailure(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
