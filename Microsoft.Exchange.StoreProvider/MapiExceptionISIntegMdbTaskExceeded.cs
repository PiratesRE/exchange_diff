﻿using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionISIntegMdbTaskExceeded : MapiRetryableException
	{
		internal MapiExceptionISIntegMdbTaskExceeded(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionISIntegMdbTaskExceeded", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionISIntegMdbTaskExceeded(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
