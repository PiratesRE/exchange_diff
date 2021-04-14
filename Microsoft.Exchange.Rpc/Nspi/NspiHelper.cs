using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Nspi;

namespace Microsoft.Exchange.Rpc.Nspi
{
	internal class NspiHelper
	{
		public unsafe static SafeRpcMemoryHandle ConvertIntArrayToPropTagArray(int[] values, [MarshalAs(UnmanagedType.U1)] bool allowEmptyArray)
		{
			SafeRpcMemoryHandle safeRpcMemoryHandle = new SafeRpcMemoryHandle();
			if (values != null)
			{
				int num = values.Length;
				if (num != 0 || allowEmptyArray)
				{
					safeRpcMemoryHandle.Allocate((ulong)((long)(num + 1) * 4L));
					int* ptr = (int*)safeRpcMemoryHandle.DangerousGetHandle().ToPointer();
					*ptr = values.Length;
					if (values.Length != 0)
					{
						IntPtr destination = new IntPtr((void*)(ptr + 4L / 4L));
						Marshal.Copy(values, 0, destination, values.Length);
					}
				}
			}
			return safeRpcMemoryHandle;
		}

		public static SafeRpcMemoryHandle ConvertIntArrayToPropTagArray(int[] values)
		{
			return NspiHelper.ConvertIntArrayToPropTagArray(values, false);
		}

		public unsafe static int[] ConvertPropTagArrayToIntArray(_SPropTagArray_r* propTagArray)
		{
			IntPtr handle = new IntPtr((void*)propTagArray);
			SafeRpcMemoryHandle safeRpcMemoryHandle = new SafeRpcMemoryHandle(handle);
			int num = *(int*)propTagArray;
			int[] array = new int[num];
			if (num != 0)
			{
				IntPtr source = new IntPtr((void*)(propTagArray + 4L / (long)sizeof(_SPropTagArray_r)));
				Marshal.Copy(source, array, 0, *(int*)propTagArray);
			}
			if (safeRpcMemoryHandle != null)
			{
				((IDisposable)safeRpcMemoryHandle).Dispose();
			}
			return array;
		}

		public unsafe static int[] ConvertCountedIntArrayFromNative(IntPtr pArray)
		{
			if (pArray != IntPtr.Zero)
			{
				_SPropTagArray_r* ptr = (_SPropTagArray_r*)pArray.ToPointer();
				int num = *(int*)ptr;
				int[] array = new int[num];
				if (num != 0)
				{
					IntPtr source = new IntPtr((void*)(ptr + 4L / (long)sizeof(_SPropTagArray_r)));
					Marshal.Copy(source, array, 0, *(int*)ptr);
				}
				return array;
			}
			return null;
		}

		public static int[] ConvertIntArrayFromNative(IntPtr pArray, int sizeArray)
		{
			if (pArray != IntPtr.Zero)
			{
				int[] array = new int[sizeArray];
				if (sizeArray > 0 && pArray != IntPtr.Zero)
				{
					Marshal.Copy(pArray, array, 0, sizeArray);
				}
				return array;
			}
			return null;
		}

		public static SafeRpcMemoryHandle ConvertCountedIntArrayToNative(int[] intArray)
		{
			return NspiHelper.ConvertIntArrayToPropTagArray(intArray, true);
		}

		public unsafe static string[] ConvertCountedStringArrayFromNative(IntPtr pArray, [MarshalAs(UnmanagedType.U1)] bool isAnsi)
		{
			string[] array = null;
			if (pArray != IntPtr.Zero)
			{
				if (isAnsi)
				{
					_StringsArray* ptr = (_StringsArray*)pArray.ToPointer();
					int num = *(int*)ptr;
					array = new string[num];
					uint num2 = 0;
					if (0 < num)
					{
						do
						{
							ulong num3 = (ulong)((num2 + 1UL / 8UL) * 8L)[ptr / 8];
							string text;
							if (num3 != 0UL)
							{
								IntPtr ptr2 = new IntPtr(num3);
								text = Marshal.PtrToStringAnsi(ptr2);
							}
							else
							{
								text = string.Empty;
							}
							array[num2] = text;
							num2++;
						}
						while (num2 < *(int*)ptr);
					}
				}
				else
				{
					_WStringsArray* ptr3 = (_WStringsArray*)pArray.ToPointer();
					int num4 = *(int*)ptr3;
					array = new string[num4];
					uint num5 = 0;
					if (0 < num4)
					{
						do
						{
							ulong num6 = (ulong)((num5 + 1UL / 8UL) * 8L)[ptr3 / 8];
							string text2;
							if (num6 != 0UL)
							{
								IntPtr ptr4 = new IntPtr(num6);
								text2 = Marshal.PtrToStringUni(ptr4);
							}
							else
							{
								text2 = string.Empty;
							}
							array[num5] = text2;
							num5++;
						}
						while (num5 < *(int*)ptr3);
					}
				}
			}
			return array;
		}

		public unsafe static byte[][] ConvertCountedByteStringArrayFromNative(IntPtr pArray)
		{
			byte[][] array = null;
			if (pArray != IntPtr.Zero)
			{
				_StringsArray* ptr = (_StringsArray*)pArray.ToPointer();
				int num = *(int*)ptr;
				array = new byte[num][];
				uint num2 = 0;
				if (0 < num)
				{
					do
					{
						_StringsArray* ptr2 = (num2 + 1UL / (ulong)sizeof(_StringsArray)) * 8L + ptr / sizeof(_StringsArray);
						ulong num3 = (ulong)(*(long*)ptr2);
						if (num3 != 0UL)
						{
							long num4 = (long)num3;
							long num5 = num4;
							if (*num4 != 0)
							{
								do
								{
									num5 += 1L;
								}
								while (*num5 != 0);
							}
							int num6 = (int)(num5 - num4);
							array[num2] = new byte[num6];
							IntPtr source = new IntPtr(*(long*)ptr2);
							Marshal.Copy(source, array[num2], 0, num6);
						}
						else
						{
							array[num2] = null;
						}
						num2++;
					}
					while (num2 < *(int*)ptr);
				}
			}
			return array;
		}

		public unsafe static byte[][] ConvertCountedEntryIdArrayFromNative(IntPtr pArray)
		{
			byte[][] array = null;
			if (pArray != IntPtr.Zero)
			{
				_SBinaryArray_r* ptr = (_SBinaryArray_r*)pArray.ToPointer();
				uint num = (uint)(*(int*)ptr);
				if (num == 0U || *(long*)(ptr + 8L / (long)sizeof(_SBinaryArray_r)) != 0L)
				{
					int num2 = (int)num;
					if (num2 < 0)
					{
						throw new FailRpcException(string.Format("ENTRYID_r array length cannot be negative; length={0}", num2), -2147467259);
					}
					array = new byte[num2][];
					if (*(long*)(ptr + 8L / (long)sizeof(_SBinaryArray_r)) != 0L)
					{
						int num3 = 0;
						long num4 = (long)num2;
						if (0L < num4)
						{
							long num5 = 0L;
							ulong num6 = (ulong)num4;
							do
							{
								_SBinary_r* ptr2 = num5 + *(long*)(ptr + 8L / (long)sizeof(_SBinaryArray_r));
								uint num7 = (uint)(*(int*)ptr2);
								if (num7 == 0U || *(long*)(ptr2 + 8L / (long)sizeof(_SBinary_r)) != 0L)
								{
									array[num3] = new byte[num7];
									IntPtr source = new IntPtr(*(long*)(ptr2 + 8L / (long)sizeof(_SBinary_r)));
									Marshal.Copy(source, array[num3], 0, *(int*)ptr2);
								}
								num3++;
								num5 += 16L;
								num6 -= 1UL;
							}
							while (num6 > 0UL);
						}
					}
				}
			}
			return array;
		}

		public static SafeRpcMemoryHandle ConvertNspiStateToNative(NspiState state)
		{
			SafeRpcMemoryHandle safeRpcMemoryHandle = new SafeRpcMemoryHandle();
			if (state != null)
			{
				safeRpcMemoryHandle.Allocate(36);
				IntPtr dst = safeRpcMemoryHandle.DangerousGetHandle();
				state.MarshalToNative(dst);
			}
			return safeRpcMemoryHandle;
		}

		public static NspiState ConvertNspiStateFromNative(SafeRpcMemoryHandle rpcMemoryHandle)
		{
			NspiState result = null;
			if (rpcMemoryHandle != null && rpcMemoryHandle.DangerousGetHandle() != IntPtr.Zero)
			{
				result = new NspiState(rpcMemoryHandle.DangerousGetHandle());
			}
			return result;
		}

		public static SafeRpcMemoryHandle ConvertGuidToNative(Guid guid)
		{
			SafeRpcMemoryHandle safeRpcMemoryHandle = new SafeRpcMemoryHandle(16);
			IntPtr ptr = safeRpcMemoryHandle.DangerousGetHandle();
			Marshal.StructureToPtr(guid, ptr, false);
			return safeRpcMemoryHandle;
		}

		public static Guid ConvertGuidFromNative(IntPtr pGuid)
		{
			Guid result = Guid.Empty;
			if (pGuid != IntPtr.Zero)
			{
				result = (Guid)Marshal.PtrToStructure(pGuid, typeof(Guid));
			}
			return result;
		}
	}
}
