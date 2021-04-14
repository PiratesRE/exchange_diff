using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace Microsoft.Office.CompliancePolicy.Classification
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("74A483B1-65A8-4F0E-B201-549673C6F2E1")]
	[TypeLibType(TypeLibTypeFlags.FNonExtensible)]
	[ComImport]
	public interface ICAClassificationStreamSession
	{
		[MethodImpl(MethodImplOptions.InternalCall)]
		[return: MarshalAs(UnmanagedType.Interface)]
		ICAClassificationResultCollection ClassifyStream([MarshalAs(UnmanagedType.Interface)] [In] IStream stream);
	}
}
