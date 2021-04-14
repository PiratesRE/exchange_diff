using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionUserInformationNotFound : MapiPermanentException
	{
		internal MapiExceptionUserInformationNotFound(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionUserInformationNotFound", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionUserInformationNotFound(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
