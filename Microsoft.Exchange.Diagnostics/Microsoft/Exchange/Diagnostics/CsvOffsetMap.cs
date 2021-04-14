using System;
using Microsoft.Exchange.Diagnostics.Components.Diagnostics;

namespace Microsoft.Exchange.Diagnostics
{
	internal class CsvOffsetMap
	{
		public CsvOffsetMap(byte delimiter, byte quote)
		{
			this.parser = new CsvParser(delimiter, quote);
			this.offset = new int[256];
			this.length = new int[256];
		}

		public int Count
		{
			get
			{
				return this.fields;
			}
		}

		public void Reset()
		{
			this.parser.Reset();
			this.fields = 0;
			this.passedCR = false;
			this.afterQuote = false;
		}

		public int Parse(byte[] buffer, int offset, int count, int rowBase)
		{
			ExTraceGlobals.CommonTracer.TraceDebug<int, int, int>((long)this.GetHashCode(), "CsvOffsetMap Parse with offset {0}, count {1}, base {2}", offset, count, rowBase);
			int i = offset;
			int num = offset + count;
			if (this.parser.State == CsvParserState.LineEnd)
			{
				this.fields = 0;
			}
			while (i < num)
			{
				i = this.parser.Parse(buffer, i, num - i);
				if (i == -1)
				{
					return -1;
				}
				if (this.fields == 256)
				{
					if (this.parser.State == CsvParserState.LineEnd)
					{
						return i;
					}
				}
				else
				{
					switch (this.parser.State)
					{
					case CsvParserState.Whitespace:
					case CsvParserState.LineEnd:
						if (this.afterQuote)
						{
							this.afterQuote = false;
						}
						else
						{
							this.length[this.fields] = i - 1 - rowBase - this.offset[this.fields] - (this.passedCR ? 1 : 0);
							this.fields++;
							this.passedCR = false;
						}
						break;
					case CsvParserState.Field:
						if (!this.passedCR)
						{
							this.offset[this.fields] = i - rowBase;
						}
						else
						{
							this.passedCR = false;
						}
						break;
					case CsvParserState.FieldCR:
					case CsvParserState.QuotedFieldCR:
						this.passedCR = true;
						break;
					case CsvParserState.QuotedField:
						if (!this.passedCR)
						{
							if (!this.inQuote)
							{
								this.offset[this.fields] = i - 1 - rowBase;
							}
						}
						else
						{
							this.passedCR = false;
						}
						break;
					case CsvParserState.QuotedFieldQuote:
						this.inQuote = true;
						break;
					case CsvParserState.EndQuote:
						this.passedCR = false;
						this.inQuote = false;
						i--;
						break;
					case CsvParserState.EndQuoteIgnore:
						this.length[this.fields] = i - 1 - rowBase - this.offset[this.fields];
						this.fields++;
						this.afterQuote = true;
						break;
					}
					if (this.parser.State == CsvParserState.LineEnd)
					{
						return i;
					}
				}
			}
			return -1;
		}

		public int GetOffset(int index)
		{
			return this.offset[index];
		}

		public int GetLength(int index)
		{
			return this.length[index];
		}

		private const int FieldLimit = 256;

		private CsvParser parser;

		private int[] offset;

		private int[] length;

		private int fields;

		private bool passedCR;

		private bool afterQuote;

		private bool inQuote;
	}
}
