using System;
using System.Text;
using Microsoft.Exchange.Nspi;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal static class WriterMapiHttpExtensions
	{
		public static void WriteNspiState(this Writer writer, NspiState value)
		{
			writer.WriteBool(value != null);
			if (value != null)
			{
				writer.WriteInt32(value.SortType);
				writer.WriteInt32(value.ContainerId);
				writer.WriteInt32(value.CurrentRecord);
				writer.WriteInt32(value.Delta);
				writer.WriteInt32(value.Position);
				writer.WriteInt32(value.TotalRecords);
				writer.WriteInt32(value.CodePage);
				writer.WriteInt32(value.TemplateLocale);
				writer.WriteInt32(value.SortLocale);
			}
		}

		public static void WriteNullableInt32(this Writer writer, int? value)
		{
			bool flag = value != null;
			writer.WriteBool(flag);
			if (flag)
			{
				writer.WriteInt32(value.Value);
			}
		}

		public static void WriteNullableUInt32(this Writer writer, uint? value)
		{
			bool flag = value != null;
			writer.WriteBool(flag);
			if (flag)
			{
				writer.WriteUInt32(value.Value);
			}
		}

		public static void WriteNullableCountAndString8List(this Writer writer, string[] values, Encoding encoding, StringFlags flags, FieldLength fieldLength)
		{
			bool flag = values != null;
			writer.WriteBool(flag);
			if (flag)
			{
				writer.WriteCountAndString8List(values, encoding, flags, fieldLength);
			}
		}

		public static void WriteNullableSizeAndIntegerArray(this Writer writer, int[] integers, FieldLength fieldLength)
		{
			bool flag = integers != null;
			writer.WriteBool(flag);
			if (flag)
			{
				writer.WriteSizeAndIntegerArray(integers, fieldLength);
			}
		}

		public static void WriteNullableRestriction(this Writer writer, Restriction restriction, Encoding string8Encoding)
		{
			bool flag = restriction != null;
			writer.WriteBool(flag);
			if (flag)
			{
				restriction.Serialize(writer, string8Encoding, WireFormatStyle.Nspi);
			}
		}

		public static void WriteNullableNamedProperty(this Writer writer, NamedProperty namedProperty)
		{
			bool flag = namedProperty != null;
			writer.WriteBool(flag);
			if (flag)
			{
				writer.WriteGuid(namedProperty.Guid);
				writer.WriteUInt32(namedProperty.Id);
			}
		}

		public static void WriteNullableCountAndPropertyTagArray(this Writer writer, PropertyTag[] propertyTags, FieldLength fieldLength)
		{
			bool flag = propertyTags != null;
			writer.WriteBool(flag);
			if (flag)
			{
				writer.WriteCountAndPropertyTagArray(propertyTags, fieldLength);
			}
		}

		public static void WriteNullableCountAndPropertyValueList(this Writer writer, PropertyValue[] propertyValues, Encoding string8Encoding, WireFormatStyle wireFormatStyle)
		{
			bool flag = propertyValues != null;
			writer.WriteBool(flag);
			if (flag)
			{
				writer.WriteCountAndPropertyValueList(propertyValues, string8Encoding, wireFormatStyle);
			}
		}

		public static void WriteNullableCountAndPropertyValueListList(this Writer writer, PropertyValue[][] propertyValues, Encoding string8Encoding, WireFormatStyle wireFormatStyle)
		{
			bool flag = propertyValues != null;
			writer.WriteBool(flag);
			if (flag)
			{
				writer.WriteCountAndPropertyValueListList(propertyValues, string8Encoding, wireFormatStyle);
			}
		}

		public static void WriteNullableAsciiString(this Writer writer, string value)
		{
			bool flag = value != null;
			writer.WriteBool(flag);
			if (flag)
			{
				writer.WriteAsciiString(value, StringFlags.IncludeNull);
			}
		}

		public static void WriteNullableByteArrayList(this Writer writer, byte[][] values, FieldLength lengthSize)
		{
			bool flag = values != null;
			writer.WriteBool(flag);
			if (flag)
			{
				writer.WriteCountAndByteArrayList(values, lengthSize);
			}
		}

		public static void WriteNullableCountAndUnicodeStringList(this Writer writer, string[] values, StringFlags flags, FieldLength fieldLength)
		{
			bool flag = values != null;
			writer.WriteBool(flag);
			if (flag)
			{
				writer.WriteCountAndUnicodeStringList(values, flags, fieldLength);
			}
		}
	}
}
