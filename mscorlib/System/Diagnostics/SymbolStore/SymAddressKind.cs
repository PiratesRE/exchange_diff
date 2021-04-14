using System;
using System.Runtime.InteropServices;

namespace System.Diagnostics.SymbolStore
{
	[ComVisible(true)]
	[Serializable]
	public enum SymAddressKind
	{
		ILOffset = 1,
		NativeRVA,
		NativeRegister,
		NativeRegisterRelative,
		NativeOffset,
		NativeRegisterRegister,
		NativeRegisterStack,
		NativeStackRegister,
		BitField,
		NativeSectionOffset
	}
}
