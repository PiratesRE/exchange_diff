using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionJetErrorDatabaseBufferDependenciesCorrupted : MapiPermanentException
	{
		internal MapiExceptionJetErrorDatabaseBufferDependenciesCorrupted(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionJetErrorDatabaseBufferDependenciesCorrupted", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionJetErrorDatabaseBufferDependenciesCorrupted(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
