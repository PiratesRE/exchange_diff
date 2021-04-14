using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Diagnostics
{
	internal class CsvArrayDecoder
	{
		public CsvArrayDecoder(Type elementType)
		{
			this.elementType = elementType;
		}

		public object Decode(byte[] src, int offset, int count)
		{
			CsvDecoderCallback decoder = CsvDecoder.GetDecoder(this.elementType);
			List<object> list = new List<object>();
			new CsvParser(59, 39);
			byte[] array = new byte[4096];
			int i = offset;
			int num = offset + count;
			CsvArrayDecoder.State state = CsvArrayDecoder.State.Whitespace;
			int num2 = 0;
			while (i < num)
			{
				byte b = src[i++];
				switch (state)
				{
				case CsvArrayDecoder.State.Whitespace:
					if (b == 39)
					{
						state = CsvArrayDecoder.State.QuotedField;
						num2 = 0;
					}
					else
					{
						byte b2 = b;
						switch (b2)
						{
						case 9:
						case 10:
						case 13:
							continue;
						case 11:
						case 12:
							break;
						default:
							if (b2 == 32)
							{
								continue;
							}
							break;
						}
						state = CsvArrayDecoder.State.Field;
						i--;
						num2 = i;
					}
					break;
				case CsvArrayDecoder.State.Field:
					if (b == 59)
					{
						state = CsvArrayDecoder.State.Whitespace;
						list.Add(decoder(src, num2, i - num2 - 1));
						num2 = 0;
					}
					break;
				case CsvArrayDecoder.State.QuotedField:
					if (b == 39)
					{
						state = CsvArrayDecoder.State.EndQuote;
					}
					else if (num2 < array.Length)
					{
						array[num2++] = b;
					}
					break;
				case CsvArrayDecoder.State.EndQuote:
					if (b == 39)
					{
						state = CsvArrayDecoder.State.QuotedField;
						if (num2 < array.Length)
						{
							array[num2++] = 39;
						}
					}
					else
					{
						state = CsvArrayDecoder.State.EndQuoteIgnore;
						i--;
						list.Add(decoder(array, 0, num2));
					}
					break;
				case CsvArrayDecoder.State.EndQuoteIgnore:
					if (b == 59)
					{
						state = CsvArrayDecoder.State.Whitespace;
						num2 = 0;
					}
					break;
				}
			}
			switch (state)
			{
			case CsvArrayDecoder.State.Whitespace:
			case CsvArrayDecoder.State.QuotedField:
			case CsvArrayDecoder.State.EndQuote:
				list.Add(decoder(array, 0, num2));
				break;
			case CsvArrayDecoder.State.Field:
				list.Add(decoder(src, num2, i - num2));
				break;
			}
			Array array2 = Array.CreateInstance(this.elementType, list.Count);
			for (int j = 0; j < array2.Length; j++)
			{
				object obj = list[j];
				if (obj == null)
				{
					if (this.elementType.IsValueType)
					{
						return null;
					}
				}
				else if (!this.elementType.Equals(obj.GetType()))
				{
					return null;
				}
				array2.SetValue(obj, j);
			}
			return array2;
		}

		private const int ElementCapacity = 4096;

		private const byte Delimiter = 59;

		private const byte Quote = 39;

		private Type elementType;

		private enum State
		{
			Whitespace,
			Field,
			QuotedField,
			EndQuote,
			EndQuoteIgnore
		}
	}
}
