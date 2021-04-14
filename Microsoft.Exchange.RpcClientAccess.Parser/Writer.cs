using System;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal abstract class Writer : BaseObject
	{
		public abstract long Position { get; set; }

		public void WriteBool(bool value)
		{
			this.WriteBool(value, byte.MaxValue);
		}

		public void WriteBool(bool value, byte trueValue)
		{
			this.WriteByte(value ? trueValue : 0);
		}

		public void WriteBytes(byte[] value)
		{
			this.WriteBytes(value, 0, value.Length);
		}

		public void WriteBytes(byte[] value, int offset, int count)
		{
			if (count == 0)
			{
				return;
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			if (value.Length - offset < count)
			{
				throw new ArgumentException("Offset and length were out of bounds for the array or count is greater than the number of elements from index to the end of the source collection.");
			}
			this.InternalWriteBytes(value, offset, count);
		}

		public void WriteBytesSegment(ArraySegment<byte> value)
		{
			this.WriteBytes(value.Array, value.Offset, value.Count);
		}

		public void WriteSizedBytesSegment(ArraySegment<byte> value, FieldLength lengthSize)
		{
			this.WriteCountOrSize(value.Count, lengthSize);
			if (value.Count > 0)
			{
				this.WriteBytes(value.Array, value.Offset, value.Count);
			}
		}

		public void WriteSizedBytes(byte[] value)
		{
			this.WriteSizedBytes(value, FieldLength.WordSize);
		}

		public void WriteSizedBytes(byte[] value, FieldLength lengthSize)
		{
			Util.ThrowOnNullArgument(value, "value");
			this.WriteSizeAndBytesArray(value, value.Length, lengthSize);
		}

		public void WriteSizeAndBytesArray(byte[] value, int byteCount, FieldLength lengthSize)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (byteCount > value.Length)
			{
				throw new ArgumentOutOfRangeException("Number of bytes to write exceeds the size of the buffer.");
			}
			this.WriteCountOrSize(byteCount, lengthSize);
			if (byteCount > 0)
			{
				this.WriteBytes(value, 0, byteCount);
			}
		}

		public void WriteCountAndByteArrayList(byte[][] values, FieldLength lengthSize)
		{
			if (values == null)
			{
				throw new ArgumentNullException("values");
			}
			this.WriteCountOrSize(values.GetLength(0), lengthSize);
			foreach (byte[] value in values)
			{
				this.WriteSizedBytes(value, lengthSize);
			}
		}

		public void WriteUnicodeString(string value, StringFlags flags)
		{
			this.WriteString(value, Encoding.Unicode, flags);
		}

		public void WriteAsciiString(string value, StringFlags flags)
		{
			this.WriteString(value, CTSGlobals.AsciiEncoding, flags);
		}

		public void WriteString8(string value, Encoding encoding, StringFlags flags)
		{
			if ((flags & StringFlags.IncludeNull) != StringFlags.IncludeNull)
			{
				throw new NotSupportedException("Writing non-terminated String8 values is not supported.");
			}
			String8Encodings.ThrowIfInvalidString8Encoding(encoding);
			this.WriteString(value, encoding, flags);
		}

		public void UpdateSize(long startPosition, long endPosition)
		{
			this.Position = startPosition;
			this.WriteUInt16((ushort)(endPosition - startPosition - 2L));
			this.Position = endPosition;
		}

		public void WriteSecurityIdentifier(SecurityIdentifier sid)
		{
			if (sid == null)
			{
				throw new ArgumentNullException("sid");
			}
			byte[] array = new byte[sid.BinaryLength];
			sid.GetBinaryForm(array, 0);
			this.WriteBytes(array);
		}

		public void WritePropertyTag(PropertyTag value)
		{
			this.WriteUInt32(value);
		}

		public void WritePropertyValue(PropertyValue value, Encoding string8Encoding, WireFormatStyle wireFormatStyle)
		{
			this.WritePropertyTag(value.PropertyTag);
			this.WritePropertyValueWithoutTag(value, string8Encoding, wireFormatStyle);
		}

		public void WriteNullablePropertyValue(PropertyValue? propertyValue, Encoding string8Encoding, WireFormatStyle wireFormatStyle)
		{
			bool flag = propertyValue != null;
			this.WriteBool(flag);
			if (flag)
			{
				this.WritePropertyValue(propertyValue.Value, string8Encoding, wireFormatStyle);
			}
		}

		public void WritePropertyValueWithoutTag(PropertyValue value, Encoding string8Encoding, WireFormatStyle wireFormatStyle)
		{
			if (string8Encoding == null)
			{
				throw new ArgumentNullException("string8Encoding");
			}
			if (value.IsNullValue && value.PropertyTag.PropertyType != PropertyType.Null && wireFormatStyle != WireFormatStyle.Nspi)
			{
				throw new NotSupportedException(string.Format("Property value cannot be null: {0}.", value.PropertyTag));
			}
			if (value.IsError)
			{
				this.WriteUInt32((uint)value.ErrorCodeValue);
				return;
			}
			this.WritePropertyValueInternal(value.PropertyTag.PropertyType, value.Value, string8Encoding, wireFormatStyle);
		}

		public abstract void WriteByte(byte value);

		public abstract void WriteDouble(double value);

		public abstract void WriteSingle(float value);

		public abstract void WriteGuid(Guid value);

		public abstract void WriteInt32(int value);

		public abstract void WriteInt64(long value);

		public abstract void WriteInt16(short value);

		public abstract void WriteUInt32(uint value);

		public abstract void WriteUInt64(ulong value);

		public abstract void WriteUInt16(ushort value);

		public abstract void SkipArraySegment(ArraySegment<byte> buffer);

		protected abstract void InternalWriteBytes(byte[] value, int offset, int count);

		protected abstract void InternalWriteString(string value, int length, Encoding encoding);

		internal void WriteCountOrSize(int count, FieldLength lengthSize)
		{
			if (count < 0)
			{
				throw new ArgumentException("Cannot serialize a negative count", "count");
			}
			this.WriteCount((uint)count, lengthSize);
		}

		internal void WriteCount(uint count, FieldLength lengthSize)
		{
			switch (lengthSize)
			{
			case FieldLength.WordSize:
				if (count > 65535U)
				{
					string message = string.Format("Value ({0}) is too large to be serialized as a ushort", count);
					throw new ArgumentException(message, "count");
				}
				this.WriteUInt16((ushort)count);
				return;
			case FieldLength.DWordSize:
				this.WriteUInt32(count);
				return;
			default:
				throw new ArgumentException("Unrecognized FieldLength: " + lengthSize, "lengthSize");
			}
		}

		internal void WriteCountOrSize(int count, WireFormatStyle wireFormatStyle)
		{
			if (count < 0)
			{
				throw new ArgumentException("Cannot serialize a negative count", "count");
			}
			this.WriteCount((uint)count, wireFormatStyle);
		}

		internal void WriteCount(uint count, WireFormatStyle wireFormatStyle)
		{
			switch (wireFormatStyle)
			{
			case WireFormatStyle.Rop:
				if (count > 65535U)
				{
					string message = string.Format("Value ({0}) is too large to be serialized as a ushort", count);
					throw new ArgumentException(message, "count");
				}
				this.WriteUInt16((ushort)count);
				return;
			case WireFormatStyle.Nspi:
				this.WriteUInt32(count);
				return;
			default:
				throw new ArgumentException("Unrecognized wire format style: " + wireFormatStyle, "wireFormatStyle");
			}
		}

		private bool TryWriteHasValue(object value, WireFormatStyle wireFormatStyle)
		{
			if (wireFormatStyle == WireFormatStyle.Nspi)
			{
				bool flag = value != null;
				this.WriteBool(flag, byte.MaxValue);
				return flag;
			}
			return true;
		}

		private void WritePropertyValueInternal(PropertyType propertyType, object value, Encoding string8Encoding, WireFormatStyle wireFormatStyle)
		{
			if (wireFormatStyle != WireFormatStyle.Nspi && propertyType != PropertyType.Null && value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (propertyType <= PropertyType.Binary)
			{
				if (propertyType <= PropertyType.SysTime)
				{
					switch (propertyType)
					{
					case PropertyType.Unspecified:
					case (PropertyType)8:
					case (PropertyType)9:
					case (PropertyType)12:
					case (PropertyType)14:
					case (PropertyType)15:
					case (PropertyType)16:
					case (PropertyType)17:
					case (PropertyType)18:
					case (PropertyType)19:
						break;
					case PropertyType.Null:
						return;
					case PropertyType.Int16:
						this.WriteInt16((short)value);
						return;
					case PropertyType.Int32:
						this.WriteInt32((int)value);
						return;
					case PropertyType.Float:
						this.WriteSingle((float)value);
						return;
					case PropertyType.Double:
					case PropertyType.AppTime:
						this.WriteDouble((double)value);
						return;
					case PropertyType.Currency:
					case PropertyType.Int64:
						this.WriteInt64((long)value);
						return;
					case PropertyType.Error:
						this.WriteUInt32((uint)value);
						return;
					case PropertyType.Bool:
						this.WriteBool((bool)value, 1);
						return;
					case PropertyType.Object:
						if (wireFormatStyle != WireFormatStyle.Nspi)
						{
							throw new NotImplementedException(string.Format("Property type not implemented: {0}.", propertyType));
						}
						return;
					default:
						switch (propertyType)
						{
						case PropertyType.String8:
							if (this.TryWriteHasValue(value, wireFormatStyle))
							{
								this.WriteString8((string)value, string8Encoding, StringFlags.IncludeNull);
								return;
							}
							return;
						case PropertyType.Unicode:
							if (this.TryWriteHasValue(value, wireFormatStyle))
							{
								this.WriteUnicodeString((string)value, StringFlags.IncludeNull);
								return;
							}
							return;
						default:
							if (propertyType == PropertyType.SysTime)
							{
								this.WriteInt64(PropertyValue.ExDateTimeToFileTimeUtc((ExDateTime)value));
								return;
							}
							break;
						}
						break;
					}
				}
				else if (propertyType != PropertyType.Guid)
				{
					switch (propertyType)
					{
					case PropertyType.ServerId:
						break;
					case (PropertyType)252:
						goto IL_3A1;
					case PropertyType.Restriction:
						if (this.TryWriteHasValue(value, wireFormatStyle))
						{
							((Restriction)value).Serialize(this, string8Encoding, wireFormatStyle);
							return;
						}
						return;
					case PropertyType.Actions:
						if (wireFormatStyle == WireFormatStyle.Nspi)
						{
							throw new NotSupportedException(string.Format("Property type not supported: {0}.", propertyType));
						}
						this.WriteSizedRuleActions((RuleAction[])value, string8Encoding);
						return;
					default:
						if (propertyType != PropertyType.Binary)
						{
							goto IL_3A1;
						}
						break;
					}
					if (this.TryWriteHasValue(value, wireFormatStyle))
					{
						byte[] array = (byte[])value;
						this.WriteCountOrSize(array.Length, wireFormatStyle);
						this.WriteBytes(array);
						return;
					}
					return;
				}
				else
				{
					if (this.TryWriteHasValue(value, wireFormatStyle))
					{
						this.WriteGuid((Guid)value);
						return;
					}
					return;
				}
			}
			else if (propertyType <= PropertyType.MultiValueUnicode)
			{
				switch (propertyType)
				{
				case PropertyType.MultiValueInt16:
					if (this.TryWriteHasValue(value, wireFormatStyle))
					{
						this.SerializeMultiValue<short>(PropertyType.Int16, value, string8Encoding, wireFormatStyle);
						return;
					}
					return;
				case PropertyType.MultiValueInt32:
					if (this.TryWriteHasValue(value, wireFormatStyle))
					{
						this.SerializeMultiValue<int>(PropertyType.Int32, value, string8Encoding, wireFormatStyle);
						return;
					}
					return;
				case PropertyType.MultiValueFloat:
					if (this.TryWriteHasValue(value, wireFormatStyle))
					{
						this.SerializeMultiValue<float>(PropertyType.Float, value, string8Encoding, wireFormatStyle);
						return;
					}
					return;
				case PropertyType.MultiValueDouble:
				case PropertyType.MultiValueAppTime:
					if (this.TryWriteHasValue(value, wireFormatStyle))
					{
						this.SerializeMultiValue<double>(PropertyType.Double, value, string8Encoding, wireFormatStyle);
						return;
					}
					return;
				case PropertyType.MultiValueCurrency:
					break;
				default:
					if (propertyType != PropertyType.MultiValueInt64)
					{
						switch (propertyType)
						{
						case PropertyType.MultiValueString8:
							if (this.TryWriteHasValue(value, wireFormatStyle))
							{
								this.SerializeMultiValue<string>(PropertyType.String8, value, string8Encoding, wireFormatStyle);
								return;
							}
							return;
						case PropertyType.MultiValueUnicode:
							if (this.TryWriteHasValue(value, wireFormatStyle))
							{
								this.SerializeMultiValue<string>(PropertyType.Unicode, value, string8Encoding, wireFormatStyle);
								return;
							}
							return;
						default:
							goto IL_3A1;
						}
					}
					break;
				}
				if (this.TryWriteHasValue(value, wireFormatStyle))
				{
					this.SerializeMultiValue<long>(PropertyType.Int64, value, string8Encoding, wireFormatStyle);
					return;
				}
				return;
			}
			else if (propertyType != PropertyType.MultiValueSysTime)
			{
				if (propertyType != PropertyType.MultiValueGuid)
				{
					if (propertyType == PropertyType.MultiValueBinary)
					{
						if (this.TryWriteHasValue(value, wireFormatStyle))
						{
							this.SerializeMultiValue<byte[]>(PropertyType.Binary, value, string8Encoding, wireFormatStyle);
							return;
						}
						return;
					}
				}
				else
				{
					if (this.TryWriteHasValue(value, wireFormatStyle))
					{
						this.SerializeMultiValue<Guid>(PropertyType.Guid, value, string8Encoding, wireFormatStyle);
						return;
					}
					return;
				}
			}
			else
			{
				if (this.TryWriteHasValue(value, wireFormatStyle))
				{
					this.SerializeMultiValue<ExDateTime>(PropertyType.SysTime, value, string8Encoding, wireFormatStyle);
					return;
				}
				return;
			}
			IL_3A1:
			throw new NotSupportedException(string.Format("Property type not supported: {0}.", propertyType));
		}

		private void WriteString(string value, Encoding encoding, StringFlags flags)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			int byteCount = encoding.GetByteCount(value);
			byte[] array = null;
			if ((flags & StringFlags.IncludeNull) == StringFlags.IncludeNull)
			{
				array = ((encoding == Encoding.Unicode) ? Writer.UnicodeNullTerminator : Writer.AsciiNullTerminator);
			}
			if ((flags & (StringFlags.Sized | StringFlags.Sized16 | StringFlags.Sized32)) != StringFlags.None)
			{
				int num = byteCount + ((array != null) ? array.Length : 0);
				if ((flags & StringFlags.Sized16) == StringFlags.Sized16)
				{
					if (num > 65535)
					{
						throw new BufferParseException("Sized Unicode string is larger than the maximum size allowed.");
					}
					this.WriteUInt16((ushort)num);
				}
				else if ((flags & StringFlags.Sized) == StringFlags.Sized)
				{
					if (num > 255)
					{
						string message = string.Format("Sized string serialization size {0} larger than {1} limit.", num, byte.MaxValue);
						throw new BufferParseException(message);
					}
					this.WriteByte((byte)num);
				}
				else
				{
					this.WriteUInt32((uint)num);
				}
			}
			this.InternalWriteString(value, byteCount, encoding);
			if (array != null)
			{
				this.WriteBytes(array);
			}
		}

		private void SerializeMultiValue<T>(PropertyType elementPropertyType, object value, Encoding string8Encoding, WireFormatStyle wireFormatStyle)
		{
			T[] array = value as T[];
			if (array == null)
			{
				throw new ArgumentException("Value is of the incorrect type.", "value");
			}
			this.WriteUInt32((uint)array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				this.WritePropertyValueInternal(elementPropertyType, array[i], string8Encoding, wireFormatStyle);
			}
		}

		// Note: this type is marked as 'beforefieldinit'.
		static Writer()
		{
			byte[] asciiNullTerminator = new byte[1];
			Writer.AsciiNullTerminator = asciiNullTerminator;
			byte[] unicodeNullTerminator = new byte[2];
			Writer.UnicodeNullTerminator = unicodeNullTerminator;
		}

		private static readonly byte[] AsciiNullTerminator;

		private static readonly byte[] UnicodeNullTerminator;
	}
}
