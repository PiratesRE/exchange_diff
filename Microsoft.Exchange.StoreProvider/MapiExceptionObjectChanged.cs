using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionObjectChanged : MapiRetryableException
	{
		internal MapiExceptionObjectChanged(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionObjectChanged", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionObjectChanged(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
