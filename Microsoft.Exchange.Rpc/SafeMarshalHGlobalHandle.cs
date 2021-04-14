using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Rpc
{
	public class SafeMarshalHGlobalHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		public SafeMarshalHGlobalHandle(IntPtr handle) : base(true)
		{
			try
			{
				base.SetHandle(handle);
			}
			catch
			{
				base.Dispose(true);
				throw;
			}
		}

		public SafeMarshalHGlobalHandle() : base(true)
		{
		}

		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
		[return: MarshalAs(UnmanagedType.U1)]
		protected override bool ReleaseHandle()
		{
			Marshal.FreeHGlobal(this.handle);
			return true;
		}
	}
}
