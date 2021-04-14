using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorDatabaseInvalidPath : MapiPermanentException
	{
		internal MapiExceptionJetErrorDatabaseInvalidPath(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorDatabaseInvalidPath", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorDatabaseInvalidPath(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
