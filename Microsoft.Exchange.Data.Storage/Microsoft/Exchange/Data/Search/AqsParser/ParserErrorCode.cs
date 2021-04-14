using System;

namespace Microsoft.Exchange.Data.Search.AqsParser
{
	internal enum ParserErrorCode
	{
		NoError,
		InvalidKindFormat,
		InvalidDateTimeFormat,
		InvalidDateTimeRange,
		InvalidPropertyKey,
		MissingPropertyValue,
		MissingOperand,
		InvalidOperator,
		UnbalancedQuote,
		UnbalancedParenthesis,
		SuffixMatchNotSupported,
		UnexpectedToken,
		InvalidModifier,
		StructuredQueryException,
		KqlParseException,
		ParserError
	}
}
