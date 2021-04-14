using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionFolderCycle : MapiPermanentException
	{
		internal MapiExceptionFolderCycle(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionFolderCycle", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionFolderCycle(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
