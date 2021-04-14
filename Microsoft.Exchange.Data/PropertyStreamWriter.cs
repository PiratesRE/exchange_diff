using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.Exchange.Conversion;

namespace Microsoft.Exchange.Data
{
	internal static class PropertyStreamWriter
	{
		public static int SizeOf(StreamPropertyType propType, object value)
		{
			return 2 + PropertyStreamWriter.SizeOfValue(propType, value);
		}

		public static void Serialize(StreamPropertyType propId, object value, byte[] buffer, ref int offset)
		{
			if (buffer == null || offset > buffer.Length)
			{
				throw new ArgumentException("buffer");
			}
			offset += ExBitConverter.Write((short)propId, buffer, offset);
			PropertyStreamWriter.WriteValue(propId, value, buffer, ref offset);
		}

		internal static int SizeOfValue(StreamPropertyType propType, object value)
		{
			int num = 0;
			if (propType <= (StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.List))
			{
				switch (propType)
				{
				case StreamPropertyType.Null:
					return num;
				case StreamPropertyType.Bool:
				case StreamPropertyType.Byte:
				case StreamPropertyType.SByte:
					return num + 1;
				case StreamPropertyType.Int16:
				case StreamPropertyType.UInt16:
					return num + 2;
				case StreamPropertyType.Int32:
				case StreamPropertyType.UInt32:
				case StreamPropertyType.Single:
				case StreamPropertyType.RecipientType:
					return num + 4;
				case StreamPropertyType.Int64:
				case StreamPropertyType.UInt64:
				case StreamPropertyType.Double:
				case StreamPropertyType.DateTime:
					return num + 8;
				case StreamPropertyType.Decimal:
				case StreamPropertyType.Guid:
				case StreamPropertyType.IPAddress:
					return num + 16;
				case StreamPropertyType.Char:
					return num + PropertyStreamWriter.SizeOfValue(StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.Array, new char[]
					{
						(char)value
					});
				case StreamPropertyType.String:
				{
					num += 4;
					string text = (string)value;
					if (!string.IsNullOrEmpty(text))
					{
						return num + Encoding.UTF8.GetByteCount(text);
					}
					return num;
				}
				case StreamPropertyType.IPEndPoint:
					return num + 18;
				case StreamPropertyType.RoutingAddress:
				case StreamPropertyType.ADObjectId:
					goto IL_48F;
				default:
					switch (propType)
					{
					case StreamPropertyType.Bool | StreamPropertyType.Array:
					case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.Array:
					case StreamPropertyType.SByte | StreamPropertyType.Array:
						return num + (4 + ((Array)value).Length);
					case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.Array:
					case StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.Array:
						return num + (4 + ((Array)value).Length * 2);
					case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.Array:
					case StreamPropertyType.UInt32 | StreamPropertyType.Array:
					case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.UInt32 | StreamPropertyType.Array:
						return num + (4 + ((Array)value).Length * 4);
					case StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.Array:
					case StreamPropertyType.Bool | StreamPropertyType.UInt32 | StreamPropertyType.Array:
					case StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.Array:
					case StreamPropertyType.DateTime | StreamPropertyType.Array:
						return num + (4 + ((Array)value).Length * 8);
					case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.Array:
					case StreamPropertyType.Null | StreamPropertyType.DateTime | StreamPropertyType.Array:
					case StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.Array:
						return num + (4 + ((Array)value).Length * 16);
					case StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.Array:
						num += 4;
						if (value != null)
						{
							return num + Encoding.UTF8.GetByteCount((char[])value);
						}
						return num;
					case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.Array:
					{
						num += 4;
						string[] array = (string[])value;
						for (int i = 0; i < array.Length; i++)
						{
							num += PropertyStreamWriter.SizeOfValue(StreamPropertyType.String, array[i]);
						}
						return num;
					}
					case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.Array:
						return num + (4 + ((Array)value).Length * 18);
					default:
						switch (propType)
						{
						case StreamPropertyType.Bool | StreamPropertyType.List:
						case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.List:
						case StreamPropertyType.SByte | StreamPropertyType.List:
							break;
						case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.List:
						case StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.List:
							goto IL_308;
						case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.List:
						case StreamPropertyType.UInt32 | StreamPropertyType.List:
						case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.UInt32 | StreamPropertyType.List:
							goto IL_31F;
						case StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.List:
						case StreamPropertyType.Bool | StreamPropertyType.UInt32 | StreamPropertyType.List:
						case StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.List:
						case StreamPropertyType.DateTime | StreamPropertyType.List:
							goto IL_336;
						case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.List:
						case StreamPropertyType.Null | StreamPropertyType.DateTime | StreamPropertyType.List:
						case StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.List:
							goto IL_34D;
						case StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.List:
						{
							List<char> list = (List<char>)value;
							char[] chars = list.ToArray();
							num += 4;
							if (value != null)
							{
								return num + Encoding.UTF8.GetByteCount(chars);
							}
							return num;
						}
						case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.List:
							goto IL_3DB;
						case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.List:
							goto IL_365;
						default:
							goto IL_48F;
						}
						break;
					}
					break;
				}
			}
			else
			{
				if (propType == (StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.Array | StreamPropertyType.List))
				{
					num += 4;
					List<byte[]> list2 = (List<byte[]>)value;
					for (int j = 0; j < list2.Count; j++)
					{
						num += PropertyStreamWriter.SizeOfValue(propType & ~StreamPropertyType.List, list2[j]);
					}
					return num;
				}
				switch (propType)
				{
				case StreamPropertyType.Bool | StreamPropertyType.MultiValuedProperty:
				case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.MultiValuedProperty:
				case StreamPropertyType.SByte | StreamPropertyType.MultiValuedProperty:
					break;
				case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.MultiValuedProperty:
				case StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.MultiValuedProperty:
					goto IL_308;
				case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.MultiValuedProperty:
				case StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty:
				case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty:
					goto IL_31F;
				case StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty:
				case StreamPropertyType.Bool | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty:
				case StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty:
				case StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
					goto IL_336;
				case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty:
				case StreamPropertyType.Null | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
				case StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
					goto IL_34D;
				case StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty:
				{
					MultiValuedProperty<char> multiValuedProperty = (MultiValuedProperty<char>)value;
					char[] chars2 = multiValuedProperty.ToArray();
					num += 4;
					if (value != null)
					{
						return num + Encoding.UTF8.GetByteCount(chars2);
					}
					return num;
				}
				case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty:
					goto IL_3DB;
				case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
					goto IL_365;
				default:
				{
					if (propType != (StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.Array | StreamPropertyType.MultiValuedProperty))
					{
						goto IL_48F;
					}
					num += 4;
					MultiValuedProperty<byte[]> multiValuedProperty2 = (MultiValuedProperty<byte[]>)value;
					for (int k = 0; k < multiValuedProperty2.Count; k++)
					{
						num += PropertyStreamWriter.SizeOfValue(propType & ~StreamPropertyType.MultiValuedProperty, multiValuedProperty2[k]);
					}
					return num;
				}
				}
			}
			return num + (4 + ((ICollection)value).Count);
			IL_308:
			return num + (4 + ((ICollection)value).Count * 2);
			IL_31F:
			return num + (4 + ((ICollection)value).Count * 4);
			IL_336:
			return num + (4 + ((ICollection)value).Count * 8);
			IL_34D:
			return num + (4 + ((ICollection)value).Count * 16);
			IL_365:
			return num + (4 + ((ICollection)value).Count * 18);
			IL_3DB:
			num += 4;
			IList<string> list3 = (IList<string>)value;
			for (int l = 0; l < list3.Count; l++)
			{
				num += PropertyStreamWriter.SizeOfValue(StreamPropertyType.String, list3[l]);
			}
			return num;
			IL_48F:
			throw new InvalidOperationException(string.Format("Data type {0} is unknown", propType));
		}

		public static void WriteValue(StreamPropertyType propId, object value, byte[] buffer, ref int offset)
		{
			if (buffer == null || offset > buffer.Length)
			{
				throw new ArgumentException("buffer");
			}
			if (propId <= (StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.List))
			{
				switch (propId)
				{
				case StreamPropertyType.Null:
					return;
				case StreamPropertyType.Bool:
					buffer[offset++] = (((bool)value) ? 1 : 0);
					return;
				case StreamPropertyType.Byte:
					buffer[offset++] = (byte)value;
					return;
				case StreamPropertyType.SByte:
				{
					sbyte b = (sbyte)value;
					buffer[offset++] = (byte)b;
					return;
				}
				case StreamPropertyType.Int16:
					offset += ExBitConverter.Write((short)value, buffer, offset);
					return;
				case StreamPropertyType.UInt16:
					offset += ExBitConverter.Write((ushort)value, buffer, offset);
					return;
				case StreamPropertyType.Int32:
					offset += ExBitConverter.Write((int)value, buffer, offset);
					return;
				case StreamPropertyType.UInt32:
					offset += ExBitConverter.Write((uint)value, buffer, offset);
					return;
				case StreamPropertyType.Int64:
					offset += ExBitConverter.Write((long)value, buffer, offset);
					return;
				case StreamPropertyType.UInt64:
					offset += ExBitConverter.Write((ulong)value, buffer, offset);
					return;
				case StreamPropertyType.Single:
					offset += ExBitConverter.Write((float)value, buffer, offset);
					return;
				case StreamPropertyType.Double:
					offset += ExBitConverter.Write((double)value, buffer, offset);
					return;
				case StreamPropertyType.Decimal:
				{
					decimal d = (decimal)value;
					int[] bits = decimal.GetBits(d);
					for (int i = 0; i < bits.Length; i++)
					{
						offset += ExBitConverter.Write(bits[i], buffer, offset);
					}
					return;
				}
				case StreamPropertyType.Char:
					value = new char[]
					{
						(char)value
					};
					break;
				case StreamPropertyType.String:
				{
					int num = 0;
					string text = (string)value;
					if (!string.IsNullOrEmpty(text))
					{
						num = Encoding.UTF8.GetBytes(text, 0, text.Length, buffer, offset + 4);
					}
					offset += ExBitConverter.Write(num, buffer, offset);
					offset += num;
					return;
				}
				case StreamPropertyType.DateTime:
				{
					DateTime dateTime = (DateTime)value;
					offset += ExBitConverter.Write(dateTime.ToFileTimeUtc(), buffer, offset);
					return;
				}
				case StreamPropertyType.Guid:
					offset += ExBitConverter.Write((Guid)value, buffer, offset);
					return;
				case StreamPropertyType.IPAddress:
				{
					IPvxAddress pvxAddress = new IPvxAddress((value == null) ? IPAddress.None : ((IPAddress)value));
					long value2 = (long)((ulong)((uint)pvxAddress));
					offset += ExBitConverter.Write(value2, buffer, offset);
					value2 = (long)((ulong)((uint)(pvxAddress >> 64)));
					offset += ExBitConverter.Write(value2, buffer, offset);
					return;
				}
				case StreamPropertyType.IPEndPoint:
				{
					if (value == null)
					{
						PropertyStreamWriter.WriteValue(StreamPropertyType.IPAddress, IPAddress.None, buffer, ref offset);
						PropertyStreamWriter.WriteValue(StreamPropertyType.UInt16, 0, buffer, ref offset);
						return;
					}
					IPEndPoint ipendPoint = (IPEndPoint)value;
					PropertyStreamWriter.WriteValue(StreamPropertyType.IPAddress, ipendPoint.Address, buffer, ref offset);
					PropertyStreamWriter.WriteValue(StreamPropertyType.UInt16, (ushort)ipendPoint.Port, buffer, ref offset);
					return;
				}
				case StreamPropertyType.RoutingAddress:
				case StreamPropertyType.ADObjectId:
				case StreamPropertyType.RecipientType:
				case StreamPropertyType.ADObjectIdUTF8:
				case StreamPropertyType.ADObjectIdWithString:
				case StreamPropertyType.ProxyAddress:
					goto IL_808;
				default:
					switch (propId)
					{
					case StreamPropertyType.Bool | StreamPropertyType.Array:
						PropertyStreamWriter.WriteArray<bool>(propId, (bool[])value, buffer, ref offset);
						return;
					case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.Array:
					{
						byte[] array = (byte[])value;
						offset += ExBitConverter.Write(array.Length, buffer, offset);
						if (array.Length != 0)
						{
							Array.Copy(array, 0, buffer, offset, array.Length);
							offset += array.Length;
							return;
						}
						return;
					}
					case StreamPropertyType.SByte | StreamPropertyType.Array:
						PropertyStreamWriter.WriteArray<sbyte>(propId, (sbyte[])value, buffer, ref offset);
						return;
					case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.Array:
						PropertyStreamWriter.WriteArray<short>(propId, (short[])value, buffer, ref offset);
						return;
					case StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.Array:
						PropertyStreamWriter.WriteArray<ushort>(propId, (ushort[])value, buffer, ref offset);
						return;
					case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.Array:
						PropertyStreamWriter.WriteArray<int>(propId, (int[])value, buffer, ref offset);
						return;
					case StreamPropertyType.UInt32 | StreamPropertyType.Array:
						PropertyStreamWriter.WriteArray<uint>(propId, (uint[])value, buffer, ref offset);
						return;
					case StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.Array:
						PropertyStreamWriter.WriteArray<long>(propId, (long[])value, buffer, ref offset);
						return;
					case StreamPropertyType.Bool | StreamPropertyType.UInt32 | StreamPropertyType.Array:
						PropertyStreamWriter.WriteArray<ulong>(propId, (ulong[])value, buffer, ref offset);
						return;
					case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.UInt32 | StreamPropertyType.Array:
						PropertyStreamWriter.WriteArray<float>(propId, (float[])value, buffer, ref offset);
						return;
					case StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.Array:
						PropertyStreamWriter.WriteArray<double>(propId, (double[])value, buffer, ref offset);
						return;
					case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.Array:
						PropertyStreamWriter.WriteArray<decimal>(propId, (decimal[])value, buffer, ref offset);
						return;
					case StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.Array:
						break;
					case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.Array:
						PropertyStreamWriter.WriteArray<string>(propId, (string[])value, buffer, ref offset);
						return;
					case StreamPropertyType.DateTime | StreamPropertyType.Array:
						PropertyStreamWriter.WriteArray<DateTime>(propId, (DateTime[])value, buffer, ref offset);
						return;
					case StreamPropertyType.Null | StreamPropertyType.DateTime | StreamPropertyType.Array:
						PropertyStreamWriter.WriteArray<Guid>(propId, (Guid[])value, buffer, ref offset);
						return;
					case StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.Array:
						PropertyStreamWriter.WriteArray<IPAddress>(propId, (IPAddress[])value, buffer, ref offset);
						return;
					case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.Array:
						PropertyStreamWriter.WriteArray<IPEndPoint>(propId, (IPEndPoint[])value, buffer, ref offset);
						return;
					case StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.Array:
					case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.Array:
					case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.Array:
					case StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.Array:
					case StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.Array:
						goto IL_808;
					case StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.Array:
						return;
					default:
						switch (propId)
						{
						case StreamPropertyType.Bool | StreamPropertyType.List:
							PropertyStreamWriter.WriteList<bool>(propId, (List<bool>)value, buffer, ref offset);
							return;
						case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.List:
							PropertyStreamWriter.WriteList<byte>(propId, (List<byte>)value, buffer, ref offset);
							return;
						case StreamPropertyType.SByte | StreamPropertyType.List:
							PropertyStreamWriter.WriteList<sbyte>(propId, (List<sbyte>)value, buffer, ref offset);
							return;
						case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.List:
							PropertyStreamWriter.WriteList<short>(propId, (List<short>)value, buffer, ref offset);
							return;
						case StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.List:
							PropertyStreamWriter.WriteList<ushort>(propId, (List<ushort>)value, buffer, ref offset);
							return;
						case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.List:
							PropertyStreamWriter.WriteList<int>(propId, (List<int>)value, buffer, ref offset);
							return;
						case StreamPropertyType.UInt32 | StreamPropertyType.List:
							PropertyStreamWriter.WriteList<uint>(propId, (List<uint>)value, buffer, ref offset);
							return;
						case StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.List:
							PropertyStreamWriter.WriteList<long>(propId, (List<long>)value, buffer, ref offset);
							return;
						case StreamPropertyType.Bool | StreamPropertyType.UInt32 | StreamPropertyType.List:
							PropertyStreamWriter.WriteList<ulong>(propId, (List<ulong>)value, buffer, ref offset);
							return;
						case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.UInt32 | StreamPropertyType.List:
							PropertyStreamWriter.WriteList<float>(propId, (List<float>)value, buffer, ref offset);
							return;
						case StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.List:
							PropertyStreamWriter.WriteList<double>(propId, (List<double>)value, buffer, ref offset);
							return;
						case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.List:
							PropertyStreamWriter.WriteList<decimal>(propId, (List<decimal>)value, buffer, ref offset);
							return;
						case StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.List:
						{
							char[] value3 = (value != null) ? ((List<char>)value).ToArray() : null;
							PropertyStreamWriter.WriteValue(StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.Array, value3, buffer, ref offset);
							return;
						}
						case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.List:
							PropertyStreamWriter.WriteList<string>(propId, (List<string>)value, buffer, ref offset);
							return;
						case StreamPropertyType.DateTime | StreamPropertyType.List:
							PropertyStreamWriter.WriteList<DateTime>(propId, (List<DateTime>)value, buffer, ref offset);
							return;
						case StreamPropertyType.Null | StreamPropertyType.DateTime | StreamPropertyType.List:
							PropertyStreamWriter.WriteList<Guid>(propId, (List<Guid>)value, buffer, ref offset);
							return;
						case StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.List:
							PropertyStreamWriter.WriteList<IPAddress>(propId, (List<IPAddress>)value, buffer, ref offset);
							return;
						case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.List:
							PropertyStreamWriter.WriteList<IPEndPoint>(propId, (List<IPEndPoint>)value, buffer, ref offset);
							return;
						case StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.List:
						case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.List:
						case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.List:
						case StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.List:
						case StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.List:
							goto IL_808;
						case StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.List:
							return;
						default:
							return;
						}
						break;
					}
					break;
				}
				int num2 = 0;
				if (value != null)
				{
					char[] array2 = (char[])value;
					if (array2.Length != 0)
					{
						num2 = Encoding.UTF8.GetBytes(array2, 0, array2.Length, buffer, offset + 4);
					}
				}
				offset += ExBitConverter.Write(num2, buffer, offset);
				offset += num2;
				return;
			}
			if (propId == (StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.Array | StreamPropertyType.List))
			{
				PropertyStreamWriter.WriteList<byte[]>(propId, (List<byte[]>)value, buffer, ref offset);
				return;
			}
			switch (propId)
			{
			case StreamPropertyType.Bool | StreamPropertyType.MultiValuedProperty:
				PropertyStreamWriter.WriteMultiValuedProperty<bool>(propId, (MultiValuedProperty<bool>)value, buffer, ref offset);
				return;
			case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.MultiValuedProperty:
				PropertyStreamWriter.WriteMultiValuedProperty<byte>(propId, (MultiValuedProperty<byte>)value, buffer, ref offset);
				return;
			case StreamPropertyType.SByte | StreamPropertyType.MultiValuedProperty:
				PropertyStreamWriter.WriteMultiValuedProperty<sbyte>(propId, (MultiValuedProperty<sbyte>)value, buffer, ref offset);
				return;
			case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.MultiValuedProperty:
				PropertyStreamWriter.WriteMultiValuedProperty<short>(propId, (MultiValuedProperty<short>)value, buffer, ref offset);
				return;
			case StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.MultiValuedProperty:
				PropertyStreamWriter.WriteMultiValuedProperty<ushort>(propId, (MultiValuedProperty<ushort>)value, buffer, ref offset);
				return;
			case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.MultiValuedProperty:
				PropertyStreamWriter.WriteMultiValuedProperty<int>(propId, (MultiValuedProperty<int>)value, buffer, ref offset);
				return;
			case StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty:
				PropertyStreamWriter.WriteMultiValuedProperty<uint>(propId, (MultiValuedProperty<uint>)value, buffer, ref offset);
				return;
			case StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty:
				PropertyStreamWriter.WriteMultiValuedProperty<long>(propId, (MultiValuedProperty<long>)value, buffer, ref offset);
				return;
			case StreamPropertyType.Bool | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty:
				PropertyStreamWriter.WriteMultiValuedProperty<ulong>(propId, (MultiValuedProperty<ulong>)value, buffer, ref offset);
				return;
			case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty:
				PropertyStreamWriter.WriteMultiValuedProperty<float>(propId, (MultiValuedProperty<float>)value, buffer, ref offset);
				return;
			case StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty:
				PropertyStreamWriter.WriteMultiValuedProperty<double>(propId, (MultiValuedProperty<double>)value, buffer, ref offset);
				return;
			case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty:
				PropertyStreamWriter.WriteMultiValuedProperty<decimal>(propId, (MultiValuedProperty<decimal>)value, buffer, ref offset);
				return;
			case StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty:
			{
				char[] value4 = (value != null) ? ((MultiValuedProperty<char>)value).ToArray() : null;
				PropertyStreamWriter.WriteValue(StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.Array, value4, buffer, ref offset);
				return;
			}
			case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty:
				PropertyStreamWriter.WriteMultiValuedProperty<string>(propId, (MultiValuedProperty<string>)value, buffer, ref offset);
				return;
			case StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
				PropertyStreamWriter.WriteMultiValuedProperty<DateTime>(propId, (MultiValuedProperty<DateTime>)value, buffer, ref offset);
				return;
			case StreamPropertyType.Null | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
				PropertyStreamWriter.WriteMultiValuedProperty<Guid>(propId, (MultiValuedProperty<Guid>)value, buffer, ref offset);
				return;
			case StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
				PropertyStreamWriter.WriteMultiValuedProperty<IPAddress>(propId, (MultiValuedProperty<IPAddress>)value, buffer, ref offset);
				return;
			case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
				PropertyStreamWriter.WriteMultiValuedProperty<IPEndPoint>(propId, (MultiValuedProperty<IPEndPoint>)value, buffer, ref offset);
				return;
			case StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
			case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
			case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
			case StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
			case StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
				break;
			case StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
				return;
			default:
				if (propId != (StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.Array | StreamPropertyType.MultiValuedProperty))
				{
					return;
				}
				PropertyStreamWriter.WriteMultiValuedProperty<byte[]>(propId, (MultiValuedProperty<byte[]>)value, buffer, ref offset);
				return;
			}
			IL_808:
			throw new InvalidOperationException(string.Format("Dont know how to handle type {0}", propId));
		}

		public static void WritePropertyKeyValue(string propertyName, StreamPropertyType valueType, object value, ref byte[] bytes, ref int offset)
		{
			int num = PropertyStreamWriter.SizeOf(StreamPropertyType.String, propertyName) + PropertyStreamWriter.SizeOf(valueType, value);
			if (offset + num >= bytes.Length)
			{
				int num2 = (offset + num > bytes.Length * 2) ? (offset + num + bytes.Length) : (bytes.Length * 2);
				byte[] array = new byte[num2];
				Array.Copy(bytes, array, bytes.Length);
				bytes = array;
			}
			PropertyStreamWriter.Serialize(StreamPropertyType.String, propertyName, bytes, ref offset);
			PropertyStreamWriter.Serialize(valueType, value, bytes, ref offset);
		}

		private static void WriteArray<T>(StreamPropertyType propId, T[] array, byte[] buffer, ref int offset)
		{
			int num = (array != null) ? array.Length : 0;
			offset += ExBitConverter.Write(num, buffer, offset);
			for (int i = 0; i < num; i++)
			{
				PropertyStreamWriter.WriteValue(propId & ~StreamPropertyType.Array, array[i], buffer, ref offset);
			}
		}

		private static void WriteList<T>(StreamPropertyType propId, List<T> list, byte[] buffer, ref int offset)
		{
			int num = (list != null) ? list.Count : 0;
			offset += ExBitConverter.Write(num, buffer, offset);
			for (int i = 0; i < num; i++)
			{
				PropertyStreamWriter.WriteValue(propId & ~StreamPropertyType.List, list[i], buffer, ref offset);
			}
		}

		private static void WriteMultiValuedProperty<T>(StreamPropertyType propId, MultiValuedProperty<T> list, byte[] buffer, ref int offset)
		{
			int num = (list != null) ? list.Count : 0;
			offset += ExBitConverter.Write(num, buffer, offset);
			for (int i = 0; i < num; i++)
			{
				PropertyStreamWriter.WriteValue(propId & ~StreamPropertyType.MultiValuedProperty, list[i], buffer, ref offset);
			}
		}
	}
}
