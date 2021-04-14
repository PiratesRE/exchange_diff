using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc
{
	internal class MarshalledString : IDisposable
	{
		public MarshalledString(string stringToMarshal)
		{
			if (stringToMarshal != null)
			{
				IntPtr intPtr = Marshal.StringToHGlobalUni(stringToMarshal);
				IntPtr hglobal = intPtr;
				bool flag = false;
				try
				{
					this.nativeString = new SafeMarshalHGlobalHandle(intPtr);
					flag = true;
					return;
				}
				finally
				{
					if (!flag)
					{
						Marshal.FreeHGlobal(hglobal);
					}
				}
			}
			this.nativeString = new SafeMarshalHGlobalHandle();
		}

		private void ~MarshalledString()
		{
			IDisposable disposable = this.nativeString;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}

		public unsafe implicit operator ushort*()
		{
			return (ushort*)this.nativeString.DangerousGetHandle().ToPointer();
		}

		public unsafe int Length
		{
			get
			{
				ulong num;
				if (this.nativeString.IsInvalid)
				{
					num = 0UL;
				}
				else
				{
					void* ptr = this.nativeString.DangerousGetHandle().ToPointer();
					void* ptr2 = ptr;
					if (*(short*)ptr != 0)
					{
						do
						{
							ptr2 = (void*)((byte*)ptr2 + 2L);
						}
						while (*(short*)ptr2 != 0);
					}
					num = (ulong)(ptr2 - ptr >> 1);
				}
				return (int)num;
			}
		}

		protected virtual void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
		{
			if (A_0)
			{
				this.~MarshalledString();
			}
			else
			{
				base.Finalize();
			}
		}

		public sealed void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private SafeMarshalHGlobalHandle nativeString;
	}
}
