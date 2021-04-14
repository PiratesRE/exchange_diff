using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Mapi.Common;
using Microsoft.Exchange.Diagnostics.Components.Data.Mapi;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Mapi
{
	internal static class MapiPropValueConvertor
	{
		public static Type TypeFromPropType(PropType propertyType, bool extractItemTypeInArray)
		{
			if (extractItemTypeInArray)
			{
				propertyType &= (PropType)(-12289);
			}
			PropType propType = propertyType;
			if (propType <= PropType.Binary)
			{
				if (propType <= PropType.String)
				{
					switch (propType)
					{
					case PropType.Unspecified:
						return typeof(object);
					case PropType.Null:
						return typeof(object);
					case PropType.Short:
						return typeof(short);
					case PropType.Int:
						return typeof(int);
					case PropType.Float:
						return typeof(float);
					case PropType.Double:
						return typeof(double);
					case PropType.Currency:
						return typeof(long);
					case PropType.AppTime:
						return typeof(double);
					case (PropType)8:
					case (PropType)9:
					case (PropType)12:
					case (PropType)14:
					case (PropType)15:
					case (PropType)16:
					case (PropType)17:
					case (PropType)18:
					case (PropType)19:
						break;
					case PropType.Error:
						return typeof(int);
					case PropType.Boolean:
						return typeof(bool);
					case PropType.Object:
						return typeof(IntPtr);
					case PropType.Long:
						return typeof(long);
					default:
						switch (propType)
						{
						case PropType.AnsiString:
							return typeof(string);
						case PropType.String:
							return typeof(string);
						}
						break;
					}
				}
				else
				{
					if (propType == PropType.SysTime)
					{
						return typeof(DateTime);
					}
					if (propType == PropType.Guid)
					{
						return typeof(Guid);
					}
					if (propType == PropType.Binary)
					{
						return typeof(byte[]);
					}
				}
			}
			else if (propType <= PropType.StringArray)
			{
				switch (propType)
				{
				case PropType.ShortArray:
					return typeof(short[]);
				case PropType.IntArray:
					return typeof(int[]);
				case PropType.FloatArray:
					return typeof(float[]);
				case PropType.DoubleArray:
					return typeof(double[]);
				case PropType.CurrencyArray:
					return typeof(long[]);
				case PropType.AppTimeArray:
					return typeof(double[]);
				case (PropType)4104:
				case (PropType)4105:
				case (PropType)4106:
				case (PropType)4107:
				case (PropType)4108:
					break;
				case PropType.ObjectArray:
					return typeof(object);
				default:
					if (propType == PropType.LongArray)
					{
						return typeof(long[]);
					}
					switch (propType)
					{
					case PropType.AnsiStringArray:
						return typeof(string[]);
					case PropType.StringArray:
						return typeof(string[]);
					}
					break;
				}
			}
			else
			{
				if (propType == PropType.SysTimeArray)
				{
					return typeof(DateTime[]);
				}
				if (propType == PropType.GuidArray)
				{
					return typeof(Guid[]);
				}
				if (propType == PropType.BinaryArray)
				{
					return typeof(byte[][]);
				}
			}
			throw new NotSupportedException(Strings.ExceptionNoIdeaConvertPropType(propertyType.ToString()));
		}

		public static MultiValuedPropertyBase MultiValuedPropertyFromCollection(ICollection values, MapiPropertyDefinition propertyDefinition, bool forceReadOnly)
		{
			if (!propertyDefinition.IsMultivalued)
			{
				throw new NotSupportedException(Strings.ExceptionNotMultiValuedPropertyDefinition(propertyDefinition.Name));
			}
			Type type = propertyDefinition.Type;
			bool readOnly = propertyDefinition.IsReadOnly || forceReadOnly;
			bool flag = propertyDefinition.IsReadOnly && 0 == values.Count;
			if (typeof(short) == type)
			{
				if (!flag)
				{
					return new MultiValuedProperty<short>(readOnly, propertyDefinition, values);
				}
				return MultiValuedProperty<short>.Empty;
			}
			else if (typeof(int) == type)
			{
				if (!flag)
				{
					return new MultiValuedProperty<int>(readOnly, propertyDefinition, values);
				}
				return MultiValuedProperty<int>.Empty;
			}
			else if (typeof(long) == type)
			{
				if (!flag)
				{
					return new MultiValuedProperty<long>(readOnly, propertyDefinition, values);
				}
				return MultiValuedProperty<long>.Empty;
			}
			else if (typeof(float) == type)
			{
				if (!flag)
				{
					return new MultiValuedProperty<float>(readOnly, propertyDefinition, values);
				}
				return MultiValuedProperty<float>.Empty;
			}
			else if (typeof(double) == type)
			{
				if (!flag)
				{
					return new MultiValuedProperty<double>(readOnly, propertyDefinition, values);
				}
				return MultiValuedProperty<double>.Empty;
			}
			else if (typeof(DateTime) == type)
			{
				if (!flag)
				{
					return new MultiValuedProperty<DateTime>(readOnly, propertyDefinition, values);
				}
				return MultiValuedProperty<DateTime>.Empty;
			}
			else if (typeof(bool) == type)
			{
				if (!flag)
				{
					return new MultiValuedProperty<bool>(readOnly, propertyDefinition, values);
				}
				return MultiValuedProperty<bool>.Empty;
			}
			else if (typeof(string) == type)
			{
				if (!flag)
				{
					return new MultiValuedProperty<string>(readOnly, propertyDefinition, values);
				}
				return MultiValuedProperty<string>.Empty;
			}
			else if (typeof(Guid) == type)
			{
				if (!flag)
				{
					return new MultiValuedProperty<Guid>(readOnly, propertyDefinition, values);
				}
				return MultiValuedProperty<Guid>.Empty;
			}
			else if (typeof(byte[]) == type)
			{
				if (!flag)
				{
					return new MultiValuedProperty<byte[]>(readOnly, propertyDefinition, values);
				}
				return MultiValuedProperty<byte[]>.Empty;
			}
			else
			{
				if (!(typeof(ObjectId) == type))
				{
					throw new NotSupportedException(Strings.ExceptionNoIdeaGenerateMultiValuedProperty(propertyDefinition.Type.ToString()));
				}
				if (!flag)
				{
					return new MultiValuedProperty<ObjectId>(readOnly, propertyDefinition, values);
				}
				return MultiValuedProperty<ObjectId>.Empty;
			}
		}

		internal static bool TryCastValueToExtract(PropValue value, Type type, out object valueToExtract)
		{
			valueToExtract = null;
			try
			{
				if (type.IsEnum)
				{
					object obj = Convert.ChangeType(value.RawValue, Enum.GetUnderlyingType(type));
					if (obj != null)
					{
						try
						{
							valueToExtract = Enum.Parse(type, obj.ToString());
						}
						catch (ArgumentException)
						{
							return false;
						}
						return true;
					}
					return false;
				}
				else
				{
					valueToExtract = Convert.ChangeType(value.RawValue, type);
				}
			}
			catch (ArgumentNullException)
			{
				return false;
			}
			catch (InvalidCastException)
			{
				return false;
			}
			catch (OverflowException)
			{
				return false;
			}
			return true;
		}

		internal static MapiConvertingException ConstructExtractingException(PropValue value, MapiPropertyDefinition propertyDefinition, LocalizedString errorDetails)
		{
			if (errorDetails.IsEmpty)
			{
				errorDetails = Strings.ConstantNa;
			}
			MapiConvertingException ex = new MapiExtractingException(propertyDefinition.Name, value.PropTag.ToString(), value.PropType.ToString(), (value.RawValue == null) ? Strings.ConstantNull : value.RawValue.ToString(), (value.RawValue == null) ? Strings.ConstantNull : value.RawValue.GetType().ToString(), propertyDefinition.Type.ToString(), propertyDefinition.IsMultivalued.ToString(), errorDetails);
			ExTraceGlobals.ConvertorTracer.TraceError((long)typeof(MapiPropValueConvertor).GetHashCode(), ex.Message);
			return ex;
		}

		public static object Extract(PropValue value, MapiPropertyDefinition propertyDefinition)
		{
			bool isReadOnly = propertyDefinition.IsReadOnly;
			Type type = propertyDefinition.Type;
			Type underlyingType = Nullable.GetUnderlyingType(type);
			if (null != underlyingType)
			{
				type = underlyingType;
			}
			if (propertyDefinition.PropertyTag.Id() != value.PropTag.Id())
			{
				throw MapiPropValueConvertor.ConstructExtractingException(value, propertyDefinition, Strings.ExceptionUnmatchedPropTag(propertyDefinition.PropertyTag.ToString(), value.PropTag.ToString()));
			}
			if (PropType.Binary == value.PropType)
			{
				if (typeof(Guid) == type)
				{
					return new Guid(value.GetBytes());
				}
				if (typeof(Schedule) == type)
				{
					return Schedule.FromByteArray(value.GetBytes());
				}
				if (typeof(MapiEntryId) == type)
				{
					return new MapiEntryId(value.GetBytes());
				}
			}
			if (typeof(short) == type)
			{
				if (propertyDefinition.IsMultivalued)
				{
					return new MultiValuedProperty<short>(isReadOnly, propertyDefinition, value.GetShortArray());
				}
				return value.GetShort();
			}
			else if (typeof(int) == type)
			{
				if (propertyDefinition.IsMultivalued)
				{
					return new MultiValuedProperty<int>(isReadOnly, propertyDefinition, value.GetIntArray());
				}
				return value.GetInt();
			}
			else if (typeof(long) == type)
			{
				if (propertyDefinition.IsMultivalued)
				{
					return new MultiValuedProperty<long>(isReadOnly, propertyDefinition, value.GetLongArray());
				}
				return value.GetLong();
			}
			else if (typeof(float) == type)
			{
				if (propertyDefinition.IsMultivalued)
				{
					return new MultiValuedProperty<float>(isReadOnly, propertyDefinition, value.GetFloatArray());
				}
				return value.GetFloat();
			}
			else if (typeof(double) == type)
			{
				if (propertyDefinition.IsMultivalued)
				{
					return new MultiValuedProperty<double>(isReadOnly, propertyDefinition, value.GetDoubleArray());
				}
				return value.GetDouble();
			}
			else if (typeof(DateTime) == type)
			{
				if (propertyDefinition.IsMultivalued)
				{
					DateTime[] dateTimeArray = value.GetDateTimeArray();
					List<DateTime> list = new List<DateTime>(dateTimeArray.Length);
					foreach (DateTime dateTime in dateTimeArray)
					{
						list.Add(dateTime.ToLocalTime());
					}
					return new MultiValuedProperty<DateTime>(isReadOnly, propertyDefinition, list.ToArray());
				}
				return value.GetDateTime().ToLocalTime();
			}
			else if (typeof(bool) == type)
			{
				if (propertyDefinition.IsMultivalued)
				{
					return new MultiValuedProperty<bool>(isReadOnly, propertyDefinition, value.GetBoolArray());
				}
				return value.GetBoolean();
			}
			else if (typeof(string) == type)
			{
				if (propertyDefinition.IsMultivalued)
				{
					return new MultiValuedProperty<string>(isReadOnly, propertyDefinition, value.GetStringArray());
				}
				return value.GetString();
			}
			else if (typeof(Guid) == type)
			{
				if (propertyDefinition.IsMultivalued)
				{
					return new MultiValuedProperty<Guid>(isReadOnly, propertyDefinition, value.GetGuidArray());
				}
				return value.GetGuid();
			}
			else if (typeof(byte[]) == type)
			{
				if (propertyDefinition.IsMultivalued)
				{
					return new MultiValuedProperty<byte[]>(isReadOnly, propertyDefinition, value.GetBytesArray());
				}
				return value.GetBytes();
			}
			else
			{
				if (typeof(short[]) == type)
				{
					return value.GetShortArray();
				}
				if (typeof(int[]) == type)
				{
					return value.GetIntArray();
				}
				if (typeof(long[]) == type)
				{
					return value.GetLongArray();
				}
				if (typeof(float[]) == type)
				{
					return value.GetFloatArray();
				}
				if (typeof(double[]) == type)
				{
					return value.GetDoubleArray();
				}
				if (typeof(DateTime[]) == type)
				{
					DateTime[] dateTimeArray2 = value.GetDateTimeArray();
					List<DateTime> list2 = new List<DateTime>(dateTimeArray2.Length);
					foreach (DateTime dateTime2 in dateTimeArray2)
					{
						list2.Add(dateTime2.ToLocalTime());
					}
					return list2.ToArray();
				}
				if (typeof(bool[]) == type)
				{
					return value.GetBoolArray();
				}
				if (typeof(string[]) == type)
				{
					return value.GetStringArray();
				}
				if (typeof(Guid[]) == type)
				{
					return value.GetGuidArray();
				}
				if (typeof(byte[][]) == type)
				{
					return value.GetBytesArray();
				}
				if (type.IsAssignableFrom(MapiPropValueConvertor.TypeFromPropType(value.PropType, false)))
				{
					return value.RawValue;
				}
				object result = null;
				if (MapiPropValueConvertor.TryCastValueToExtract(value, type, out result))
				{
					return result;
				}
				throw MapiPropValueConvertor.ConstructExtractingException(value, propertyDefinition, Strings.ConstantNa);
			}
		}

		internal static bool TryCastValueToPack(object value, PropType type, out object valueToPack)
		{
			valueToPack = null;
			try
			{
				valueToPack = Convert.ChangeType(value, MapiPropValueConvertor.TypeFromPropType(type, false));
			}
			catch (ArgumentNullException)
			{
				return false;
			}
			catch (InvalidCastException)
			{
				return false;
			}
			catch (OverflowException)
			{
				return false;
			}
			return true;
		}

		internal static MapiConvertingException ConstructPackingException(object value, MapiPropertyDefinition propertyDefinition, LocalizedString errorDetails)
		{
			if (errorDetails.IsEmpty)
			{
				errorDetails = Strings.ConstantNa;
			}
			MapiConvertingException ex = new MapiPackingException(propertyDefinition.Name, (value == null) ? Strings.ConstantNull : value.ToString(), (value == null) ? Strings.ConstantNull : value.GetType().Name, propertyDefinition.IsMultivalued.ToString(), propertyDefinition.PropertyTag.ToString(), propertyDefinition.PropertyTag.ValueType().ToString(), errorDetails);
			ExTraceGlobals.ConvertorTracer.TraceError((long)typeof(MapiPropValueConvertor).GetHashCode(), ex.Message);
			return ex;
		}

		public static PropValue Pack(object value, MapiPropertyDefinition propertyDefinition)
		{
			PropTag propertyTag = propertyDefinition.PropertyTag;
			PropType propType = propertyTag.ValueType();
			Type type = MapiPropValueConvertor.TypeFromPropType(propType, false);
			if (value != null)
			{
				if (typeof(byte[]) == type)
				{
					if (typeof(Guid) == value.GetType())
					{
						return new PropValue(propertyTag, ((Guid)value).ToByteArray());
					}
					if (typeof(Schedule) == value.GetType())
					{
						return new PropValue(propertyTag, ((Schedule)value).ToByteArray());
					}
				}
				if (typeof(DateTime) == type && typeof(DateTime) == value.GetType())
				{
					return new PropValue(propertyTag, ((DateTime)value).ToUniversalTime());
				}
				if (typeof(DateTime[]) == type)
				{
					Type type2 = value.GetType();
					if (typeof(MultiValuedProperty<DateTime>) == type2 || typeof(DateTime[]) == type2)
					{
						List<DateTime> list = new List<DateTime>(((ICollection)value).Count);
						foreach (object obj in ((ICollection)value))
						{
							list.Add(((DateTime)obj).ToUniversalTime());
						}
						return new PropValue(propertyTag, list.ToArray());
					}
				}
				if (type.IsAssignableFrom(value.GetType()))
				{
					return new PropValue(propertyTag, value);
				}
			}
			object value2 = null;
			if (MapiPropValueConvertor.TryCastValueToPack(value, propType, out value2))
			{
				return new PropValue(propertyTag, value2);
			}
			throw MapiPropValueConvertor.ConstructPackingException(value, propertyDefinition, Strings.ConstantNa);
		}
	}
}
