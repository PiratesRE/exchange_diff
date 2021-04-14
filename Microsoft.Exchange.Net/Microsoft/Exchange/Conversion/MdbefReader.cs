using System;
using System.Text;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Conversion
{
	internal class MdbefReader
	{
		public MdbefReader(byte[] data, int startIndex, int length)
		{
			this.data = data;
			this.currentIndex = startIndex;
			this.endIndex = startIndex + length;
			this.expectedCount = this.ReadInt32();
			if (this.expectedCount > length / 4)
			{
				throw new MdbefException(NetException.TruncatedData);
			}
		}

		public int PropertyId
		{
			get
			{
				return this.propId;
			}
		}

		public object Value
		{
			get
			{
				return this.value;
			}
		}

		public bool ReadNextProperty()
		{
			bool flag = true;
			while (flag)
			{
				if (this.currentIndex == this.endIndex)
				{
					if (this.parsedCount < this.expectedCount)
					{
						throw new MdbefException(NetException.TruncatedData);
					}
					return false;
				}
				else
				{
					if (this.parsedCount == this.expectedCount)
					{
						throw new MdbefException(NetException.ReadPastEnd);
					}
					this.value = null;
					this.propId = this.ReadInt32();
					MapiPropType mapiPropType = (MapiPropType)(this.propId & 65535);
					flag = false;
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
								goto IL_33F;
							case MapiPropType.Short:
								this.value = this.ReadInt16();
								goto IL_34F;
							case MapiPropType.Int:
								this.value = this.ReadInt32();
								goto IL_34F;
							case MapiPropType.Float:
								this.value = this.ReadSingle();
								goto IL_34F;
							case MapiPropType.Double:
							case MapiPropType.AppTime:
								this.value = this.ReadDouble();
								goto IL_34F;
							case MapiPropType.Currency:
								break;
							case MapiPropType.Boolean:
								this.value = this.ReadBoolean();
								goto IL_34F;
							default:
								if (mapiPropType2 != MapiPropType.Long)
								{
									switch (mapiPropType2)
									{
									case MapiPropType.AnsiString:
										try
										{
											this.value = this.ReadANSIString();
											goto IL_34F;
										}
										catch (DecoderFallbackException innerException)
										{
											if (this.propId >> 16 == 3098 || this.propId >> 16 == 12289)
											{
												flag = true;
												goto IL_34F;
											}
											throw new MdbefException("Invalid ANSI string while decoding property with propId: " + this.propId, innerException);
										}
										break;
									case MapiPropType.String:
										break;
									default:
										goto IL_33F;
									}
									this.value = this.ReadUTF16String();
									goto IL_34F;
								}
								break;
							}
							this.value = this.ReadInt64();
						}
						else if (mapiPropType2 != MapiPropType.SysTime)
						{
							if (mapiPropType2 != MapiPropType.Guid)
							{
								if (mapiPropType2 != MapiPropType.ServerId)
								{
									goto IL_33F;
								}
								goto IL_29C;
							}
							else
							{
								this.value = this.ReadGuid();
							}
						}
						else
						{
							this.value = this.ReadDateTime();
						}
					}
					else if (mapiPropType2 <= MapiPropType.LongArray)
					{
						if (mapiPropType2 == MapiPropType.Binary)
						{
							goto IL_29C;
						}
						switch (mapiPropType2)
						{
						case MapiPropType.ShortArray:
							this.value = this.ReadInt16Array();
							goto IL_34F;
						case MapiPropType.IntArray:
							this.value = this.ReadInt32Array();
							goto IL_34F;
						case MapiPropType.FloatArray:
							this.value = this.ReadSingleArray();
							goto IL_34F;
						case MapiPropType.DoubleArray:
						case MapiPropType.AppTimeArray:
							this.value = this.ReadDoubleArray();
							goto IL_34F;
						case MapiPropType.CurrencyArray:
							break;
						default:
							if (mapiPropType2 != MapiPropType.LongArray)
							{
								goto IL_33F;
							}
							break;
						}
						this.value = this.ReadInt64Array();
					}
					else if (mapiPropType2 <= MapiPropType.SysTimeArray)
					{
						switch (mapiPropType2)
						{
						case MapiPropType.AnsiStringArray:
							this.value = this.ReadANSIStringArray();
							break;
						case MapiPropType.StringArray:
							this.value = this.ReadUTF16StringArray();
							break;
						default:
							if (mapiPropType2 != MapiPropType.SysTimeArray)
							{
								goto IL_33F;
							}
							this.value = this.ReadDateTimeArray();
							break;
						}
					}
					else if (mapiPropType2 != MapiPropType.GuidArray)
					{
						if (mapiPropType2 != MapiPropType.BinaryArray)
						{
							goto IL_33F;
						}
						this.value = this.ReadBinaryArray();
					}
					else
					{
						this.value = this.ReadGuidArray();
					}
					IL_34F:
					this.parsedCount++;
					continue;
					IL_29C:
					this.value = this.ReadByteArray();
					goto IL_34F;
					IL_33F:
					throw new MdbefException(NetException.UnknownPropertyType);
				}
			}
			return true;
		}

		private short ReadInt16()
		{
			if (this.currentIndex > this.endIndex - 2)
			{
				throw new MdbefException(NetException.TruncatedData);
			}
			short result = BitConverter.ToInt16(this.data, this.currentIndex);
			this.currentIndex += 2;
			return result;
		}

		private bool ReadBoolean()
		{
			return 0 != this.ReadInt16();
		}

		private int ReadInt32()
		{
			if (this.currentIndex > this.endIndex - 4)
			{
				throw new MdbefException(NetException.TruncatedData);
			}
			int result = BitConverter.ToInt32(this.data, this.currentIndex);
			this.currentIndex += 4;
			return result;
		}

		private long ReadInt64()
		{
			if (this.currentIndex > this.endIndex - 8)
			{
				throw new MdbefException(NetException.TruncatedData);
			}
			long result = BitConverter.ToInt64(this.data, this.currentIndex);
			this.currentIndex += 8;
			return result;
		}

		private float ReadSingle()
		{
			if (this.currentIndex > this.endIndex - 4)
			{
				throw new MdbefException(NetException.TruncatedData);
			}
			float result = BitConverter.ToSingle(this.data, this.currentIndex);
			this.currentIndex += 4;
			return result;
		}

		private double ReadDouble()
		{
			if (this.currentIndex > this.endIndex - 8)
			{
				throw new MdbefException(NetException.TruncatedData);
			}
			double result = BitConverter.ToDouble(this.data, this.currentIndex);
			this.currentIndex += 8;
			return result;
		}

		private DateTime ReadDateTime()
		{
			DateTime result;
			try
			{
				result = DateTime.FromFileTimeUtc(this.ReadInt64());
			}
			catch (ArgumentOutOfRangeException innerException)
			{
				throw new MdbefException(NetException.InvalidDateValue, innerException);
			}
			return result;
		}

		private Guid ReadGuid()
		{
			if (this.currentIndex > this.endIndex - 16)
			{
				throw new MdbefException(NetException.TruncatedData);
			}
			byte[] array = new byte[16];
			Array.Copy(this.data, this.currentIndex, array, 0, 16);
			this.currentIndex += 16;
			return new Guid(array);
		}

		private string ReadANSIString()
		{
			int num = this.ReadInt32();
			if (num < 1 || num > this.endIndex - this.currentIndex)
			{
				throw new MdbefException("invalid string length prefix");
			}
			int num2 = Array.IndexOf<byte>(this.data, 0, this.currentIndex, num);
			if (num2 == -1)
			{
				throw new MdbefException("string is not null-terminated");
			}
			FeInboundCharsetDetector feInboundCharsetDetector = new FeInboundCharsetDetector(Encoding.ASCII.CodePage, false, true, true, true);
			feInboundCharsetDetector.AddBytes(this.data, this.currentIndex, num2 - this.currentIndex, false);
			int codePageChoice = feInboundCharsetDetector.GetCodePageChoice();
			Encoding encoding = Encoding.GetEncoding(codePageChoice, new EncoderExceptionFallback(), new DecoderExceptionFallback());
			string result = null;
			try
			{
				result = encoding.GetString(this.data, this.currentIndex, num2 - this.currentIndex);
			}
			finally
			{
				this.currentIndex += num;
			}
			return result;
		}

		private string ReadUTF16String()
		{
			int num = this.ReadInt32();
			if (num < 2 || num > this.endIndex - this.currentIndex)
			{
				throw new MdbefException(NetException.CorruptStringSize);
			}
			int num2 = this.currentIndex;
			while (num2 < this.currentIndex + num - 2 && (this.data[num2] != 0 || this.data[num2 + 1] != 0))
			{
				num2 += 2;
			}
			if (this.data[num2] != 0 || this.data[num2 + 1] != 0)
			{
				throw new MdbefException(NetException.MissingNullTerminator);
			}
			string result = null;
			try
			{
				result = MdbefReader.CheckedUTF16.GetString(this.data, this.currentIndex, num2 - this.currentIndex);
			}
			catch (DecoderFallbackException innerException)
			{
				throw new MdbefException(NetException.InvalidUnicodeString, innerException);
			}
			this.currentIndex += num;
			return result;
		}

		private byte[] ReadByteArray()
		{
			int num = this.ReadInt32();
			if (num < 0 || num > this.endIndex - this.currentIndex)
			{
				throw new MdbefException(NetException.CorruptArraySize);
			}
			byte[] array = new byte[num];
			Array.Copy(this.data, this.currentIndex, array, 0, num);
			this.currentIndex += num;
			return array;
		}

		private T[] ReadArray<T>(MdbefReader.ReadOne<T> readOne, int minimumElementSize)
		{
			int num = this.ReadInt32();
			if (num < 0)
			{
				throw new MdbefException(NetException.CorruptArraySize);
			}
			if (num > (this.endIndex - this.currentIndex) / minimumElementSize)
			{
				throw new MdbefException(NetException.CorruptArraySize);
			}
			T[] array = new T[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = readOne();
			}
			return array;
		}

		private short[] ReadInt16Array()
		{
			return this.ReadArray<short>(new MdbefReader.ReadOne<short>(this.ReadInt16), 2);
		}

		private int[] ReadInt32Array()
		{
			return this.ReadArray<int>(new MdbefReader.ReadOne<int>(this.ReadInt32), 4);
		}

		private long[] ReadInt64Array()
		{
			return this.ReadArray<long>(new MdbefReader.ReadOne<long>(this.ReadInt64), 8);
		}

		private float[] ReadSingleArray()
		{
			return this.ReadArray<float>(new MdbefReader.ReadOne<float>(this.ReadSingle), 4);
		}

		private double[] ReadDoubleArray()
		{
			return this.ReadArray<double>(new MdbefReader.ReadOne<double>(this.ReadDouble), 8);
		}

		private DateTime[] ReadDateTimeArray()
		{
			return this.ReadArray<DateTime>(new MdbefReader.ReadOne<DateTime>(this.ReadDateTime), 8);
		}

		private Guid[] ReadGuidArray()
		{
			return this.ReadArray<Guid>(new MdbefReader.ReadOne<Guid>(this.ReadGuid), 16);
		}

		private string[] ReadANSIStringArray()
		{
			return this.ReadArray<string>(new MdbefReader.ReadOne<string>(this.ReadANSIString), 4);
		}

		private string[] ReadUTF16StringArray()
		{
			return this.ReadArray<string>(new MdbefReader.ReadOne<string>(this.ReadUTF16String), 4);
		}

		private byte[][] ReadBinaryArray()
		{
			return this.ReadArray<byte[]>(new MdbefReader.ReadOne<byte[]>(this.ReadByteArray), 4);
		}

		private static readonly Encoding CheckedUTF16 = new UnicodeEncoding(false, false, true);

		private int expectedCount;

		private int parsedCount;

		private byte[] data;

		private int currentIndex;

		private int endIndex;

		private int propId;

		private object value;

		private delegate T ReadOne<T>();
	}
}
