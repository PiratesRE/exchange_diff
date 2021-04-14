using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionHasFolders : MapiPermanentException
	{
		internal MapiExceptionHasFolders(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionHasFolders", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionHasFolders(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
