using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionSearchFolderNotEmpty : MapiPermanentException
	{
		internal MapiExceptionSearchFolderNotEmpty(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionSearchFolderNotEmpty", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionSearchFolderNotEmpty(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
