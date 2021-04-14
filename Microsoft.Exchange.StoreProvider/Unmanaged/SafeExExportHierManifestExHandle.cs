using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ComVisible(false)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SafeExExportHierManifestExHandle : SafeExInterfaceHandle
	{
		protected SafeExExportHierManifestExHandle()
		{
		}

		internal SafeExExportHierManifestExHandle(IntPtr handle) : base(handle)
		{
		}

		internal SafeExExportHierManifestExHandle(SafeExInterfaceHandle innerHandle) : base(innerHandle)
		{
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SafeExExportHierManifestExHandle>(this);
		}

		internal unsafe int Config(byte[] pbIdsetGiven, int cbIdsetGiven, byte[] pbCnsetSeen, int cbCnsetSeen, SyncConfigFlags flags, IExchangeHierManifestCallback iExchangeHierManifestCallback, SRestriction* lpRestriction, PropTag[] lpIncludeProps, PropTag[] lpExcludeProps)
		{
			return SafeExExportHierManifestExHandle.IExchangeExportHierManifestEx_Config(this.handle, pbIdsetGiven, cbIdsetGiven, pbCnsetSeen, cbCnsetSeen, flags, iExchangeHierManifestCallback, lpRestriction, lpIncludeProps, lpExcludeProps);
		}

		internal int Synchronize(int ulFlags)
		{
			return SafeExExportHierManifestExHandle.IExchangeExportHierManifestEx_Synchronize(this.handle, ulFlags);
		}

		internal int GetState(out IntPtr pbIdsetGiven, out int cbIdsetGiven, out IntPtr pbCnsetSeen, out int cbCnsetSeen)
		{
			return SafeExExportHierManifestExHandle.IExchangeExportHierManifestEx_GetState(this.handle, out pbIdsetGiven, out cbIdsetGiven, out pbCnsetSeen, out cbCnsetSeen);
		}

		internal int Checkpoint(byte[] pbCheckpointIdsetGiven, int cbCheckpointIdsetGiven, byte[] pbCheckpointCnsetSeen, int cbCheckpointCnsetSeen, long[] changeFids, long[] changeCns, long[] deleteMids, out SafeExMemoryHandle pbIdsetGiven, out int cbIdsetGiven, out SafeExMemoryHandle pbCnsetSeen, out int cbCnsetSeen)
		{
			return SafeExExportHierManifestExHandle.IExchangeExportHierManifestEx_Checkpoint(this.handle, pbCheckpointIdsetGiven, cbCheckpointIdsetGiven, pbCheckpointCnsetSeen, cbCheckpointCnsetSeen, changeFids, changeCns, deleteMids, out pbIdsetGiven, out cbIdsetGiven, out pbCnsetSeen, out cbCnsetSeen);
		}

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IExchangeExportHierManifestEx_Config(IntPtr iExchangeExportHierManifestEx, [MarshalAs(UnmanagedType.LPArray)] byte[] pbIdsetGiven, int cbIdsetGiven, [MarshalAs(UnmanagedType.LPArray)] byte[] pbCnsetSeen, int cbCnsetSeen, SyncConfigFlags flags, [In] IExchangeHierManifestCallback iExchangeHierManifestCallback, [In] SRestriction* lpRestriction, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpIncludeProps, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpExcludeProps);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExchangeExportHierManifestEx_Synchronize(IntPtr iExchangeExportHierManifestEx, int ulFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExchangeExportHierManifestEx_GetState(IntPtr iExchangeExportHierManifestEx, out IntPtr pbIdsetGiven, out int cbIdsetGiven, out IntPtr pbCnsetSeen, out int cbCnsetSeen);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExchangeExportHierManifestEx_Checkpoint(IntPtr iExchangeExportHierManifestEx, [MarshalAs(UnmanagedType.LPArray)] byte[] pbCheckpointIdsetGiven, int cbCheckpointIdsetGiven, [MarshalAs(UnmanagedType.LPArray)] byte[] pbCheckpointCnsetSeen, int cbCheckpointCnsetSeen, [MarshalAs(UnmanagedType.LPArray)] [In] long[] changeFids, [MarshalAs(UnmanagedType.LPArray)] [In] long[] changeCns, [MarshalAs(UnmanagedType.LPArray)] [In] long[] deleteMids, out SafeExMemoryHandle pbIdsetGiven, out int cbIdsetGiven, out SafeExMemoryHandle pbCnsetSeen, out int cbCnsetSeen);
	}
}
