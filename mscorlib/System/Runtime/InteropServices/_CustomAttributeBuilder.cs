using System;
using System.Reflection.Emit;

namespace System.Runtime.InteropServices
{
	[Guid("BE9ACCE8-AAFF-3B91-81AE-8211663F5CAD")]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[CLSCompliant(false)]
	[TypeLibImportClass(typeof(CustomAttributeBuilder))]
	[ComVisible(true)]
	public interface _CustomAttributeBuilder
	{
		void GetTypeInfoCount(out uint pcTInfo);

		void GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo);

		void GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId);

		void Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr);
	}
}
