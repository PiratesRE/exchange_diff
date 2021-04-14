using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionPartialCompletion : MapiPermanentException
	{
		internal MapiExceptionPartialCompletion(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionPartialCompletion", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionPartialCompletion(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
