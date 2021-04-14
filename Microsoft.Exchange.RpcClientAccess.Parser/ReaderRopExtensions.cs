using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal static class ReaderRopExtensions
	{
		public static ErrorCode ReadErrorCode(this Reader reader)
		{
			return (ErrorCode)reader.ReadUInt32();
		}

		public static string ReadFormattedString(this Reader reader, Encoding string8Encoding)
		{
			StringFormatType stringFormatType = (StringFormatType)reader.ReadByte();
			switch (stringFormatType)
			{
			case StringFormatType.NotPresent:
				return null;
			case StringFormatType.EmptyString:
				return string.Empty;
			case StringFormatType.String8:
				return reader.ReadString8(string8Encoding, StringFlags.IncludeNull);
			case StringFormatType.ReduceUnicode:
				return reader.ReadString8(String8Encodings.ReducedUnicode, StringFlags.IncludeNull);
			case StringFormatType.FullUnicode:
				return reader.ReadUnicodeString(StringFlags.IncludeNull);
			default:
				throw new BufferParseException(string.Format("Unrecognized format type: {0}", stringFormatType));
			}
		}

		public static NamedProperty[] ReadNamedPropertyArray(this Reader reader, ushort length)
		{
			reader.CheckBoundary((uint)length, NamedProperty.MinimumSize);
			NamedProperty[] array = new NamedProperty[(int)length];
			for (int i = 0; i < (int)length; i++)
			{
				array[i] = NamedProperty.Parse(reader);
			}
			return array;
		}

		public static Restriction ReadSizeAndRestriction(this Reader reader, WireFormatStyle wireFormatStyle)
		{
			uint num = (uint)reader.ReadUInt16();
			if (num == 0U)
			{
				return null;
			}
			long position = reader.Position;
			Restriction result = Restriction.Parse(reader, wireFormatStyle);
			long position2 = reader.Position;
			if ((uint)(position2 - position) != num)
			{
				throw new BufferParseException("Restriction wasn't the size reported");
			}
			return result;
		}

		public static StoreIdPair[] ReadSizeAndStoreIdPairArray(this Reader reader)
		{
			uint num = reader.ReadUInt32();
			if (num == 0U)
			{
				return null;
			}
			StoreIdPair[] array = new StoreIdPair[num];
			for (uint num2 = 0U; num2 < num; num2 += 1U)
			{
				array[(int)((UIntPtr)num2)] = StoreIdPair.Parse(reader);
			}
			return array;
		}

		public static StoreId[] ReadSizeAndStoreIdArray(this Reader reader)
		{
			uint length = (uint)reader.ReadUInt16();
			return reader.ReadStoreIdArray(length);
		}

		public static StoreId[] ReadStoreIdArray(this Reader reader, uint length)
		{
			if (length == 0U)
			{
				return null;
			}
			reader.CheckBoundary(length, 8U);
			StoreId[] array = new StoreId[length];
			for (uint num = 0U; num < length; num += 1U)
			{
				array[(int)((UIntPtr)num)] = StoreId.Parse(reader);
			}
			return array;
		}

		public static ulong[] ReadUInt64Array(this Reader reader, uint length)
		{
			if (length == 0U)
			{
				return null;
			}
			reader.CheckBoundary(length, 8U);
			ulong[] array = new ulong[length];
			for (uint num = 0U; num < length; num += 1U)
			{
				array[(int)((UIntPtr)num)] = reader.ReadUInt64();
			}
			return array;
		}

		public static StoreLongTermId[] ReadSizeAndStoreLongTermIdArray(this Reader reader)
		{
			int num = (int)reader.ReadUInt16();
			if (num == 0)
			{
				return Array<StoreLongTermId>.Empty;
			}
			reader.CheckBoundary((uint)num, (uint)StoreLongTermId.Size);
			StoreLongTermId[] array = new StoreLongTermId[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = StoreLongTermId.Parse(reader);
			}
			return array;
		}

		public static NamedProperty[] ReadSizeAndNamedPropertyArray(this Reader reader)
		{
			ushort num = reader.ReadUInt16();
			if (num == 0)
			{
				return Array<NamedProperty>.Empty;
			}
			return reader.ReadNamedPropertyArray(num);
		}

		public static PropertyId[] ReadSizeAndPropertyIdArray(this Reader reader)
		{
			int num = (int)reader.ReadUInt16();
			if (num == 0)
			{
				return Array<PropertyId>.Empty;
			}
			reader.CheckBoundary((uint)num, 2U);
			PropertyId[] array = new PropertyId[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = (PropertyId)reader.ReadUInt16();
			}
			return array;
		}

		public static PropertyProblem[] ReadSizeAndPropertyProblemArray(this Reader reader)
		{
			int num = (int)reader.ReadUInt16();
			if (num == 0)
			{
				return Array<PropertyProblem>.Empty;
			}
			reader.CheckBoundary((uint)num, PropertyProblem.MinimumSize);
			PropertyProblem[] array = new PropertyProblem[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = PropertyProblem.Parse(reader);
			}
			return array;
		}

		public static List<PropertyRow> ReadSizeAndPropertyRowList(this Reader reader, PropertyTag[] columns, Encoding string8Encoding, WireFormatStyle wireFormatStyle)
		{
			uint num = reader.ReadCountOrSize(wireFormatStyle);
			reader.CheckBoundary(num, 4U);
			List<PropertyRow> list = new List<PropertyRow>((int)num);
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				PropertyRow item = PropertyRow.Parse(reader, columns, wireFormatStyle);
				item.ResolveString8Values(string8Encoding);
				list.Add(item);
				num2++;
			}
			return list;
		}

		public static PropertyTag[] ReadCountAndPropertyTagArray(this Reader reader, FieldLength fieldLength)
		{
			uint num = reader.ReadCountOrSize(fieldLength);
			if (num == 0U)
			{
				return Array<PropertyTag>.Empty;
			}
			reader.CheckBoundary(num, 4U);
			PropertyTag[] array = new PropertyTag[num];
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				array[num2] = reader.ReadPropertyTag();
				num2++;
			}
			return array;
		}

		public static string[] ReadCountAndUnicodeStringList(this Reader reader, StringFlags flags, FieldLength fieldLength)
		{
			uint num = reader.ReadCountOrSize(fieldLength);
			if (num == 0U)
			{
				return Array<string>.Empty;
			}
			reader.CheckBoundary(num, ReaderRopExtensions.GetMinimumStringEncodingLength(flags, 2U));
			string[] array = new string[num];
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				array[num2] = reader.ReadUnicodeString(flags);
				num2++;
			}
			return array;
		}

		public static string[] ReadCountAndString8List(this Reader reader, Encoding encoding, StringFlags flags, FieldLength fieldLength)
		{
			uint num = reader.ReadCountOrSize(fieldLength);
			if (num == 0U)
			{
				return Array<string>.Empty;
			}
			reader.CheckBoundary(num, ReaderRopExtensions.GetMinimumStringEncodingLength(flags, 1U));
			string[] array = new string[num];
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				array[num2] = reader.ReadString8(encoding, flags);
				num2++;
			}
			return array;
		}

		public static MessageReadState[] ReadSizeAndMessageReadStateArray(this Reader reader)
		{
			uint num = (uint)reader.ReadUInt16();
			long num2 = (long)((ulong)num + (ulong)reader.Position);
			List<MessageReadState> list = new List<MessageReadState>((int)(num / MessageReadState.MinimumSize));
			while (reader.Position < num2)
			{
				list.Add(MessageReadState.Parse(reader));
			}
			return list.ToArray();
		}

		public static LongTermIdRange[] ReadSizeAndLongTermIdRangeArray(this Reader reader)
		{
			uint num = (uint)reader.ReadUInt16();
			long num2 = (long)((ulong)num + (ulong)reader.Position);
			uint num3 = reader.ReadUInt32();
			LongTermIdRange[] array;
			if (num3 == 0U)
			{
				array = Array<LongTermIdRange>.Empty;
			}
			else
			{
				reader.CheckBoundary(num3, 4U);
				array = new LongTermIdRange[num3];
				int num4 = 0;
				while ((long)num4 < (long)((ulong)num3))
				{
					array[num4] = LongTermIdRange.Parse(reader);
					num4++;
				}
			}
			if ((ulong)((uint)num2) != (ulong)reader.Position)
			{
				throw new BufferParseException("LongTermIdRange[] wasn't the size reported.");
			}
			return array;
		}

		public static PropertyValue[] ReadCountAndPropertyValueList(this Reader reader, WireFormatStyle wireFormatStyle)
		{
			return reader.ReadCountAndPropertyValueList(null, wireFormatStyle);
		}

		public static PropertyValue[] ReadCountAndPropertyValueList(this Reader reader, Encoding string8Encoding, WireFormatStyle wireFormatStyle)
		{
			uint num = reader.ReadCountOrSize(wireFormatStyle);
			if (num == 0U)
			{
				return Array<PropertyValue>.Empty;
			}
			reader.CheckBoundary(num, 4U);
			PropertyValue[] array = new PropertyValue[num];
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				array[num2] = reader.ReadPropertyValue(wireFormatStyle);
				if (string8Encoding != null)
				{
					array[num2].ResolveString8Values(string8Encoding);
				}
				num2++;
			}
			return array;
		}

		public static PropertyValue[][] ReadCountAndPropertyValueListList(this Reader reader, Encoding string8Encoding, WireFormatStyle wireFormatStyle)
		{
			uint num = reader.ReadCountOrSize(wireFormatStyle);
			if (num == 0U)
			{
				return Array<PropertyValue[]>.Empty;
			}
			PropertyValue[][] array = new PropertyValue[num][];
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				array[num2] = reader.ReadCountAndPropertyValueList(string8Encoding, wireFormatStyle);
				num2++;
			}
			return array;
		}

		public static int[] ReadSizeAndIntegerArray(this Reader reader, FieldLength fieldLength)
		{
			uint num;
			if (fieldLength == FieldLength.WordSize)
			{
				num = (uint)reader.ReadUInt16();
			}
			else
			{
				num = reader.ReadUInt32();
			}
			if (num == 0U)
			{
				return Array<int>.Empty;
			}
			reader.CheckBoundary(num, 4U);
			int[] array = new int[num];
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				array[num2] = reader.ReadInt32();
				num2++;
			}
			return array;
		}

		public static ModifyTableRow[] ReadSizeAndModifyTableRowArray(this Reader reader)
		{
			int num = (int)reader.ReadUInt16();
			if (num == 0)
			{
				return Array<ModifyTableRow>.Empty;
			}
			reader.CheckBoundary((uint)num, 3U);
			ModifyTableRow[] array = new ModifyTableRow[num];
			for (int i = 0; i < num; i++)
			{
				array[i] = ModifyTableRow.Parse(reader);
			}
			return array;
		}

		private static uint GetMinimumStringEncodingLength(StringFlags flags, uint terminatorLength)
		{
			if ((flags & StringFlags.Sized) == StringFlags.Sized)
			{
				return 1U;
			}
			if ((flags & StringFlags.Sized16) == StringFlags.Sized16)
			{
				return 2U;
			}
			if ((flags & StringFlags.Sized32) == StringFlags.Sized32)
			{
				return 4U;
			}
			return terminatorLength;
		}
	}
}
