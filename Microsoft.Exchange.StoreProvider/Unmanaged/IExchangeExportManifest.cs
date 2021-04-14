using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	[Guid("82D370F5-6F10-457d-99F9-11977856A7AA")]
	[ComImport]
	internal interface IExchangeExportManifest
	{
		[PreserveSig]
		unsafe int GetLastError(int hResult, int ulFlags, out MAPIERROR* lpMapiError);

		[PreserveSig]
		unsafe int Config(IStream pIStream, SyncConfigFlags flags, [In] IExchangeManifestCallback pCallback, [In] SRestriction* lpRestriction, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpIncludeProps);

		[PreserveSig]
		int Synchronize(int ulFlags);

		[PreserveSig]
		int Checkpoint(IStream lpStream, [MarshalAs(UnmanagedType.Bool)] [In] bool clearCnsets, [MarshalAs(UnmanagedType.LPArray)] [In] long[] changeMids, [MarshalAs(UnmanagedType.LPArray)] [In] long[] changeCns, [MarshalAs(UnmanagedType.LPArray)] [In] long[] changeAssociatedCns, [MarshalAs(UnmanagedType.LPArray)] [In] long[] deleteMids);
	}
}
