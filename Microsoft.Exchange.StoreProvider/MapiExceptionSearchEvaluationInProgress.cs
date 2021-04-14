using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionSearchEvaluationInProgress : MapiRetryableException
	{
		internal MapiExceptionSearchEvaluationInProgress(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionSearchEvaluationInProgress", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionSearchEvaluationInProgress(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
