using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("7846EDBA-8287-4d76-BD5F-1E0513D10E0C")]
	[ComImport]
	internal interface IExchangeImportHierarchyChanges2 : IExchangeImportHierarchyChanges
	{
		[PreserveSig]
		unsafe int GetLastError(int hResult, int ulFlags, out MAPIERROR* lpMapiError);

		[PreserveSig]
		int Config(IStream pIStream, int ulFlags);

		[PreserveSig]
		int UpdateState(IStream pIStream);

		[PreserveSig]
		unsafe int ImportFolderChange(int cpvalChanges, SPropValue* ppvalChanges);

		[PreserveSig]
		unsafe int ImportFolderDeletion(int ulFlags, _SBinaryArray* lpSrcEntryList);

		[PreserveSig]
		int ConfigEx([MarshalAs(UnmanagedType.LPArray)] byte[] pbIdsetGiven, int cbIdsetGiven, [MarshalAs(UnmanagedType.LPArray)] byte[] pbCnsetSeen, int cbCnsetSeen, int ulFlags);

		[PreserveSig]
		int UpdateStateEx(out IntPtr pbIdsetGiven, out int cbIdsetGiven, out IntPtr pbCnsetSeen, out int cbCnsetSeen);
	}
}
