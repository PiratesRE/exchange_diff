using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Win32
{
	internal sealed class SafeHGlobalHandle : SafeHGlobalHandleBase
	{
		internal SafeHGlobalHandle(IntPtr handle)
		{
			base.SetHandle(handle);
		}

		private SafeHGlobalHandle()
		{
		}

		private SafeHGlobalHandle(IntPtr handle, bool ownsHandle) : base(handle, ownsHandle)
		{
		}

		public static SafeHGlobalHandle InvalidHandle
		{
			get
			{
				return new SafeHGlobalHandle(IntPtr.Zero, false);
			}
		}

		public static SafeHGlobalHandle AllocHGlobal(int size)
		{
			return new SafeHGlobalHandle(Marshal.AllocHGlobal(size));
		}

		public static SafeHGlobalHandle CopyToHGlobal(byte[] bytes)
		{
			bool flag = false;
			SafeHGlobalHandle safeHGlobalHandle = SafeHGlobalHandle.AllocHGlobal(bytes.Length);
			SafeHGlobalHandle result;
			try
			{
				Marshal.Copy(bytes, 0, safeHGlobalHandle.DangerousGetHandle(), bytes.Length);
				flag = true;
				result = safeHGlobalHandle;
			}
			finally
			{
				if (!flag)
				{
					safeHGlobalHandle.Dispose();
				}
			}
			return result;
		}
	}
}
