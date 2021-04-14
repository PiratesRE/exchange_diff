using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Security;

namespace System
{
	[Serializable]
	internal struct Variant
	{
		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern double GetR8FromVar();

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern float GetR4FromVar();

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetFieldsR4(float val);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetFieldsR8(double val);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void SetFieldsObject(object val);

		internal long GetI8FromVar()
		{
			return (long)this.m_data2 << 32 | ((long)this.m_data1 & (long)((ulong)-1));
		}

		internal Variant(int flags, object or, int data1, int data2)
		{
			this.m_flags = flags;
			this.m_objref = or;
			this.m_data1 = data1;
			this.m_data2 = data2;
		}

		public Variant(bool val)
		{
			this.m_objref = null;
			this.m_flags = 2;
			this.m_data1 = (val ? 1 : 0);
			this.m_data2 = 0;
		}

		public Variant(sbyte val)
		{
			this.m_objref = null;
			this.m_flags = 4;
			this.m_data1 = (int)val;
			this.m_data2 = (int)((long)val >> 32);
		}

		public Variant(byte val)
		{
			this.m_objref = null;
			this.m_flags = 5;
			this.m_data1 = (int)val;
			this.m_data2 = 0;
		}

		public Variant(short val)
		{
			this.m_objref = null;
			this.m_flags = 6;
			this.m_data1 = (int)val;
			this.m_data2 = (int)((long)val >> 32);
		}

		public Variant(ushort val)
		{
			this.m_objref = null;
			this.m_flags = 7;
			this.m_data1 = (int)val;
			this.m_data2 = 0;
		}

		public Variant(char val)
		{
			this.m_objref = null;
			this.m_flags = 3;
			this.m_data1 = (int)val;
			this.m_data2 = 0;
		}

		public Variant(int val)
		{
			this.m_objref = null;
			this.m_flags = 8;
			this.m_data1 = val;
			this.m_data2 = val >> 31;
		}

		public Variant(uint val)
		{
			this.m_objref = null;
			this.m_flags = 9;
			this.m_data1 = (int)val;
			this.m_data2 = 0;
		}

		public Variant(long val)
		{
			this.m_objref = null;
			this.m_flags = 10;
			this.m_data1 = (int)val;
			this.m_data2 = (int)(val >> 32);
		}

		public Variant(ulong val)
		{
			this.m_objref = null;
			this.m_flags = 11;
			this.m_data1 = (int)val;
			this.m_data2 = (int)(val >> 32);
		}

		[SecuritySafeCritical]
		public Variant(float val)
		{
			this.m_objref = null;
			this.m_flags = 12;
			this.m_data1 = 0;
			this.m_data2 = 0;
			this.SetFieldsR4(val);
		}

		[SecurityCritical]
		public Variant(double val)
		{
			this.m_objref = null;
			this.m_flags = 13;
			this.m_data1 = 0;
			this.m_data2 = 0;
			this.SetFieldsR8(val);
		}

		public Variant(DateTime val)
		{
			this.m_objref = null;
			this.m_flags = 16;
			ulong ticks = (ulong)val.Ticks;
			this.m_data1 = (int)ticks;
			this.m_data2 = (int)(ticks >> 32);
		}

		public Variant(decimal val)
		{
			this.m_objref = val;
			this.m_flags = 19;
			this.m_data1 = 0;
			this.m_data2 = 0;
		}

		[SecuritySafeCritical]
		public Variant(object obj)
		{
			this.m_data1 = 0;
			this.m_data2 = 0;
			VarEnum varEnum = VarEnum.VT_EMPTY;
			if (obj is DateTime)
			{
				this.m_objref = null;
				this.m_flags = 16;
				ulong ticks = (ulong)((DateTime)obj).Ticks;
				this.m_data1 = (int)ticks;
				this.m_data2 = (int)(ticks >> 32);
				return;
			}
			if (obj is string)
			{
				this.m_flags = 14;
				this.m_objref = obj;
				return;
			}
			if (obj == null)
			{
				this = System.Variant.Empty;
				return;
			}
			if (obj == System.DBNull.Value)
			{
				this = System.Variant.DBNull;
				return;
			}
			if (obj == Type.Missing)
			{
				this = System.Variant.Missing;
				return;
			}
			if (obj is Array)
			{
				this.m_flags = 65554;
				this.m_objref = obj;
				return;
			}
			this.m_flags = 0;
			this.m_objref = null;
			if (obj is UnknownWrapper)
			{
				varEnum = VarEnum.VT_UNKNOWN;
				obj = ((UnknownWrapper)obj).WrappedObject;
			}
			else if (obj is DispatchWrapper)
			{
				varEnum = VarEnum.VT_DISPATCH;
				obj = ((DispatchWrapper)obj).WrappedObject;
			}
			else if (obj is ErrorWrapper)
			{
				varEnum = VarEnum.VT_ERROR;
				obj = ((ErrorWrapper)obj).ErrorCode;
			}
			else if (obj is CurrencyWrapper)
			{
				varEnum = VarEnum.VT_CY;
				obj = ((CurrencyWrapper)obj).WrappedObject;
			}
			else if (obj is BStrWrapper)
			{
				varEnum = VarEnum.VT_BSTR;
				obj = ((BStrWrapper)obj).WrappedObject;
			}
			if (obj != null)
			{
				this.SetFieldsObject(obj);
			}
			if (varEnum != VarEnum.VT_EMPTY)
			{
				this.m_flags |= (int)((int)varEnum << 24);
			}
		}

		[SecurityCritical]
		public unsafe Variant(void* voidPointer, Type pointerType)
		{
			if (pointerType == null)
			{
				throw new ArgumentNullException("pointerType");
			}
			if (!pointerType.IsPointer)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_MustBePointer"), "pointerType");
			}
			this.m_objref = pointerType;
			this.m_flags = 15;
			this.m_data1 = voidPointer;
			this.m_data2 = 0;
		}

		internal int CVType
		{
			get
			{
				return this.m_flags & 65535;
			}
		}

		[SecuritySafeCritical]
		public object ToObject()
		{
			switch (this.CVType)
			{
			case 0:
				return null;
			case 2:
				return this.m_data1 != 0;
			case 3:
				return (char)this.m_data1;
			case 4:
				return (sbyte)this.m_data1;
			case 5:
				return (byte)this.m_data1;
			case 6:
				return (short)this.m_data1;
			case 7:
				return (ushort)this.m_data1;
			case 8:
				return this.m_data1;
			case 9:
				return (uint)this.m_data1;
			case 10:
				return this.GetI8FromVar();
			case 11:
				return (ulong)this.GetI8FromVar();
			case 12:
				return this.GetR4FromVar();
			case 13:
				return this.GetR8FromVar();
			case 16:
				return new DateTime(this.GetI8FromVar());
			case 17:
				return new TimeSpan(this.GetI8FromVar());
			case 21:
				return this.BoxEnum();
			case 22:
				return Type.Missing;
			case 23:
				return System.DBNull.Value;
			}
			return this.m_objref;
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private extern object BoxEnum();

		[SecuritySafeCritical]
		internal static void MarshalHelperConvertObjectToVariant(object o, ref System.Variant v)
		{
			IConvertible convertible = RemotingServices.IsTransparentProxy(o) ? null : (o as IConvertible);
			if (o == null)
			{
				v = System.Variant.Empty;
				return;
			}
			if (convertible == null)
			{
				v = new System.Variant(o);
				return;
			}
			IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
			switch (convertible.GetTypeCode())
			{
			case TypeCode.Empty:
				v = System.Variant.Empty;
				return;
			case TypeCode.Object:
				v = new System.Variant(o);
				return;
			case TypeCode.DBNull:
				v = System.Variant.DBNull;
				return;
			case TypeCode.Boolean:
				v = new System.Variant(convertible.ToBoolean(invariantCulture));
				return;
			case TypeCode.Char:
				v = new System.Variant(convertible.ToChar(invariantCulture));
				return;
			case TypeCode.SByte:
				v = new System.Variant(convertible.ToSByte(invariantCulture));
				return;
			case TypeCode.Byte:
				v = new System.Variant(convertible.ToByte(invariantCulture));
				return;
			case TypeCode.Int16:
				v = new System.Variant(convertible.ToInt16(invariantCulture));
				return;
			case TypeCode.UInt16:
				v = new System.Variant(convertible.ToUInt16(invariantCulture));
				return;
			case TypeCode.Int32:
				v = new System.Variant(convertible.ToInt32(invariantCulture));
				return;
			case TypeCode.UInt32:
				v = new System.Variant(convertible.ToUInt32(invariantCulture));
				return;
			case TypeCode.Int64:
				v = new System.Variant(convertible.ToInt64(invariantCulture));
				return;
			case TypeCode.UInt64:
				v = new System.Variant(convertible.ToUInt64(invariantCulture));
				return;
			case TypeCode.Single:
				v = new System.Variant(convertible.ToSingle(invariantCulture));
				return;
			case TypeCode.Double:
				v = new System.Variant(convertible.ToDouble(invariantCulture));
				return;
			case TypeCode.Decimal:
				v = new System.Variant(convertible.ToDecimal(invariantCulture));
				return;
			case TypeCode.DateTime:
				v = new System.Variant(convertible.ToDateTime(invariantCulture));
				return;
			case TypeCode.String:
				v = new System.Variant(convertible.ToString(invariantCulture));
				return;
			}
			throw new NotSupportedException(Environment.GetResourceString("NotSupported_UnknownTypeCode", new object[]
			{
				convertible.GetTypeCode()
			}));
		}

		internal static object MarshalHelperConvertVariantToObject(ref System.Variant v)
		{
			return v.ToObject();
		}

		[SecurityCritical]
		internal static void MarshalHelperCastVariant(object pValue, int vt, ref System.Variant v)
		{
			IConvertible convertible = pValue as IConvertible;
			if (convertible == null)
			{
				switch (vt)
				{
				case 8:
					if (pValue == null)
					{
						v = new System.Variant(null);
						v.m_flags = 14;
						return;
					}
					throw new InvalidCastException(Environment.GetResourceString("InvalidCast_CannotCoerceByRefVariant"));
				case 9:
					v = new System.Variant(new DispatchWrapper(pValue));
					return;
				case 10:
				case 11:
					break;
				case 12:
					v = new System.Variant(pValue);
					return;
				case 13:
					v = new System.Variant(new UnknownWrapper(pValue));
					return;
				default:
					if (vt == 36)
					{
						v = new System.Variant(pValue);
						return;
					}
					break;
				}
				throw new InvalidCastException(Environment.GetResourceString("InvalidCast_CannotCoerceByRefVariant"));
			}
			IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
			switch (vt)
			{
			case 0:
				v = System.Variant.Empty;
				return;
			case 1:
				v = System.Variant.DBNull;
				return;
			case 2:
				v = new System.Variant(convertible.ToInt16(invariantCulture));
				return;
			case 3:
				v = new System.Variant(convertible.ToInt32(invariantCulture));
				return;
			case 4:
				v = new System.Variant(convertible.ToSingle(invariantCulture));
				return;
			case 5:
				v = new System.Variant(convertible.ToDouble(invariantCulture));
				return;
			case 6:
				v = new System.Variant(new CurrencyWrapper(convertible.ToDecimal(invariantCulture)));
				return;
			case 7:
				v = new System.Variant(convertible.ToDateTime(invariantCulture));
				return;
			case 8:
				v = new System.Variant(convertible.ToString(invariantCulture));
				return;
			case 9:
				v = new System.Variant(new DispatchWrapper(convertible));
				return;
			case 10:
				v = new System.Variant(new ErrorWrapper(convertible.ToInt32(invariantCulture)));
				return;
			case 11:
				v = new System.Variant(convertible.ToBoolean(invariantCulture));
				return;
			case 12:
				v = new System.Variant(convertible);
				return;
			case 13:
				v = new System.Variant(new UnknownWrapper(convertible));
				return;
			case 14:
				v = new System.Variant(convertible.ToDecimal(invariantCulture));
				return;
			case 16:
				v = new System.Variant(convertible.ToSByte(invariantCulture));
				return;
			case 17:
				v = new System.Variant(convertible.ToByte(invariantCulture));
				return;
			case 18:
				v = new System.Variant(convertible.ToUInt16(invariantCulture));
				return;
			case 19:
				v = new System.Variant(convertible.ToUInt32(invariantCulture));
				return;
			case 20:
				v = new System.Variant(convertible.ToInt64(invariantCulture));
				return;
			case 21:
				v = new System.Variant(convertible.ToUInt64(invariantCulture));
				return;
			case 22:
				v = new System.Variant(convertible.ToInt32(invariantCulture));
				return;
			case 23:
				v = new System.Variant(convertible.ToUInt32(invariantCulture));
				return;
			}
			throw new InvalidCastException(Environment.GetResourceString("InvalidCast_CannotCoerceByRefVariant"));
		}

		private object m_objref;

		private int m_data1;

		private int m_data2;

		private int m_flags;

		internal const int CV_EMPTY = 0;

		internal const int CV_VOID = 1;

		internal const int CV_BOOLEAN = 2;

		internal const int CV_CHAR = 3;

		internal const int CV_I1 = 4;

		internal const int CV_U1 = 5;

		internal const int CV_I2 = 6;

		internal const int CV_U2 = 7;

		internal const int CV_I4 = 8;

		internal const int CV_U4 = 9;

		internal const int CV_I8 = 10;

		internal const int CV_U8 = 11;

		internal const int CV_R4 = 12;

		internal const int CV_R8 = 13;

		internal const int CV_STRING = 14;

		internal const int CV_PTR = 15;

		internal const int CV_DATETIME = 16;

		internal const int CV_TIMESPAN = 17;

		internal const int CV_OBJECT = 18;

		internal const int CV_DECIMAL = 19;

		internal const int CV_ENUM = 21;

		internal const int CV_MISSING = 22;

		internal const int CV_NULL = 23;

		internal const int CV_LAST = 24;

		internal const int TypeCodeBitMask = 65535;

		internal const int VTBitMask = -16777216;

		internal const int VTBitShift = 24;

		internal const int ArrayBitMask = 65536;

		internal const int EnumI1 = 1048576;

		internal const int EnumU1 = 2097152;

		internal const int EnumI2 = 3145728;

		internal const int EnumU2 = 4194304;

		internal const int EnumI4 = 5242880;

		internal const int EnumU4 = 6291456;

		internal const int EnumI8 = 7340032;

		internal const int EnumU8 = 8388608;

		internal const int EnumMask = 15728640;

		internal static readonly Type[] ClassTypes = new Type[]
		{
			typeof(Empty),
			typeof(void),
			typeof(bool),
			typeof(char),
			typeof(sbyte),
			typeof(byte),
			typeof(short),
			typeof(ushort),
			typeof(int),
			typeof(uint),
			typeof(long),
			typeof(ulong),
			typeof(float),
			typeof(double),
			typeof(string),
			typeof(void),
			typeof(DateTime),
			typeof(TimeSpan),
			typeof(object),
			typeof(decimal),
			typeof(object),
			typeof(Missing),
			typeof(DBNull)
		};

		internal static readonly System.Variant Empty = default(System.Variant);

		internal static readonly System.Variant Missing = new System.Variant(22, Type.Missing, 0, 0);

		internal static readonly System.Variant DBNull = new System.Variant(23, System.DBNull.Value, 0, 0);
	}
}
