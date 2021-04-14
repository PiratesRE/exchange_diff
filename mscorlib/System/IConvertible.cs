using System;
using System.Runtime.InteropServices;

namespace System
{
	[CLSCompliant(false)]
	[ComVisible(true)]
	[__DynamicallyInvokable]
	public interface IConvertible
	{
		[__DynamicallyInvokable]
		TypeCode GetTypeCode();

		[__DynamicallyInvokable]
		bool ToBoolean(IFormatProvider provider);

		[__DynamicallyInvokable]
		char ToChar(IFormatProvider provider);

		[__DynamicallyInvokable]
		sbyte ToSByte(IFormatProvider provider);

		[__DynamicallyInvokable]
		byte ToByte(IFormatProvider provider);

		[__DynamicallyInvokable]
		short ToInt16(IFormatProvider provider);

		[__DynamicallyInvokable]
		ushort ToUInt16(IFormatProvider provider);

		[__DynamicallyInvokable]
		int ToInt32(IFormatProvider provider);

		[__DynamicallyInvokable]
		uint ToUInt32(IFormatProvider provider);

		[__DynamicallyInvokable]
		long ToInt64(IFormatProvider provider);

		[__DynamicallyInvokable]
		ulong ToUInt64(IFormatProvider provider);

		[__DynamicallyInvokable]
		float ToSingle(IFormatProvider provider);

		[__DynamicallyInvokable]
		double ToDouble(IFormatProvider provider);

		[__DynamicallyInvokable]
		decimal ToDecimal(IFormatProvider provider);

		[__DynamicallyInvokable]
		DateTime ToDateTime(IFormatProvider provider);

		[__DynamicallyInvokable]
		string ToString(IFormatProvider provider);

		[__DynamicallyInvokable]
		object ToType(Type conversionType, IFormatProvider provider);
	}
}
