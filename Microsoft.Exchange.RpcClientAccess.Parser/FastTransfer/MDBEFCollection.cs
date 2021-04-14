using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MDBEFCollection : IEnumerable<AnnotatedPropertyValue>, IEnumerable
	{
		internal MDBEFCollection()
		{
		}

		public static MDBEFCollection CreateFrom(byte[] buffer, Encoding string8Encoding)
		{
			MDBEFCollection mdbefcollection = new MDBEFCollection();
			using (StreamReader streamReader = new StreamReader(new MemoryStream(buffer, false)))
			{
				while (streamReader.Position < streamReader.Length)
				{
					int num = 4095;
					PropertyTag propertyTag = new PropertyTag(FastTransferReader.NormalizeTag(streamReader.ReadUInt32(), out num));
					NamedProperty namedProperty = null;
					if (propertyTag.IsNamedProperty)
					{
						namedProperty = MDBEFCollection.ParseNamedPropertyDefinition(streamReader);
					}
					Encoding string8Encoding2;
					if (num == 4095)
					{
						string8Encoding2 = string8Encoding;
					}
					else if ((ushort)num == 1201)
					{
						string8Encoding2 = String8Encodings.ReducedUnicode;
					}
					else
					{
						string8Encoding2 = CodePageMap.GetEncoding(num);
					}
					PropertyValue propertyValue = new PropertyValue(propertyTag, MDBEFCollection.ParsePropertyValue(streamReader, propertyTag.PropertyType, string8Encoding2));
					mdbefcollection.AddAnnotatedProperty(new AnnotatedPropertyValue(propertyTag, propertyValue, namedProperty));
				}
			}
			return mdbefcollection;
		}

		public void AddAnnotatedProperty(AnnotatedPropertyValue property)
		{
			this.listOfProperties.Add(property);
		}

		public IEnumerator<AnnotatedPropertyValue> GetEnumerator()
		{
			return this.listOfProperties.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		public byte[] Serialize(Encoding string8Encoding)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (StreamWriter streamWriter = new StreamWriter(memoryStream))
				{
					foreach (AnnotatedPropertyValue annotatedPropertyValue in this.listOfProperties)
					{
						streamWriter.WriteUInt32(FastTransferWriter.DenormalizeTag(annotatedPropertyValue.PropertyTag.ElementPropertyType != PropertyType.String8, true, annotatedPropertyValue.PropertyTag));
						if (annotatedPropertyValue.PropertyTag.IsNamedProperty)
						{
							MDBEFCollection.SerializeNamedPropertyDefinition(streamWriter, annotatedPropertyValue.NamedProperty);
						}
						MDBEFCollection.SerializePropertyValue(annotatedPropertyValue.PropertyTag.PropertyType, annotatedPropertyValue.PropertyValue.Value, streamWriter, string8Encoding);
					}
					result = memoryStream.ToArray();
				}
			}
			return result;
		}

		private static NamedProperty ParseNamedPropertyDefinition(Reader reader)
		{
			Guid guid = reader.ReadGuid();
			NamedPropertyKind namedPropertyKind = (NamedPropertyKind)reader.ReadByte();
			if (namedPropertyKind == NamedPropertyKind.String)
			{
				return new NamedProperty(guid, reader.ReadUnicodeString(StringFlags.IncludeNull));
			}
			if (namedPropertyKind == NamedPropertyKind.Id)
			{
				return new NamedProperty(guid, reader.ReadUInt32());
			}
			throw new BufferParseException("Invalid named property kind");
		}

		private static void SerializeNamedPropertyDefinition(Writer writer, NamedProperty namedProperty)
		{
			writer.WriteGuid(namedProperty.Guid);
			writer.WriteByte((byte)namedProperty.Kind);
			if (namedProperty.Kind == NamedPropertyKind.String)
			{
				writer.WriteUnicodeString(namedProperty.Name, StringFlags.IncludeNull);
				return;
			}
			if (namedProperty.Kind == NamedPropertyKind.Id)
			{
				writer.WriteUInt32(namedProperty.Id);
			}
		}

		private static void SerializePropertyValue(PropertyType propertyType, object value, Writer writer, Encoding string8Encoding)
		{
			if (propertyType <= PropertyType.ServerId)
			{
				if (propertyType <= PropertyType.Unicode)
				{
					switch (propertyType)
					{
					case PropertyType.Null:
					case PropertyType.Error:
						return;
					case PropertyType.Int16:
						writer.WriteInt16((short)value);
						return;
					case PropertyType.Int32:
						writer.WriteInt32((int)value);
						return;
					case PropertyType.Float:
						writer.WriteSingle((float)value);
						return;
					case PropertyType.Double:
					case PropertyType.AppTime:
						writer.WriteDouble((double)value);
						return;
					case PropertyType.Currency:
						break;
					case (PropertyType)8:
					case (PropertyType)9:
						goto IL_246;
					case PropertyType.Bool:
						writer.WriteInt16(((bool)value) ? 1 : 0);
						return;
					default:
						if (propertyType != PropertyType.Int64)
						{
							switch (propertyType)
							{
							case PropertyType.String8:
							{
								string text = (string)value;
								writer.WriteInt32(text.Length + 1);
								writer.WriteString8(text, string8Encoding, StringFlags.IncludeNull);
								return;
							}
							case PropertyType.Unicode:
							{
								string text2 = (string)value;
								writer.WriteInt32((text2.Length + 1) * 2);
								writer.WriteUnicodeString(text2, StringFlags.IncludeNull);
								return;
							}
							default:
								goto IL_246;
							}
						}
						break;
					}
					writer.WriteInt64((long)value);
					return;
				}
				if (propertyType == PropertyType.SysTime)
				{
					writer.WriteInt64(PropertyValue.ExDateTimeToFileTimeUtc((ExDateTime)value));
					return;
				}
				if (propertyType == PropertyType.Guid)
				{
					writer.WriteGuid((Guid)value);
					return;
				}
				if (propertyType != PropertyType.ServerId)
				{
					goto IL_246;
				}
			}
			else if (propertyType <= PropertyType.MultiValueInt64)
			{
				if (propertyType != PropertyType.Binary)
				{
					switch (propertyType)
					{
					case PropertyType.MultiValueInt16:
						MDBEFCollection.SerializeMultiValue<short>(PropertyType.Int16, value, writer, string8Encoding);
						return;
					case PropertyType.MultiValueInt32:
						MDBEFCollection.SerializeMultiValue<int>(PropertyType.Int32, value, writer, string8Encoding);
						return;
					case PropertyType.MultiValueFloat:
						MDBEFCollection.SerializeMultiValue<float>(PropertyType.Float, value, writer, string8Encoding);
						return;
					case PropertyType.MultiValueDouble:
					case PropertyType.MultiValueAppTime:
						MDBEFCollection.SerializeMultiValue<double>(PropertyType.Double, value, writer, string8Encoding);
						return;
					case PropertyType.MultiValueCurrency:
						break;
					default:
						if (propertyType != PropertyType.MultiValueInt64)
						{
							goto IL_246;
						}
						break;
					}
					MDBEFCollection.SerializeMultiValue<long>(PropertyType.Int64, value, writer, string8Encoding);
					return;
				}
			}
			else if (propertyType <= PropertyType.MultiValueSysTime)
			{
				switch (propertyType)
				{
				case PropertyType.MultiValueString8:
					MDBEFCollection.SerializeMultiValue<string>(PropertyType.String8, value, writer, string8Encoding);
					return;
				case PropertyType.MultiValueUnicode:
					MDBEFCollection.SerializeMultiValue<string>(PropertyType.Unicode, value, writer, string8Encoding);
					return;
				default:
					if (propertyType != PropertyType.MultiValueSysTime)
					{
						goto IL_246;
					}
					MDBEFCollection.SerializeMultiValue<ExDateTime>(PropertyType.SysTime, value, writer, string8Encoding);
					return;
				}
			}
			else
			{
				if (propertyType == PropertyType.MultiValueGuid)
				{
					MDBEFCollection.SerializeMultiValue<Guid>(PropertyType.Guid, value, writer, string8Encoding);
					return;
				}
				if (propertyType != PropertyType.MultiValueBinary)
				{
					goto IL_246;
				}
				MDBEFCollection.SerializeMultiValue<byte[]>(PropertyType.Binary, value, writer, string8Encoding);
				return;
			}
			byte[] array = (byte[])value;
			writer.WriteInt32(array.Length);
			writer.WriteBytes(array);
			return;
			IL_246:
			throw new NotSupportedException(string.Format("Property type not supported: {0}.", propertyType));
		}

		private static void SerializeMultiValue<T>(PropertyType elementPropertyType, object value, Writer writer, Encoding string8Encoding)
		{
			T[] array = value as T[];
			if (array == null)
			{
				throw new ArgumentException("Value is of the incorrect type.", "value");
			}
			writer.WriteUInt32((uint)array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				MDBEFCollection.SerializePropertyValue(elementPropertyType, array[i], writer, string8Encoding);
			}
		}

		private static object ParsePropertyValue(Reader reader, PropertyType propertyType, Encoding string8Encoding)
		{
			if (propertyType <= PropertyType.ServerId)
			{
				if (propertyType <= PropertyType.Unicode)
				{
					switch (propertyType)
					{
					case PropertyType.Null:
					case PropertyType.Error:
						return null;
					case PropertyType.Int16:
						return reader.ReadInt16();
					case PropertyType.Int32:
						return reader.ReadInt32();
					case PropertyType.Float:
						return reader.ReadSingle();
					case PropertyType.Double:
					case PropertyType.AppTime:
						return reader.ReadDouble();
					case PropertyType.Currency:
						break;
					case (PropertyType)8:
					case (PropertyType)9:
						goto IL_230;
					case PropertyType.Bool:
					{
						short num = reader.ReadInt16();
						return num != 0;
					}
					default:
						if (propertyType != PropertyType.Int64)
						{
							switch (propertyType)
							{
							case PropertyType.String8:
								reader.ReadInt32();
								return reader.ReadString8(string8Encoding, StringFlags.IncludeNull);
							case PropertyType.Unicode:
								reader.ReadInt32();
								return reader.ReadUnicodeString(StringFlags.IncludeNull);
							default:
								goto IL_230;
							}
						}
						break;
					}
					return reader.ReadInt64();
				}
				if (propertyType == PropertyType.SysTime)
				{
					long fileTimeAsInt = reader.ReadInt64();
					return PropertyValue.ExDateTimeFromFileTimeUtc(fileTimeAsInt);
				}
				if (propertyType == PropertyType.Guid)
				{
					return reader.ReadGuid();
				}
				if (propertyType != PropertyType.ServerId)
				{
					goto IL_230;
				}
			}
			else if (propertyType <= PropertyType.MultiValueInt64)
			{
				if (propertyType != PropertyType.Binary)
				{
					switch (propertyType)
					{
					case PropertyType.MultiValueInt16:
						return MDBEFCollection.ParseMultiValue<short>(PropertyType.Int16, reader, 2U, string8Encoding);
					case PropertyType.MultiValueInt32:
						return MDBEFCollection.ParseMultiValue<int>(PropertyType.Int32, reader, 4U, string8Encoding);
					case PropertyType.MultiValueFloat:
						return MDBEFCollection.ParseMultiValue<float>(PropertyType.Float, reader, 4U, string8Encoding);
					case PropertyType.MultiValueDouble:
						return MDBEFCollection.ParseMultiValue<double>(PropertyType.Double, reader, 8U, string8Encoding);
					case PropertyType.MultiValueCurrency:
						return MDBEFCollection.ParseMultiValue<long>(PropertyType.Currency, reader, 8U, string8Encoding);
					case PropertyType.MultiValueAppTime:
						return MDBEFCollection.ParseMultiValue<double>(PropertyType.AppTime, reader, 8U, string8Encoding);
					default:
						if (propertyType != PropertyType.MultiValueInt64)
						{
							goto IL_230;
						}
						return MDBEFCollection.ParseMultiValue<long>(PropertyType.Int64, reader, 8U, string8Encoding);
					}
				}
			}
			else if (propertyType <= PropertyType.MultiValueSysTime)
			{
				switch (propertyType)
				{
				case PropertyType.MultiValueString8:
					return MDBEFCollection.ParseMultiValue<string>(PropertyType.String8, reader, 1U, string8Encoding);
				case PropertyType.MultiValueUnicode:
					return MDBEFCollection.ParseMultiValue<string>(PropertyType.Unicode, reader, 2U, string8Encoding);
				default:
					if (propertyType != PropertyType.MultiValueSysTime)
					{
						goto IL_230;
					}
					return MDBEFCollection.ParseMultiValue<ExDateTime>(PropertyType.SysTime, reader, 8U, string8Encoding);
				}
			}
			else
			{
				if (propertyType == PropertyType.MultiValueGuid)
				{
					return MDBEFCollection.ParseMultiValue<Guid>(PropertyType.Guid, reader, 16U, string8Encoding);
				}
				if (propertyType != PropertyType.MultiValueBinary)
				{
					goto IL_230;
				}
				return MDBEFCollection.ParseMultiValue<byte[]>(PropertyType.Binary, reader, 1U, string8Encoding);
			}
			uint count = reader.ReadUInt32();
			return reader.ReadBytes(count);
			IL_230:
			throw new NotSupportedException(string.Format("Property type not supported: {0}.", propertyType));
		}

		private static T[] ParseMultiValue<T>(PropertyType elementPropertyType, Reader reader, uint minimumElementSize, Encoding string8Encoding)
		{
			uint num = reader.ReadUInt32();
			reader.CheckBoundary(num, minimumElementSize);
			T[] array = new T[num];
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				array[num2] = (T)((object)MDBEFCollection.ParsePropertyValue(reader, elementPropertyType, string8Encoding));
				num2++;
			}
			return array;
		}

		private List<AnnotatedPropertyValue> listOfProperties = new List<AnnotatedPropertyValue>();
	}
}
