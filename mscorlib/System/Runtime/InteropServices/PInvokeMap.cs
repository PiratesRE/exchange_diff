using System;

namespace System.Runtime.InteropServices
{
	[Serializable]
	internal enum PInvokeMap
	{
		NoMangle = 1,
		CharSetMask = 6,
		CharSetNotSpec = 0,
		CharSetAnsi = 2,
		CharSetUnicode = 4,
		CharSetAuto = 6,
		PinvokeOLE = 32,
		SupportsLastError = 64,
		BestFitMask = 48,
		BestFitEnabled = 16,
		BestFitDisabled = 32,
		BestFitUseAsm = 48,
		ThrowOnUnmappableCharMask = 12288,
		ThrowOnUnmappableCharEnabled = 4096,
		ThrowOnUnmappableCharDisabled = 8192,
		ThrowOnUnmappableCharUseAsm = 12288,
		CallConvMask = 1792,
		CallConvWinapi = 256,
		CallConvCdecl = 512,
		CallConvStdcall = 768,
		CallConvThiscall = 1024,
		CallConvFastcall = 1280
	}
}
