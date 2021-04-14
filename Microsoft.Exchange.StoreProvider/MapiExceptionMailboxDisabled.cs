using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionMailboxDisabled : MapiPermanentException
	{
		internal MapiExceptionMailboxDisabled(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionMailboxDisabled", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionMailboxDisabled(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
