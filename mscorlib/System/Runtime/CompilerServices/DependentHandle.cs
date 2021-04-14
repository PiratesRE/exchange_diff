using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.CompilerServices
{
	[ComVisible(false)]
	internal struct DependentHandle
	{
		[SecurityCritical]
		public DependentHandle(object primary, object secondary)
		{
			IntPtr handle = (IntPtr)0;
			DependentHandle.nInitialize(primary, secondary, out handle);
			this._handle = handle;
		}

		public bool IsAllocated
		{
			get
			{
				return this._handle != (IntPtr)0;
			}
		}

		[SecurityCritical]
		public object GetPrimary()
		{
			object result;
			DependentHandle.nGetPrimary(this._handle, out result);
			return result;
		}

		[SecurityCritical]
		public void GetPrimaryAndSecondary(out object primary, out object secondary)
		{
			DependentHandle.nGetPrimaryAndSecondary(this._handle, out primary, out secondary);
		}

		[SecurityCritical]
		public void Free()
		{
			if (this._handle != (IntPtr)0)
			{
				IntPtr handle = this._handle;
				this._handle = (IntPtr)0;
				DependentHandle.nFree(handle);
			}
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void nInitialize(object primary, object secondary, out IntPtr dependentHandle);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void nGetPrimary(IntPtr dependentHandle, out object primary);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void nGetPrimaryAndSecondary(IntPtr dependentHandle, out object primary, out object secondary);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void nFree(IntPtr dependentHandle);

		private IntPtr _handle;
	}
}
