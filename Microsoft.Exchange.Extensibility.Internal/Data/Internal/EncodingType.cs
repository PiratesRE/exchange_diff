using System;

namespace Microsoft.Exchange.Data.Internal
{
	internal enum EncodingType : byte
	{
		Unknown,
		Boolean,
		Integer,
		BitString,
		OctetString,
		Null,
		ObjectIdentifier,
		ObjectDescriptor,
		External,
		Real,
		Enumerated,
		EmbeddedPdv,
		Utf8String,
		RelativeObjectIdentifier,
		Sequence = 16,
		Set,
		NumericString,
		PrintableString,
		TeletexString,
		VideotexString,
		IA5String,
		UtcTime,
		GeneralizedTime,
		GraphicString,
		VisibleString,
		GeneralString,
		UniversalString,
		UnrestrictedCharacterString,
		BMPString
	}
}
