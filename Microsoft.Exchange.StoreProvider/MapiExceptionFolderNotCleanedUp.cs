using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionFolderNotCleanedUp : MapiPermanentException
	{
		internal MapiExceptionFolderNotCleanedUp(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionFolderNotCleanedUp", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionFolderNotCleanedUp(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
