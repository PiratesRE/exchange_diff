using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionDataLoss : MapiPermanentException
	{
		internal MapiExceptionDataLoss(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionDataLoss", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionDataLoss(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
