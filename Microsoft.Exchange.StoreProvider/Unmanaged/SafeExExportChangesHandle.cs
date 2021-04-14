using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ComVisible(false)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SafeExExportChangesHandle : SafeExInterfaceHandle, IExExportChanges, IExInterface, IDisposeTrackable, IDisposable
	{
		protected SafeExExportChangesHandle()
		{
		}

		internal SafeExExportChangesHandle(IntPtr handle) : base(handle)
		{
		}

		internal SafeExExportChangesHandle(SafeExInterfaceHandle innerHandle) : base(innerHandle)
		{
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SafeExExportChangesHandle>(this);
		}

		public unsafe int Config(IStream iStream, int ulFlags, IntPtr iCollector, SRestriction* lpRestriction, PropTag[] lpIncludeProps, PropTag[] lpExcludeProps, int ulBufferSize)
		{
			return SafeExExportChangesHandle.IExchangeExportChanges_Config(this.handle, iStream, ulFlags, iCollector, lpRestriction, lpIncludeProps, lpExcludeProps, ulBufferSize);
		}

		public int Synchronize(out int lpulSteps, out int lpulProgress)
		{
			return SafeExExportChangesHandle.IExchangeExportChanges_Synchronize(this.handle, out lpulSteps, out lpulProgress);
		}

		public int UpdateState(IStream iStream)
		{
			return SafeExExportChangesHandle.IExchangeExportChanges_UpdateState(this.handle, iStream);
		}

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private unsafe static extern int IExchangeExportChanges_Config(IntPtr iExchangeExportChanges, IStream iStream, int ulFlags, IntPtr iCollector, [In] SRestriction* lpRestriction, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpIncludeProps, [MarshalAs(UnmanagedType.LPArray)] [In] PropTag[] lpExcludeProps, int ulBufferSize);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExchangeExportChanges_Synchronize(IntPtr iExchangeExportChanges, out int lpulSteps, out int lpulProgress);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExchangeExportChanges_UpdateState(IntPtr iExchangeExportChanges, IStream iStream);
	}
}
