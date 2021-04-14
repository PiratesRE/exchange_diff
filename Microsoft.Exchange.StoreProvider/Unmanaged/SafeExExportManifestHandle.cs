using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[ComVisible(false)]
	internal class SafeExExportManifestHandle : SafeExInterfaceHandle, IExExportManifest, IExInterface, IDisposeTrackable, IDisposable
	{
		protected SafeExExportManifestHandle()
		{
		}

		internal SafeExExportManifestHandle(IntPtr handle) : base(handle)
		{
		}

		internal SafeExExportManifestHandle(SafeExInterfaceHandle innerHandle) : base(innerHandle)
		{
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SafeExExportManifestHandle>(this);
		}

		public unsafe int Config(IStream iStream, SyncConfigFlags flags, IExchangeManifestCallback pCallback, SRestriction* lpRestriction, PropTag[] lpIncludeProps)
		{
			return SafeExExportManifestHandle.IExchangeExportManifest_Config(this.handle, iStream, flags, pCallback, lpRestriction, lpIncludeProps);
		}

		public int Synchronize(int ulFlags)
		{
			return SafeExExportManifestHandle.IExchangeExportManifest_Synchronize(this.handle, ulFlags);
		}

		public int Checkpoint(IStream iStream, bool clearCnsets, long[] changeMids, long[] changeCns, long[] changeAssociatedCns, long[] deleteMids, long[] readCns)
		{
			return SafeExExportManifestHandle.IExchangeExportManifest_Checkpoint(this.handle, iStream, clearCnsets, changeMids, changeCns, changeAssociatedCns, deleteMids, readCns);
		}

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IExchangeExportManifest_Config(IntPtr iExchangeExportManifest, IStream iStream, SyncConfigFlags flags, [In] IExchangeManifestCallback pCallback, [In] SRestriction* lpRestriction, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpIncludeProps);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExchangeExportManifest_Synchronize(IntPtr iExchangeExportManifest, int ulFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExchangeExportManifest_Checkpoint(IntPtr iExchangeExportManifest, IStream iStream, [MarshalAs(UnmanagedType.Bool)] [In] bool clearCnsets, [MarshalAs(UnmanagedType.LPArray)] [In] long[] changeMids, [MarshalAs(UnmanagedType.LPArray)] [In] long[] changeCns, [MarshalAs(UnmanagedType.LPArray)] [In] long[] changeAssociatedCns, [MarshalAs(UnmanagedType.LPArray)] [In] long[] deleteMids, [MarshalAs(UnmanagedType.LPArray)] [In] long[] readCns);
	}
}
