﻿using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionDiskError : MapiRetryableException
	{
		internal MapiExceptionDiskError(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionDiskError", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionDiskError(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
