using System;

namespace System.Reflection
{
	[Flags]
	[Serializable]
	internal enum MdSigCallingConvention : byte
	{
		CallConvMask = 15,
		Default = 0,
		C = 1,
		StdCall = 2,
		ThisCall = 3,
		FastCall = 4,
		Vararg = 5,
		Field = 6,
		LocalSig = 7,
		Property = 8,
		Unmgd = 9,
		GenericInst = 10,
		Generic = 16,
		HasThis = 32,
		ExplicitThis = 64
	}
}
