using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionWrongProvisionedFid : MapiPermanentException
	{
		internal MapiExceptionWrongProvisionedFid(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionWrongProvisionedFid", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionWrongProvisionedFid(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
