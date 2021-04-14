using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Exchange.Win32
{
	internal sealed class SafeSecureHGlobalHandle : SafeHGlobalHandleBase
	{
		private SafeSecureHGlobalHandle(IntPtr handle, int length) : base(handle)
		{
			this.length = length;
		}

		public int Length
		{
			get
			{
				return this.length;
			}
		}

		public static SafeSecureHGlobalHandle AllocHGlobal(int size)
		{
			SafeSecureHGlobalHandle safeSecureHGlobalHandle = new SafeSecureHGlobalHandle(Marshal.AllocHGlobal(size), size);
			safeSecureHGlobalHandle.ZeroMemory();
			return safeSecureHGlobalHandle;
		}

		public static SafeSecureHGlobalHandle CopyToHGlobal(byte[] bytes)
		{
			bool flag = false;
			SafeSecureHGlobalHandle safeSecureHGlobalHandle = SafeSecureHGlobalHandle.AllocHGlobal(bytes.Length);
			SafeSecureHGlobalHandle result;
			try
			{
				Marshal.Copy(bytes, 0, safeSecureHGlobalHandle.DangerousGetHandle(), bytes.Length);
				flag = true;
				result = safeSecureHGlobalHandle;
			}
			finally
			{
				if (!flag)
				{
					safeSecureHGlobalHandle.Dispose();
				}
			}
			return result;
		}

		public static SafeSecureHGlobalHandle DecryptAndAllocHGlobal(SecureString secureString)
		{
			int num = secureString.Length * 2;
			return new SafeSecureHGlobalHandle(Marshal.SecureStringToGlobalAllocUnicode(secureString), num);
		}

		public static SafeSecureHGlobalHandle Assign(IntPtr handle, int size)
		{
			return new SafeSecureHGlobalHandle(handle, size);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		protected override bool ReleaseHandle()
		{
			this.ZeroMemory();
			return base.ReleaseHandle();
		}

		private void ZeroMemory()
		{
			if (!this.IsInvalid && this.length != 0)
			{
				NativeMethods.ZeroMemory(this.handle, (uint)this.length);
			}
		}

		private readonly int length;
	}
}
