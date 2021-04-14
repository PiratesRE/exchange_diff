using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ComVisible(false)]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SafeExImportContentsChanges4Handle : SafeExImportContentsChangesHandle
	{
		protected SafeExImportContentsChanges4Handle()
		{
		}

		internal SafeExImportContentsChanges4Handle(IntPtr handle) : base(handle)
		{
		}

		internal SafeExImportContentsChanges4Handle(SafeExInterfaceHandle innerHandle) : base(innerHandle)
		{
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SafeExImportContentsChanges4Handle>(this);
		}

		internal int ConfigEx(byte[] pbIdsetGiven, int cbIdsetGiven, byte[] pbCnsetSeen, int cbCnsetSeen, byte[] pbCnsetSeenFAI, int cbCnsetSeenFAI, byte[] pbCnsetRead, int cbCnsetRead, int ulFlags)
		{
			return SafeExImportContentsChanges4Handle.IExchangeImportContentsChanges4_ConfigEx(this.handle, pbIdsetGiven, cbIdsetGiven, pbCnsetSeen, cbCnsetSeen, pbCnsetSeenFAI, cbCnsetSeenFAI, pbCnsetRead, cbCnsetRead, ulFlags);
		}

		internal int UpdateStateEx(out IntPtr pbIdsetGiven, out int cbIdsetGiven, out IntPtr pbCnsetSeen, out int cbCnsetSeen, out IntPtr pbCnsetSeenFAI, out int cbCnsetSeenFAI, out IntPtr pbCnsetRead, out int cbCnsetRead)
		{
			return SafeExImportContentsChanges4Handle.IExchangeImportContentsChanges4_UpdateStateEx(this.handle, out pbIdsetGiven, out cbIdsetGiven, out pbCnsetSeen, out cbCnsetSeen, out pbCnsetSeenFAI, out cbCnsetSeenFAI, out pbCnsetRead, out cbCnsetRead);
		}

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExchangeImportContentsChanges4_ConfigEx(IntPtr iExchangeImportContentsChanges4, [MarshalAs(UnmanagedType.LPArray)] byte[] pbIdsetGiven, int cbIdsetGiven, [MarshalAs(UnmanagedType.LPArray)] byte[] pbCnsetSeen, int cbCnsetSeen, [MarshalAs(UnmanagedType.LPArray)] byte[] pbCnsetSeenFAI, int cbCnsetSeenFAI, [MarshalAs(UnmanagedType.LPArray)] byte[] pbCnsetRead, int cbCnsetRead, int ulFlags);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IExchangeImportContentsChanges4_UpdateStateEx(IntPtr iExchangeImportContentsChanges4, out IntPtr pbIdsetGiven, out int cbIdsetGiven, out IntPtr pbCnsetSeen, out int cbCnsetSeen, out IntPtr pbCnsetSeenFAI, out int cbCnsetSeenFAI, out IntPtr pbCnsetRead, out int cbCnsetRead);
	}
}
