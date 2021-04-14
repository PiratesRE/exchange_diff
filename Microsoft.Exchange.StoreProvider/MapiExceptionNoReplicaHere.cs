using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionNoReplicaHere : MapiPermanentException
	{
		internal MapiExceptionNoReplicaHere(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionNoReplicaHere", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionNoReplicaHere(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
