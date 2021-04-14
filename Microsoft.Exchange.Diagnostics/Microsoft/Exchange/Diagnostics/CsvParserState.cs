using System;

namespace Microsoft.Exchange.Diagnostics
{
	internal enum CsvParserState
	{
		Whitespace,
		LineEnd,
		Field,
		FieldCR,
		QuotedField,
		QuotedFieldCR,
		QuotedFieldQuote,
		EndQuote,
		EndQuoteIgnore
	}
}
