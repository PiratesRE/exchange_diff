using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Security.Cryptography
{
	internal sealed class SafeHashHandle : SafeHandleZeroOrMinusOneIsInvalid
	{
		internal SafeHashHandle(IntPtr handle) : base(true)
		{
			base.SetHandle(handle);
		}

		private SafeHashHandle() : base(true)
		{
		}

		public static SafeHashHandle InvalidHandle
		{
			get
			{
				return new SafeHashHandle(IntPtr.Zero);
			}
		}

		public static SafeHashHandle Create(SafeCryptProvHandle provider, CapiNativeMethods.AlgorithmId hashType)
		{
			SafeHashHandle invalidHandle = SafeHashHandle.InvalidHandle;
			if (!CapiNativeMethods.CryptCreateHash(provider.DangerousGetHandle(), hashType, IntPtr.Zero, 0, ref invalidHandle))
			{
				throw new CryptographicException(Marshal.GetLastWin32Error());
			}
			return invalidHandle;
		}

		public unsafe void HashData(byte[] bytes, int offset, int size)
		{
			if (offset < 0 || size < 0 || size > bytes.Length || offset > bytes.Length - size)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (bytes.Length == 0)
			{
				return;
			}
			fixed (byte* ptr = &bytes[offset])
			{
				if (!CapiNativeMethods.CryptHashData(this, ptr, (uint)size, 0))
				{
					throw new CryptographicException(Marshal.GetLastWin32Error());
				}
			}
		}

		public byte[] HashFinal()
		{
			byte[] array = null;
			uint num = 0U;
			if (!CapiNativeMethods.CryptGetHashParam(this, CapiNativeMethods.HashParameter.HashValue, array, ref num, 0U))
			{
				throw new CryptographicException(Marshal.GetLastWin32Error());
			}
			array = new byte[num];
			if (!CapiNativeMethods.CryptGetHashParam(this, CapiNativeMethods.HashParameter.HashValue, array, ref num, 0U))
			{
				throw new CryptographicException(Marshal.GetLastWin32Error());
			}
			return array;
		}

		protected override bool ReleaseHandle()
		{
			return CapiNativeMethods.CryptDestroyHash(this.handle);
		}
	}
}
