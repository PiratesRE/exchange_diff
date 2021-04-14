using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionUserInformationNoAccess : MapiPermanentException
	{
		internal MapiExceptionUserInformationNoAccess(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionUserInformationNoAccess", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionUserInformationNoAccess(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
