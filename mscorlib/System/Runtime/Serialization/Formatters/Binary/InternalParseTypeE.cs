﻿using System;

namespace System.Runtime.Serialization.Formatters.Binary
{
	[Serializable]
	internal enum InternalParseTypeE
	{
		Empty,
		SerializedStreamHeader,
		Object,
		Member,
		ObjectEnd,
		MemberEnd,
		Headers,
		HeadersEnd,
		SerializedStreamHeaderEnd,
		Envelope,
		EnvelopeEnd,
		Body,
		BodyEnd
	}
}
