using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("2DC76CDD-1AA6-4157-808F-E68D2AD29FE8")]
	[ComImport]
	internal interface IExchangeExportHierManifestEx
	{
		[PreserveSig]
		unsafe int GetLastError(int hResult, int ulFlags, out MAPIERROR* lpMapiError);

		[PreserveSig]
		unsafe int Config([MarshalAs(UnmanagedType.LPArray)] byte[] pbIdsetGiven, int cbIdsetGiven, [MarshalAs(UnmanagedType.LPArray)] byte[] pbCnsetSeen, int cbCnsetSeen, SyncConfigFlags flags, [In] IExchangeHierManifestCallback pCallback, [In] SRestriction* lpRestriction, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpIncludeProps, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpExcludeProps);

		[PreserveSig]
		int Synchronize(int ulFlags);

		[PreserveSig]
		int GetState(out IntPtr pbIdsetGiven, out int cbIdsetGiven, out IntPtr pbCnsetSeen, out int cbCnsetSeen);

		[PreserveSig]
		int Checkpoint([MarshalAs(UnmanagedType.LPArray)] byte[] pbCheckpointIdsetGiven, int cbCheckpointIdsetGiven, [MarshalAs(UnmanagedType.LPArray)] byte[] pbCheckpointCnsetSeen, int cbCheckpointCnsetSeen, [MarshalAs(UnmanagedType.LPArray)] [In] long[] changeFids, [MarshalAs(UnmanagedType.LPArray)] [In] long[] changeCns, [MarshalAs(UnmanagedType.LPArray)] [In] long[] deleteMids, out SafeExMemoryHandle pbIdsetGiven, out int cbIdsetGiven, out SafeExMemoryHandle pbCnsetSeen, out int cbCnsetSeen);
	}
}
