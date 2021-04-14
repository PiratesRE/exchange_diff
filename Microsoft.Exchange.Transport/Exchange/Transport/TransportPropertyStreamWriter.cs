using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Transport
{
	internal static class TransportPropertyStreamWriter
	{
		public static int SizeOf(StreamPropertyType propType, object value)
		{
			return 2 + TransportPropertyStreamWriter.SizeOfValue(propType, value);
		}

		public static void Serialize(StreamPropertyType propId, object value, byte[] buffer, ref int offset)
		{
			if (buffer == null || offset > buffer.Length)
			{
				throw new ArgumentException("buffer");
			}
			offset += ExBitConverter.Write((short)propId, buffer, offset);
			TransportPropertyStreamWriter.WriteValue(propId, value, buffer, ref offset);
		}

		public static void WriteValue(StreamPropertyType propId, object value, byte[] buffer, ref int offset)
		{
			if (buffer == null || offset > buffer.Length)
			{
				throw new ArgumentException("buffer");
			}
			if (propId <= (StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.Array))
			{
				switch (propId)
				{
				case StreamPropertyType.RoutingAddress:
				{
					int num = 0;
					byte[] bytes = ((RoutingAddress)value).GetBytes();
					if (bytes != null && bytes.Length != 0)
					{
						num = bytes.Length;
						Array.Copy(bytes, 0, buffer, offset + 4, num);
					}
					offset += ExBitConverter.Write(num, buffer, offset);
					offset += num;
					return;
				}
				case StreamPropertyType.ADObjectId:
				case StreamPropertyType.ADObjectIdUTF8:
				{
					Encoding encoding = (propId == StreamPropertyType.ADObjectId) ? Encoding.Unicode : Encoding.UTF8;
					byte[] bytes2 = ((ADObjectId)value).GetBytes(encoding);
					if (bytes2 == null || bytes2.Length == 0)
					{
						throw new InvalidOperationException("Unexpected length of bytes when serializing ADObjectId");
					}
					int num2 = bytes2.Length;
					Array.Copy(bytes2, 0, buffer, offset + 4, num2);
					offset += ExBitConverter.Write(num2, buffer, offset);
					offset += num2;
					return;
				}
				case StreamPropertyType.RecipientType:
					offset += ExBitConverter.Write((int)((Microsoft.Exchange.Data.Directory.Recipient.RecipientType)value), buffer, offset);
					return;
				case StreamPropertyType.ADObjectIdWithString:
				{
					byte[] bytes3 = ((ADObjectIdWithString)value).GetBytes();
					if (bytes3 == null || bytes3.Length == 0)
					{
						throw new InvalidOperationException("Unexpected length of bytes when serializing ADObjectIdWithString");
					}
					int num3 = bytes3.Length;
					Array.Copy(bytes3, 0, buffer, offset + 4, num3);
					offset += ExBitConverter.Write(num3, buffer, offset);
					offset += num3;
					return;
				}
				case StreamPropertyType.ProxyAddress:
				{
					byte[] bytes4 = Encoding.UTF8.GetBytes(((ProxyAddress)value).ProxyAddressString);
					if (bytes4 == null || bytes4.Length == 0)
					{
						throw new InvalidOperationException("Unexpected length of bytes when serializing ProxyAddress");
					}
					int num4 = bytes4.Length;
					Array.Copy(bytes4, 0, buffer, offset + 4, num4);
					offset += ExBitConverter.Write(num4, buffer, offset);
					offset += num4;
					return;
				}
				default:
					switch (propId)
					{
					case StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.Array:
						TransportPropertyStreamWriter.WriteArray<RoutingAddress>(propId, (RoutingAddress[])value, buffer, ref offset);
						return;
					case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.Array:
					case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.Array:
						TransportPropertyStreamWriter.WriteArray<ADObjectId>(propId, (ADObjectId[])value, buffer, ref offset);
						return;
					case StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.Array:
						TransportPropertyStreamWriter.WriteArray<ADObjectIdWithString>(propId, (ADObjectIdWithString[])value, buffer, ref offset);
						return;
					case StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.Array:
						TransportPropertyStreamWriter.WriteArray<ProxyAddress>(propId, (ProxyAddress[])value, buffer, ref offset);
						return;
					}
					break;
				}
			}
			else
			{
				switch (propId)
				{
				case StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.List:
					TransportPropertyStreamWriter.WriteList<RoutingAddress>(propId, (List<RoutingAddress>)value, buffer, ref offset);
					return;
				case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.List:
				case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.List:
					TransportPropertyStreamWriter.WriteList<ADObjectId>(propId, (List<ADObjectId>)value, buffer, ref offset);
					return;
				case StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.List:
					break;
				case StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.List:
					TransportPropertyStreamWriter.WriteList<ADObjectIdWithString>(propId, (List<ADObjectIdWithString>)value, buffer, ref offset);
					return;
				case StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.List:
					TransportPropertyStreamWriter.WriteList<ProxyAddress>(propId, (List<ProxyAddress>)value, buffer, ref offset);
					return;
				default:
					switch (propId)
					{
					case StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
						TransportPropertyStreamWriter.WriteMultiValuedProperty<RoutingAddress>(propId, (MultiValuedProperty<RoutingAddress>)value, buffer, ref offset);
						return;
					case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
					case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
						TransportPropertyStreamWriter.WriteMultiValuedProperty<ADObjectId>(propId, (MultiValuedProperty<ADObjectId>)value, buffer, ref offset);
						return;
					case StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
						TransportPropertyStreamWriter.WriteMultiValuedProperty<ADObjectIdWithString>(propId, (MultiValuedProperty<ADObjectIdWithString>)value, buffer, ref offset);
						return;
					case StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
						TransportPropertyStreamWriter.WriteMultiValuedProperty<ProxyAddress>(propId, (MultiValuedProperty<ProxyAddress>)value, buffer, ref offset);
						return;
					}
					break;
				}
			}
			PropertyStreamWriter.WriteValue(propId, value, buffer, ref offset);
		}

		internal static int SizeOfValue(StreamPropertyType propType, object value)
		{
			int num = 0;
			if (propType > (StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.Array))
			{
				switch (propType)
				{
				case StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.List:
					break;
				case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.List:
				case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.List:
				{
					num += 4;
					List<ADObjectId> list = (List<ADObjectId>)value;
					for (int i = 0; i < list.Count; i++)
					{
						num += TransportPropertyStreamWriter.SizeOfValue(propType & ~StreamPropertyType.List, list[i]);
					}
					return num;
				}
				case StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.List:
					goto IL_3E4;
				case StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.List:
				{
					num += 4;
					List<ADObjectIdWithString> list2 = (List<ADObjectIdWithString>)value;
					for (int j = 0; j < list2.Count; j++)
					{
						num += TransportPropertyStreamWriter.SizeOfValue(propType & ~StreamPropertyType.List, list2[j]);
					}
					return num;
				}
				case StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.List:
				{
					num += 4;
					List<ProxyAddress> list3 = (List<ProxyAddress>)value;
					for (int k = 0; k < list3.Count; k++)
					{
						num += TransportPropertyStreamWriter.SizeOfValue(propType & ~StreamPropertyType.List, list3[k]);
					}
					return num;
				}
				default:
					switch (propType)
					{
					case StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
						break;
					case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
					case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
					{
						num += 4;
						MultiValuedProperty<ADObjectId> multiValuedProperty = (MultiValuedProperty<ADObjectId>)value;
						for (int l = 0; l < multiValuedProperty.Count; l++)
						{
							num += TransportPropertyStreamWriter.SizeOfValue(propType & ~StreamPropertyType.MultiValuedProperty, multiValuedProperty[l]);
						}
						return num;
					}
					case StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
						goto IL_3E4;
					case StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
					{
						num += 4;
						MultiValuedProperty<ADObjectIdWithString> multiValuedProperty2 = (MultiValuedProperty<ADObjectIdWithString>)value;
						for (int m = 0; m < multiValuedProperty2.Count; m++)
						{
							num += TransportPropertyStreamWriter.SizeOfValue(propType & ~StreamPropertyType.MultiValuedProperty, multiValuedProperty2[m]);
						}
						return num;
					}
					case StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
					{
						num += 4;
						MultiValuedProperty<ProxyAddress> multiValuedProperty3 = (MultiValuedProperty<ProxyAddress>)value;
						for (int n = 0; n < multiValuedProperty3.Count; n++)
						{
							num += TransportPropertyStreamWriter.SizeOfValue(propType & ~StreamPropertyType.MultiValuedProperty, multiValuedProperty3[n]);
						}
						return num;
					}
					default:
						goto IL_3E4;
					}
					break;
				}
				num += 4;
				IList<RoutingAddress> list4 = (IList<RoutingAddress>)value;
				for (int num2 = 0; num2 < list4.Count; num2++)
				{
					num += TransportPropertyStreamWriter.SizeOfValue(StreamPropertyType.RoutingAddress, list4[num2]);
				}
				return num;
			}
			switch (propType)
			{
			case StreamPropertyType.RoutingAddress:
			{
				num += 4;
				byte[] bytes = ((RoutingAddress)value).GetBytes();
				if (bytes != null)
				{
					return num + bytes.Length;
				}
				return num;
			}
			case StreamPropertyType.ADObjectId:
			case StreamPropertyType.ADObjectIdUTF8:
				num += 4;
				if (value != null)
				{
					Encoding encoding = (propType == StreamPropertyType.ADObjectId) ? Encoding.Unicode : Encoding.UTF8;
					return num + ((ADObjectId)value).GetByteCount(encoding);
				}
				return num;
			case StreamPropertyType.RecipientType:
				break;
			case StreamPropertyType.ADObjectIdWithString:
				num += 4;
				if (value != null)
				{
					return num + ((ADObjectIdWithString)value).GetByteCount();
				}
				return num;
			case StreamPropertyType.ProxyAddress:
				num += 4;
				if (value != null)
				{
					return num + Encoding.UTF8.GetByteCount(((ProxyAddress)value).ProxyAddressString);
				}
				return num;
			default:
				switch (propType)
				{
				case StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.Array:
				{
					num += 4;
					RoutingAddress[] array = (RoutingAddress[])value;
					for (int num3 = 0; num3 < array.Length; num3++)
					{
						num += TransportPropertyStreamWriter.SizeOfValue(StreamPropertyType.RoutingAddress, array[num3]);
					}
					return num;
				}
				case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.Array:
				case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.Array:
				{
					num += 4;
					ADObjectId[] array2 = (ADObjectId[])value;
					for (int num4 = 0; num4 < array2.Length; num4++)
					{
						num += TransportPropertyStreamWriter.SizeOfValue(propType & ~StreamPropertyType.Array, array2[num4]);
					}
					return num;
				}
				case StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.Array:
				{
					num += 4;
					ADObjectIdWithString[] array3 = (ADObjectIdWithString[])value;
					for (int num5 = 0; num5 < array3.Length; num5++)
					{
						num += TransportPropertyStreamWriter.SizeOfValue(propType & ~StreamPropertyType.Array, array3[num5]);
					}
					return num;
				}
				case StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.Array:
				{
					num += 4;
					ProxyAddress[] array4 = (ProxyAddress[])value;
					for (int num6 = 0; num6 < array4.Length; num6++)
					{
						num += TransportPropertyStreamWriter.SizeOfValue(propType & ~StreamPropertyType.Array, array4[num6]);
					}
					return num;
				}
				}
				break;
			}
			IL_3E4:
			return PropertyStreamWriter.SizeOfValue(propType, value);
		}

		private static void WriteArray<T>(StreamPropertyType propId, T[] array, byte[] buffer, ref int offset)
		{
			int num = (array != null) ? array.Length : 0;
			offset += ExBitConverter.Write(num, buffer, offset);
			for (int i = 0; i < num; i++)
			{
				TransportPropertyStreamWriter.WriteValue(propId & ~StreamPropertyType.Array, array[i], buffer, ref offset);
			}
		}

		private static void WriteList<T>(StreamPropertyType propId, List<T> list, byte[] buffer, ref int offset)
		{
			int num = (list != null) ? list.Count : 0;
			offset += ExBitConverter.Write(num, buffer, offset);
			for (int i = 0; i < num; i++)
			{
				TransportPropertyStreamWriter.WriteValue(propId & ~StreamPropertyType.List, list[i], buffer, ref offset);
			}
		}

		private static void WriteMultiValuedProperty<T>(StreamPropertyType propId, MultiValuedProperty<T> list, byte[] buffer, ref int offset)
		{
			int num = (list != null) ? list.Count : 0;
			offset += ExBitConverter.Write(num, buffer, offset);
			for (int i = 0; i < num; i++)
			{
				TransportPropertyStreamWriter.WriteValue(propId & ~StreamPropertyType.MultiValuedProperty, list[i], buffer, ref offset);
			}
		}
	}
}
