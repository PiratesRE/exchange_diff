using System;

namespace System.Reflection
{
	[Serializable]
	internal enum CustomAttributeEncoding
	{
		Undefined,
		Boolean = 2,
		Char,
		SByte,
		Byte,
		Int16,
		UInt16,
		Int32,
		UInt32,
		Int64,
		UInt64,
		Float,
		Double,
		String,
		Array = 29,
		Type = 80,
		Object,
		Field = 83,
		Property,
		Enum
	}
}
