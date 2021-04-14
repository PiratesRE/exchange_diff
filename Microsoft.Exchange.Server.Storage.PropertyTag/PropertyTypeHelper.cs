using System;
using System.Globalization;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.PropTags
{
	public class PropertyTypeHelper
	{
		public static string PropertyTypeToString(PropertyType inType)
		{
			if (inType <= PropertyType.MVSysTime)
			{
				if (inType <= PropertyType.Actions)
				{
					if (inType <= PropertyType.Unicode)
					{
						switch (inType)
						{
						case PropertyType.Unspecified:
							return "Unspecified";
						case PropertyType.Null:
							return "Null";
						case PropertyType.Int16:
							return "Int16";
						case PropertyType.Int32:
							return "Int32";
						case PropertyType.Real32:
							return "Real32";
						case PropertyType.Real64:
							return "Real64";
						case PropertyType.Currency:
							return "Currency";
						case PropertyType.AppTime:
							return "AppTime";
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
						case PropertyType.Error:
							return "Error";
						case PropertyType.Boolean:
							return "Boolean";
						case PropertyType.Object:
							return "Object";
						case PropertyType.Int64:
							return "Int64";
						default:
							switch (inType)
							{
							case PropertyType.String8:
								return "String8";
							case PropertyType.Unicode:
								return "Unicode";
							}
							break;
						}
					}
					else
					{
						if (inType == PropertyType.SysTime)
						{
							return "SysTime";
						}
						if (inType == PropertyType.Guid)
						{
							return "Guid";
						}
						switch (inType)
						{
						case PropertyType.SvrEid:
							return "SvrEid";
						case PropertyType.SRestriction:
							return "SRestriction";
						case PropertyType.Actions:
							return "Actions";
						}
					}
				}
				else if (inType <= PropertyType.MVAppTime)
				{
					if (inType == PropertyType.Binary)
					{
						return "Binary";
					}
					switch (inType)
					{
					case PropertyType.Invalid:
						return "Invalid";
					case PropertyType.MVNull:
						return "MV-Null";
					case PropertyType.MVInt16:
						return "MV-Int16";
					case PropertyType.MVInt32:
						return "MV-Int32";
					case PropertyType.MVReal32:
						return "MV-Real32";
					case PropertyType.MVReal64:
						return "MV-Real64";
					case PropertyType.MVCurrency:
						return "MV-Currency";
					case PropertyType.MVAppTime:
						return "MV-AppTime";
					}
				}
				else
				{
					if (inType == PropertyType.MVInt64)
					{
						return "MV-Int64";
					}
					switch (inType)
					{
					case PropertyType.MVString8:
						return "MV-String8";
					case PropertyType.MVUnicode:
						return "MV-Unicode";
					default:
						if (inType == PropertyType.MVSysTime)
						{
							return "MV-SysTime";
						}
						break;
					}
				}
			}
			else if (inType <= PropertyType.MVIInt64)
			{
				if (inType <= PropertyType.MVBinary)
				{
					if (inType == PropertyType.MVGuid)
					{
						return "MV-Guid";
					}
					if (inType == PropertyType.MVBinary)
					{
						return "MV-Binary";
					}
				}
				else
				{
					if (inType == PropertyType.MVInvalid)
					{
						return "MV-Invalid";
					}
					switch (inType)
					{
					case PropertyType.MVINull:
						return "MVI-Null";
					case PropertyType.MVIInt16:
						return "MVI-Int16";
					case PropertyType.MVIInt32:
						return "MVI-Int32";
					case PropertyType.MVIReal32:
						return "MVI-Real32";
					case PropertyType.MVIReal64:
						return "MVI-Real64";
					case PropertyType.MVICurrency:
						return "MVI-Currency";
					case PropertyType.MVIAppTime:
						return "MVI-AppTime";
					default:
						if (inType == PropertyType.MVIInt64)
						{
							return "MVI-Int64";
						}
						break;
					}
				}
			}
			else if (inType <= PropertyType.MVISysTime)
			{
				switch (inType)
				{
				case PropertyType.MVIString8:
					return "MVI-String8";
				case PropertyType.MVIUnicode:
					return "MVI-Unicode";
				default:
					if (inType == PropertyType.MVISysTime)
					{
						return "MVI-SysTime";
					}
					break;
				}
			}
			else
			{
				if (inType == PropertyType.MVIGuid)
				{
					return "MVI-Guid";
				}
				if (inType == PropertyType.MVIBinary)
				{
					return "MVI-Binary";
				}
				if (inType == PropertyType.MVIInvalid)
				{
					return "MVI-Invalid";
				}
			}
			ushort num = (ushort)inType;
			return num.ToString("X4", CultureInfo.InvariantCulture);
		}

		public static Type ClrTypeFromPropType(PropertyType propType)
		{
			if (propType <= PropertyType.MVInt64)
			{
				if (propType <= PropertyType.SysTime)
				{
					if (propType <= PropertyType.Int64)
					{
						switch (propType)
						{
						case PropertyType.Int16:
							return typeof(short);
						case PropertyType.Int32:
							return typeof(int);
						case PropertyType.Real32:
							return typeof(float);
						case PropertyType.Real64:
							return typeof(double);
						case PropertyType.Currency:
							return typeof(long);
						case PropertyType.AppTime:
							return typeof(double);
						case (PropertyType)8:
						case (PropertyType)9:
						case PropertyType.Error:
						case (PropertyType)12:
							break;
						case PropertyType.Boolean:
							return typeof(bool);
						case PropertyType.Object:
							return typeof(byte[]);
						default:
							if (propType == PropertyType.Int64)
							{
								return typeof(long);
							}
							break;
						}
					}
					else
					{
						if (propType == PropertyType.Unicode)
						{
							return typeof(string);
						}
						if (propType == PropertyType.SysTime)
						{
							return typeof(DateTime);
						}
					}
				}
				else if (propType <= PropertyType.Actions)
				{
					if (propType == PropertyType.Guid)
					{
						return typeof(Guid);
					}
					switch (propType)
					{
					case PropertyType.SvrEid:
						return typeof(byte[]);
					case PropertyType.SRestriction:
						return typeof(byte[]);
					case PropertyType.Actions:
						return typeof(byte[]);
					}
				}
				else
				{
					if (propType == PropertyType.Binary)
					{
						return typeof(byte[]);
					}
					switch (propType)
					{
					case PropertyType.MVInt16:
						return typeof(short[]);
					case PropertyType.MVInt32:
						return typeof(int[]);
					case PropertyType.MVReal32:
						return typeof(float[]);
					case PropertyType.MVReal64:
						return typeof(double[]);
					case PropertyType.MVCurrency:
						return typeof(long[]);
					case PropertyType.MVAppTime:
						return typeof(double[]);
					default:
						if (propType == PropertyType.MVInt64)
						{
							return typeof(long[]);
						}
						break;
					}
				}
			}
			else if (propType <= PropertyType.MVIAppTime)
			{
				if (propType <= PropertyType.MVSysTime)
				{
					if (propType == PropertyType.MVUnicode)
					{
						return typeof(string[]);
					}
					if (propType == PropertyType.MVSysTime)
					{
						return typeof(DateTime[]);
					}
				}
				else
				{
					if (propType == PropertyType.MVGuid)
					{
						return typeof(Guid[]);
					}
					if (propType == PropertyType.MVBinary)
					{
						return typeof(byte[][]);
					}
					switch (propType)
					{
					case PropertyType.MVIInt16:
						return typeof(short);
					case PropertyType.MVIInt32:
						return typeof(int);
					case PropertyType.MVIReal32:
						return typeof(float);
					case PropertyType.MVIReal64:
						return typeof(double);
					case PropertyType.MVICurrency:
						return typeof(long);
					case PropertyType.MVIAppTime:
						return typeof(double);
					}
				}
			}
			else if (propType <= PropertyType.MVIUnicode)
			{
				if (propType == PropertyType.MVIInt64)
				{
					return typeof(long);
				}
				if (propType == PropertyType.MVIUnicode)
				{
					return typeof(string);
				}
			}
			else
			{
				if (propType == PropertyType.MVISysTime)
				{
					return typeof(DateTime);
				}
				if (propType == PropertyType.MVIGuid)
				{
					return typeof(Guid);
				}
				if (propType == PropertyType.MVIBinary)
				{
					return typeof(byte[]);
				}
			}
			throw new ArgumentException("invalid property type: " + propType.ToString());
		}

		public static PropertyType PropTypeFromClrType(Type type)
		{
			ExtendedTypeCode extendedTypeCodeNoAssert = ValueTypeHelper.GetExtendedTypeCodeNoAssert(type);
			return PropertyTypeHelper.PropTypeFromExtendedTypeCode(extendedTypeCodeNoAssert);
		}

		public static PropertyType PropTypeFromExtendedTypeCode(ExtendedTypeCode extendedTypeCode)
		{
			switch (extendedTypeCode)
			{
			case ExtendedTypeCode.Boolean:
				return PropertyType.Boolean;
			case ExtendedTypeCode.Int16:
				return PropertyType.Int16;
			case ExtendedTypeCode.Int32:
				return PropertyType.Int32;
			case ExtendedTypeCode.Int64:
				return PropertyType.Int64;
			case ExtendedTypeCode.Single:
				return PropertyType.Real32;
			case ExtendedTypeCode.Double:
				return PropertyType.Real64;
			case ExtendedTypeCode.DateTime:
				return PropertyType.SysTime;
			case ExtendedTypeCode.Guid:
				return PropertyType.Guid;
			case ExtendedTypeCode.String:
				return PropertyType.Unicode;
			case ExtendedTypeCode.Binary:
				return PropertyType.Binary;
			case ExtendedTypeCode.MVInt16:
				return PropertyType.MVInt16;
			case ExtendedTypeCode.MVInt32:
				return PropertyType.MVInt32;
			case ExtendedTypeCode.MVInt64:
				return PropertyType.MVInt64;
			case ExtendedTypeCode.MVSingle:
				return PropertyType.MVReal32;
			case ExtendedTypeCode.MVDouble:
				return PropertyType.MVReal64;
			case ExtendedTypeCode.MVDateTime:
				return PropertyType.MVSysTime;
			case ExtendedTypeCode.MVGuid:
				return PropertyType.MVGuid;
			case ExtendedTypeCode.MVString:
				return PropertyType.MVUnicode;
			case ExtendedTypeCode.MVBinary:
				return PropertyType.MVBinary;
			}
			throw new ArgumentException("unexpected value type: " + extendedTypeCode.ToString());
		}

		public static int SizeFromPropType(PropertyType inType)
		{
			if (inType <= PropertyType.MVInt64)
			{
				if (inType <= PropertyType.SysTime)
				{
					if (inType <= PropertyType.Int64)
					{
						switch (inType)
						{
						case PropertyType.Int16:
							goto IL_176;
						case PropertyType.Int32:
						case PropertyType.Real32:
						case PropertyType.AppTime:
							goto IL_17A;
						case PropertyType.Real64:
						case PropertyType.Currency:
							goto IL_17E;
						case (PropertyType)8:
						case (PropertyType)9:
						case PropertyType.Error:
						case (PropertyType)12:
							goto IL_187;
						case PropertyType.Boolean:
							return 1;
						case PropertyType.Object:
							break;
						default:
							if (inType != PropertyType.Int64)
							{
								goto IL_187;
							}
							goto IL_17E;
						}
					}
					else if (inType != PropertyType.Unicode)
					{
						if (inType != PropertyType.SysTime)
						{
							goto IL_187;
						}
						goto IL_17E;
					}
				}
				else if (inType <= PropertyType.Actions)
				{
					if (inType == PropertyType.Guid)
					{
						goto IL_182;
					}
					switch (inType)
					{
					case PropertyType.SvrEid:
					case PropertyType.SRestriction:
					case PropertyType.Actions:
						break;
					case (PropertyType)252:
						goto IL_187;
					default:
						goto IL_187;
					}
				}
				else if (inType != PropertyType.Binary)
				{
					switch (inType)
					{
					case PropertyType.MVInt16:
					case PropertyType.MVInt32:
					case PropertyType.MVReal32:
					case PropertyType.MVReal64:
					case PropertyType.MVCurrency:
					case PropertyType.MVAppTime:
						break;
					default:
						if (inType != PropertyType.MVInt64)
						{
							goto IL_187;
						}
						break;
					}
				}
			}
			else if (inType <= PropertyType.MVIAppTime)
			{
				if (inType <= PropertyType.MVSysTime)
				{
					if (inType != PropertyType.MVUnicode && inType != PropertyType.MVSysTime)
					{
						goto IL_187;
					}
				}
				else if (inType != PropertyType.MVGuid && inType != PropertyType.MVBinary)
				{
					switch (inType)
					{
					case PropertyType.MVIInt16:
						goto IL_176;
					case PropertyType.MVIInt32:
					case PropertyType.MVIReal32:
					case PropertyType.MVIAppTime:
						goto IL_17A;
					case PropertyType.MVIReal64:
					case PropertyType.MVICurrency:
						goto IL_17E;
					default:
						goto IL_187;
					}
				}
			}
			else if (inType <= PropertyType.MVIUnicode)
			{
				if (inType == PropertyType.MVIInt64)
				{
					goto IL_17E;
				}
				if (inType != PropertyType.MVIUnicode)
				{
					goto IL_187;
				}
			}
			else
			{
				if (inType == PropertyType.MVISysTime)
				{
					goto IL_17E;
				}
				if (inType == PropertyType.MVIGuid)
				{
					goto IL_182;
				}
				if (inType != PropertyType.MVIBinary)
				{
					goto IL_187;
				}
			}
			return 0;
			IL_176:
			return 2;
			IL_17A:
			return 4;
			IL_17E:
			return 8;
			IL_182:
			return 16;
			IL_187:
			throw new ArgumentException("SizeFromPropTypeForIndex: type " + inType + " is NYI");
		}

		public static int MaxLengthFromPropType(PropertyType inType)
		{
			if (inType <= PropertyType.MVInt64)
			{
				if (inType <= PropertyType.SysTime)
				{
					if (inType <= PropertyType.Int64)
					{
						switch (inType)
						{
						case PropertyType.Int16:
						case PropertyType.Int32:
						case PropertyType.Real32:
						case PropertyType.Real64:
						case PropertyType.Currency:
						case PropertyType.AppTime:
						case PropertyType.Boolean:
							goto IL_17B;
						case (PropertyType)8:
						case (PropertyType)9:
						case PropertyType.Error:
						case (PropertyType)12:
							goto IL_17F;
						case PropertyType.Object:
							break;
						default:
							if (inType != PropertyType.Int64)
							{
								goto IL_17F;
							}
							goto IL_17B;
						}
					}
					else if (inType != PropertyType.Unicode)
					{
						if (inType != PropertyType.SysTime)
						{
							goto IL_17F;
						}
						goto IL_17B;
					}
				}
				else if (inType <= PropertyType.Actions)
				{
					if (inType == PropertyType.Guid)
					{
						goto IL_17B;
					}
					switch (inType)
					{
					case PropertyType.SvrEid:
						return 1048576;
					case (PropertyType)252:
						goto IL_17F;
					case PropertyType.SRestriction:
					case PropertyType.Actions:
						break;
					default:
						goto IL_17F;
					}
				}
				else if (inType != PropertyType.Binary)
				{
					switch (inType)
					{
					case PropertyType.MVInt16:
					case PropertyType.MVInt32:
					case PropertyType.MVReal32:
					case PropertyType.MVReal64:
					case PropertyType.MVCurrency:
					case PropertyType.MVAppTime:
						break;
					default:
						if (inType != PropertyType.MVInt64)
						{
							goto IL_17F;
						}
						break;
					}
				}
			}
			else if (inType <= PropertyType.MVIAppTime)
			{
				if (inType <= PropertyType.MVSysTime)
				{
					if (inType != PropertyType.MVUnicode && inType != PropertyType.MVSysTime)
					{
						goto IL_17F;
					}
				}
				else if (inType != PropertyType.MVGuid && inType != PropertyType.MVBinary)
				{
					switch (inType)
					{
					case PropertyType.MVIInt16:
					case PropertyType.MVIInt32:
					case PropertyType.MVIReal32:
					case PropertyType.MVIReal64:
					case PropertyType.MVICurrency:
					case PropertyType.MVIAppTime:
						goto IL_17B;
					default:
						goto IL_17F;
					}
				}
			}
			else if (inType <= PropertyType.MVIUnicode)
			{
				if (inType == PropertyType.MVIInt64)
				{
					goto IL_17B;
				}
				if (inType != PropertyType.MVIUnicode)
				{
					goto IL_17F;
				}
			}
			else
			{
				if (inType == PropertyType.MVISysTime || inType == PropertyType.MVIGuid)
				{
					goto IL_17B;
				}
				if (inType != PropertyType.MVIBinary)
				{
					goto IL_17F;
				}
			}
			return 1073741823;
			IL_17B:
			return 0;
			IL_17F:
			throw new ArgumentException("MaxLengthFromPropTypeForIndex: type " + inType + " is NYI");
		}

		public static PropertyType MapToInternalPropertyType(PropertyType externalPropType)
		{
			if (externalPropType <= PropertyType.MVUnicode)
			{
				if (externalPropType <= PropertyType.Guid)
				{
					if (externalPropType <= PropertyType.Unicode)
					{
						switch (externalPropType)
						{
						case PropertyType.Unspecified:
						case PropertyType.Null:
						case PropertyType.Int16:
						case PropertyType.Int32:
						case PropertyType.Real32:
						case PropertyType.Real64:
						case PropertyType.Currency:
						case PropertyType.AppTime:
						case PropertyType.Error:
						case PropertyType.Boolean:
						case PropertyType.Object:
						case PropertyType.Int64:
							break;
						case (PropertyType)8:
						case (PropertyType)9:
						case (PropertyType)12:
						case (PropertyType)14:
						case (PropertyType)15:
						case (PropertyType)16:
						case (PropertyType)17:
						case (PropertyType)18:
						case (PropertyType)19:
							return PropertyType.Invalid;
						default:
							switch (externalPropType)
							{
							case PropertyType.String8:
								return PropertyType.Unicode;
							case PropertyType.Unicode:
								break;
							default:
								return PropertyType.Invalid;
							}
							break;
						}
					}
					else if (externalPropType != PropertyType.SysTime && externalPropType != PropertyType.Guid)
					{
						return PropertyType.Invalid;
					}
				}
				else if (externalPropType <= PropertyType.Binary)
				{
					switch (externalPropType)
					{
					case PropertyType.SvrEid:
					case PropertyType.SRestriction:
					case PropertyType.Actions:
						break;
					case (PropertyType)252:
						return PropertyType.Invalid;
					default:
						if (externalPropType != PropertyType.Binary)
						{
							return PropertyType.Invalid;
						}
						break;
					}
				}
				else
				{
					switch (externalPropType)
					{
					case PropertyType.MVInt16:
					case PropertyType.MVInt32:
					case PropertyType.MVReal32:
					case PropertyType.MVReal64:
					case PropertyType.MVCurrency:
					case PropertyType.MVAppTime:
						break;
					default:
						if (externalPropType != PropertyType.MVInt64)
						{
							switch (externalPropType)
							{
							case PropertyType.MVString8:
								return PropertyType.MVUnicode;
							case PropertyType.MVUnicode:
								break;
							default:
								return PropertyType.Invalid;
							}
						}
						break;
					}
				}
			}
			else if (externalPropType <= PropertyType.MVIAppTime)
			{
				if (externalPropType <= PropertyType.MVGuid)
				{
					if (externalPropType != PropertyType.MVSysTime && externalPropType != PropertyType.MVGuid)
					{
						return PropertyType.Invalid;
					}
				}
				else if (externalPropType != PropertyType.MVBinary)
				{
					switch (externalPropType)
					{
					case PropertyType.MVIInt16:
					case PropertyType.MVIInt32:
					case PropertyType.MVIReal32:
					case PropertyType.MVIReal64:
					case PropertyType.MVICurrency:
					case PropertyType.MVIAppTime:
						break;
					default:
						return PropertyType.Invalid;
					}
				}
			}
			else if (externalPropType <= PropertyType.MVIUnicode)
			{
				if (externalPropType != PropertyType.MVIInt64)
				{
					switch (externalPropType)
					{
					case PropertyType.MVIString8:
						return PropertyType.MVIUnicode;
					case PropertyType.MVIUnicode:
						break;
					default:
						return PropertyType.Invalid;
					}
				}
			}
			else if (externalPropType != PropertyType.MVISysTime && externalPropType != PropertyType.MVIGuid && externalPropType != PropertyType.MVIBinary)
			{
				return PropertyType.Invalid;
			}
			return externalPropType;
		}

		public static ExtendedTypeCode GetExtendedTypeCode(PropertyType propertyType)
		{
			if (propertyType <= PropertyType.MVInt64)
			{
				if (propertyType <= PropertyType.SysTime)
				{
					if (propertyType <= PropertyType.Int64)
					{
						switch (propertyType)
						{
						case PropertyType.Null:
							return ExtendedTypeCode.Invalid;
						case PropertyType.Int16:
							return ExtendedTypeCode.Int16;
						case PropertyType.Int32:
							return ExtendedTypeCode.Int32;
						case PropertyType.Real32:
							return ExtendedTypeCode.Single;
						case PropertyType.Real64:
							return ExtendedTypeCode.Double;
						case PropertyType.Currency:
							return ExtendedTypeCode.Int64;
						case PropertyType.AppTime:
							return ExtendedTypeCode.Double;
						case (PropertyType)8:
						case (PropertyType)9:
						case PropertyType.Error:
						case (PropertyType)12:
							return ExtendedTypeCode.Invalid;
						case PropertyType.Boolean:
							return ExtendedTypeCode.Boolean;
						case PropertyType.Object:
							break;
						default:
							if (propertyType != PropertyType.Int64)
							{
								return ExtendedTypeCode.Invalid;
							}
							return ExtendedTypeCode.Int64;
						}
					}
					else
					{
						if (propertyType == PropertyType.Unicode)
						{
							return ExtendedTypeCode.String;
						}
						if (propertyType != PropertyType.SysTime)
						{
							return ExtendedTypeCode.Invalid;
						}
						return ExtendedTypeCode.DateTime;
					}
				}
				else if (propertyType <= PropertyType.Actions)
				{
					if (propertyType == PropertyType.Guid)
					{
						return ExtendedTypeCode.Guid;
					}
					switch (propertyType)
					{
					case PropertyType.SvrEid:
					case PropertyType.SRestriction:
					case PropertyType.Actions:
						break;
					case (PropertyType)252:
						return ExtendedTypeCode.Invalid;
					default:
						return ExtendedTypeCode.Invalid;
					}
				}
				else if (propertyType != PropertyType.Binary)
				{
					switch (propertyType)
					{
					case PropertyType.MVInt16:
						return ExtendedTypeCode.MVInt16;
					case PropertyType.MVInt32:
						return ExtendedTypeCode.MVInt32;
					case PropertyType.MVReal32:
						return ExtendedTypeCode.MVSingle;
					case PropertyType.MVReal64:
						return ExtendedTypeCode.MVDouble;
					case PropertyType.MVCurrency:
						return ExtendedTypeCode.MVInt64;
					case PropertyType.MVAppTime:
						return ExtendedTypeCode.MVDouble;
					default:
						if (propertyType != PropertyType.MVInt64)
						{
							return ExtendedTypeCode.Invalid;
						}
						return ExtendedTypeCode.MVInt64;
					}
				}
				return ExtendedTypeCode.Binary;
			}
			if (propertyType <= PropertyType.MVIAppTime)
			{
				if (propertyType <= PropertyType.MVSysTime)
				{
					if (propertyType == PropertyType.MVUnicode)
					{
						return ExtendedTypeCode.MVString;
					}
					if (propertyType == PropertyType.MVSysTime)
					{
						return ExtendedTypeCode.MVDateTime;
					}
				}
				else
				{
					if (propertyType == PropertyType.MVGuid)
					{
						return ExtendedTypeCode.MVGuid;
					}
					if (propertyType == PropertyType.MVBinary)
					{
						return ExtendedTypeCode.MVBinary;
					}
					switch (propertyType)
					{
					case PropertyType.MVIInt16:
						return ExtendedTypeCode.Int16;
					case PropertyType.MVIInt32:
						return ExtendedTypeCode.Int32;
					case PropertyType.MVIReal32:
						return ExtendedTypeCode.Single;
					case PropertyType.MVIReal64:
						return ExtendedTypeCode.Double;
					case PropertyType.MVICurrency:
						return ExtendedTypeCode.Int64;
					case PropertyType.MVIAppTime:
						return ExtendedTypeCode.Double;
					}
				}
			}
			else if (propertyType <= PropertyType.MVIUnicode)
			{
				if (propertyType == PropertyType.MVIInt64)
				{
					return ExtendedTypeCode.Int64;
				}
				if (propertyType == PropertyType.MVIUnicode)
				{
					return ExtendedTypeCode.String;
				}
			}
			else
			{
				if (propertyType == PropertyType.MVISysTime)
				{
					return ExtendedTypeCode.DateTime;
				}
				if (propertyType == PropertyType.MVIGuid)
				{
					return ExtendedTypeCode.Guid;
				}
				if (propertyType == PropertyType.MVIBinary)
				{
					return ExtendedTypeCode.Binary;
				}
			}
			return ExtendedTypeCode.Invalid;
		}
	}
}
