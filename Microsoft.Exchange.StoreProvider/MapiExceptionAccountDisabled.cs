using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionAccountDisabled : MapiPermanentException
	{
		internal MapiExceptionAccountDisabled(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionAccountDisabled", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionAccountDisabled(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
