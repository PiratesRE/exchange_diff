using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionNoReplicaAvailable : MapiPermanentException
	{
		internal MapiExceptionNoReplicaAvailable(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionNoReplicaAvailable", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionNoReplicaAvailable(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
