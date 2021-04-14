using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Search.AqsParser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ParserErrorInfo
	{
		private static LocalizedString FormatErrorCode(ParserErrorCode errorCode)
		{
			switch (errorCode)
			{
			case ParserErrorCode.InvalidKindFormat:
				return ServerStrings.InvalidKindFormat;
			case ParserErrorCode.InvalidDateTimeFormat:
				return ServerStrings.InvalidDateTimeFormat;
			case ParserErrorCode.InvalidDateTimeRange:
				return ServerStrings.InvalidDateTimeRange;
			case ParserErrorCode.InvalidPropertyKey:
				return ServerStrings.InvalidPropertyKey;
			case ParserErrorCode.MissingPropertyValue:
				return ServerStrings.MissingPropertyValue;
			case ParserErrorCode.MissingOperand:
				return ServerStrings.MissingOperand;
			case ParserErrorCode.InvalidOperator:
				return ServerStrings.InvalidOperator;
			case ParserErrorCode.UnbalancedQuote:
				return ServerStrings.UnbalancedQuote;
			case ParserErrorCode.UnbalancedParenthesis:
				return ServerStrings.UnbalancedParenthesis;
			case ParserErrorCode.SuffixMatchNotSupported:
				return ServerStrings.SuffixMatchNotSupported;
			case ParserErrorCode.UnexpectedToken:
				return ServerStrings.UnexpectedToken;
			case ParserErrorCode.InvalidModifier:
				return ServerStrings.InvalidModifier;
			case ParserErrorCode.StructuredQueryException:
				return ServerStrings.StructuredQueryException;
			case ParserErrorCode.KqlParseException:
				return ServerStrings.KqlParseException;
			case ParserErrorCode.ParserError:
				return ServerStrings.InternalParserError;
			default:
				return default(LocalizedString);
			}
		}

		internal ParserErrorCode ErrorCode { get; set; }

		internal LocalizedString Message { get; set; }

		internal TokenInfo ErrorToken { get; set; }

		internal ParserErrorInfo(ParserErrorCode errorCode) : this(errorCode, null)
		{
		}

		internal ParserErrorInfo(ParserErrorCode errorCode, TokenInfo errorToken) : this(errorCode, ParserErrorInfo.FormatErrorCode(errorCode), errorToken)
		{
		}

		internal ParserErrorInfo(ParserErrorCode errorCode, LocalizedString message, TokenInfo errorToken)
		{
			this.ErrorCode = errorCode;
			this.Message = message;
			this.ErrorToken = errorToken;
		}

		public override string ToString()
		{
			if (this.ErrorToken == null || !this.ErrorToken.IsValid)
			{
				return this.Message;
			}
			if (this.ErrorToken.Length <= 0)
			{
				return this.Message + string.Format(" Error position: {0}", this.ErrorToken.FirstChar);
			}
			return this.Message + string.Format(" Error position: {0}, length: {1}", this.ErrorToken.FirstChar, this.ErrorToken.Length);
		}
	}
}
