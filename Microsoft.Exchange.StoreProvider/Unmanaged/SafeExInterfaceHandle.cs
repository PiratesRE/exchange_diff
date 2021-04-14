using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[ComVisible(false)]
	internal class SafeExInterfaceHandle : DisposeTrackableSafeHandleZeroOrMinusOneIsInvalid, IExInterface, IDisposeTrackable, IDisposable
	{
		protected SafeExInterfaceHandle()
		{
		}

		internal SafeExInterfaceHandle(IntPtr handle) : base(handle)
		{
		}

		internal SafeExInterfaceHandle(SafeExInterfaceHandle innerHandle) : base(innerHandle.DangerousGetHandle(), false)
		{
			this.innerHandle = innerHandle;
		}

		public override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<SafeExInterfaceHandle>(this);
		}

		public int QueryInterface(Guid riid, out IExInterface iObj)
		{
			SafeExInterfaceHandle safeExInterfaceHandle = null;
			iObj = null;
			int result;
			try
			{
				int num = SafeExInterfaceHandle.IUnknown_QueryInterface(this.handle, riid, out safeExInterfaceHandle);
				if (num == 0)
				{
					iObj = safeExInterfaceHandle;
					safeExInterfaceHandle = null;
				}
				result = num;
			}
			finally
			{
				safeExInterfaceHandle.DisposeIfValid();
			}
			return result;
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && this.innerHandle != null)
				{
					this.innerHandle.Dispose();
					this.innerHandle = null;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		protected virtual void InternalReleaseHandle()
		{
		}

		protected override bool ReleaseHandle()
		{
			this.InternalReleaseHandle();
			SafeExInterfaceHandle.IUnknown_Release(this.handle);
			return true;
		}

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IUnknown_Release(IntPtr iUnknown);

		[DllImport("exrpc32.dll", ExactSpelling = true)]
		private static extern int IUnknown_QueryInterface(IntPtr iUnknown, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, out SafeExInterfaceHandle iObj);

		private SafeExInterfaceHandle innerHandle;
	}
}
