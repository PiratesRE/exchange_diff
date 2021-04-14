using System;

namespace Microsoft.Mapi
{
	internal enum PropType
	{
		Unspecified,
		Null,
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
		Restriction = 253,
		Actions,
		Binary = 258,
		MultiValueFlag = 4096,
		ShortArray = 4098,
		IntArray,
		FloatArray,
		DoubleArray,
		CurrencyArray,
		AppTimeArray,
		ObjectArray = 4109,
		LongArray = 4116,
		AnsiStringArray = 4126,
		StringArray,
		SysTimeArray = 4160,
		GuidArray = 4168,
		BinaryArray = 4354,
		MultiInstanceFlag = 8192
	}
}
