using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionMailboxSoftDeleted : MapiPermanentException
	{
		internal MapiExceptionMailboxSoftDeleted(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionMailboxSoftDeleted", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionMailboxSoftDeleted(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
