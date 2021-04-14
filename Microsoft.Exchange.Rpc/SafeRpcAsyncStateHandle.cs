using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Threading;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Rpc
{
	internal class SafeRpcAsyncStateHandle : SafeHandleZeroIsInvalid
	{
		public SafeRpcAsyncStateHandle(IntPtr handle)
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

		public SafeRpcAsyncStateHandle()
		{
		}

		public IntPtr Detach()
		{
			IntPtr intPtr = Interlocked.CompareExchange(ref this.handle, IntPtr.Zero, this.handle);
			if (IntPtr.Zero != intPtr)
			{
				base.SetHandleAsInvalid();
			}
			return intPtr;
		}

		public unsafe void CompleteCall(int result)
		{
			if (!this.IsInvalid)
			{
				IntPtr intPtr = this.Detach();
				IntPtr intPtr2 = intPtr;
				if (IntPtr.Zero != intPtr)
				{
					int num = result;
					<Module>.RpcAsyncCompleteCall((_RPC_ASYNC_STATE*)intPtr2.ToPointer(), (void*)(&num));
				}
				if (this != null)
				{
					((IDisposable)this).Dispose();
				}
			}
		}

		public unsafe void AbortCall(uint exceptionCode)
		{
			if (!this.IsInvalid)
			{
				IntPtr intPtr = this.Detach();
				IntPtr intPtr2 = intPtr;
				if (IntPtr.Zero != intPtr)
				{
					<Module>.RpcAsyncAbortCall((_RPC_ASYNC_STATE*)intPtr2.ToPointer(), exceptionCode);
				}
				if (this != null)
				{
					((IDisposable)this).Dispose();
				}
			}
		}

		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
		[return: MarshalAs(UnmanagedType.U1)]
		protected override bool ReleaseHandle()
		{
			if (this.IsInvalid)
			{
				return true;
			}
			this.AbortCall(1726U);
			return true;
		}

		private unsafe static int FilterAccessViolations(uint code, _EXCEPTION_POINTERS* ep)
		{
			return (code == 3221225477U) ? 1 : 0;
		}

		private static FileVersionInfo rpcRuntimeVersionInfo;
	}
}
