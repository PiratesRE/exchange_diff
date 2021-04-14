using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionFolderDisabled : MapiPermanentException
	{
		internal MapiExceptionFolderDisabled(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionFolderDisabled", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionFolderDisabled(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
