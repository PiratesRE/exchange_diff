using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionVirusMessageDeleted : MapiPermanentException
	{
		internal MapiExceptionVirusMessageDeleted(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionVirusMessageDeleted", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionVirusMessageDeleted(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
