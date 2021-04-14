using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Common
{
	public static class SerializedValue
	{
		public static object BoxedTrue
		{
			get
			{
				return SerializedValue.boxedTrue;
			}
		}

		public static object BoxedFalse
		{
			get
			{
				return SerializedValue.boxedFalse;
			}
		}

		public static bool TryParse(ValueFormat format, byte[] buffer, int offset, out object value)
		{
			value = null;
			if (!ParseSerialize.CheckOffsetLength(buffer, offset, 1))
			{
				return false;
			}
			ValueFormat valueFormat = (ValueFormat)buffer[offset++];
			return (byte)(format & ValueFormat.TypeMask) == (byte)(valueFormat & ValueFormat.TypeMask) && SerializedValue.TryParseValue(valueFormat, buffer, ref offset, ref value);
		}

		public static bool TryParse(byte[] buffer, int offset, out object value)
		{
			return SerializedValue.TryParse(buffer, ref offset, out value);
		}

		public static bool TryParse(byte[] buffer, ref int offset, out object value)
		{
			value = null;
			if (!ParseSerialize.CheckOffsetLength(buffer, offset, 1))
			{
				return false;
			}
			ValueFormat format = (ValueFormat)buffer[offset++];
			return SerializedValue.TryParseValue(format, buffer, ref offset, ref value);
		}

		public static bool TryGetSize(ValueFormat format, byte[] buffer, int offset, out int size)
		{
			if (!ParseSerialize.CheckOffsetLength(buffer, offset, 1))
			{
				size = 0;
				return false;
			}
			ValueFormat valueFormat = (ValueFormat)buffer[offset++];
			if ((byte)(format & ValueFormat.TypeMask) != (byte)(valueFormat & ValueFormat.TypeMask))
			{
				size = 0;
				return false;
			}
			return SerializedValue.TryGetValueSize(valueFormat, buffer, offset, out size);
		}

		public static bool TryGetSize(byte[] buffer, int offset, out int size)
		{
			return SerializedValue.TryGetSize(buffer, ref offset, out size);
		}

		public static bool TryGetSize(byte[] buffer, ref int offset, out int size)
		{
			if (!ParseSerialize.CheckOffsetLength(buffer, offset, 1))
			{
				size = 0;
				return false;
			}
			ValueFormat format = (ValueFormat)buffer[offset++];
			return SerializedValue.TryGetValueSize(format, buffer, offset, out size);
		}

		public static object Parse(byte[] buffer, int offset)
		{
			return SerializedValue.Parse(buffer, ref offset);
		}

		public static object Parse(byte[] buffer, ref int offset)
		{
			object result;
			if (!SerializedValue.TryParse(buffer, ref offset, out result))
			{
				throw new InvalidSerializedFormatException("value parsing error");
			}
			return result;
		}

		public static object Parse(byte[] buffer)
		{
			object result;
			if (!SerializedValue.TryParse(buffer, 0, out result))
			{
				throw new InvalidSerializedFormatException("value parsing error");
			}
			return result;
		}

		public static bool TryParseList(byte[] buffer, ref int offset, out IList<object> valueList)
		{
			List<object> list = null;
			while (offset != buffer.Length)
			{
				object item;
				if (!SerializedValue.TryParse(buffer, ref offset, out item))
				{
					valueList = null;
					return false;
				}
				if (list == null)
				{
					list = new List<object>(4);
				}
				list.Add(item);
			}
			valueList = list;
			return true;
		}

		public static IList<object> ParseList(byte[] buffer, ref int offset)
		{
			IList<object> result;
			if (!SerializedValue.TryParseList(buffer, ref offset, out result))
			{
				throw new InvalidSerializedFormatException("value parsing error");
			}
			return result;
		}

		public static bool ParseBoolean(byte[] buffer, ref int offset)
		{
			ValueFormat format = SerializedValue.ParseAndAssertFormat(buffer, ref offset, ValueFormat.Boolean);
			bool result;
			if (!SerializedValue.TryParseBooleanValue(format, buffer, ref offset, out result))
			{
				throw new InvalidSerializedFormatException("value parsing error");
			}
			return result;
		}

		public static short ParseInt16(byte[] buffer, ref int offset)
		{
			ValueFormat format = SerializedValue.ParseAndAssertFormat(buffer, ref offset, ValueFormat.Int16);
			short result;
			if (!SerializedValue.TryParseInt16Value(format, buffer, ref offset, out result))
			{
				throw new InvalidSerializedFormatException("value parsing error");
			}
			return result;
		}

		public static int ParseInt32(byte[] buffer, ref int offset)
		{
			ValueFormat format = SerializedValue.ParseAndAssertFormat(buffer, ref offset, ValueFormat.Int32);
			int result;
			if (!SerializedValue.TryParseInt32Value(format, buffer, ref offset, out result))
			{
				throw new InvalidSerializedFormatException("value parsing error");
			}
			return result;
		}

		public static long ParseInt64(byte[] buffer, ref int offset)
		{
			ValueFormat format = SerializedValue.ParseAndAssertFormat(buffer, ref offset, ValueFormat.Int64);
			long result;
			if (!SerializedValue.TryParseInt64Value(format, buffer, ref offset, out result))
			{
				throw new InvalidSerializedFormatException("value parsing error");
			}
			return result;
		}

		public static float ParseSingle(byte[] buffer, ref int offset)
		{
			ValueFormat format = SerializedValue.ParseAndAssertFormat(buffer, ref offset, ValueFormat.Single);
			float result;
			if (!SerializedValue.TryParseSingleValue(format, buffer, ref offset, out result))
			{
				throw new InvalidSerializedFormatException("value parsing error");
			}
			return result;
		}

		public static double ParseDouble(byte[] buffer, ref int offset)
		{
			ValueFormat format = SerializedValue.ParseAndAssertFormat(buffer, ref offset, ValueFormat.Double);
			double result;
			if (!SerializedValue.TryParseDoubleValue(format, buffer, ref offset, out result))
			{
				throw new InvalidSerializedFormatException("value parsing error");
			}
			return result;
		}

		public static DateTime ParseDateTime(byte[] buffer, ref int offset)
		{
			ValueFormat format = SerializedValue.ParseAndAssertFormat(buffer, ref offset, ValueFormat.DateTime);
			DateTime result;
			if (!SerializedValue.TryParseDateTimeValue(format, buffer, ref offset, out result))
			{
				throw new InvalidSerializedFormatException("value parsing error");
			}
			return result;
		}

		public static Guid ParseGuid(byte[] buffer, ref int offset)
		{
			ValueFormat format = SerializedValue.ParseAndAssertFormat(buffer, ref offset, ValueFormat.Guid);
			Guid result;
			if (!SerializedValue.TryParseGuidValue(format, buffer, ref offset, out result))
			{
				throw new InvalidSerializedFormatException("value parsing error");
			}
			return result;
		}

		public static byte[] ParseBinary(byte[] buffer, ref int offset)
		{
			ValueFormat valueFormat = SerializedValue.ParseAndAssertFormatOrNull(buffer, ref offset, ValueFormat.Binary);
			if (valueFormat == ValueFormat.FormatModifierShift)
			{
				return null;
			}
			byte[] result;
			if (!SerializedValue.TryParseBinaryValue(valueFormat, buffer, ref offset, out result))
			{
				throw new InvalidSerializedFormatException("value parsing error");
			}
			return result;
		}

		public static string ParseString(byte[] buffer, ref int offset)
		{
			ValueFormat valueFormat = SerializedValue.ParseAndAssertFormatOrNull(buffer, ref offset, ValueFormat.String);
			if (valueFormat == ValueFormat.FormatModifierShift)
			{
				return null;
			}
			string result;
			if (!SerializedValue.TryParseStringValue(valueFormat, buffer, ref offset, out result))
			{
				throw new InvalidSerializedFormatException("value parsing error");
			}
			return result;
		}

		public static short[] ParseMVInt16(byte[] buffer, ref int offset)
		{
			ValueFormat valueFormat = SerializedValue.ParseAndAssertFormatOrNull(buffer, ref offset, ValueFormat.MVInt16);
			if (valueFormat == ValueFormat.FormatModifierShift)
			{
				return null;
			}
			short[] result;
			if (!SerializedValue.TryParseMVInt16Value(valueFormat, buffer, ref offset, out result))
			{
				throw new InvalidSerializedFormatException("value parsing error");
			}
			return result;
		}

		public static int[] ParseMVInt32(byte[] buffer, ref int offset)
		{
			ValueFormat valueFormat = SerializedValue.ParseAndAssertFormatOrNull(buffer, ref offset, ValueFormat.MVInt32);
			if (valueFormat == ValueFormat.FormatModifierShift)
			{
				return null;
			}
			int[] result;
			if (!SerializedValue.TryParseMVInt32Value(valueFormat, buffer, ref offset, out result))
			{
				throw new InvalidSerializedFormatException("value parsing error");
			}
			return result;
		}

		public static long[] ParseMVInt64(byte[] buffer, ref int offset)
		{
			ValueFormat valueFormat = SerializedValue.ParseAndAssertFormatOrNull(buffer, ref offset, ValueFormat.MVInt64);
			if (valueFormat == ValueFormat.FormatModifierShift)
			{
				return null;
			}
			long[] result;
			if (!SerializedValue.TryParseMVInt64Value(valueFormat, buffer, ref offset, out result))
			{
				throw new InvalidSerializedFormatException("value parsing error");
			}
			return result;
		}

		public static float[] ParseMVSingle(byte[] buffer, ref int offset)
		{
			ValueFormat valueFormat = SerializedValue.ParseAndAssertFormatOrNull(buffer, ref offset, ValueFormat.MVSingle);
			if (valueFormat == ValueFormat.FormatModifierShift)
			{
				return null;
			}
			float[] result;
			if (!SerializedValue.TryParseMVSingleValue(valueFormat, buffer, ref offset, out result))
			{
				throw new InvalidSerializedFormatException("value parsing error");
			}
			return result;
		}

		public static double[] ParseMVDouble(byte[] buffer, ref int offset)
		{
			ValueFormat valueFormat = SerializedValue.ParseAndAssertFormatOrNull(buffer, ref offset, ValueFormat.MVDouble);
			if (valueFormat == ValueFormat.FormatModifierShift)
			{
				return null;
			}
			double[] result;
			if (!SerializedValue.TryParseMVDoubleValue(valueFormat, buffer, ref offset, out result))
			{
				throw new InvalidSerializedFormatException("value parsing error");
			}
			return result;
		}

		public static DateTime[] ParseMVDateTime(byte[] buffer, ref int offset)
		{
			ValueFormat valueFormat = SerializedValue.ParseAndAssertFormatOrNull(buffer, ref offset, ValueFormat.MVDateTime);
			if (valueFormat == ValueFormat.FormatModifierShift)
			{
				return null;
			}
			DateTime[] result;
			if (!SerializedValue.TryParseMVDateTimeValue(valueFormat, buffer, ref offset, out result))
			{
				throw new InvalidSerializedFormatException("value parsing error");
			}
			return result;
		}

		public static Guid[] ParseMVGuid(byte[] buffer, ref int offset)
		{
			ValueFormat valueFormat = SerializedValue.ParseAndAssertFormatOrNull(buffer, ref offset, ValueFormat.MVGuid);
			if (valueFormat == ValueFormat.FormatModifierShift)
			{
				return null;
			}
			Guid[] result;
			if (!SerializedValue.TryParseMVGuidValue(valueFormat, buffer, ref offset, out result))
			{
				throw new InvalidSerializedFormatException("value parsing error");
			}
			return result;
		}

		public static byte[][] ParseMVBinary(byte[] buffer, ref int offset)
		{
			ValueFormat valueFormat = SerializedValue.ParseAndAssertFormatOrNull(buffer, ref offset, ValueFormat.MVBinary);
			if (valueFormat == ValueFormat.FormatModifierShift)
			{
				return null;
			}
			byte[][] result;
			if (!SerializedValue.TryParseMVBinaryValue(valueFormat, buffer, ref offset, out result))
			{
				throw new InvalidSerializedFormatException("value parsing error");
			}
			return result;
		}

		public static string[] ParseMVString(byte[] buffer, ref int offset)
		{
			ValueFormat valueFormat = SerializedValue.ParseAndAssertFormatOrNull(buffer, ref offset, ValueFormat.MVString);
			if (valueFormat == ValueFormat.FormatModifierShift)
			{
				return null;
			}
			string[] result;
			if (!SerializedValue.TryParseMVStringValue(valueFormat, buffer, ref offset, out result))
			{
				throw new InvalidSerializedFormatException("value parsing error");
			}
			return result;
		}

		public static object GetBoxedInt32(int value)
		{
			if (0 <= value && value < SerializedValue.boxedInts.Length)
			{
				if (SerializedValue.boxedInts[value] == null)
				{
					SerializedValue.boxedInts[value] = value;
				}
				return SerializedValue.boxedInts[value];
			}
			return value;
		}

		private static ValueFormat ParseAndAssertFormatOrNull(byte[] buffer, ref int offset, ValueFormat expectedFormat)
		{
			if (!ParseSerialize.CheckOffsetLength(buffer, offset, 1))
			{
				throw new InvalidSerializedFormatException("value parsing error");
			}
			ValueFormat valueFormat = (ValueFormat)buffer[offset++];
			if ((valueFormat & ValueFormat.TypeMask) != expectedFormat && valueFormat != ValueFormat.FormatModifierShift)
			{
				throw new InvalidSerializedFormatException("value parsing error - unexpected value format");
			}
			return valueFormat;
		}

		private static ValueFormat ParseAndAssertFormat(byte[] buffer, ref int offset, ValueFormat expectedFormat)
		{
			if (!ParseSerialize.CheckOffsetLength(buffer, offset, 1))
			{
				throw new InvalidSerializedFormatException("value parsing error");
			}
			ValueFormat valueFormat = (ValueFormat)buffer[offset++];
			if ((valueFormat & ValueFormat.TypeMask) != expectedFormat)
			{
				throw new InvalidSerializedFormatException("value parsing error - unexpeced value format");
			}
			return valueFormat;
		}

		private static bool TryParseValue(ValueFormat format, byte[] buffer, ref int offset, ref object value)
		{
			ValueReference valueReference;
			if (format <= ValueFormat.Double)
			{
				if (format <= (ValueFormat)9)
				{
					if (format == ValueFormat.FormatModifierShift)
					{
						return true;
					}
					switch (format)
					{
					case ValueFormat.Boolean:
					case (ValueFormat)9:
					{
						bool flag;
						if (SerializedValue.TryParseBooleanValue(format, buffer, ref offset, out flag))
						{
							value = (flag ? SerializedValue.BoxedTrue : SerializedValue.BoxedFalse);
							return true;
						}
						break;
					}
					}
				}
				else
				{
					switch (format)
					{
					case ValueFormat.Int16:
					case (ValueFormat)17:
					case (ValueFormat)18:
					{
						short num;
						if (SerializedValue.TryParseInt16Value(format, buffer, ref offset, out num))
						{
							value = num;
							return true;
						}
						break;
					}
					case (ValueFormat)19:
					case (ValueFormat)20:
					case (ValueFormat)21:
					case (ValueFormat)22:
					case (ValueFormat)23:
					case (ValueFormat)28:
					case (ValueFormat)29:
					case (ValueFormat)30:
					case (ValueFormat)31:
					case (ValueFormat)37:
					case (ValueFormat)38:
					case (ValueFormat)39:
						break;
					case ValueFormat.Int32:
					case (ValueFormat)25:
					case (ValueFormat)26:
					case (ValueFormat)27:
					{
						int num2;
						if (SerializedValue.TryParseInt32Value(format, buffer, ref offset, out num2))
						{
							value = num2;
							return true;
						}
						break;
					}
					case ValueFormat.Int64:
					case (ValueFormat)33:
					case (ValueFormat)34:
					case (ValueFormat)35:
					case (ValueFormat)36:
					{
						long num3;
						if (SerializedValue.TryParseInt64Value(format, buffer, ref offset, out num3))
						{
							value = num3;
							return true;
						}
						break;
					}
					case ValueFormat.Single:
					{
						float num4;
						if (SerializedValue.TryParseSingleValue(format, buffer, ref offset, out num4))
						{
							value = num4;
							return true;
						}
						break;
					}
					default:
						if (format == ValueFormat.Double)
						{
							double num5;
							if (SerializedValue.TryParseDoubleValue(format, buffer, ref offset, out num5))
							{
								value = num5;
								return true;
							}
						}
						break;
					}
				}
			}
			else if (format <= (ValueFormat)83)
			{
				DateTime dateTime;
				if (format != ValueFormat.DateTime)
				{
					switch (format)
					{
					case ValueFormat.Guid:
					{
						Guid guid;
						if (SerializedValue.TryParseGuidValue(format, buffer, ref offset, out guid))
						{
							value = guid;
							return true;
						}
						break;
					}
					case ValueFormat.String:
					case (ValueFormat)73:
					case (ValueFormat)74:
					case (ValueFormat)75:
					case (ValueFormat)77:
					case (ValueFormat)78:
					case (ValueFormat)79:
					{
						string text;
						if (SerializedValue.TryParseStringValue(format, buffer, ref offset, out text))
						{
							value = text;
							return true;
						}
						break;
					}
					case ValueFormat.Binary:
					case (ValueFormat)81:
					case (ValueFormat)82:
					case (ValueFormat)83:
					{
						byte[] array;
						if (SerializedValue.TryParseBinaryValue(format, buffer, ref offset, out array))
						{
							value = array;
							return true;
						}
						break;
					}
					}
				}
				else if (SerializedValue.TryParseDateTimeValue(format, buffer, ref offset, out dateTime))
				{
					value = dateTime;
					return true;
				}
			}
			else if (format != ValueFormat.Reference && format != (ValueFormat)124)
			{
				switch (format)
				{
				case ValueFormat.MVInt16:
				case (ValueFormat)145:
				case (ValueFormat)146:
				case (ValueFormat)147:
				{
					short[] array2;
					if (SerializedValue.TryParseMVInt16Value(format, buffer, ref offset, out array2))
					{
						value = array2;
						return true;
					}
					break;
				}
				case ValueFormat.MVInt32:
				case (ValueFormat)153:
				case (ValueFormat)154:
				case (ValueFormat)155:
				{
					int[] array3;
					if (SerializedValue.TryParseMVInt32Value(format, buffer, ref offset, out array3))
					{
						value = array3;
						return true;
					}
					break;
				}
				case ValueFormat.MVInt64:
				case (ValueFormat)161:
				case (ValueFormat)162:
				case (ValueFormat)163:
				{
					long[] array4;
					if (SerializedValue.TryParseMVInt64Value(format, buffer, ref offset, out array4))
					{
						value = array4;
						return true;
					}
					break;
				}
				case ValueFormat.MVSingle:
				case (ValueFormat)169:
				case (ValueFormat)170:
				case (ValueFormat)171:
				{
					float[] array5;
					if (SerializedValue.TryParseMVSingleValue(format, buffer, ref offset, out array5))
					{
						value = array5;
						return true;
					}
					break;
				}
				case ValueFormat.MVDouble:
				case (ValueFormat)177:
				case (ValueFormat)178:
				case (ValueFormat)179:
				{
					double[] array6;
					if (SerializedValue.TryParseMVDoubleValue(format, buffer, ref offset, out array6))
					{
						value = array6;
						return true;
					}
					break;
				}
				case ValueFormat.MVDateTime:
				case (ValueFormat)185:
				case (ValueFormat)186:
				case (ValueFormat)187:
				{
					DateTime[] array7;
					if (SerializedValue.TryParseMVDateTimeValue(format, buffer, ref offset, out array7))
					{
						value = array7;
						return true;
					}
					break;
				}
				case ValueFormat.MVGuid:
				case (ValueFormat)193:
				case (ValueFormat)194:
				case (ValueFormat)195:
				{
					Guid[] array8;
					if (SerializedValue.TryParseMVGuidValue(format, buffer, ref offset, out array8))
					{
						value = array8;
						return true;
					}
					break;
				}
				case ValueFormat.MVString:
				case (ValueFormat)201:
				case (ValueFormat)202:
				case (ValueFormat)203:
				{
					string[] array9;
					if (SerializedValue.TryParseMVStringValue(format, buffer, ref offset, out array9))
					{
						value = array9;
						return true;
					}
					break;
				}
				case ValueFormat.MVBinary:
				case (ValueFormat)209:
				case (ValueFormat)210:
				case (ValueFormat)211:
				{
					byte[][] array10;
					if (SerializedValue.TryParseMVBinaryValue(format, buffer, ref offset, out array10))
					{
						value = array10;
						return true;
					}
					break;
				}
				}
			}
			else if (SerializedValue.TryParseReferenceValue(format, buffer, ref offset, out valueReference))
			{
				value = valueReference;
				return true;
			}
			return false;
		}

		private static bool TryGetValueSize(ValueFormat format, byte[] buffer, int offset, out int size)
		{
			if (format <= ValueFormat.Double)
			{
				if (format <= (ValueFormat)9)
				{
					if (format == ValueFormat.FormatModifierShift)
					{
						size = 0;
						return true;
					}
					switch (format)
					{
					case ValueFormat.Boolean:
					case (ValueFormat)9:
						size = 1;
						return true;
					}
				}
				else
				{
					switch (format)
					{
					case ValueFormat.Int16:
					case (ValueFormat)17:
					case (ValueFormat)18:
						size = 2;
						return true;
					case (ValueFormat)19:
					case (ValueFormat)20:
					case (ValueFormat)21:
					case (ValueFormat)22:
					case (ValueFormat)23:
					case (ValueFormat)28:
					case (ValueFormat)29:
					case (ValueFormat)30:
					case (ValueFormat)31:
					case (ValueFormat)37:
					case (ValueFormat)38:
					case (ValueFormat)39:
						break;
					case ValueFormat.Int32:
					case (ValueFormat)25:
					case (ValueFormat)26:
					case (ValueFormat)27:
						size = 4;
						return true;
					case ValueFormat.Int64:
					case (ValueFormat)33:
					case (ValueFormat)34:
					case (ValueFormat)35:
					case (ValueFormat)36:
						size = 8;
						return true;
					case ValueFormat.Single:
						size = 4;
						return true;
					default:
						if (format == ValueFormat.Double)
						{
							size = 8;
							return true;
						}
						break;
					}
				}
			}
			else
			{
				int num;
				if (format <= (ValueFormat)83)
				{
					if (format == ValueFormat.DateTime)
					{
						size = 8;
						return true;
					}
					switch (format)
					{
					case ValueFormat.Guid:
						size = 16;
						return true;
					case (ValueFormat)65:
					case (ValueFormat)66:
					case (ValueFormat)67:
					case (ValueFormat)68:
					case (ValueFormat)69:
					case (ValueFormat)70:
					case (ValueFormat)71:
					case (ValueFormat)76:
						goto IL_2DB;
					case ValueFormat.String:
					case (ValueFormat)73:
					case (ValueFormat)74:
					case (ValueFormat)75:
					case (ValueFormat)77:
					case (ValueFormat)78:
					case (ValueFormat)79:
						if (SerializedValue.TryGetStringValueSize(format, buffer, ref offset, out size))
						{
							return true;
						}
						goto IL_2DB;
					case ValueFormat.Binary:
					case (ValueFormat)81:
					case (ValueFormat)82:
					case (ValueFormat)83:
						num = 1;
						break;
					default:
						goto IL_2DB;
					}
				}
				else
				{
					if (format == ValueFormat.Reference || format == (ValueFormat)124)
					{
						size = 0;
						return true;
					}
					switch (format)
					{
					case ValueFormat.MVInt16:
					case (ValueFormat)145:
					case (ValueFormat)146:
					case (ValueFormat)147:
						num = 2;
						break;
					case (ValueFormat)148:
					case (ValueFormat)149:
					case (ValueFormat)150:
					case (ValueFormat)151:
					case (ValueFormat)156:
					case (ValueFormat)157:
					case (ValueFormat)158:
					case (ValueFormat)159:
					case (ValueFormat)164:
					case (ValueFormat)165:
					case (ValueFormat)166:
					case (ValueFormat)167:
					case (ValueFormat)172:
					case (ValueFormat)173:
					case (ValueFormat)174:
					case (ValueFormat)175:
					case (ValueFormat)180:
					case (ValueFormat)181:
					case (ValueFormat)182:
					case (ValueFormat)183:
					case (ValueFormat)188:
					case (ValueFormat)189:
					case (ValueFormat)190:
					case (ValueFormat)191:
					case (ValueFormat)196:
					case (ValueFormat)197:
					case (ValueFormat)198:
					case (ValueFormat)199:
					case (ValueFormat)204:
					case (ValueFormat)205:
					case (ValueFormat)206:
					case (ValueFormat)207:
						goto IL_2DB;
					case ValueFormat.MVInt32:
					case (ValueFormat)153:
					case (ValueFormat)154:
					case (ValueFormat)155:
						num = 4;
						break;
					case ValueFormat.MVInt64:
					case (ValueFormat)161:
					case (ValueFormat)162:
					case (ValueFormat)163:
						num = 8;
						break;
					case ValueFormat.MVSingle:
					case (ValueFormat)169:
					case (ValueFormat)170:
					case (ValueFormat)171:
						num = 4;
						break;
					case ValueFormat.MVDouble:
					case (ValueFormat)177:
					case (ValueFormat)178:
					case (ValueFormat)179:
						num = 8;
						break;
					case ValueFormat.MVDateTime:
					case (ValueFormat)185:
					case (ValueFormat)186:
					case (ValueFormat)187:
						num = 8;
						break;
					case ValueFormat.MVGuid:
					case (ValueFormat)193:
					case (ValueFormat)194:
					case (ValueFormat)195:
						num = 16;
						break;
					case ValueFormat.MVString:
					case (ValueFormat)201:
					case (ValueFormat)202:
					case (ValueFormat)203:
						if (SerializedValue.TryGetMVStringValueSize(format, buffer, ref offset, out size))
						{
							return true;
						}
						goto IL_2DB;
					case ValueFormat.MVBinary:
					case (ValueFormat)209:
					case (ValueFormat)210:
					case (ValueFormat)211:
						if (SerializedValue.TryGetMVBinaryValueSize(format, buffer, ref offset, out size))
						{
							return true;
						}
						goto IL_2DB;
					default:
						goto IL_2DB;
					}
				}
				int num2;
				if (SerializedValue.TryParseLength(format, buffer, ref offset, out num2) && ParseSerialize.CheckOffsetLength(buffer, offset, num2))
				{
					size = num2 * num;
					return true;
				}
			}
			IL_2DB:
			size = 0;
			return false;
		}

		private static bool TryParseBooleanValue(ValueFormat format, byte[] buffer, ref int offset, out bool value)
		{
			value = (format == (ValueFormat)9);
			return true;
		}

		private static bool TryParseInt16Value(ValueFormat format, byte[] buffer, ref int offset, out short value)
		{
			switch ((byte)(format & ValueFormat.FormatModifierMask))
			{
			case 0:
				value = 0;
				return true;
			case 1:
				if (ParseSerialize.CheckOffsetLength(buffer, offset, 1))
				{
					value = (short)((sbyte)buffer[offset++]);
					return true;
				}
				break;
			case 2:
				if (ParseSerialize.CheckOffsetLength(buffer, offset, 2))
				{
					value = ParseSerialize.ParseInt16(buffer, offset);
					offset += 2;
					return true;
				}
				break;
			}
			value = 0;
			return false;
		}

		private static bool TryParseInt32Value(ValueFormat format, byte[] buffer, ref int offset, out int value)
		{
			switch ((byte)(format & ValueFormat.FormatModifierMask))
			{
			case 0:
				value = 0;
				return true;
			case 1:
				if (ParseSerialize.CheckOffsetLength(buffer, offset, 1))
				{
					value = (int)((sbyte)buffer[offset++]);
					return true;
				}
				break;
			case 2:
				if (ParseSerialize.CheckOffsetLength(buffer, offset, 2))
				{
					value = (int)ParseSerialize.ParseInt16(buffer, offset);
					offset += 2;
					return true;
				}
				break;
			case 3:
				if (ParseSerialize.CheckOffsetLength(buffer, offset, 4))
				{
					value = ParseSerialize.ParseInt32(buffer, offset);
					offset += 4;
					return true;
				}
				break;
			}
			value = 0;
			return false;
		}

		private static bool TryParseInt64Value(ValueFormat format, byte[] buffer, ref int offset, out long value)
		{
			switch ((byte)(format & ValueFormat.FormatModifierMask))
			{
			case 0:
				value = 0L;
				return true;
			case 1:
				if (ParseSerialize.CheckOffsetLength(buffer, offset, 1))
				{
					value = (long)((sbyte)buffer[offset++]);
					return true;
				}
				break;
			case 2:
				if (ParseSerialize.CheckOffsetLength(buffer, offset, 2))
				{
					value = (long)ParseSerialize.ParseInt16(buffer, offset);
					offset += 2;
					return true;
				}
				break;
			case 3:
				if (ParseSerialize.CheckOffsetLength(buffer, offset, 4))
				{
					value = (long)ParseSerialize.ParseInt32(buffer, offset);
					offset += 4;
					return true;
				}
				break;
			case 4:
				if (ParseSerialize.CheckOffsetLength(buffer, offset, 8))
				{
					value = ParseSerialize.ParseInt64(buffer, offset);
					offset += 8;
					return true;
				}
				break;
			}
			value = 0L;
			return false;
		}

		private static bool TryParseSingleValue(ValueFormat format, byte[] buffer, ref int offset, out float value)
		{
			if (ParseSerialize.CheckOffsetLength(buffer, offset, 4))
			{
				value = ParseSerialize.ParseSingle(buffer, offset);
				offset += 4;
				return true;
			}
			value = 0f;
			return false;
		}

		private static bool TryParseDoubleValue(ValueFormat format, byte[] buffer, ref int offset, out double value)
		{
			if (ParseSerialize.CheckOffsetLength(buffer, offset, 8))
			{
				value = ParseSerialize.ParseDouble(buffer, offset);
				offset += 8;
				return true;
			}
			value = 0.0;
			return false;
		}

		private static bool TryParseDateTimeValue(ValueFormat format, byte[] buffer, ref int offset, out DateTime value)
		{
			if (ParseSerialize.CheckOffsetLength(buffer, offset, 8) && ParseSerialize.TryParseFileTime(buffer, offset, out value))
			{
				offset += 8;
				return true;
			}
			value = default(DateTime);
			return false;
		}

		private static bool TryParseGuidValue(ValueFormat format, byte[] buffer, ref int offset, out Guid value)
		{
			if (ParseSerialize.CheckOffsetLength(buffer, offset, 16))
			{
				value = ParseSerialize.ParseGuid(buffer, offset);
				offset += 16;
				return true;
			}
			value = default(Guid);
			return false;
		}

		private static bool TryParseBinaryValue(ValueFormat format, byte[] buffer, ref int offset, out byte[] value)
		{
			int num;
			if (SerializedValue.TryParseLength(format, buffer, ref offset, out num) && ParseSerialize.CheckOffsetLength(buffer, offset, num))
			{
				value = ParseSerialize.ParseBinary(buffer, offset, num);
				offset += num;
				return true;
			}
			value = null;
			return false;
		}

		private static bool TryParseStringValue(ValueFormat format, byte[] buffer, ref int offset, out string value)
		{
			int num;
			if (SerializedValue.TryParseLength(format, buffer, ref offset, out num) && ParseSerialize.CheckOffsetLength(buffer, offset, num))
			{
				if ((byte)(format & ValueFormat.StringEncodingMask) != 0)
				{
					value = ParseSerialize.ParseUtf8String(buffer, offset, num);
					offset += num;
					return true;
				}
				if ((num & 1) == 0)
				{
					value = ParseSerialize.ParseUcs16String(buffer, offset, num);
					offset += num;
					return true;
				}
			}
			value = null;
			return false;
		}

		private static bool TryGetStringValueSize(ValueFormat format, byte[] buffer, ref int offset, out int size)
		{
			int num;
			if (SerializedValue.TryParseLength(format, buffer, ref offset, out num) && ParseSerialize.CheckOffsetLength(buffer, offset, num))
			{
				if ((byte)(format & ValueFormat.StringEncodingMask) != 0)
				{
					size = ParseSerialize.GetLengthOfUtf8String(buffer, offset, num) * 2;
					offset += num;
					return true;
				}
				if ((num & 1) == 0)
				{
					size = num;
					offset += num;
					return true;
				}
			}
			size = 0;
			return false;
		}

		private static bool TryParseMVInt16Value(ValueFormat format, byte[] buffer, ref int offset, out short[] value)
		{
			int num;
			if (SerializedValue.TryParseLength(format, buffer, ref offset, out num) && num >= 0 && num <= (buffer.Length - offset) / 2 && ParseSerialize.CheckOffsetLength(buffer, offset, num * 2))
			{
				value = new short[num];
				for (int i = 0; i < num; i++)
				{
					value[i] = ParseSerialize.ParseInt16(buffer, offset);
					offset += 2;
				}
				return true;
			}
			value = null;
			return false;
		}

		private static bool TryParseMVInt32Value(ValueFormat format, byte[] buffer, ref int offset, out int[] value)
		{
			int num;
			if (SerializedValue.TryParseLength(format, buffer, ref offset, out num) && num >= 0 && num <= (buffer.Length - offset) / 4 && ParseSerialize.CheckOffsetLength(buffer, offset, num * 4))
			{
				value = new int[num];
				for (int i = 0; i < num; i++)
				{
					value[i] = ParseSerialize.ParseInt32(buffer, offset);
					offset += 4;
				}
				return true;
			}
			value = null;
			return false;
		}

		private static bool TryParseMVInt64Value(ValueFormat format, byte[] buffer, ref int offset, out long[] value)
		{
			int num;
			if (SerializedValue.TryParseLength(format, buffer, ref offset, out num) && num >= 0 && num <= (buffer.Length - offset) / 8 && ParseSerialize.CheckOffsetLength(buffer, offset, num * 8))
			{
				value = new long[num];
				for (int i = 0; i < num; i++)
				{
					value[i] = ParseSerialize.ParseInt64(buffer, offset);
					offset += 8;
				}
				return true;
			}
			value = null;
			return false;
		}

		private static bool TryParseMVSingleValue(ValueFormat format, byte[] buffer, ref int offset, out float[] value)
		{
			int num;
			if (SerializedValue.TryParseLength(format, buffer, ref offset, out num) && num >= 0 && num <= (buffer.Length - offset) / 4 && ParseSerialize.CheckOffsetLength(buffer, offset, num * 4))
			{
				value = new float[num];
				for (int i = 0; i < num; i++)
				{
					value[i] = ParseSerialize.ParseSingle(buffer, offset);
					offset += 4;
				}
				return true;
			}
			value = null;
			return false;
		}

		private static bool TryParseMVDoubleValue(ValueFormat format, byte[] buffer, ref int offset, out double[] value)
		{
			int num;
			if (SerializedValue.TryParseLength(format, buffer, ref offset, out num) && num >= 0 && num <= (buffer.Length - offset) / 8 && ParseSerialize.CheckOffsetLength(buffer, offset, num * 8))
			{
				value = new double[num];
				for (int i = 0; i < num; i++)
				{
					value[i] = ParseSerialize.ParseDouble(buffer, offset);
					offset += 8;
				}
				return true;
			}
			value = null;
			return false;
		}

		private static bool TryParseMVDateTimeValue(ValueFormat format, byte[] buffer, ref int offset, out DateTime[] value)
		{
			int num;
			if (SerializedValue.TryParseLength(format, buffer, ref offset, out num) && num >= 0 && num <= (buffer.Length - offset) / 8 && ParseSerialize.CheckOffsetLength(buffer, offset, num * 8))
			{
				value = new DateTime[num];
				for (int i = 0; i < num; i++)
				{
					if (!ParseSerialize.TryParseFileTime(buffer, offset, out value[i]))
					{
						value = null;
						return false;
					}
					offset += 8;
				}
				return true;
			}
			value = null;
			return false;
		}

		private static bool TryParseMVGuidValue(ValueFormat format, byte[] buffer, ref int offset, out Guid[] value)
		{
			int num;
			if (SerializedValue.TryParseLength(format, buffer, ref offset, out num) && num >= 0 && num <= (buffer.Length - offset) / 16 && ParseSerialize.CheckOffsetLength(buffer, offset, num * 16))
			{
				value = new Guid[num];
				for (int i = 0; i < num; i++)
				{
					value[i] = ParseSerialize.ParseGuid(buffer, offset);
					offset += 16;
				}
				return true;
			}
			value = null;
			return false;
		}

		private static bool TryParseMVBinaryValue(ValueFormat format, byte[] buffer, ref int offset, out byte[][] value)
		{
			int num;
			if (SerializedValue.TryParseLength(format, buffer, ref offset, out num) && num >= 0 && num <= (buffer.Length - offset) / 1 && ParseSerialize.CheckOffsetLength(buffer, offset, num))
			{
				value = new byte[num][];
				for (int i = 0; i < num; i++)
				{
					if (!ParseSerialize.CheckOffsetLength(buffer, offset, 1))
					{
						value = null;
						return false;
					}
					ValueFormat valueFormat = (ValueFormat)buffer[offset++];
					if (valueFormat == ValueFormat.FormatModifierShift)
					{
						value[i] = null;
					}
					else
					{
						if ((byte)(valueFormat & ValueFormat.TypeMask) != 80)
						{
							value = null;
							return false;
						}
						if (!SerializedValue.TryParseBinaryValue(valueFormat, buffer, ref offset, out value[i]))
						{
							value = null;
							return false;
						}
					}
				}
				return true;
			}
			value = null;
			return false;
		}

		private static bool TryGetMVBinaryValueSize(ValueFormat format, byte[] buffer, ref int offset, out int size)
		{
			size = 0;
			int num;
			if (SerializedValue.TryParseLength(format, buffer, ref offset, out num) && num >= 0 && num <= (buffer.Length - offset) / 1 && ParseSerialize.CheckOffsetLength(buffer, offset, num))
			{
				for (int i = 0; i < num; i++)
				{
					if (!ParseSerialize.CheckOffsetLength(buffer, offset, 1))
					{
						return false;
					}
					ValueFormat valueFormat = (ValueFormat)buffer[offset++];
					if (valueFormat != ValueFormat.FormatModifierShift)
					{
						if ((byte)(valueFormat & ValueFormat.TypeMask) != 80)
						{
							return false;
						}
						int num2;
						if (!SerializedValue.TryParseLength(valueFormat, buffer, ref offset, out num2) || !ParseSerialize.CheckOffsetLength(buffer, offset, num2))
						{
							return false;
						}
						size += num2;
						offset += num2;
					}
				}
				return true;
			}
			return false;
		}

		private static bool TryParseMVStringValue(ValueFormat format, byte[] buffer, ref int offset, out string[] value)
		{
			int num;
			if (SerializedValue.TryParseLength(format, buffer, ref offset, out num) && num >= 0 && num <= (buffer.Length - offset) / 1 && ParseSerialize.CheckOffsetLength(buffer, offset, num))
			{
				value = new string[num];
				for (int i = 0; i < num; i++)
				{
					if (!ParseSerialize.CheckOffsetLength(buffer, offset, 1))
					{
						value = null;
						return false;
					}
					ValueFormat valueFormat = (ValueFormat)buffer[offset++];
					if (valueFormat == ValueFormat.FormatModifierShift)
					{
						value[i] = null;
					}
					else
					{
						if ((byte)(valueFormat & ValueFormat.TypeMask) != 72)
						{
							value = null;
							return false;
						}
						if (!SerializedValue.TryParseStringValue(valueFormat, buffer, ref offset, out value[i]))
						{
							value = null;
							return false;
						}
					}
				}
				return true;
			}
			value = null;
			return false;
		}

		private static bool TryGetMVStringValueSize(ValueFormat format, byte[] buffer, ref int offset, out int size)
		{
			size = 0;
			int num;
			if (SerializedValue.TryParseLength(format, buffer, ref offset, out num) && num >= 0 && num <= (buffer.Length - offset) / 1 && ParseSerialize.CheckOffsetLength(buffer, offset, num))
			{
				for (int i = 0; i < num; i++)
				{
					if (!ParseSerialize.CheckOffsetLength(buffer, offset, 1))
					{
						return false;
					}
					ValueFormat valueFormat = (ValueFormat)buffer[offset++];
					if (valueFormat != ValueFormat.FormatModifierShift)
					{
						if ((byte)(valueFormat & ValueFormat.TypeMask) != 72)
						{
							return false;
						}
						int num2;
						if (!SerializedValue.TryGetStringValueSize(valueFormat, buffer, ref offset, out num2))
						{
							return false;
						}
						size += num2;
					}
				}
				return true;
			}
			return false;
		}

		private static bool TryParseReferenceValue(ValueFormat format, byte[] buffer, ref int offset, out ValueReference value)
		{
			if ((byte)(format & ValueFormat.StringEncodingMask) != 0)
			{
				value = ValueReference.Zero;
				return true;
			}
			value = null;
			return false;
		}

		private static bool TryParseLength(ValueFormat format, byte[] buffer, ref int offset, out int length)
		{
			switch ((byte)(format & ValueFormat.TypeShift))
			{
			case 0:
				length = 0;
				return true;
			case 1:
				if (ParseSerialize.CheckOffsetLength(buffer, offset, 1))
				{
					length = (int)buffer[offset++];
					return true;
				}
				break;
			case 2:
				if (ParseSerialize.CheckOffsetLength(buffer, offset, 2))
				{
					length = (int)((ushort)ParseSerialize.ParseInt16(buffer, offset));
					offset += 2;
					return true;
				}
				break;
			case 3:
				if (ParseSerialize.CheckOffsetLength(buffer, offset, 4))
				{
					length = ParseSerialize.ParseInt32(buffer, offset);
					offset += 4;
					return true;
				}
				break;
			}
			length = 0;
			return false;
		}

		public static int ComputeSize(object value)
		{
			return SerializedValue.SerializeImpl(value, null, 0);
		}

		public static int ComputeSize(ValueFormat format, object value)
		{
			return SerializedValue.SerializeImpl(format, value, null, 0);
		}

		public static int ComputeSize(IList<object> valueList)
		{
			int num = 0;
			foreach (object value in valueList)
			{
				num += SerializedValue.SerializeImpl(value, null, 0);
			}
			return num;
		}

		public static byte[] Serialize(object value)
		{
			int num = SerializedValue.ComputeSize(value);
			byte[] array = new byte[num];
			int num2 = 0;
			SerializedValue.Serialize(value, array, ref num2);
			return array;
		}

		public static byte[] Serialize(IList<object> valueList)
		{
			int num = SerializedValue.ComputeSize(valueList);
			byte[] array = new byte[num];
			int num2 = 0;
			SerializedValue.Serialize(valueList, array, ref num2);
			return array;
		}

		public static byte[] Serialize(params object[] valueList)
		{
			int num = SerializedValue.ComputeSize(valueList);
			byte[] array = new byte[num];
			int num2 = 0;
			SerializedValue.Serialize(valueList, array, ref num2);
			return array;
		}

		public static void Serialize(object value, byte[] buffer, ref int offset)
		{
			offset += SerializedValue.SerializeImpl(value, buffer, offset);
		}

		public static void Serialize(ValueFormat format, object value, byte[] buffer, ref int offset)
		{
			offset += SerializedValue.SerializeImpl(format, value, buffer, offset);
		}

		public static void Serialize(IList<object> valueList, byte[] buffer, ref int offset)
		{
			foreach (object value in valueList)
			{
				offset += SerializedValue.SerializeImpl(value, buffer, offset);
			}
		}

		private static int SerializeImpl(object value, byte[] buffer, int offset)
		{
			ValueFormat valueFormatType = ValueFormat.FormatModifierShift;
			if (value != null && !(value is ValueReference))
			{
				switch (ValueTypeHelper.GetExtendedTypeCode(value.GetType()))
				{
				case ExtendedTypeCode.Boolean:
					valueFormatType = ValueFormat.Boolean;
					break;
				case ExtendedTypeCode.Int16:
					valueFormatType = ValueFormat.Int16;
					break;
				case ExtendedTypeCode.Int32:
					valueFormatType = ValueFormat.Int32;
					break;
				case ExtendedTypeCode.Int64:
					valueFormatType = ValueFormat.Int64;
					break;
				case ExtendedTypeCode.Single:
					valueFormatType = ValueFormat.Single;
					break;
				case ExtendedTypeCode.Double:
					valueFormatType = ValueFormat.Double;
					break;
				case ExtendedTypeCode.DateTime:
					valueFormatType = ValueFormat.DateTime;
					break;
				case ExtendedTypeCode.Guid:
					valueFormatType = ValueFormat.Guid;
					break;
				case ExtendedTypeCode.String:
					valueFormatType = ValueFormat.String;
					break;
				case ExtendedTypeCode.Binary:
					valueFormatType = ValueFormat.Binary;
					break;
				case ExtendedTypeCode.MVInt16:
					valueFormatType = ValueFormat.MVInt16;
					break;
				case ExtendedTypeCode.MVInt32:
					valueFormatType = ValueFormat.MVInt32;
					break;
				case ExtendedTypeCode.MVInt64:
					valueFormatType = ValueFormat.MVInt64;
					break;
				case ExtendedTypeCode.MVSingle:
					valueFormatType = ValueFormat.MVSingle;
					break;
				case ExtendedTypeCode.MVDouble:
					valueFormatType = ValueFormat.MVDouble;
					break;
				case ExtendedTypeCode.MVDateTime:
					valueFormatType = ValueFormat.MVDateTime;
					break;
				case ExtendedTypeCode.MVGuid:
					valueFormatType = ValueFormat.MVGuid;
					break;
				case ExtendedTypeCode.MVString:
					valueFormatType = ValueFormat.MVString;
					break;
				case ExtendedTypeCode.MVBinary:
					valueFormatType = ValueFormat.MVBinary;
					break;
				}
			}
			return SerializedValue.SerializeImpl(valueFormatType, value, buffer, offset);
		}

		private static int SerializeImpl(ValueFormat valueFormatType, object value, byte[] buffer, int offset)
		{
			if (value == null)
			{
				valueFormatType = ValueFormat.FormatModifierShift;
			}
			else if (value is ValueReference)
			{
				valueFormatType = ValueFormat.Reference;
			}
			ValueFormat valueFormat = valueFormatType;
			if (valueFormat <= ValueFormat.String)
			{
				if (valueFormat <= ValueFormat.Int64)
				{
					if (valueFormat <= ValueFormat.Boolean)
					{
						if (valueFormat == ValueFormat.FormatModifierShift)
						{
							return SerializedValue.SerializeNull(buffer, offset);
						}
						if (valueFormat == ValueFormat.Boolean)
						{
							return SerializedValue.SerializeBoolean((bool)value, buffer, offset);
						}
					}
					else
					{
						if (valueFormat == ValueFormat.Int16)
						{
							return SerializedValue.SerializeInt16((short)value, buffer, offset);
						}
						if (valueFormat == ValueFormat.Int32)
						{
							return SerializedValue.SerializeInt32((int)value, buffer, offset);
						}
						if (valueFormat == ValueFormat.Int64)
						{
							return SerializedValue.SerializeInt64((long)value, buffer, offset);
						}
					}
				}
				else if (valueFormat <= ValueFormat.Double)
				{
					if (valueFormat == ValueFormat.Single)
					{
						return SerializedValue.SerializeSingle((float)value, buffer, offset);
					}
					if (valueFormat == ValueFormat.Double)
					{
						return SerializedValue.SerializeDouble((double)value, buffer, offset);
					}
				}
				else
				{
					if (valueFormat == ValueFormat.DateTime)
					{
						return SerializedValue.SerializeDateTime((DateTime)value, buffer, offset);
					}
					if (valueFormat == ValueFormat.Guid)
					{
						return SerializedValue.SerializeGuid((Guid)value, buffer, offset);
					}
					if (valueFormat == ValueFormat.String)
					{
						return SerializedValue.SerializeString((string)value, buffer, offset);
					}
				}
			}
			else if (valueFormat <= ValueFormat.MVInt64)
			{
				if (valueFormat <= ValueFormat.Reference)
				{
					if (valueFormat == ValueFormat.Binary)
					{
						return SerializedValue.SerializeBinary((byte[])value, buffer, offset);
					}
					if (valueFormat == ValueFormat.Reference)
					{
						return SerializedValue.SerializeReference((ValueReference)value, buffer, offset);
					}
				}
				else
				{
					if (valueFormat == ValueFormat.MVInt16)
					{
						return SerializedValue.SerializeMVInt16((short[])value, buffer, offset);
					}
					if (valueFormat == ValueFormat.MVInt32)
					{
						return SerializedValue.SerializeMVInt32((int[])value, buffer, offset);
					}
					if (valueFormat == ValueFormat.MVInt64)
					{
						return SerializedValue.SerializeMVInt64((long[])value, buffer, offset);
					}
				}
			}
			else if (valueFormat <= ValueFormat.MVDateTime)
			{
				if (valueFormat == ValueFormat.MVSingle)
				{
					return SerializedValue.SerializeMVSingle((float[])value, buffer, offset);
				}
				if (valueFormat == ValueFormat.MVDouble)
				{
					return SerializedValue.SerializeMVDouble((double[])value, buffer, offset);
				}
				if (valueFormat == ValueFormat.MVDateTime)
				{
					return SerializedValue.SerializeMVDateTime((DateTime[])value, buffer, offset);
				}
			}
			else
			{
				if (valueFormat == ValueFormat.MVGuid)
				{
					return SerializedValue.SerializeMVGuid((Guid[])value, buffer, offset);
				}
				if (valueFormat == ValueFormat.MVString)
				{
					return SerializedValue.SerializeMVString((string[])value, buffer, offset);
				}
				if (valueFormat == ValueFormat.MVBinary)
				{
					return SerializedValue.SerializeMVBinary((byte[][])value, buffer, offset);
				}
			}
			return 0;
		}

		public static int SerializeNull(byte[] buffer, int offset)
		{
			if (buffer != null)
			{
				buffer[offset] = 0;
			}
			return 1;
		}

		public static int SerializeBoolean(bool value, byte[] buffer, int offset)
		{
			if (buffer != null)
			{
				buffer[offset] = (byte)(8 | (value ? 1 : 0));
			}
			return 1;
		}

		public static int SerializeInt16(short value, byte[] buffer, int offset)
		{
			if (value == 0)
			{
				if (buffer != null)
				{
					buffer[offset] = 16;
				}
				return 1;
			}
			if (-128 <= value && value <= 127)
			{
				if (buffer != null)
				{
					buffer[offset] = 17;
					buffer[offset + 1] = (byte)value;
				}
				return 2;
			}
			if (buffer != null)
			{
				buffer[offset] = 18;
				ParseSerialize.SerializeInt16(value, buffer, offset + 1);
			}
			return 3;
		}

		public static int SerializeInt32(int value, byte[] buffer, int offset)
		{
			if (value == 0)
			{
				if (buffer != null)
				{
					buffer[offset] = 24;
				}
				return 1;
			}
			if (-128 <= value && value <= 127)
			{
				if (buffer != null)
				{
					buffer[offset] = 25;
					buffer[offset + 1] = (byte)value;
				}
				return 2;
			}
			if (-32768 <= value && value <= 32767)
			{
				if (buffer != null)
				{
					buffer[offset] = 26;
					ParseSerialize.SerializeInt16((short)value, buffer, offset + 1);
				}
				return 3;
			}
			if (buffer != null)
			{
				buffer[offset] = 27;
				ParseSerialize.SerializeInt32(value, buffer, offset + 1);
			}
			return 5;
		}

		public static int SerializeInt64(long value, byte[] buffer, int offset)
		{
			if (value == 0L)
			{
				if (buffer != null)
				{
					buffer[offset] = 32;
				}
				return 1;
			}
			if (-128L <= value && value <= 127L)
			{
				if (buffer != null)
				{
					buffer[offset] = 33;
					buffer[offset + 1] = (byte)value;
				}
				return 2;
			}
			if (-32768L <= value && value <= 32767L)
			{
				if (buffer != null)
				{
					buffer[offset] = 34;
					ParseSerialize.SerializeInt16((short)value, buffer, offset + 1);
				}
				return 3;
			}
			if (-2147483648L <= value && value <= 2147483647L)
			{
				if (buffer != null)
				{
					buffer[offset] = 35;
					ParseSerialize.SerializeInt32((int)value, buffer, offset + 1);
				}
				return 5;
			}
			if (buffer != null)
			{
				buffer[offset] = 36;
				ParseSerialize.SerializeInt64(value, buffer, offset + 1);
			}
			return 9;
		}

		public static int SerializeSingle(float value, byte[] buffer, int offset)
		{
			if (buffer != null)
			{
				buffer[offset] = 40;
				ParseSerialize.SerializeSingle(value, buffer, offset + 1);
			}
			return 5;
		}

		public static int SerializeDouble(double value, byte[] buffer, int offset)
		{
			if (buffer != null)
			{
				buffer[offset] = 48;
				ParseSerialize.SerializeDouble(value, buffer, offset + 1);
			}
			return 9;
		}

		public static int SerializeDateTime(DateTime value, byte[] buffer, int offset)
		{
			if (buffer != null)
			{
				buffer[offset] = 56;
				ParseSerialize.SerializeFileTime(value, buffer, offset + 1);
			}
			return 9;
		}

		public static int SerializeGuid(Guid value, byte[] buffer, int offset)
		{
			if (buffer != null)
			{
				buffer[offset] = 64;
				ParseSerialize.SerializeGuid(value, buffer, offset + 1);
			}
			return 17;
		}

		public static int SerializeString(string value, byte[] buffer, int offset)
		{
			if (value == null)
			{
				return SerializedValue.SerializeNull(buffer, offset);
			}
			ValueFormat valueFormat;
			int num;
			if (value.Length == 0)
			{
				valueFormat = ValueFormat.String;
				num = 0;
			}
			else
			{
				int byteCount = Encoding.Unicode.GetByteCount(value);
				int byteCount2 = Encoding.UTF8.GetByteCount(value);
				if (byteCount <= byteCount2)
				{
					valueFormat = ValueFormat.String;
					num = byteCount;
				}
				else
				{
					valueFormat = (ValueFormat)76;
					num = byteCount2;
				}
			}
			int num2 = SerializedValue.SerializeHeader(valueFormat, num, buffer, offset);
			if (buffer != null && value.Length != 0)
			{
				if (valueFormat == (ValueFormat)76)
				{
					Encoding.UTF8.GetBytes(value, 0, value.Length, buffer, offset + num2);
				}
				else
				{
					Encoding.Unicode.GetBytes(value, 0, value.Length, buffer, offset + num2);
				}
			}
			return num2 + num;
		}

		public static int SerializeBinary(byte[] value, byte[] buffer, int offset)
		{
			if (value == null)
			{
				return SerializedValue.SerializeNull(buffer, offset);
			}
			int num = SerializedValue.SerializeHeader(ValueFormat.Binary, value.Length, buffer, offset);
			if (buffer != null)
			{
				Buffer.BlockCopy(value, 0, buffer, offset + num, value.Length);
			}
			return num + value.Length;
		}

		public static int SerializeMVInt16(short[] value, byte[] buffer, int offset)
		{
			if (value == null)
			{
				return SerializedValue.SerializeNull(buffer, offset);
			}
			int num = SerializedValue.SerializeHeader(ValueFormat.MVInt16, value.Length, buffer, offset);
			if (buffer != null)
			{
				for (int i = 0; i < value.Length; i++)
				{
					ParseSerialize.SerializeInt16(value[i], buffer, offset + num + i * 2);
				}
			}
			return num + value.Length * 2;
		}

		public static int SerializeMVInt32(int[] value, byte[] buffer, int offset)
		{
			if (value == null)
			{
				return SerializedValue.SerializeNull(buffer, offset);
			}
			int num = SerializedValue.SerializeHeader(ValueFormat.MVInt32, value.Length, buffer, offset);
			if (buffer != null)
			{
				for (int i = 0; i < value.Length; i++)
				{
					ParseSerialize.SerializeInt32(value[i], buffer, offset + num + i * 4);
				}
			}
			return num + value.Length * 4;
		}

		public static int SerializeMVInt64(long[] value, byte[] buffer, int offset)
		{
			if (value == null)
			{
				return SerializedValue.SerializeNull(buffer, offset);
			}
			int num = SerializedValue.SerializeHeader(ValueFormat.MVInt64, value.Length, buffer, offset);
			if (buffer != null)
			{
				for (int i = 0; i < value.Length; i++)
				{
					ParseSerialize.SerializeInt64(value[i], buffer, offset + num + i * 8);
				}
			}
			return num + value.Length * 8;
		}

		public static int SerializeMVSingle(float[] value, byte[] buffer, int offset)
		{
			if (value == null)
			{
				return SerializedValue.SerializeNull(buffer, offset);
			}
			int num = SerializedValue.SerializeHeader(ValueFormat.MVSingle, value.Length, buffer, offset);
			if (buffer != null)
			{
				for (int i = 0; i < value.Length; i++)
				{
					ParseSerialize.SerializeSingle(value[i], buffer, offset + num + i * 4);
				}
			}
			return num + value.Length * 4;
		}

		public static int SerializeMVDouble(double[] value, byte[] buffer, int offset)
		{
			if (value == null)
			{
				return SerializedValue.SerializeNull(buffer, offset);
			}
			int num = SerializedValue.SerializeHeader(ValueFormat.MVDouble, value.Length, buffer, offset);
			if (buffer != null)
			{
				for (int i = 0; i < value.Length; i++)
				{
					ParseSerialize.SerializeDouble(value[i], buffer, offset + num + i * 8);
				}
			}
			return num + value.Length * 8;
		}

		public static int SerializeMVDateTime(DateTime[] value, byte[] buffer, int offset)
		{
			if (value == null)
			{
				return SerializedValue.SerializeNull(buffer, offset);
			}
			int num = SerializedValue.SerializeHeader(ValueFormat.MVDateTime, value.Length, buffer, offset);
			if (buffer != null)
			{
				for (int i = 0; i < value.Length; i++)
				{
					ParseSerialize.SerializeFileTime(value[i], buffer, offset + num + i * 8);
				}
			}
			return num + value.Length * 8;
		}

		public static int SerializeMVGuid(Guid[] value, byte[] buffer, int offset)
		{
			if (value == null)
			{
				return SerializedValue.SerializeNull(buffer, offset);
			}
			int num = SerializedValue.SerializeHeader(ValueFormat.MVGuid, value.Length, buffer, offset);
			if (buffer != null)
			{
				for (int i = 0; i < value.Length; i++)
				{
					ParseSerialize.SerializeGuid(value[i], buffer, offset + num + i * 16);
				}
			}
			return num + value.Length * 16;
		}

		public static int SerializeMVString(string[] value, byte[] buffer, int offset)
		{
			if (value == null)
			{
				return SerializedValue.SerializeNull(buffer, offset);
			}
			int num = SerializedValue.SerializeHeader(ValueFormat.MVString, value.Length, buffer, offset);
			offset += num;
			for (int i = 0; i < value.Length; i++)
			{
				int num2;
				if (value[i] == null)
				{
					num2 = SerializedValue.SerializeNull(buffer, offset);
				}
				else
				{
					num2 = SerializedValue.SerializeString(value[i], buffer, offset);
				}
				num += num2;
				offset += num2;
			}
			return num;
		}

		public static int SerializeMVBinary(byte[][] value, byte[] buffer, int offset)
		{
			if (value == null)
			{
				return SerializedValue.SerializeNull(buffer, offset);
			}
			int num = SerializedValue.SerializeHeader(ValueFormat.MVBinary, value.Length, buffer, offset);
			offset += num;
			for (int i = 0; i < value.Length; i++)
			{
				int num2;
				if (value[i] == null)
				{
					num2 = SerializedValue.SerializeNull(buffer, offset);
				}
				else
				{
					num2 = SerializedValue.SerializeBinary(value[i], buffer, offset);
				}
				num += num2;
				offset += num2;
			}
			return num;
		}

		public static int SerializeReference(ValueReference value, byte[] buffer, int offset)
		{
			if (value == null)
			{
				return SerializedValue.SerializeNull(buffer, offset);
			}
			if (value.IsZero)
			{
				if (buffer != null)
				{
					buffer[offset] = 124;
				}
				return 1;
			}
			ExAssert.RetailAssert(false, "unexpected reference value");
			return 0;
		}

		private static int SerializeHeader(ValueFormat format, int length, byte[] buffer, int offset)
		{
			int num;
			if (length == 0)
			{
				num = 0;
				format = format;
			}
			else if (length <= 255)
			{
				num = 1;
				format |= ValueFormat.LengthSizeOneByte;
			}
			else if (length <= 65535)
			{
				num = 2;
				format |= ValueFormat.LengthSizeTwoBytes;
			}
			else
			{
				num = 4;
				format |= ValueFormat.TypeShift;
			}
			if (buffer != null)
			{
				buffer[offset] = (byte)format;
				switch ((byte)(format & ValueFormat.TypeShift))
				{
				case 1:
					buffer[offset + 1] = (byte)length;
					break;
				case 2:
					ParseSerialize.SerializeInt16((short)length, buffer, offset + 1);
					break;
				case 3:
					ParseSerialize.SerializeInt32(length, buffer, offset + 1);
					break;
				}
			}
			return 1 + num;
		}

		private const int MaximumInt32ToCache = 100;

		private static readonly object boxedTrue = true;

		private static readonly object boxedFalse = false;

		private static readonly object[] boxedInts = new object[100];
	}
}
