using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Search.AqsParser
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	[Serializable]
	internal class ParserException : LocalizedException
	{
		private static bool IsNullOrEmpty(List<ParserErrorInfo> list)
		{
			return list == null || list.Count <= 0;
		}

		internal new ParserErrorCode ErrorCode
		{
			get
			{
				if (!ParserException.IsNullOrEmpty(this.ParserErrors))
				{
					return this.ParserErrors[0].ErrorCode;
				}
				return ParserErrorCode.NoError;
			}
		}

		internal List<ParserErrorInfo> ParserErrors { get; set; }

		internal ParserException(ParserErrorInfo parserError) : base(parserError.Message)
		{
			this.ParserErrors = new List<ParserErrorInfo>();
			this.ParserErrors.Add(parserError);
		}

		internal ParserException(ParserErrorInfo parserError, Exception innerException) : base(parserError.Message, innerException)
		{
			this.ParserErrors = new List<ParserErrorInfo>();
			this.ParserErrors.Add(parserError);
		}

		internal ParserException(List<ParserErrorInfo> parserErrors) : base(ParserException.IsNullOrEmpty(parserErrors) ? default(LocalizedString) : parserErrors[0].Message)
		{
			if (ParserException.IsNullOrEmpty(parserErrors))
			{
				throw new ArgumentException("parserErrors");
			}
			this.ParserErrors = new List<ParserErrorInfo>();
			this.ParserErrors.AddRange(parserErrors);
		}
	}
}
