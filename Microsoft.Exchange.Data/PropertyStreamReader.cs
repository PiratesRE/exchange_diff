using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Exchange.Conversion;

namespace Microsoft.Exchange.Data
{
	internal class PropertyStreamReader
	{
		public PropertyStreamReader(Stream stream)
		{
			this.stream = stream;
		}

		public bool Read(out KeyValuePair<string, object> item)
		{
			bool flag;
			string text = this.ReadKey(out flag);
			if (string.IsNullOrEmpty(text))
			{
				text = string.Empty;
			}
			if (!flag)
			{
				item = new KeyValuePair<string, object>(text, null);
				return false;
			}
			object value = this.ReadValue();
			item = new KeyValuePair<string, object>(text, value);
			return true;
		}

		public object ReadValue()
		{
			this.Read(this.buffer, 2);
			StreamPropertyType propId = (StreamPropertyType)BitConverter.ToInt16(this.buffer, 0);
			return this.ReadRawValue(propId);
		}

		internal static T GetValue<T>(KeyValuePair<string, object> item)
		{
			if (!(item.Value is T))
			{
				throw new SerializationException(string.Format("{0} is not of type {1} as expected", item.Key, typeof(T).Name));
			}
			return (T)((object)item.Value);
		}

		protected void Read(byte[] buffer, int length)
		{
			int num = 0;
			for (;;)
			{
				int num2 = this.stream.Read(buffer, num, length);
				if (num2 == 0)
				{
					break;
				}
				length -= num2;
				num += num2;
				if (length <= 0)
				{
					return;
				}
			}
			throw new FormatException("Extended property stream. Unexpected truncation of serialized data.");
		}

		protected virtual string ConvertTypedKey(TypedValue key)
		{
			return key.Value.ToString();
		}

		protected virtual object ReadRawValue(StreamPropertyType propId)
		{
			if (propId <= (StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.List))
			{
				switch (propId)
				{
				case StreamPropertyType.Null:
					return null;
				case StreamPropertyType.Bool:
					this.Read(this.buffer, 1);
					return this.buffer[0] == 1;
				case StreamPropertyType.Byte:
					this.Read(this.buffer, 1);
					return this.buffer[0];
				case StreamPropertyType.SByte:
					this.Read(this.buffer, 1);
					return (sbyte)this.buffer[0];
				case StreamPropertyType.Int16:
					this.Read(this.buffer, 2);
					return BitConverter.ToInt16(this.buffer, 0);
				case StreamPropertyType.UInt16:
					this.Read(this.buffer, 2);
					return BitConverter.ToUInt16(this.buffer, 0);
				case StreamPropertyType.Int32:
					this.Read(this.buffer, 4);
					return BitConverter.ToInt32(this.buffer, 0);
				case StreamPropertyType.UInt32:
					this.Read(this.buffer, 4);
					return BitConverter.ToUInt32(this.buffer, 0);
				case StreamPropertyType.Int64:
					this.Read(this.buffer, 8);
					return BitConverter.ToInt64(this.buffer, 0);
				case StreamPropertyType.UInt64:
					this.Read(this.buffer, 8);
					return BitConverter.ToUInt64(this.buffer, 0);
				case StreamPropertyType.Single:
					this.Read(this.buffer, 4);
					return BitConverter.ToSingle(this.buffer, 0);
				case StreamPropertyType.Double:
					this.Read(this.buffer, 8);
					return BitConverter.ToDouble(this.buffer, 0);
				case StreamPropertyType.Decimal:
				{
					this.Read(this.buffer, 16);
					int[] array = new int[4];
					for (int i = 0; i < 4; i++)
					{
						array[i] = BitConverter.ToInt32(this.buffer, i * 4);
					}
					return new decimal(array);
				}
				case StreamPropertyType.Char:
				{
					char[] array2 = (char[])this.ReadRawValue(StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.Array);
					if (array2 == null || array2.Length != 1)
					{
						throw new FormatException("Extended property stream. Invalid character content.");
					}
					return array2[0];
				}
				case StreamPropertyType.String:
				{
					this.Read(this.buffer, 4);
					int num = BitConverter.ToInt32(this.buffer, 0);
					if (num == 0)
					{
						return string.Empty;
					}
					if (num > this.buffer.Length)
					{
						byte[] bytes = new byte[num];
						this.Read(bytes, num);
						return Encoding.UTF8.GetString(bytes, 0, num);
					}
					this.Read(this.buffer, num);
					return Encoding.UTF8.GetString(this.buffer, 0, num);
				}
				case StreamPropertyType.DateTime:
				{
					this.Read(this.buffer, 8);
					long fileTime = BitConverter.ToInt64(this.buffer, 0);
					return DateTime.FromFileTimeUtc(fileTime);
				}
				case StreamPropertyType.Guid:
					this.Read(this.buffer, 16);
					return ExBitConverter.ReadGuid(this.buffer, 0);
				case StreamPropertyType.IPAddress:
				{
					this.Read(this.buffer, 16);
					IPvxAddress address = new IPvxAddress(BitConverter.ToUInt64(this.buffer, 8), BitConverter.ToUInt64(this.buffer, 0));
					return address;
				}
				case StreamPropertyType.IPEndPoint:
				{
					IPAddress ipaddress = (IPAddress)this.ReadRawValue(StreamPropertyType.IPAddress);
					ushort num2 = (ushort)this.ReadRawValue(StreamPropertyType.UInt16);
					if (ipaddress != IPAddress.None || num2 != 0)
					{
						return new IPEndPoint(ipaddress, (int)num2);
					}
					return null;
				}
				case StreamPropertyType.RoutingAddress:
				case StreamPropertyType.ADObjectId:
				case StreamPropertyType.RecipientType:
				case StreamPropertyType.ADObjectIdUTF8:
				case StreamPropertyType.ADObjectIdWithString:
				case StreamPropertyType.ProxyAddress:
					break;
				default:
					switch (propId)
					{
					case StreamPropertyType.Bool | StreamPropertyType.Array:
					{
						bool[] result;
						this.ReadArray<bool>(propId, out result);
						return result;
					}
					case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.Array:
					{
						byte[] result2;
						this.ReadArray<byte>(propId, out result2);
						return result2;
					}
					case StreamPropertyType.SByte | StreamPropertyType.Array:
					{
						sbyte[] result3;
						this.ReadArray<sbyte>(propId, out result3);
						return result3;
					}
					case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.Array:
					{
						short[] result4;
						this.ReadArray<short>(propId, out result4);
						return result4;
					}
					case StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.Array:
					{
						ushort[] result5;
						this.ReadArray<ushort>(propId, out result5);
						return result5;
					}
					case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.Array:
					{
						int[] result6;
						this.ReadArray<int>(propId, out result6);
						return result6;
					}
					case StreamPropertyType.UInt32 | StreamPropertyType.Array:
					{
						uint[] result7;
						this.ReadArray<uint>(propId, out result7);
						return result7;
					}
					case StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.Array:
					{
						long[] result8;
						this.ReadArray<long>(propId, out result8);
						return result8;
					}
					case StreamPropertyType.Bool | StreamPropertyType.UInt32 | StreamPropertyType.Array:
					{
						ulong[] result9;
						this.ReadArray<ulong>(propId, out result9);
						return result9;
					}
					case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.UInt32 | StreamPropertyType.Array:
					{
						float[] result10;
						this.ReadArray<float>(propId, out result10);
						return result10;
					}
					case StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.Array:
					{
						double[] result11;
						this.ReadArray<double>(propId, out result11);
						return result11;
					}
					case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.Array:
					{
						decimal[] result12;
						this.ReadArray<decimal>(propId, out result12);
						return result12;
					}
					case StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.Array:
					{
						this.Read(this.buffer, 4);
						int num3 = BitConverter.ToInt32(this.buffer, 0);
						if (num3 == 0)
						{
							return new char[0];
						}
						if (num3 > this.buffer.Length)
						{
							byte[] bytes2 = new byte[num3];
							this.Read(bytes2, num3);
							return Encoding.UTF8.GetChars(bytes2);
						}
						this.Read(this.buffer, num3);
						return Encoding.UTF8.GetChars(this.buffer, 0, num3);
					}
					case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.Array:
					{
						string[] result13;
						this.ReadArray<string>(propId, out result13);
						return result13;
					}
					case StreamPropertyType.DateTime | StreamPropertyType.Array:
					{
						DateTime[] result14;
						this.ReadArray<DateTime>(propId, out result14);
						return result14;
					}
					case StreamPropertyType.Null | StreamPropertyType.DateTime | StreamPropertyType.Array:
					{
						Guid[] result15;
						this.ReadArray<Guid>(propId, out result15);
						return result15;
					}
					case StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.Array:
					{
						IPAddress[] result16;
						this.ReadArray<IPAddress>(propId, out result16);
						return result16;
					}
					case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.Array:
					{
						IPEndPoint[] result17;
						this.ReadArray<IPEndPoint>(propId, out result17);
						return result17;
					}
					case StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.Array:
					case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.Array:
					case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.Array:
					case StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.Array:
					case StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.Array:
						break;
					case StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.Array:
						goto IL_8D3;
					default:
						switch (propId)
						{
						case StreamPropertyType.Bool | StreamPropertyType.List:
						{
							List<bool> result18;
							this.ReadList<bool>(propId, out result18);
							return result18;
						}
						case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.List:
						{
							List<byte> result19;
							this.ReadList<byte>(propId, out result19);
							return result19;
						}
						case StreamPropertyType.SByte | StreamPropertyType.List:
						{
							List<sbyte> result20;
							this.ReadList<sbyte>(propId, out result20);
							return result20;
						}
						case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.List:
						{
							List<short> result21;
							this.ReadList<short>(propId, out result21);
							return result21;
						}
						case StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.List:
						{
							List<ushort> result22;
							this.ReadList<ushort>(propId, out result22);
							return result22;
						}
						case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.List:
						{
							List<int> result23;
							this.ReadList<int>(propId, out result23);
							return result23;
						}
						case StreamPropertyType.UInt32 | StreamPropertyType.List:
						{
							List<uint> result24;
							this.ReadList<uint>(propId, out result24);
							return result24;
						}
						case StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.List:
						{
							List<long> result25;
							this.ReadList<long>(propId, out result25);
							return result25;
						}
						case StreamPropertyType.Bool | StreamPropertyType.UInt32 | StreamPropertyType.List:
						{
							List<ulong> result26;
							this.ReadList<ulong>(propId, out result26);
							return result26;
						}
						case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.UInt32 | StreamPropertyType.List:
						{
							List<float> result27;
							this.ReadList<float>(propId, out result27);
							return result27;
						}
						case StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.List:
						{
							List<double> result28;
							this.ReadList<double>(propId, out result28);
							return result28;
						}
						case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.List:
						{
							List<decimal> result29;
							this.ReadList<decimal>(propId, out result29);
							return result29;
						}
						case StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.List:
						{
							this.Read(this.buffer, 4);
							int num4 = BitConverter.ToInt32(this.buffer, 0);
							if (num4 == 0)
							{
								return new char[0];
							}
							if (num4 > this.buffer.Length)
							{
								byte[] bytes3 = new byte[num4];
								this.Read(bytes3, num4);
								return new List<char>(Encoding.UTF8.GetChars(bytes3));
							}
							this.Read(this.buffer, num4);
							return new List<char>(Encoding.UTF8.GetChars(this.buffer, 0, num4));
						}
						case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.List:
						{
							List<string> result30;
							this.ReadList<string>(propId, out result30);
							return result30;
						}
						case StreamPropertyType.DateTime | StreamPropertyType.List:
						{
							List<DateTime> result31;
							this.ReadList<DateTime>(propId, out result31);
							return result31;
						}
						case StreamPropertyType.Null | StreamPropertyType.DateTime | StreamPropertyType.List:
						{
							List<Guid> result32;
							this.ReadList<Guid>(propId, out result32);
							return result32;
						}
						case StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.List:
						{
							List<IPAddress> result33;
							this.ReadList<IPAddress>(propId, out result33);
							return result33;
						}
						case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.List:
						{
							List<IPEndPoint> result34;
							this.ReadList<IPEndPoint>(propId, out result34);
							return result34;
						}
						case StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.List:
						case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.List:
						case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.List:
						case StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.List:
						case StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.List:
							break;
						case StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.List:
							goto IL_8D3;
						default:
							goto IL_8D3;
						}
						break;
					}
					break;
				}
			}
			else
			{
				if (propId == (StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.Array | StreamPropertyType.List))
				{
					List<byte[]> result35;
					this.ReadList<byte[]>(propId, out result35);
					return result35;
				}
				switch (propId)
				{
				case StreamPropertyType.Bool | StreamPropertyType.MultiValuedProperty:
				{
					List<bool> result36;
					this.ReadMultiValuedProperty<bool>(propId, out result36);
					return result36;
				}
				case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.MultiValuedProperty:
				{
					List<byte> result37;
					this.ReadMultiValuedProperty<byte>(propId, out result37);
					return result37;
				}
				case StreamPropertyType.SByte | StreamPropertyType.MultiValuedProperty:
				{
					List<sbyte> result38;
					this.ReadMultiValuedProperty<sbyte>(propId, out result38);
					return result38;
				}
				case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.MultiValuedProperty:
				{
					List<short> result39;
					this.ReadMultiValuedProperty<short>(propId, out result39);
					return result39;
				}
				case StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.MultiValuedProperty:
				{
					List<ushort> result40;
					this.ReadMultiValuedProperty<ushort>(propId, out result40);
					return result40;
				}
				case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.MultiValuedProperty:
				{
					List<int> result41;
					this.ReadMultiValuedProperty<int>(propId, out result41);
					return result41;
				}
				case StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty:
				{
					List<uint> result42;
					this.ReadMultiValuedProperty<uint>(propId, out result42);
					return result42;
				}
				case StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty:
				{
					List<long> result43;
					this.ReadMultiValuedProperty<long>(propId, out result43);
					return result43;
				}
				case StreamPropertyType.Bool | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty:
				{
					List<ulong> result44;
					this.ReadMultiValuedProperty<ulong>(propId, out result44);
					return result44;
				}
				case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty:
				{
					List<float> result45;
					this.ReadMultiValuedProperty<float>(propId, out result45);
					return result45;
				}
				case StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty:
				{
					List<double> result46;
					this.ReadMultiValuedProperty<double>(propId, out result46);
					return result46;
				}
				case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty:
				{
					List<decimal> result47;
					this.ReadMultiValuedProperty<decimal>(propId, out result47);
					return result47;
				}
				case StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty:
				{
					this.Read(this.buffer, 4);
					int num5 = BitConverter.ToInt32(this.buffer, 0);
					if (num5 == 0)
					{
						return new char[0];
					}
					if (num5 > this.buffer.Length)
					{
						byte[] bytes4 = new byte[num5];
						this.Read(bytes4, num5);
						return new List<char>(Encoding.UTF8.GetChars(bytes4));
					}
					this.Read(this.buffer, num5);
					return new List<char>(Encoding.UTF8.GetChars(this.buffer, 0, num5));
				}
				case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.MultiValuedProperty:
				{
					List<string> result48;
					this.ReadMultiValuedProperty<string>(propId, out result48);
					return result48;
				}
				case StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
				{
					List<DateTime> result49;
					this.ReadMultiValuedProperty<DateTime>(propId, out result49);
					return result49;
				}
				case StreamPropertyType.Null | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
				{
					List<Guid> result50;
					this.ReadMultiValuedProperty<Guid>(propId, out result50);
					return result50;
				}
				case StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
				{
					List<IPAddress> result51;
					this.ReadMultiValuedProperty<IPAddress>(propId, out result51);
					return result51;
				}
				case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
				{
					List<IPEndPoint> result52;
					this.ReadMultiValuedProperty<IPEndPoint>(propId, out result52);
					return result52;
				}
				case StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
				case StreamPropertyType.Null | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
				case StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
				case StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
				case StreamPropertyType.Null | StreamPropertyType.UInt32 | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
					break;
				case StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.DateTime | StreamPropertyType.MultiValuedProperty:
					goto IL_8D3;
				default:
				{
					if (propId != (StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.Array | StreamPropertyType.MultiValuedProperty))
					{
						goto IL_8D3;
					}
					List<byte[]> result53;
					this.ReadMultiValuedProperty<byte[]>(propId, out result53);
					return result53;
				}
				}
			}
			throw new InvalidOperationException(string.Format("The property type {0} is not supported by PropertyStreamReader, but may be supported by a subclass", propId));
			IL_8D3:
			throw new FormatException("Invalid value data " + propId);
		}

		protected void ReadArray<T>(StreamPropertyType propId, out T[] array)
		{
			this.Read(this.buffer, 4);
			int num = BitConverter.ToInt32(this.buffer, 0);
			array = new T[num];
			for (int i = 0; i < array.Length; i++)
			{
				object obj = this.ReadRawValue(propId & ~StreamPropertyType.Array);
				array[i] = (T)((object)obj);
			}
		}

		protected void ReadList<T>(StreamPropertyType propId, out List<T> list)
		{
			this.Read(this.buffer, 4);
			int num = BitConverter.ToInt32(this.buffer, 0);
			list = new List<T>(num);
			for (int i = 0; i < num; i++)
			{
				object obj = this.ReadRawValue(propId & ~StreamPropertyType.List);
				list.Add((T)((object)obj));
			}
		}

		protected void ReadMultiValuedProperty<T>(StreamPropertyType propId, out List<T> list)
		{
			this.Read(this.buffer, 4);
			int num = BitConverter.ToInt32(this.buffer, 0);
			list = new List<T>(num);
			for (int i = 0; i < num; i++)
			{
				object obj = this.ReadRawValue(propId & ~StreamPropertyType.MultiValuedProperty);
				list.Add((T)((object)obj));
			}
		}

		private string ReadKey(out bool keyReadFromStream)
		{
			int num = 2;
			int num2 = 0;
			for (;;)
			{
				int num3 = this.stream.Read(this.buffer, num2, num);
				if (num3 == 0)
				{
					break;
				}
				num -= num3;
				num2 += num3;
				if (num <= 0)
				{
					goto Block_3;
				}
			}
			if (num2 != 0)
			{
				throw new FormatException("Extended property stream. Unexpected truncation of serialized data.");
			}
			keyReadFromStream = false;
			return null;
			Block_3:
			keyReadFromStream = true;
			TypedValue key;
			key.Type = (StreamPropertyType)BitConverter.ToInt16(this.buffer, 0);
			key.Value = this.ReadRawValue(key.Type);
			return this.ConvertTypedKey(key);
		}

		protected Stream stream;

		protected byte[] buffer = new byte[1024];
	}
}
