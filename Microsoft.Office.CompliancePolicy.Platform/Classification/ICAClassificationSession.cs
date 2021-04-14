using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Microsoft.Office.CompliancePolicy.Classification
{
	[TypeLibType(TypeLibTypeFlags.FNonExtensible)]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("615A546C-8D4C-4053-A888-7B397AC3EF6E")]
	[ComImport]
	public interface ICAClassificationSession
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		void ClassifyUpdates([MarshalAs(UnmanagedType.BStr)] [In] string bstrText, [In] uint ulRelativeOffset, [In] int lTextShift, [In] uint ulModifiedTextStart, [In] uint ulModifiedTextEnd);

		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: MarshalAs(UnmanagedType.Interface)]
		ICAClassificationResultCollection GetClassificationResults();
	}
}
