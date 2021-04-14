using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.Exchange.Diagnostics
{
	internal static class Utf8Csv
	{
		public static void WriteBom(Stream output)
		{
			Utf8Csv.WriteBytes(output, Utf8Csv.Bom);
		}

		public static void WriteHeaderRow(Stream output, string[] fields)
		{
			int num = fields.Length - 1;
			for (int i = 0; i < fields.Length; i++)
			{
				if (fields[i] != null)
				{
					Utf8Csv.EncodeEscapeAndWrite(output, fields[i]);
				}
				if (i == num)
				{
					Utf8Csv.WriteBytes(output, Utf8Csv.NewLine);
				}
				else
				{
					Utf8Csv.WriteByte(output, 44);
				}
			}
		}

		public static void WriteRawRow(Stream output, byte[][] fields)
		{
			int num = fields.Length - 1;
			for (int i = 0; i < fields.Length; i++)
			{
				if (fields[i] != null)
				{
					Utf8Csv.WriteBytes(output, fields[i]);
				}
				if (i == num)
				{
					Utf8Csv.WriteBytes(output, Utf8Csv.NewLine);
				}
				else
				{
					Utf8Csv.WriteByte(output, 44);
				}
			}
		}

		public static void WriteByte(Stream output, byte data)
		{
			output.WriteByte(data);
		}

		public static void WriteBytes(Stream output, byte[] data)
		{
			output.Write(data, 0, data.Length);
		}

		public static void EncodeAndWrite(Stream output, string s)
		{
			Utf8Csv.WriteBytes(output, Utf8Csv.Encode(s));
		}

		public static void EncodeEscapeAndWrite(Stream output, string s)
		{
			Utf8Csv.WriteBytes(output, Utf8Csv.EncodeAndEscape(s));
		}

		public static void EncodeAndWriteLine(Stream output, string s)
		{
			Utf8Csv.WriteBytes(output, Utf8Csv.Encode(s));
			Utf8Csv.WriteBytes(output, Utf8Csv.NewLine);
		}

		public static void EncodeEscapeAndWriteLine(Stream output, string s)
		{
			Utf8Csv.WriteBytes(output, Utf8Csv.EncodeAndEscape(s));
			Utf8Csv.WriteBytes(output, Utf8Csv.NewLine);
		}

		internal static byte[] Encode(string s)
		{
			return Encoding.UTF8.GetBytes(s);
		}

		internal static byte[] Escape(byte[] data)
		{
			return Utf8Csv.Escape(data, false);
		}

		internal static byte[] Escape(byte[] data, bool escapeLineBreaks)
		{
			bool flag = false;
			bool flag2 = false;
			List<int> list = null;
			for (int i = 0; i < data.Length; i++)
			{
				byte b = data[i];
				if (b <= 13)
				{
					if (b == 10 || b == 13)
					{
						if (escapeLineBreaks)
						{
							flag2 = true;
							list = (list ?? new List<int>());
							list.Add(i);
						}
						else
						{
							flag = true;
						}
					}
				}
				else if (b != 34)
				{
					if (b == 44)
					{
						flag = true;
					}
				}
				else
				{
					flag = true;
					list = (list ?? new List<int>());
					list.Add(i);
				}
			}
			if (!flag && !flag2)
			{
				return data;
			}
			int num = 0;
			byte[] array;
			int num2;
			if (flag)
			{
				array = new byte[data.Length + ((list == null) ? 0 : list.Count) + 2];
				array[0] = 34;
				array[array.Length - 1] = 34;
				num2 = 1;
			}
			else
			{
				array = new byte[data.Length + list.Count];
				num2 = 0;
			}
			if (list != null)
			{
				foreach (int num3 in list)
				{
					int num4 = num3 - num;
					Buffer.BlockCopy(data, num, array, num2, num4);
					int num5 = num2 + num4;
					byte b2 = data[num3];
					if (b2 != 10)
					{
						if (b2 != 13)
						{
							if (b2 != 34)
							{
								throw new InvalidOperationException(string.Format("Cannot escape char '0x{0:x2}'.", data[num3]));
							}
							array[num5] = 34;
							array[num5 + 1] = 34;
						}
						else
						{
							array[num5] = 92;
							array[num5 + 1] = 114;
						}
					}
					else
					{
						array[num5] = 92;
						array[num5 + 1] = 110;
					}
					num = num3 + 1;
					num2 += num4 + 2;
				}
			}
			Buffer.BlockCopy(data, num, array, num2, data.Length - num);
			return array;
		}

		internal static byte[] EncodeAndEscape(string s)
		{
			return Utf8Csv.EncodeAndEscape(s, false);
		}

		internal static byte[] EncodeAndEscape(string s, bool escapeLineBreaks)
		{
			return Utf8Csv.Escape(Encoding.UTF8.GetBytes(s), escapeLineBreaks);
		}

		internal static void AppendCollectionMember(StringBuilder buffer, string s)
		{
			buffer.Append(s);
			buffer.Append(';');
		}

		internal static void EscapeAndAppendCollectionMember(StringBuilder buffer, string sourceString, bool escapeLineBreaks)
		{
			bool flag = false;
			bool flag2 = false;
			List<int> list = null;
			for (int i = 0; i < sourceString.Length; i++)
			{
				char c = sourceString[i];
				if (c <= '\r')
				{
					if (c == '\n' || c == '\r')
					{
						if (escapeLineBreaks)
						{
							list = (list ?? new List<int>());
							list.Add(i);
							flag2 = true;
						}
						else
						{
							flag = true;
						}
					}
				}
				else if (c != '\'')
				{
					if (c == ';')
					{
						flag = true;
					}
				}
				else
				{
					list = (list ?? new List<int>());
					list.Add(i);
					flag = true;
				}
			}
			if (!flag && !flag2)
			{
				buffer.Append(sourceString);
				buffer.Append(';');
				return;
			}
			if (flag)
			{
				buffer.Append('\'');
			}
			int num = 0;
			if (list != null)
			{
				foreach (int num2 in list)
				{
					buffer.Append(sourceString, num, num2 - num);
					char c2 = sourceString[num2];
					if (c2 != '\n')
					{
						if (c2 != '\r')
						{
							if (c2 != '\'')
							{
								throw new InvalidOperationException(string.Format("Cannot escape char '0x{0:x2}'.", (byte)sourceString[num2]));
							}
							buffer.Append('\'', 2);
						}
						else
						{
							buffer.Append("\\r");
						}
					}
					else
					{
						buffer.Append("\\n");
					}
					num = num2 + 1;
				}
			}
			buffer.Append(sourceString, num, sourceString.Length - num);
			if (flag)
			{
				buffer.Append('\'');
			}
			buffer.Append(';');
		}

		private const byte CommaSeparator = 44;

		private const byte DoubleQuoteEscape = 34;

		private const char SemicolonSeparator = ';';

		private const char SingleQuoteEscape = '\'';

		private const byte CR = 13;

		private const byte LF = 10;

		private const int Backslash = 92;

		private const int N = 110;

		private const int R = 114;

		private static readonly byte[] Bom = new byte[]
		{
			239,
			187,
			191
		};

		private static readonly byte[] NewLine = new byte[]
		{
			13,
			10
		};
	}
}
