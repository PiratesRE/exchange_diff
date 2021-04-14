using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[ComVisible(false)]
	internal class SafeExExportContentsChangesExHandle : SafeExInterfaceHandle
	{
		protected SafeExExportContentsChangesExHandle()
		{
		}

		internal SafeExExportContentsChangesExHandle(SafeExInterfaceHandle innerHandle) : base(innerHandle)
		{
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SafeExExportContentsChangesExHandle>(this);
		}

		internal unsafe int Config(byte[] pbIdsetGiven, int cbIdsetGiven, byte[] pbCnsetSeen, int cbCnsetSeen, byte[] pbCnsetSeenFAI, int cbCnsetSeenFAI, byte[] pbCnsetRead, int cbCnsetRead, SyncConfigFlags flags, SRestriction* lpRestriction, PropTag[] lpIncludeProps, PropTag[] lpExcludeProps, int ulBufferSize)
		{
			return SafeExExportContentsChangesExHandle.IExchangeExportContentsChangesEx_Config(this.handle, pbIdsetGiven, cbIdsetGiven, pbCnsetSeen, cbCnsetSeen, pbCnsetSeenFAI, cbCnsetSeenFAI, pbCnsetRead, cbCnsetRead, flags, lpRestriction, lpIncludeProps, lpExcludeProps, ulBufferSize);
		}

		internal int GetBuffers(out SafeExLinkedMemoryHandle ppBlocks, out int cBlocks)
		{
			return SafeExExportContentsChangesExHandle.IExchangeExportContentsChangesEx_GetBuffers(this.handle, out ppBlocks, out cBlocks);
		}

		internal int GetState(out IntPtr pbIdsetGiven, out int cbIdsetGiven, out IntPtr pbCnsetSeen, out int cbCnsetSeen, out IntPtr pbCnsetSeenFAI, out int cbCnsetSeenFAI, out IntPtr pbCnsetRead, out int cbCnsetRead)
		{
			return SafeExExportContentsChangesExHandle.IExchangeExportContentsChangesEx_GetState(this.handle, out pbIdsetGiven, out cbIdsetGiven, out pbCnsetSeen, out cbCnsetSeen, out pbCnsetSeenFAI, out cbCnsetSeenFAI, out pbCnsetRead, out cbCnsetRead);
		}

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IExchangeExportContentsChangesEx_Config(IntPtr iExchangeExportChangesEx, [MarshalAs(UnmanagedType.LPArray)] byte[] pbIdsetGiven, int cbIdsetGiven, [MarshalAs(UnmanagedType.LPArray)] byte[] pbCnsetSeen, int cbCnsetSeen, [MarshalAs(UnmanagedType.LPArray)] byte[] pbCnsetSeenFAI, int cbCnsetSeenFAI, [MarshalAs(UnmanagedType.LPArray)] byte[] pbCnsetRead, int cbCnsetRead, SyncConfigFlags flags, [In] SRestriction* lpRestriction, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpIncludeProps, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpExcludeProps, int ulBufferSize);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExchangeExportContentsChangesEx_GetBuffers(IntPtr iExchangeExportChangesEx, out SafeExLinkedMemoryHandle ppBlocks, out int cBlocks);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExchangeExportContentsChangesEx_GetState(IntPtr iExchangeExportChangesEx, out IntPtr pbIdsetGiven, out int cbIdsetGiven, out IntPtr pbCnsetSeen, out int cbCnsetSeen, out IntPtr pbCnsetSeenFAI, out int cbCnsetSeenFAI, out IntPtr pbCnsetRead, out int cbCnsetRead);
	}
}
