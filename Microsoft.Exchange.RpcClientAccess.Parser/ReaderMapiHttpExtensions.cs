using System;
using System.Text;
using Microsoft.Exchange.Nspi;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal static class ReaderMapiHttpExtensions
	{
		public static NspiState ReadNspiState(this Reader reader)
		{
			if (reader.ReadBool())
			{
				int sortType = reader.ReadInt32();
				int containerId = reader.ReadInt32();
				int currentRecord = reader.ReadInt32();
				int delta = reader.ReadInt32();
				int position = reader.ReadInt32();
				int totalRecords = reader.ReadInt32();
				int codePage = reader.ReadInt32();
				int templateLocale = reader.ReadInt32();
				int sortLocale = reader.ReadInt32();
				return new NspiState(sortType, containerId, currentRecord, delta, position, totalRecords, codePage, templateLocale, sortLocale);
			}
			return null;
		}

		public static int? ReadNullableInt32(this Reader reader)
		{
			if (reader.ReadBool())
			{
				return new int?(reader.ReadInt32());
			}
			return null;
		}

		public static uint? ReadNullableUInt32(this Reader reader)
		{
			if (reader.ReadBool())
			{
				return new uint?(reader.ReadUInt32());
			}
			return null;
		}

		public static string[] ReadNullableCountAndString8List(this Reader reader, Encoding encoding, StringFlags flags, FieldLength fieldLength)
		{
			if (reader.ReadBool())
			{
				return reader.ReadCountAndString8List(encoding, flags, fieldLength);
			}
			return null;
		}

		public static int[] ReadNullableSizeAndIntegerArray(this Reader reader, FieldLength fieldLength)
		{
			if (reader.ReadBool())
			{
				return reader.ReadSizeAndIntegerArray(fieldLength);
			}
			return null;
		}

		public static Restriction ReadNullableRestriction(this Reader reader, Encoding string8Encoding)
		{
			Restriction restriction = null;
			if (reader.ReadBool())
			{
				restriction = Restriction.Parse(reader, WireFormatStyle.Nspi);
				restriction.ResolveString8Values(string8Encoding);
			}
			return restriction;
		}

		public static NamedProperty ReadNullableNamedProperty(this Reader reader)
		{
			if (reader.ReadBool())
			{
				Guid guid = reader.ReadGuid();
				uint id = reader.ReadUInt32();
				return new NamedProperty(guid, id);
			}
			return null;
		}

		public static PropertyTag[] ReadNullableCountAndPropertyTagArray(this Reader reader, FieldLength fieldLength)
		{
			if (reader.ReadBool())
			{
				return reader.ReadCountAndPropertyTagArray(fieldLength);
			}
			return null;
		}

		public static PropertyValue[] ReadNullableCountAndPropertyValueList(this Reader reader, Encoding string8Encoding, WireFormatStyle wireFormatStyle)
		{
			if (reader.ReadBool())
			{
				return reader.ReadCountAndPropertyValueList(string8Encoding, wireFormatStyle);
			}
			return null;
		}

		public static PropertyValue[][] ReadNullableCountAndPropertyValueListList(this Reader reader, Encoding string8Encoding, WireFormatStyle wireFormatStyle)
		{
			if (reader.ReadBool())
			{
				return reader.ReadCountAndPropertyValueListList(string8Encoding, wireFormatStyle);
			}
			return null;
		}

		public static string ReadNullableAsciiString(this Reader reader, StringFlags flags)
		{
			if (reader.ReadBool())
			{
				return reader.ReadAsciiString(flags);
			}
			return null;
		}

		public static byte[][] ReadNullableCountAndByteArrayList(this Reader reader, FieldLength fieldLength)
		{
			if (reader.ReadBool())
			{
				return reader.ReadCountAndByteArrayList(fieldLength);
			}
			return null;
		}

		public static string[] ReadNullableCountAndUnicodeStringList(this Reader reader, StringFlags flags, FieldLength fieldLength)
		{
			if (reader.ReadBool())
			{
				return reader.ReadCountAndUnicodeStringList(flags, fieldLength);
			}
			return null;
		}
	}
}
