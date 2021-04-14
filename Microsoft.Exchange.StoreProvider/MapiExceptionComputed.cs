using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionComputed : MapiPermanentException
	{
		internal MapiExceptionComputed(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionComputed", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionComputed(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
