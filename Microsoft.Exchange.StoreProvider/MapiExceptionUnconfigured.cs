﻿using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionUnconfigured : MapiPermanentException
	{
		internal MapiExceptionUnconfigured(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionUnconfigured", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionUnconfigured(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
