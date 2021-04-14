using System;

namespace System.Diagnostics.Tracing
{
	internal enum TraceLoggingDataType
	{
		Nil,
		Utf16String,
		MbcsString,
		Int8,
		UInt8,
		Int16,
		UInt16,
		Int32,
		UInt32,
		Int64,
		UInt64,
		Float,
		Double,
		Boolean32,
		Binary,
		Guid,
		FileTime = 17,
		SystemTime,
		HexInt32 = 20,
		HexInt64,
		CountedUtf16String,
		CountedMbcsString,
		Struct,
		Char16 = 518,
		Char8 = 516,
		Boolean8 = 772,
		HexInt8 = 1028,
		HexInt16 = 1030,
		Utf16Xml = 2817,
		MbcsXml,
		CountedUtf16Xml = 2838,
		CountedMbcsXml,
		Utf16Json = 3073,
		MbcsJson,
		CountedUtf16Json = 3094,
		CountedMbcsJson,
		HResult = 3847
	}
}
