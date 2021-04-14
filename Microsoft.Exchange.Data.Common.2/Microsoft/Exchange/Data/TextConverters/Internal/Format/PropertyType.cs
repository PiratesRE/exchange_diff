using System;

namespace Microsoft.Exchange.Data.TextConverters.Internal.Format
{
	internal enum PropertyType : byte
	{
		Null,
		Calculated,
		Bool,
		String,
		MultiValue,
		Enum,
		Color,
		Integer,
		Fractional,
		Percentage,
		AbsLength,
		RelLength,
		Pixels,
		Ems,
		Exs,
		HtmlFontUnits,
		RelHtmlFontUnits,
		Multiple,
		Milliseconds,
		kHz,
		Degrees,
		FirstLength = 10,
		LastLength = 17
	}
}
