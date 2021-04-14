using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace Microsoft.Exchange.Rpc.Nspi
{
	public class SafeByteArraysHandle : SafeRpcMemoryHandle
	{
		public unsafe SafeByteArraysHandle(byte[][] byteArrays)
		{
			try
			{
				int num;
				if (byteArrays == null)
				{
					num = 0;
				}
				else
				{
					num = byteArrays.Length;
				}
				int num2 = (int)((long)num * 16L);
				if (num2 < num)
				{
					throw new OutOfMemoryException();
				}
				uint num3 = (uint)(num2 + 16);
				if (num3 > 2147483647U)
				{
					throw new OutOfMemoryException();
				}
				base.Allocate((int)num3);
				_SBinaryArray_r* ptr = (_SBinaryArray_r*)this.handle.ToPointer();
				*(int*)ptr = num;
				_SBinaryArray_r* ptr2 = ptr + 16L / (long)sizeof(_SBinaryArray_r);
				*(long*)(ptr + 8L / (long)sizeof(_SBinaryArray_r)) = ptr2;
				_SBinary_r* ptr3 = (_SBinary_r*)ptr2;
				int num4 = 0;
				long num5 = 0L;
				long num6 = (long)num;
				if (0L < num6)
				{
					_SBinary_r* ptr4 = ptr3;
					for (;;)
					{
						byte[] array = byteArrays[num4];
						if (array == null)
						{
							goto IL_C0;
						}
						int num7 = array.Length;
						if (num7 == 0)
						{
							goto IL_C0;
						}
						*(int*)ptr4 = num7;
						void* ptr5 = <Module>.MIDL_user_allocate((ulong)((long)byteArrays[num4].Length));
						*(long*)(ptr4 + 8L / (long)sizeof(_SBinary_r)) = ptr5;
						if (ptr5 == null)
						{
							goto IL_E3;
						}
						IntPtr destination = new IntPtr(ptr5);
						byte[] array2 = byteArrays[num4];
						Marshal.Copy(array2, 0, destination, array2.Length);
						IL_CA:
						num4++;
						num5 += 1L;
						ptr4 += 16L / (long)sizeof(_SBinary_r);
						if (num5 >= num6)
						{
							break;
						}
						continue;
						IL_C0:
						*(int*)ptr4 = 0;
						*(long*)(ptr4 + 8L / (long)sizeof(_SBinary_r)) = 0L;
						goto IL_CA;
					}
					goto IL_E9;
					IL_E3:
					throw new OutOfMemoryException();
				}
				IL_E9:;
			}
			catch
			{
				base.Dispose(true);
				throw;
			}
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
		[return: MarshalAs(UnmanagedType.U1)]
		protected unsafe override bool ReleaseHandle()
		{
			if (!this.IsInvalid)
			{
				_SBinaryArray_r* ptr = (_SBinaryArray_r*)this.handle.ToPointer();
				_SBinary_r* ptr2 = *(long*)(ptr + 8L / (long)sizeof(_SBinaryArray_r));
				uint num = 0;
				if (0 < *(int*)ptr)
				{
					do
					{
						<Module>.MIDL_user_free(*(long*)(ptr2 + num * 16UL / (ulong)sizeof(_SBinary_r) + 8L / (long)sizeof(_SBinary_r)));
						num++;
					}
					while (num < *(int*)ptr);
				}
				<Module>.MIDL_user_free(this.handle.ToPointer());
			}
			return true;
		}
	}
}
