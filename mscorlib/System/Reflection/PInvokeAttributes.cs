using System;

namespace System.Reflection
{
	[Flags]
	[Serializable]
	internal enum PInvokeAttributes
	{
		NoMangle = 1,
		CharSetMask = 6,
		CharSetNotSpec = 0,
		CharSetAnsi = 2,
		CharSetUnicode = 4,
		CharSetAuto = 6,
		BestFitUseAssem = 0,
		BestFitEnabled = 16,
		BestFitDisabled = 32,
		BestFitMask = 48,
		ThrowOnUnmappableCharUseAssem = 0,
		ThrowOnUnmappableCharEnabled = 4096,
		ThrowOnUnmappableCharDisabled = 8192,
		ThrowOnUnmappableCharMask = 12288,
		SupportsLastError = 64,
		CallConvMask = 1792,
		CallConvWinapi = 256,
		CallConvCdecl = 512,
		CallConvStdcall = 768,
		CallConvThiscall = 1024,
		CallConvFastcall = 1280,
		MaxValue = 65535
	}
}
