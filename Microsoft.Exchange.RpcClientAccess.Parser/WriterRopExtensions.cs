using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal static class WriterRopExtensions
	{
		public static void WriteSizedRestriction(this Writer writer, Restriction restriction, Encoding string8Encoding, WireFormatStyle wireFormatStyle)
		{
			long position = writer.Position;
			writer.WriteUInt16(0);
			if (restriction == null)
			{
				return;
			}
			restriction.Serialize(writer, string8Encoding, wireFormatStyle);
			writer.UpdateSize(position, writer.Position);
		}

		public static void WriteCountedStoreIdPairs(this Writer writer, StoreIdPair[] pairs)
		{
			if (pairs != null)
			{
				writer.WriteUInt32((uint)pairs.Length);
				uint num = 0U;
				while ((ulong)num < (ulong)((long)pairs.Length))
				{
					pairs[(int)((UIntPtr)num)].Serialize(writer);
					num += 1U;
				}
				return;
			}
			writer.WriteUInt32(0U);
		}

		public static void WriteCountedStoreIds(this Writer writer, StoreId[] folderIds)
		{
			if (folderIds != null)
			{
				writer.WriteUInt16((ushort)folderIds.Length);
				writer.WriteStoreIds(folderIds);
				return;
			}
			writer.WriteUInt16(0);
		}

		public static void WriteStoreIds(this Writer writer, StoreId[] elements)
		{
			if (elements != null)
			{
				for (int i = 0; i < elements.Length; i++)
				{
					elements[i].Serialize(writer);
				}
			}
		}

		public static void WriteUInt64Array(this Writer writer, ulong[] elements)
		{
			if (elements != null)
			{
				for (int i = 0; i < elements.Length; i++)
				{
					writer.WriteUInt64(elements[i]);
				}
			}
		}

		public static void WriteCountedStoreLongTermIds(this Writer writer, StoreLongTermId[] folderIds)
		{
			if (folderIds == null)
			{
				throw new ArgumentNullException("folderIds");
			}
			writer.WriteUInt16((ushort)folderIds.Length);
			foreach (StoreLongTermId storeLongTermId in folderIds)
			{
				storeLongTermId.Serialize(writer);
			}
		}

		public static void WriteCountedNamedProperties(this Writer writer, NamedProperty[] namedProperties)
		{
			if (namedProperties != null)
			{
				writer.WriteUInt16((ushort)namedProperties.Length);
				for (int i = 0; i < namedProperties.Length; i++)
				{
					namedProperties[i].Serialize(writer);
				}
				return;
			}
			writer.WriteUInt16(0);
		}

		public static void WriteCountAndPropertyTagArray(this Writer writer, PropertyTag[] propertyTags, FieldLength fieldLength)
		{
			if (propertyTags == null)
			{
				propertyTags = Array<PropertyTag>.Empty;
			}
			writer.WriteCountOrSize(propertyTags.Length, fieldLength);
			for (int i = 0; i < propertyTags.Length; i++)
			{
				writer.WritePropertyTag(propertyTags[i]);
			}
		}

		public static void WriteCountAndUnicodeStringList(this Writer writer, string[] values, StringFlags flags, FieldLength fieldLength)
		{
			if (values == null)
			{
				values = Array<string>.Empty;
			}
			writer.WriteCountOrSize(values.Length, fieldLength);
			for (int i = 0; i < values.Length; i++)
			{
				writer.WriteUnicodeString(values[i], flags);
			}
		}

		public static void WriteCountAndString8List(this Writer writer, string[] values, Encoding encoding, StringFlags flags, FieldLength fieldLength)
		{
			if (values == null)
			{
				values = Array<string>.Empty;
			}
			writer.WriteCountOrSize(values.Length, fieldLength);
			for (int i = 0; i < values.Length; i++)
			{
				writer.WriteString8(values[i], encoding, flags);
			}
		}

		public static void WriteSizedMessageReadStates(this Writer writer, MessageReadState[] messageReadStates)
		{
			long position = writer.Position;
			writer.WriteUInt16(0);
			for (int i = 0; i < messageReadStates.Length; i++)
			{
				messageReadStates[i].Serialize(writer);
			}
			writer.UpdateSize(position, writer.Position);
		}

		public static void WriteSizedLongTermIdRanges(this Writer writer, LongTermIdRange[] longTermIdRanges)
		{
			long position = writer.Position;
			writer.WriteUInt16(0);
			writer.WriteUInt32((uint)longTermIdRanges.Length);
			for (int i = 0; i < longTermIdRanges.Length; i++)
			{
				longTermIdRanges[i].Serialize(writer);
			}
			writer.UpdateSize(position, writer.Position);
		}

		public static void WriteFormattedString(this Writer writer, string value, bool useUnicode, Encoding string8Encoding)
		{
			if (value == null)
			{
				writer.WriteByte(0);
				return;
			}
			if (value.Length == 0)
			{
				writer.WriteByte(1);
				return;
			}
			if (!useUnicode)
			{
				writer.WriteByte(2);
				writer.WriteString8(value, string8Encoding, StringFlags.IncludeNull);
				return;
			}
			bool flag = ReducedUnicodeEncoding.IsStringConvertible(value);
			if (flag)
			{
				writer.WriteByte(3);
				writer.WriteString8(value, String8Encodings.ReducedUnicode, StringFlags.IncludeNull);
				return;
			}
			writer.WriteByte(4);
			writer.WriteUnicodeString(value, StringFlags.IncludeNull);
		}

		public static void WriteCountedPropertyIds(this Writer writer, PropertyId[] propertyIds)
		{
			if (propertyIds != null)
			{
				writer.WriteUInt16((ushort)propertyIds.Length);
				for (int i = 0; i < propertyIds.Length; i++)
				{
					writer.WriteUInt16((ushort)propertyIds[i]);
				}
				return;
			}
			writer.WriteUInt16(0);
		}

		public static void WriteCountedPropertyProblems(this Writer writer, PropertyProblem[] propertyProblems)
		{
			if (propertyProblems != null)
			{
				writer.WriteUInt16((ushort)propertyProblems.Length);
				for (int i = 0; i < propertyProblems.Length; i++)
				{
					propertyProblems[i].Serialize(writer);
				}
				return;
			}
			writer.WriteUInt16(0);
		}

		public static void WriteSizeAndPropertyRowList(this Writer writer, IList<PropertyRow> propertyRows, Encoding string8Encoding, WireFormatStyle wireFormatStyle)
		{
			if (propertyRows == null)
			{
				propertyRows = Array<PropertyRow>.Empty;
			}
			writer.WriteCountOrSize(propertyRows.Count, wireFormatStyle);
			foreach (PropertyRow propertyRow in propertyRows)
			{
				propertyRow.Serialize(writer, string8Encoding, wireFormatStyle);
			}
		}

		public static void WriteSizedModifyTableRows(this Writer writer, ICollection<ModifyTableRow> elements, Encoding string8Encoding)
		{
			if (elements == null)
			{
				writer.WriteInt16(0);
				return;
			}
			writer.WriteInt16((short)elements.Count);
			foreach (ModifyTableRow modifyTableRow in elements)
			{
				modifyTableRow.Serialize(writer, string8Encoding);
			}
		}

		public static void WriteCountAndPropertyValueList(this Writer writer, PropertyValue[] propertyValues, Encoding string8Encoding, WireFormatStyle wireFormatStyle)
		{
			if (propertyValues == null)
			{
				propertyValues = Array<PropertyValue>.Empty;
			}
			writer.WriteCountOrSize(propertyValues.Length, wireFormatStyle);
			for (int i = 0; i < propertyValues.Length; i++)
			{
				writer.WritePropertyValue(propertyValues[i], string8Encoding, wireFormatStyle);
			}
		}

		public static void WriteCountAndPropertyValueListList(this Writer writer, PropertyValue[][] propertyValues, Encoding string8Encoding, WireFormatStyle wireFormatStyle)
		{
			if (propertyValues == null)
			{
				propertyValues = Array<PropertyValue[]>.Empty;
			}
			writer.WriteCountOrSize(propertyValues.Length, wireFormatStyle);
			for (int i = 0; i < propertyValues.Length; i++)
			{
				writer.WriteCountAndPropertyValueList(propertyValues[i], string8Encoding, wireFormatStyle);
			}
		}

		public static void WriteSizeAndIntegerArray(this Writer writer, int[] integers, FieldLength fieldLength)
		{
			uint num = 0U;
			if (integers != null)
			{
				num = (uint)integers.Length;
			}
			if (fieldLength == FieldLength.WordSize)
			{
				writer.WriteUInt16((ushort)num);
			}
			else
			{
				writer.WriteUInt32(num);
			}
			int num2 = 0;
			while ((long)num2 < (long)((ulong)num))
			{
				writer.WriteInt32(integers[num2]);
				num2++;
			}
		}

		public static void WriteSizedRuleAction(this Writer writer, RuleAction action, Encoding string8Encoding)
		{
			if (action == null)
			{
				throw new ArgumentNullException("action");
			}
			long position = writer.Position;
			writer.WriteUInt16(0);
			action.Serialize(writer, string8Encoding);
			writer.UpdateSize(position, writer.Position);
		}

		public static void WriteSizedRuleActions(this Writer writer, RuleAction[] actions, Encoding string8Encoding)
		{
			writer.WriteUInt16((ushort)actions.Length);
			for (int i = 0; i < actions.Length; i++)
			{
				writer.WriteSizedRuleAction(actions[i], string8Encoding);
			}
		}

		public static uint WriteSizedBlock(this Writer writer, Action writeMethod)
		{
			long position = writer.Position;
			writer.WriteUInt32(0U);
			writeMethod();
			uint num = (uint)(writer.Position - position - 4L);
			long position2 = writer.Position;
			uint result;
			try
			{
				writer.Position = position;
				writer.WriteUInt32(num);
				result = num;
			}
			finally
			{
				writer.Position = position2;
			}
			return result;
		}
	}
}
