using System;

namespace System.Resources
{
	[Serializable]
	internal enum ResourceTypeCode
	{
		Null,
		String,
		Boolean,
		Char,
		Byte,
		SByte,
		Int16,
		UInt16,
		Int32,
		UInt32,
		Int64,
		UInt64,
		Single,
		Double,
		Decimal,
		DateTime,
		TimeSpan,
		LastPrimitive = 16,
		ByteArray = 32,
		Stream,
		StartOfUserTypes = 64
	}
}
