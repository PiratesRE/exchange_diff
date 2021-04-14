using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionUserInformationAlreadyExists : MapiPermanentException
	{
		internal MapiExceptionUserInformationAlreadyExists(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionUserInformationAlreadyExists", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionUserInformationAlreadyExists(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
