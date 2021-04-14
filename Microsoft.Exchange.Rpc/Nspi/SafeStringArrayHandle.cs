using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace Microsoft.Exchange.Rpc.Nspi
{
	public class SafeStringArrayHandle : SafeRpcMemoryHandle
	{
		public unsafe SafeStringArrayHandle(byte[][] strings)
		{
			try
			{
				int num;
				if (strings == null)
				{
					num = 0;
				}
				else
				{
					num = strings.Length;
				}
				int num2 = (int)((long)num * 8L);
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
				_StringsArray* ptr = (_StringsArray*)this.handle.ToPointer();
				*(int*)ptr = num;
				int num4 = 0;
				long num5 = (long)num;
				if (0L < num5)
				{
					_StringsArray* ptr2 = ptr + 8L / (long)sizeof(_StringsArray);
					ulong num6 = (ulong)num5;
					do
					{
						byte[] array = strings[num4];
						if (array != null)
						{
							int num7 = array.Length;
							IntPtr intPtr = Marshal.AllocHGlobal(num7 + 1);
							IntPtr ptr3 = intPtr;
							Marshal.Copy(strings[num4], 0, intPtr, num7);
							Marshal.WriteByte(ptr3, num7, 0);
							*(long*)ptr2 = ptr3.ToPointer();
						}
						num4++;
						ptr2 += 8L / (long)sizeof(_StringsArray);
						num6 -= 1UL;
					}
					while (num6 > 0UL);
				}
			}
			catch
			{
				base.Dispose(true);
				throw;
			}
		}

		public unsafe SafeStringArrayHandle(string[] strings, [MarshalAs(UnmanagedType.U1)] bool ansi)
		{
			try
			{
				int num;
				if (strings == null)
				{
					num = 0;
				}
				else
				{
					num = strings.Length;
				}
				int num2 = (int)((long)num * 8L);
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
				_StringsArray* ptr = (_StringsArray*)this.handle.ToPointer();
				*(int*)ptr = num;
				int num4 = 0;
				long num5 = (long)num;
				if (0L < num5)
				{
					_StringsArray* ptr2 = ptr + 8L / (long)sizeof(_StringsArray);
					ulong num6 = (ulong)num5;
					do
					{
						*(long*)ptr2 = ((!ansi) ? Marshal.StringToHGlobalUni(strings[num4]) : Marshal.StringToHGlobalAnsi(strings[num4])).ToPointer();
						num4++;
						ptr2 += 8L / (long)sizeof(_StringsArray);
						num6 -= 1UL;
					}
					while (num6 > 0UL);
				}
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
				_StringsArray* ptr = (_StringsArray*)this.handle.ToPointer();
				uint num = 0;
				if (0 < *(int*)ptr)
				{
					do
					{
						IntPtr hglobal = new IntPtr(((num + 1UL / 8UL) * 8L)[ptr / 8]);
						Marshal.FreeHGlobal(hglobal);
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
