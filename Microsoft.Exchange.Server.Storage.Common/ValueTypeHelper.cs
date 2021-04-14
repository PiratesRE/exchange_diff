using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public static class ValueTypeHelper
	{
		public static bool IsMultivalue(ExtendedTypeCode extendedTypeCode)
		{
			return 0 != (byte)(extendedTypeCode & ExtendedTypeCode.MVFlag);
		}

		public static bool TryGetExtendedTypeCode(Type type, out ExtendedTypeCode extendedTypeCode)
		{
			extendedTypeCode = ExtendedTypeCode.Invalid;
			TypeCode typeCode = ValueTypeHelper.InternalGetTypeCode(type);
			switch (typeCode)
			{
			case TypeCode.Object:
				if (type == typeof(byte[]))
				{
					extendedTypeCode = ExtendedTypeCode.Binary;
				}
				else if (type == typeof(Guid))
				{
					extendedTypeCode = ExtendedTypeCode.Guid;
				}
				else if (type.IsArray)
				{
					Type elementType = type.GetElementType();
					TypeCode typeCode2 = ValueTypeHelper.InternalGetTypeCode(elementType);
					if (typeCode2 != TypeCode.Object)
					{
						switch (typeCode2)
						{
						case TypeCode.Int16:
							extendedTypeCode = ExtendedTypeCode.MVInt16;
							break;
						case TypeCode.Int32:
							extendedTypeCode = ExtendedTypeCode.MVInt32;
							break;
						case TypeCode.Int64:
							extendedTypeCode = ExtendedTypeCode.MVInt64;
							break;
						case TypeCode.Single:
							extendedTypeCode = ExtendedTypeCode.MVSingle;
							break;
						case TypeCode.Double:
							extendedTypeCode = ExtendedTypeCode.MVDouble;
							break;
						case TypeCode.DateTime:
							extendedTypeCode = ExtendedTypeCode.MVDateTime;
							break;
						case TypeCode.String:
							extendedTypeCode = ExtendedTypeCode.MVString;
							break;
						}
					}
					else if (elementType == typeof(Guid))
					{
						extendedTypeCode = ExtendedTypeCode.MVGuid;
					}
					else if (elementType == typeof(byte[]))
					{
						extendedTypeCode = ExtendedTypeCode.MVBinary;
					}
				}
				break;
			case TypeCode.DBNull:
				break;
			case TypeCode.Boolean:
				extendedTypeCode = ExtendedTypeCode.Boolean;
				break;
			default:
				switch (typeCode)
				{
				case TypeCode.Int16:
					extendedTypeCode = ExtendedTypeCode.Int16;
					break;
				case TypeCode.Int32:
					extendedTypeCode = ExtendedTypeCode.Int32;
					break;
				case TypeCode.Int64:
					extendedTypeCode = ExtendedTypeCode.Int64;
					break;
				case TypeCode.Single:
					extendedTypeCode = ExtendedTypeCode.Single;
					break;
				case TypeCode.Double:
					extendedTypeCode = ExtendedTypeCode.Double;
					break;
				case TypeCode.DateTime:
					extendedTypeCode = ExtendedTypeCode.DateTime;
					break;
				case TypeCode.String:
					extendedTypeCode = ExtendedTypeCode.String;
					break;
				}
				break;
			}
			return extendedTypeCode != ExtendedTypeCode.Invalid;
		}

		public static ExtendedTypeCode GetExtendedTypeCode(Type type)
		{
			ExtendedTypeCode result;
			ValueTypeHelper.TryGetExtendedTypeCode(type, out result);
			return result;
		}

		public static ExtendedTypeCode GetExtendedTypeCode(object value)
		{
			return ValueTypeHelper.GetExtendedTypeCode(value.GetType());
		}

		public static ExtendedTypeCode GetExtendedTypeCodeNoAssert(Type type)
		{
			ExtendedTypeCode result;
			ValueTypeHelper.TryGetExtendedTypeCode(type, out result);
			return result;
		}

		public static int ValueSize(object value)
		{
			if (value == null)
			{
				return 0;
			}
			return ValueTypeHelper.ValueSize(ValueTypeHelper.GetExtendedTypeCode(value), value);
		}

		public static int ValueSize(ExtendedTypeCode typeCode, object value)
		{
			if (value == null)
			{
				return 0;
			}
			switch (typeCode)
			{
			case ExtendedTypeCode.Boolean:
				return 1;
			case ExtendedTypeCode.Int16:
				return 2;
			case ExtendedTypeCode.Int32:
				return 4;
			case ExtendedTypeCode.Int64:
				return 8;
			case ExtendedTypeCode.Single:
				return 4;
			case ExtendedTypeCode.Double:
				return 8;
			case ExtendedTypeCode.DateTime:
				return 8;
			case ExtendedTypeCode.Guid:
				return 16;
			case ExtendedTypeCode.String:
				return ((string)value).Length * 2;
			case ExtendedTypeCode.Binary:
				return ((Array)value).Length;
			case ExtendedTypeCode.MVInt16:
				return ((Array)value).Length * 2;
			case ExtendedTypeCode.MVInt32:
				return ((Array)value).Length * 4;
			case ExtendedTypeCode.MVInt64:
				return ((Array)value).Length * 8;
			case ExtendedTypeCode.MVSingle:
				return ((Array)value).Length * 4;
			case ExtendedTypeCode.MVDouble:
				return ((Array)value).Length * 8;
			case ExtendedTypeCode.MVDateTime:
				return ((Array)value).Length * 8;
			case ExtendedTypeCode.MVGuid:
				return ((Array)value).Length * 16;
			case ExtendedTypeCode.MVString:
			{
				string[] array = (string[])value;
				int num = 0;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] != null)
					{
						num += array[i].Length * 2;
					}
				}
				return num;
			}
			case ExtendedTypeCode.MVBinary:
			{
				byte[][] array2 = (byte[][])value;
				int num2 = 0;
				for (int j = 0; j < array2.Length; j++)
				{
					if (array2[j] != null)
					{
						num2 += array2[j].Length;
					}
				}
				return num2;
			}
			}
			return 0;
		}

		internal static TypeCode InternalGetTypeCode(Type type)
		{
			return Type.GetTypeCode(type);
		}
	}
}
