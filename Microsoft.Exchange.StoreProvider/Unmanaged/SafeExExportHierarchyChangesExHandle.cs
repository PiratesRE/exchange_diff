using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ComVisible(false)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SafeExExportHierarchyChangesExHandle : SafeExInterfaceHandle
	{
		protected SafeExExportHierarchyChangesExHandle()
		{
		}

		internal SafeExExportHierarchyChangesExHandle(SafeExInterfaceHandle innerHandle) : base(innerHandle)
		{
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SafeExExportHierarchyChangesExHandle>(this);
		}

		internal unsafe int Config(byte[] pbIdsetGiven, int cbIdsetGiven, byte[] pbCnsetSeen, int cbCnsetSeen, SyncConfigFlags flags, SRestriction* lpRestriction, PropTag[] lpIncludeProps, PropTag[] lpExcludeProps, int ulBufferSize)
		{
			return SafeExExportHierarchyChangesExHandle.IExchangeExportHierarchyChangesEx_Config(this.handle, pbIdsetGiven, cbIdsetGiven, pbCnsetSeen, cbCnsetSeen, flags, lpRestriction, lpIncludeProps, lpExcludeProps, ulBufferSize);
		}

		internal int GetBuffers(out SafeExLinkedMemoryHandle ppBlocks, out int cBlocks)
		{
			return SafeExExportHierarchyChangesExHandle.IExchangeExportHierarchyChangesEx_GetBuffers(this.handle, out ppBlocks, out cBlocks);
		}

		internal int GetState(out IntPtr pbIdsetGiven, out int cbIdsetGiven, out IntPtr pbCnsetSeen, out int cbCnsetSeen)
		{
			return SafeExExportHierarchyChangesExHandle.IExchangeExportHierarchyChangesEx_GetState(this.handle, out pbIdsetGiven, out cbIdsetGiven, out pbCnsetSeen, out cbCnsetSeen);
		}

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IExchangeExportHierarchyChangesEx_Config(IntPtr iExchangeExportChangesEx, [MarshalAs(UnmanagedType.LPArray)] byte[] pbIdsetGiven, int cbIdsetGiven, [MarshalAs(UnmanagedType.LPArray)] byte[] pbCnsetSeen, int cbCnsetSeen, SyncConfigFlags flags, [In] SRestriction* lpRestriction, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpIncludeProps, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpExcludeProps, int ulBufferSize);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExchangeExportHierarchyChangesEx_GetBuffers(IntPtr iExchangeExportChangesEx, out SafeExLinkedMemoryHandle ppBlocks, out int cBlocks);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExchangeExportHierarchyChangesEx_GetState(IntPtr iExchangeExportChangesEx, out IntPtr pbIdsetGiven, out int cbIdsetGiven, out IntPtr pbCnsetSeen, out int cbCnsetSeen);
	}
}
