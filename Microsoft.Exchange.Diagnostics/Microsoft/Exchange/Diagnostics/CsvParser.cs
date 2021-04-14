using System;
using Microsoft.Exchange.Diagnostics.Components.Diagnostics;

namespace Microsoft.Exchange.Diagnostics
{
	internal class CsvParser
	{
		public CsvParser(byte delimiter, byte quote)
		{
			this.delimiter = delimiter;
			this.quote = quote;
			this.state = CsvParserState.Whitespace;
		}

		public CsvParserState State
		{
			get
			{
				return this.state;
			}
		}

		public void Reset()
		{
			this.state = CsvParserState.Whitespace;
		}

		public int Parse(byte[] data, int offset, int count)
		{
			ExTraceGlobals.CommonTracer.TraceDebug<int, int>((long)this.GetHashCode(), "CsvParser Parse with offset {0}, count {1}", offset, count);
			int num = offset;
			int num2 = num + count;
			CsvParserState csvParserState = this.state;
			while (num < num2 && this.state == csvParserState)
			{
				byte b = data[num++];
				switch (this.state)
				{
				case CsvParserState.Whitespace:
				case CsvParserState.LineEnd:
					if (b == this.quote)
					{
						this.state = CsvParserState.QuotedField;
					}
					else
					{
						byte b2 = b;
						if (b2 != 9 && b2 != 13 && b2 != 32)
						{
							this.state = CsvParserState.Field;
							num--;
						}
					}
					break;
				case CsvParserState.Field:
					if (b == this.delimiter)
					{
						this.state = CsvParserState.Whitespace;
					}
					else if (b == 10)
					{
						this.state = CsvParserState.LineEnd;
					}
					else if (b == 13)
					{
						this.state = CsvParserState.FieldCR;
					}
					break;
				case CsvParserState.FieldCR:
					if (b == 10)
					{
						this.state = CsvParserState.LineEnd;
					}
					else if (b == this.delimiter)
					{
						this.state = CsvParserState.Whitespace;
					}
					else if (b != 13)
					{
						this.state = CsvParserState.Field;
					}
					break;
				case CsvParserState.QuotedField:
					if (b == this.quote)
					{
						this.state = CsvParserState.QuotedFieldQuote;
					}
					else if (b == 13)
					{
						this.state = CsvParserState.QuotedFieldCR;
					}
					break;
				case CsvParserState.QuotedFieldCR:
					if (b == this.quote)
					{
						this.state = CsvParserState.QuotedFieldQuote;
					}
					else if (b != 13)
					{
						this.state = CsvParserState.QuotedField;
					}
					break;
				case CsvParserState.QuotedFieldQuote:
					if (b == this.quote)
					{
						this.state = CsvParserState.QuotedField;
					}
					else
					{
						this.state = CsvParserState.EndQuote;
					}
					break;
				case CsvParserState.EndQuote:
					if (b == this.quote)
					{
						this.state = CsvParserState.QuotedField;
					}
					else if (b == this.delimiter)
					{
						this.state = CsvParserState.Whitespace;
					}
					else if (b == 10)
					{
						this.state = CsvParserState.LineEnd;
					}
					else
					{
						this.state = CsvParserState.EndQuoteIgnore;
					}
					break;
				case CsvParserState.EndQuoteIgnore:
					if (b == this.delimiter)
					{
						this.state = CsvParserState.Whitespace;
					}
					else if (b == 10)
					{
						this.state = CsvParserState.LineEnd;
					}
					break;
				}
			}
			if (this.state != csvParserState)
			{
				return num;
			}
			return -1;
		}

		private CsvParserState state;

		private byte delimiter;

		private byte quote;
	}
}
