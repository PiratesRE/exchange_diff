using System;
using System.Text;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Conversion
{
	internal static class MdbefWriter
	{
		public static int SizeOf(MapiPropType propType, object value)
		{
			int num = 4;
			if (propType <= MapiPropType.ServerId)
			{
				if (propType <= MapiPropType.String)
				{
					switch (propType)
					{
					case MapiPropType.Null:
					case (MapiPropType)8:
					case (MapiPropType)9:
					case MapiPropType.Error:
						goto IL_2B7;
					case MapiPropType.Short:
					case MapiPropType.Boolean:
						return num + 2;
					case MapiPropType.Int:
						return num + 4;
					case MapiPropType.Float:
						return num + 4;
					case MapiPropType.Double:
					case MapiPropType.AppTime:
						return num + 8;
					case MapiPropType.Currency:
						break;
					default:
						if (propType != MapiPropType.Long)
						{
							switch (propType)
							{
							case MapiPropType.AnsiString:
								return num + (4 + ((string)value).Length + 1);
							case MapiPropType.String:
								return num + (4 + Encoding.Unicode.GetByteCount((string)value) + 2);
							default:
								goto IL_2B7;
							}
						}
						break;
					}
				}
				else if (propType != MapiPropType.SysTime)
				{
					if (propType == MapiPropType.Guid)
					{
						return num + 16;
					}
					if (propType != MapiPropType.ServerId)
					{
						goto IL_2B7;
					}
					goto IL_18D;
				}
				return num + 8;
			}
			if (propType <= MapiPropType.LongArray)
			{
				if (propType != MapiPropType.Binary)
				{
					switch (propType)
					{
					case MapiPropType.ShortArray:
						return num + (4 + ((short[])value).Length * 2);
					case MapiPropType.IntArray:
						return num + (4 + ((int[])value).Length * 4);
					case MapiPropType.FloatArray:
						return num + (4 + ((float[])value).Length * 4);
					case MapiPropType.DoubleArray:
					case MapiPropType.AppTimeArray:
						return num + (4 + ((double[])value).Length * 8);
					case MapiPropType.CurrencyArray:
						break;
					default:
						if (propType != MapiPropType.LongArray)
						{
							goto IL_2B7;
						}
						break;
					}
					return num + (4 + ((long[])value).Length * 8);
				}
			}
			else if (propType <= MapiPropType.SysTimeArray)
			{
				switch (propType)
				{
				case MapiPropType.AnsiStringArray:
				{
					num += 4;
					string[] array = (string[])value;
					for (int i = 0; i < array.Length; i++)
					{
						num += 4 + array[i].Length + 1;
					}
					return num;
				}
				case MapiPropType.StringArray:
				{
					num += 4;
					string[] array2 = (string[])value;
					for (int j = 0; j < array2.Length; j++)
					{
						num += 4 + Encoding.Unicode.GetByteCount(array2[j]) + 2;
					}
					return num;
				}
				default:
					if (propType != MapiPropType.SysTimeArray)
					{
						goto IL_2B7;
					}
					return num + (4 + ((DateTime[])value).Length * 8);
				}
			}
			else
			{
				if (propType == MapiPropType.GuidArray)
				{
					return num + (4 + ((Guid[])value).Length * 16);
				}
				if (propType != MapiPropType.BinaryArray)
				{
					goto IL_2B7;
				}
				num += 4;
				byte[][] array3 = (byte[][])value;
				for (int k = 0; k < array3.Length; k++)
				{
					num += 4 + array3[k].Length;
				}
				return num;
			}
			IL_18D:
			return num + (4 + ((byte[])value).Length);
			IL_2B7:
			num = 0;
			return num;
		}

		public static void SerializeProperty(uint propId, object value, byte[] buffer, ref int offset)
		{
			MapiPropType mapiPropType = (MapiPropType)(propId & 65535U);
			int num = MdbefWriter.SizeOf(mapiPropType, value);
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || offset + num > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("offset", offset, NetException.DestinationIndexOutOfRange);
			}
			if (num == 0)
			{
				return;
			}
			offset += ExBitConverter.Write(propId, buffer, offset);
			MapiPropType mapiPropType2 = mapiPropType;
			if (mapiPropType2 <= MapiPropType.ServerId)
			{
				if (mapiPropType2 <= MapiPropType.String)
				{
					switch (mapiPropType2)
					{
					case MapiPropType.Null:
					case (MapiPropType)8:
					case (MapiPropType)9:
					case MapiPropType.Error:
						return;
					case MapiPropType.Short:
						offset += ExBitConverter.Write((short)value, buffer, offset);
						return;
					case MapiPropType.Int:
						offset += ExBitConverter.Write((int)value, buffer, offset);
						return;
					case MapiPropType.Float:
						offset += ExBitConverter.Write((float)value, buffer, offset);
						return;
					case MapiPropType.Double:
					case MapiPropType.AppTime:
						offset += ExBitConverter.Write((double)value, buffer, offset);
						return;
					case MapiPropType.Currency:
						break;
					case MapiPropType.Boolean:
					{
						bool flag = (bool)value;
						offset += ExBitConverter.Write(flag ? 1 : 0, buffer, offset);
						return;
					}
					default:
						if (mapiPropType2 != MapiPropType.Long)
						{
							switch (mapiPropType2)
							{
							case MapiPropType.AnsiString:
							{
								string text = (string)value;
								offset += ExBitConverter.Write(text.Length + 1, buffer, offset);
								offset += ExBitConverter.Write(text, false, buffer, offset);
								return;
							}
							case MapiPropType.String:
							{
								string text2 = (string)value;
								offset += ExBitConverter.Write((text2.Length + 1) * 2, buffer, offset);
								offset += ExBitConverter.Write(text2, true, buffer, offset);
								return;
							}
							default:
								return;
							}
						}
						break;
					}
					offset += ExBitConverter.Write((long)value, buffer, offset);
					return;
				}
				if (mapiPropType2 == MapiPropType.SysTime)
				{
					DateTime dateTime = (DateTime)value;
					offset += ExBitConverter.Write(dateTime.ToFileTimeUtc(), buffer, offset);
					return;
				}
				if (mapiPropType2 == MapiPropType.Guid)
				{
					offset += ExBitConverter.Write((Guid)value, buffer, offset);
					return;
				}
				if (mapiPropType2 != MapiPropType.ServerId)
				{
					return;
				}
			}
			else if (mapiPropType2 <= MapiPropType.LongArray)
			{
				if (mapiPropType2 != MapiPropType.Binary)
				{
					switch (mapiPropType2)
					{
					case MapiPropType.ShortArray:
					{
						short[] array = (short[])value;
						offset += ExBitConverter.Write(array.Length, buffer, offset);
						for (int i = 0; i < array.Length; i++)
						{
							offset += ExBitConverter.Write(array[i], buffer, offset);
						}
						return;
					}
					case MapiPropType.IntArray:
					{
						int[] array2 = (int[])value;
						offset += ExBitConverter.Write(array2.Length, buffer, offset);
						for (int j = 0; j < array2.Length; j++)
						{
							offset += ExBitConverter.Write(array2[j], buffer, offset);
						}
						return;
					}
					case MapiPropType.FloatArray:
					{
						float[] array3 = (float[])value;
						offset += ExBitConverter.Write(array3.Length, buffer, offset);
						for (int k = 0; k < array3.Length; k++)
						{
							offset += ExBitConverter.Write(array3[k], buffer, offset);
						}
						return;
					}
					case MapiPropType.DoubleArray:
					case MapiPropType.AppTimeArray:
					{
						double[] array4 = (double[])value;
						offset += ExBitConverter.Write(array4.Length, buffer, offset);
						for (int l = 0; l < array4.Length; l++)
						{
							offset += ExBitConverter.Write(array4[l], buffer, offset);
						}
						return;
					}
					case MapiPropType.CurrencyArray:
						break;
					default:
						if (mapiPropType2 != MapiPropType.LongArray)
						{
							return;
						}
						break;
					}
					long[] array5 = (long[])value;
					offset += ExBitConverter.Write(array5.Length, buffer, offset);
					for (int m = 0; m < array5.Length; m++)
					{
						offset += ExBitConverter.Write(array5[m], buffer, offset);
					}
					return;
				}
			}
			else if (mapiPropType2 <= MapiPropType.SysTimeArray)
			{
				switch (mapiPropType2)
				{
				case MapiPropType.AnsiStringArray:
				{
					string[] array6 = (string[])value;
					offset += ExBitConverter.Write(array6.Length, buffer, offset);
					foreach (string text3 in array6)
					{
						offset += ExBitConverter.Write(text3.Length + 1, buffer, offset);
						offset += ExBitConverter.Write(text3, false, buffer, offset);
					}
					return;
				}
				case MapiPropType.StringArray:
				{
					string[] array7 = (string[])value;
					offset += ExBitConverter.Write(array7.Length, buffer, offset);
					foreach (string text4 in array7)
					{
						offset += ExBitConverter.Write((text4.Length + 1) * 2, buffer, offset);
						offset += ExBitConverter.Write(text4, true, buffer, offset);
					}
					return;
				}
				default:
				{
					if (mapiPropType2 != MapiPropType.SysTimeArray)
					{
						return;
					}
					DateTime[] array8 = (DateTime[])value;
					offset += ExBitConverter.Write(array8.Length, buffer, offset);
					for (int num3 = 0; num3 < array8.Length; num3++)
					{
						offset += ExBitConverter.Write(array8[num3].ToFileTimeUtc(), buffer, offset);
					}
					return;
				}
				}
			}
			else
			{
				if (mapiPropType2 == MapiPropType.GuidArray)
				{
					Guid[] array9 = (Guid[])value;
					offset += ExBitConverter.Write(array9.Length, buffer, offset);
					for (int num4 = 0; num4 < array9.Length; num4++)
					{
						offset += ExBitConverter.Write(array9[num4], buffer, offset);
					}
					return;
				}
				if (mapiPropType2 != MapiPropType.BinaryArray)
				{
					return;
				}
				byte[][] array10 = (byte[][])value;
				offset += ExBitConverter.Write(array10.Length, buffer, offset);
				for (int num5 = 0; num5 < array10.Length; num5++)
				{
					offset += ExBitConverter.Write(array10[num5].Length, buffer, offset);
					if (array10[num5].Length > 0)
					{
						Array.Copy(array10[num5], 0, buffer, offset, array10[num5].Length);
						offset += array10[num5].Length;
					}
				}
				return;
			}
			byte[] array11 = (byte[])value;
			offset += ExBitConverter.Write(array11.Length, buffer, offset);
			if (array11.Length > 0)
			{
				Array.Copy(array11, 0, buffer, offset, array11.Length);
				offset += array11.Length;
				return;
			}
		}
	}
}
