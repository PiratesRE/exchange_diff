using System;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading;

namespace System
{
	internal class SizedReference : IDisposable
	{
		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern IntPtr CreateSizedRef(object o);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void FreeSizedRef(IntPtr h);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern object GetTargetOfSizedRef(IntPtr h);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern long GetApproximateSizeOfSizedRef(IntPtr h);

		[SecuritySafeCritical]
		private void Free()
		{
			IntPtr handle = this._handle;
			if (handle != IntPtr.Zero && Interlocked.CompareExchange(ref this._handle, IntPtr.Zero, handle) == handle)
			{
				SizedReference.FreeSizedRef(handle);
			}
		}

		[SecuritySafeCritical]
		public SizedReference(object target)
		{
			IntPtr handle = IntPtr.Zero;
			handle = SizedReference.CreateSizedRef(target);
			this._handle = handle;
		}

		~SizedReference()
		{
			this.Free();
		}

		public object Target
		{
			[SecuritySafeCritical]
			get
			{
				IntPtr handle = this._handle;
				if (handle == IntPtr.Zero)
				{
					return null;
				}
				object targetOfSizedRef = SizedReference.GetTargetOfSizedRef(handle);
				if (!(this._handle == IntPtr.Zero))
				{
					return targetOfSizedRef;
				}
				return null;
			}
		}

		public long ApproximateSize
		{
			[SecuritySafeCritical]
			get
			{
				IntPtr handle = this._handle;
				if (handle == IntPtr.Zero)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_HandleIsNotInitialized"));
				}
				long approximateSizeOfSizedRef = SizedReference.GetApproximateSizeOfSizedRef(handle);
				if (this._handle == IntPtr.Zero)
				{
					throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_HandleIsNotInitialized"));
				}
				return approximateSizeOfSizedRef;
			}
		}

		public void Dispose()
		{
			this.Free();
			GC.SuppressFinalize(this);
		}

		internal volatile IntPtr _handle;
	}
}
