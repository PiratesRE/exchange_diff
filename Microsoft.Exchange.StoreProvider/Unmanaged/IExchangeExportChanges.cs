using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Guid("a3ea9cc0-d1b2-11cd-80fc-00aa004bba0b")]
	[ComImport]
	internal interface IExchangeExportChanges
	{
		[PreserveSig]
		unsafe int GetLastError(int hResult, int ulFlags, out MAPIERROR* lpMapiError);

		[PreserveSig]
		unsafe int Config(IStream pIStream, int ulFlags, [MarshalAs(UnmanagedType.IUnknown)] [In] object pIUnknown, [In] SRestriction* lpRestriction, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpIncludeProps, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpExcludeProps, int ulBufferSize);

		[PreserveSig]
		int Synchronize(out int lpulSteps, out int lpulProgress);

		[PreserveSig]
		int UpdateState(IStream lpStream);
	}
}
