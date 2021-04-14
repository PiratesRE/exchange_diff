﻿using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal sealed class MapiExceptionFormNotValid : MapiPermanentException
	{
		internal MapiExceptionFormNotValid(string message, int hr, int ec, DiagnosticContext context, Exception innerException) : base("MapiExceptionFormNotValid", message, hr, ec, context, innerException)
		{
		}

		private MapiExceptionFormNotValid(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
