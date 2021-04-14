using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionSearchFolderScopeViolation : MapiPermanentException
	{
		internal MapiExceptionSearchFolderScopeViolation(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionSearchFolderScopeViolation", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionSearchFolderScopeViolation(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
