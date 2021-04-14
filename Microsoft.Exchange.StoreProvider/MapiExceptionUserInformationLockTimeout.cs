using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionUserInformationLockTimeout : MapiRetryableException
	{
		internal MapiExceptionUserInformationLockTimeout(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionUserInformationLockTimeout", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionUserInformationLockTimeout(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
