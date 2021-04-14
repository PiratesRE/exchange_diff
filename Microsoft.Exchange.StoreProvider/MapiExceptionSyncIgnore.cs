using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionSyncIgnore : MapiPermanentException
	{
		internal MapiExceptionSyncIgnore(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionSyncIgnore", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionSyncIgnore(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
