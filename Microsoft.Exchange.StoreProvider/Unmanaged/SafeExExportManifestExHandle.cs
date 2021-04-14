using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ComVisible(false)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SafeExExportManifestExHandle : SafeExInterfaceHandle
	{
		protected SafeExExportManifestExHandle()
		{
		}

		internal SafeExExportManifestExHandle(IntPtr handle) : base(handle)
		{
		}

		internal SafeExExportManifestExHandle(SafeExInterfaceHandle innerHandle) : base(innerHandle)
		{
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SafeExExportManifestExHandle>(this);
		}

		internal unsafe int Config(byte[] pbIdsetGiven, int cbIdsetGiven, byte[] pbCnsetSeen, int cbCnsetSeen, byte[] pbCnsetSeenFAI, int cbCnsetSeenFAI, byte[] pbCnsetRead, int cbCnsetRead, SyncConfigFlags flags, IExchangeManifestExCallback iExchangeManifestExCallback, SRestriction* lpRestriction, PropTag[] lpIncludeProps)
		{
			return SafeExExportManifestExHandle.IExchangeExportManifestEx_Config(this.handle, pbIdsetGiven, cbIdsetGiven, pbCnsetSeen, cbCnsetSeen, pbCnsetSeenFAI, cbCnsetSeenFAI, pbCnsetRead, cbCnsetRead, flags, iExchangeManifestExCallback, lpRestriction, lpIncludeProps);
		}

		internal int Synchronize(int ulFlags)
		{
			return SafeExExportManifestExHandle.IExchangeExportManifestEx_Synchronize(this.handle, ulFlags);
		}

		internal int GetState(out IntPtr pbIdsetGiven, out int cbIdsetGiven, out IntPtr pbCnsetSeen, out int cbCnsetSeen, out IntPtr pbCnsetSeenFAI, out int cbCnsetSeenFAI, out IntPtr pbCnsetRead, out int cbCnsetRead)
		{
			return SafeExExportManifestExHandle.IExchangeExportManifestEx_GetState(this.handle, out pbIdsetGiven, out cbIdsetGiven, out pbCnsetSeen, out cbCnsetSeen, out pbCnsetSeenFAI, out cbCnsetSeenFAI, out pbCnsetRead, out cbCnsetRead);
		}

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IExchangeExportManifestEx_Config(IntPtr iExchangeExportManifestEx, [MarshalAs(UnmanagedType.LPArray)] byte[] pbIdsetGiven, int cbIdsetGiven, [MarshalAs(UnmanagedType.LPArray)] byte[] pbCnsetSeen, int cbCnsetSeen, [MarshalAs(UnmanagedType.LPArray)] byte[] pbCnsetSeenFAI, int cbCnsetSeenFAI, [MarshalAs(UnmanagedType.LPArray)] byte[] pbCnsetRead, int cbCnsetRead, SyncConfigFlags flags, [In] IExchangeManifestExCallback iExchangeManifestExCallback, [In] SRestriction* lpRestriction, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpIncludeProps);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExchangeExportManifestEx_Synchronize(IntPtr iExchangeExportManifestEx, int ulFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExchangeExportManifestEx_GetState(IntPtr iExchangeExportManifestEx, out IntPtr pbIdsetGiven, out int cbIdsetGiven, out IntPtr pbCnsetSeen, out int cbCnsetSeen, out IntPtr pbCnsetSeenFAI, out int cbCnsetSeenFAI, out IntPtr pbCnsetRead, out int cbCnsetRead);
	}
}
