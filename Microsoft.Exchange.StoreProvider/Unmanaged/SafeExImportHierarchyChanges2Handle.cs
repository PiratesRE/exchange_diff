using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ComVisible(false)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SafeExImportHierarchyChanges2Handle : SafeExImportHierarchyChangesHandle
	{
		protected SafeExImportHierarchyChanges2Handle()
		{
		}

		internal SafeExImportHierarchyChanges2Handle(IntPtr handle) : base(handle)
		{
		}

		internal SafeExImportHierarchyChanges2Handle(SafeExInterfaceHandle innerHandle) : base(innerHandle)
		{
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SafeExImportHierarchyChanges2Handle>(this);
		}

		internal int ConfigEx(byte[] pbIdsetGiven, int cbIdsetGiven, byte[] pbCnsetSeen, int cbCnsetSeen, int ulFlags)
		{
			return SafeExImportHierarchyChanges2Handle.IExchangeImportHierarchyChanges2_ConfigEx(this.handle, pbIdsetGiven, cbIdsetGiven, pbCnsetSeen, cbCnsetSeen, ulFlags);
		}

		internal int UpdateStateEx(out IntPtr pbIdsetGiven, out int cbIdsetGiven, out IntPtr pbCnsetSeen, out int cbCnsetSeen)
		{
			return SafeExImportHierarchyChanges2Handle.IExchangeImportHierarchyChanges2_UpdateStateEx(this.handle, out pbIdsetGiven, out cbIdsetGiven, out pbCnsetSeen, out cbCnsetSeen);
		}

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExchangeImportHierarchyChanges2_ConfigEx(IntPtr iExchangeImportHierarchyChanges2, [MarshalAs(UnmanagedType.LPArray)] byte[] pbIdsetGiven, int cbIdsetGiven, [MarshalAs(UnmanagedType.LPArray)] byte[] pbCnsetSeen, int cbCnsetSeen, int ulFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExchangeImportHierarchyChanges2_UpdateStateEx(IntPtr iExchangeImportHierarchyChanges2, out IntPtr pbIdsetGiven, out int cbIdsetGiven, out IntPtr pbCnsetSeen, out int cbCnsetSeen);
	}
}
