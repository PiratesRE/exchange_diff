using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Security
{
	internal class SessionKey
	{
		public virtual byte[] Key
		{
			get
			{
				return this.key;
			}
		}

		protected SessionKey()
		{
		}

		internal unsafe SessionKey(byte[] memory)
		{
			fixed (IntPtr* ptr = memory)
			{
				SessionKey.KerberosSessionKey kerberosSessionKey = (SessionKey.KerberosSessionKey)Marshal.PtrToStructure(new IntPtr((void*)ptr), typeof(SessionKey.KerberosSessionKey));
				using (SafeContextBuffer safeContextBuffer = new SafeContextBuffer(kerberosSessionKey.Key))
				{
					if (kerberosSessionKey.Length <= 0)
					{
						this.key = new byte[0];
					}
					else
					{
						this.key = new byte[kerberosSessionKey.Length];
						Marshal.Copy(safeContextBuffer.DangerousGetHandle(), this.key, 0, kerberosSessionKey.Length);
					}
				}
			}
		}

		private readonly byte[] key;

		public static readonly int Size = Marshal.SizeOf(typeof(SessionKey.KerberosSessionKey));

		private struct KerberosSessionKey
		{
			public readonly int Length;

			public readonly IntPtr Key;
		}
	}
}
