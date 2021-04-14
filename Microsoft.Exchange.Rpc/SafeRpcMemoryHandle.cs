using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Rpc
{
	public class SafeRpcMemoryHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		public SafeRpcMemoryHandle(int size) : base(true)
		{
			try
			{
				this.Allocate(size);
			}
			catch
			{
				base.Dispose(true);
				throw;
			}
		}

		public SafeRpcMemoryHandle(IntPtr handle) : base(true)
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

		public SafeRpcMemoryHandle() : base(true)
		{
		}

		public void Allocate(int size)
		{
			if (size < 0)
			{
				throw new ArgumentException("size");
			}
			this.Allocate((ulong)((long)size));
		}

		public void Allocate(ulong size)
		{
			this.ReleaseHandle();
			IntPtr handle = new IntPtr(<Module>.MIDL_user_allocate(size));
			this.handle = handle;
			if (this.handle == IntPtr.Zero)
			{
				throw new OutOfMemoryException();
			}
			initblk(this.handle.ToPointer(), 0, size);
		}

		public void AddAssociatedHandle(SafeRpcMemoryHandle midlHandle)
		{
			if (this.associatedHandles == null)
			{
				this.associatedHandles = new List<SafeRpcMemoryHandle>(10);
			}
			this.associatedHandles.Add(midlHandle);
		}

		public IntPtr Detach()
		{
			List<SafeRpcMemoryHandle> list = this.associatedHandles;
			if (list != null)
			{
				List<SafeRpcMemoryHandle>.Enumerator enumerator = list.GetEnumerator();
				if (enumerator.MoveNext())
				{
					do
					{
						SafeRpcMemoryHandle safeRpcMemoryHandle = enumerator.Current;
						safeRpcMemoryHandle.Detach();
						if (safeRpcMemoryHandle != null)
						{
							((IDisposable)safeRpcMemoryHandle).Dispose();
						}
					}
					while (enumerator.MoveNext());
				}
				this.associatedHandles.Clear();
				this.associatedHandles = null;
			}
			IntPtr handle = this.handle;
			this.handle = IntPtr.Zero;
			return handle;
		}

		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
		[return: MarshalAs(UnmanagedType.U1)]
		protected override bool ReleaseHandle()
		{
			if (!this.IsInvalid)
			{
				List<SafeRpcMemoryHandle> list = this.associatedHandles;
				if (list != null)
				{
					List<SafeRpcMemoryHandle>.Enumerator enumerator = list.GetEnumerator();
					if (enumerator.MoveNext())
					{
						do
						{
							IDisposable disposable = enumerator.Current;
							if (disposable != null)
							{
								disposable.Dispose();
							}
						}
						while (enumerator.MoveNext());
					}
					this.associatedHandles.Clear();
					this.associatedHandles = null;
				}
				<Module>.MIDL_user_free(this.handle.ToPointer());
			}
			return true;
		}

		private List<SafeRpcMemoryHandle> associatedHandles;
	}
}
