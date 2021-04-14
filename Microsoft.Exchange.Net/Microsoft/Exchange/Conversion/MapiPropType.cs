using System;

namespace Microsoft.Exchange.Conversion
{
	internal enum MapiPropType
	{
		Null = 1,
		Short,
		Int,
		Float,
		Double,
		Currency,
		AppTime,
		Error = 10,
		Boolean,
		Object = 13,
		Long = 20,
		AnsiString = 30,
		String,
		SysTime = 64,
		Guid = 72,
		ServerId = 251,
		Binary = 258,
		ShortArray = 4098,
		IntArray,
		FloatArray,
		DoubleArray,
		CurrencyArray,
		AppTimeArray,
		BooleanArray = 4107,
		ObjectArray = 4109,
		LongArray = 4116,
		AnsiStringArray = 4126,
		StringArray,
		SysTimeArray = 4160,
		GuidArray = 4168,
		BinaryArray = 4354
	}
}
