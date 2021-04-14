using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionRecursiveSearchChainTooDeep : MapiPermanentException
	{
		internal MapiExceptionRecursiveSearchChainTooDeep(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionRecursiveSearchChainTooDeep", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionRecursiveSearchChainTooDeep(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
