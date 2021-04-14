using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionNoCreateSubfolderRight : MapiPermanentException
	{
		internal MapiExceptionNoCreateSubfolderRight(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionNoCreateSubfolderRight", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionNoCreateSubfolderRight(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
