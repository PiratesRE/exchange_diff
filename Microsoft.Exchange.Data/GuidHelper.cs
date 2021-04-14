using System;

namespace Microsoft.Exchange.Data
{
	internal static class GuidHelper
	{
		public static bool TryParseGuid(string g, out Guid guid)
		{
			int a = 0;
			short b = 0;
			short c = 0;
			byte d = 0;
			byte e = 0;
			byte f = 0;
			byte g2 = 0;
			byte h = 0;
			byte i = 0;
			byte j = 0;
			byte k = 0;
			try
			{
				if (string.IsNullOrEmpty(g))
				{
					return false;
				}
				int num = 0;
				int num2 = 0;
				if (char.IsWhiteSpace(g[0]) || char.IsWhiteSpace(g[g.Length - 1]))
				{
					g = g.Trim();
				}
				if (g.IndexOf('-', 0) >= 0)
				{
					if (g[0] == '{')
					{
						if (g.Length != 38 || g[37] != '}')
						{
							return false;
						}
						num = 1;
					}
					else if (g[0] == '(')
					{
						if (g.Length != 38 || g[37] != ')')
						{
							return false;
						}
						num = 1;
					}
					else if (g.Length != 36)
					{
						return false;
					}
					if (g[8 + num] != '-' || g[13 + num] != '-' || g[18 + num] != '-' || g[23 + num] != '-')
					{
						return false;
					}
					num2 = num;
					a = GuidHelper.StringToInt(g, 16, ref num2, 8);
					num2++;
					b = (short)GuidHelper.StringToInt(g, 16, ref num2, 4);
					num2++;
					c = (short)GuidHelper.StringToInt(g, 16, ref num2, 4);
					num2++;
					int num3 = GuidHelper.StringToInt(g, 16, ref num2, 4);
					num2++;
					long num4 = GuidHelper.StringToLong(g, 16, ref num2, 12);
					d = (byte)(num3 >> 8);
					e = (byte)num3;
					num3 = (int)(num4 >> 32);
					f = (byte)(num3 >> 8);
					g2 = (byte)num3;
					num3 = (int)num4;
					h = (byte)(num3 >> 24);
					i = (byte)(num3 >> 16);
					j = (byte)(num3 >> 8);
					k = (byte)num3;
				}
				else if (g.IndexOf('{', 0) >= 0)
				{
					g = GuidHelper.EatAllWhitespace(g);
					if (g[0] != '{')
					{
						return false;
					}
					if (!GuidHelper.IsHexPrefix(g, 1))
					{
						return false;
					}
					int num5 = 3;
					int num6 = g.IndexOf(',', num5) - num5;
					if (num6 <= 0)
					{
						return false;
					}
					a = Convert.ToInt32(g.Substring(num5, num6), 16);
					if (!GuidHelper.IsHexPrefix(g, num5 + num6 + 1))
					{
						return false;
					}
					num5 = num5 + num6 + 3;
					num6 = g.IndexOf(',', num5) - num5;
					if (num6 <= 0)
					{
						return false;
					}
					b = (short)Convert.ToInt32(g.Substring(num5, num6), 16);
					if (!GuidHelper.IsHexPrefix(g, num5 + num6 + 1))
					{
						return false;
					}
					num5 = num5 + num6 + 3;
					num6 = g.IndexOf(',', num5) - num5;
					if (num6 <= 0)
					{
						return false;
					}
					c = (short)Convert.ToInt32(g.Substring(num5, num6), 16);
					if (g.Length <= num5 + num6 + 1 || g[num5 + num6 + 1] != '{')
					{
						return false;
					}
					num6++;
					byte[] array = new byte[8];
					for (int l = 0; l < 8; l++)
					{
						if (!GuidHelper.IsHexPrefix(g, num5 + num6 + 1))
						{
							return false;
						}
						num5 = num5 + num6 + 3;
						if (l < 7)
						{
							num6 = g.IndexOf(',', num5) - num5;
							if (num6 <= 0)
							{
								return false;
							}
						}
						else
						{
							num6 = g.IndexOf('}', num5) - num5;
							if (num6 <= 0)
							{
								return false;
							}
						}
						uint num7 = (uint)Convert.ToInt32(g.Substring(num5, num6), 16);
						if (num7 > 255U)
						{
							return false;
						}
						array[l] = (byte)num7;
					}
					d = array[0];
					e = array[1];
					f = array[2];
					g2 = array[3];
					h = array[4];
					i = array[5];
					j = array[6];
					k = array[7];
					if (num5 + num6 + 1 >= g.Length || g[num5 + num6 + 1] != '}')
					{
						return false;
					}
					if (num5 + num6 + 1 != g.Length - 1)
					{
						return false;
					}
				}
				else
				{
					if (g.Length != 32)
					{
						return false;
					}
					foreach (char c2 in g)
					{
						if (c2 < '0' || c2 > '9')
						{
							char c3 = char.ToUpperInvariant(c2);
							if (c3 < 'A' || c3 > 'F')
							{
								return false;
							}
						}
					}
					a = Convert.ToInt32(g.Substring(num, 8), 16);
					num += 8;
					b = (short)Convert.ToInt32(g.Substring(num, 4), 16);
					num += 4;
					c = (short)Convert.ToInt32(g.Substring(num, 4), 16);
					num += 4;
					int num3 = (int)((short)Convert.ToInt32(g.Substring(num, 4), 16));
					num += 4;
					num2 = num;
					long num4 = GuidHelper.StringToLong(g, 16, ref num2, 12);
					d = (byte)(num3 >> 8);
					e = (byte)num3;
					num3 = (int)(num4 >> 32);
					f = (byte)(num3 >> 8);
					g2 = (byte)num3;
					num3 = (int)num4;
					h = (byte)(num3 >> 24);
					i = (byte)(num3 >> 16);
					j = (byte)(num3 >> 8);
					k = (byte)num3;
				}
				return true;
			}
			catch (ArgumentException)
			{
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
			}
			catch (IndexOutOfRangeException)
			{
			}
			finally
			{
				guid = new Guid(a, b, c, d, e, f, g2, h, i, j, k);
			}
			return false;
		}

		private static bool IsHexPrefix(string str, int i)
		{
			return str[i] == '0' && char.ToLowerInvariant(str[i + 1]) == 'x';
		}

		private static string EatAllWhitespace(string str)
		{
			int length = 0;
			char[] array = new char[str.Length];
			foreach (char c in str)
			{
				if (!char.IsWhiteSpace(c))
				{
					array[length++] = c;
				}
			}
			return new string(array, 0, length);
		}

		private static int StringToInt(string s, int radix, ref int currPos, int requiredLength)
		{
			int result;
			try
			{
				result = Convert.ToInt32(s.Substring(currPos, requiredLength), radix);
			}
			finally
			{
				currPos += requiredLength;
			}
			return result;
		}

		private static long StringToLong(string s, int radix, ref int currPos, int requiredLength)
		{
			long result;
			try
			{
				result = Convert.ToInt64(s.Substring(currPos, requiredLength), radix);
			}
			finally
			{
				currPos += requiredLength;
			}
			return result;
		}
	}
}
