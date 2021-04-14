using System;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	public static class Buffer
	{
		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void BlockCopy(Array src, int srcOffset, Array dst, int dstOffset, int count);

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void InternalBlockCopy(Array src, int srcOffsetBytes, Array dst, int dstOffsetBytes, int byteCount);

		[SecurityCritical]
		internal unsafe static int IndexOfByte(byte* src, byte value, int index, int count)
		{
			byte* ptr = src + index;
			while ((ptr & 3) != 0)
			{
				if (count == 0)
				{
					return -1;
				}
				if (*ptr == value)
				{
					return (int)((long)(ptr - src));
				}
				count--;
				ptr++;
			}
			uint num = (uint)(((int)value << 8) + (int)value);
			num = (num << 16) + num;
			while (count > 3)
			{
				uint num2 = *(uint*)ptr;
				num2 ^= num;
				uint num3 = 2130640639U + num2;
				num2 ^= uint.MaxValue;
				num2 ^= num3;
				num2 &= 2164326656U;
				if (num2 != 0U)
				{
					int num4 = (int)((long)(ptr - src));
					if (*ptr == value)
					{
						return num4;
					}
					if (ptr[1] == value)
					{
						return num4 + 1;
					}
					if (ptr[2] == value)
					{
						return num4 + 2;
					}
					if (ptr[3] == value)
					{
						return num4 + 3;
					}
				}
				count -= 4;
				ptr += 4;
			}
			while (count > 0)
			{
				if (*ptr == value)
				{
					return (int)((long)(ptr - src));
				}
				count--;
				ptr++;
			}
			return -1;
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool IsPrimitiveTypeArray(Array array);

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern byte _GetByte(Array array, int index);

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public static byte GetByte(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (!Buffer.IsPrimitiveTypeArray(array))
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_MustBePrimArray"), "array");
			}
			if (index < 0 || index >= Buffer._ByteLength(array))
			{
				throw new ArgumentOutOfRangeException("index");
			}
			return Buffer._GetByte(array, index);
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void _SetByte(Array array, int index, byte value);

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public static void SetByte(Array array, int index, byte value)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (!Buffer.IsPrimitiveTypeArray(array))
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_MustBePrimArray"), "array");
			}
			if (index < 0 || index >= Buffer._ByteLength(array))
			{
				throw new ArgumentOutOfRangeException("index");
			}
			Buffer._SetByte(array, index, value);
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int _ByteLength(Array array);

		[SecuritySafeCritical]
		[__DynamicallyInvokable]
		public static int ByteLength(Array array)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (!Buffer.IsPrimitiveTypeArray(array))
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_MustBePrimArray"), "array");
			}
			return Buffer._ByteLength(array);
		}

		[SecurityCritical]
		internal unsafe static void ZeroMemory(byte* src, long len)
		{
			for (;;)
			{
				long num = len;
				len = num - 1L;
				if (num <= 0L)
				{
					break;
				}
				src[len] = 0;
			}
		}

		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		internal unsafe static void Memcpy(byte[] dest, int destIndex, byte* src, int srcIndex, int len)
		{
			if (len == 0)
			{
				return;
			}
			fixed (byte* ptr = dest)
			{
				Buffer.Memcpy(ptr + destIndex, src + srcIndex, len);
			}
		}

		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		internal unsafe static void Memcpy(byte* pDest, int destIndex, byte[] src, int srcIndex, int len)
		{
			if (len == 0)
			{
				return;
			}
			fixed (byte* ptr = src)
			{
				Buffer.Memcpy(pDest + destIndex, ptr + srcIndex, len);
			}
		}

		[FriendAccessAllowed]
		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal unsafe static void Memcpy(byte* dest, byte* src, int len)
		{
			Buffer.Memmove(dest, src, (ulong)len);
		}

		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		internal unsafe static void Memmove(byte* dest, byte* src, ulong len)
		{
			if ((ulong)(dest - src) >= len && (ulong)(src - dest) >= len)
			{
				byte* ptr = src + len;
				byte* ptr2 = dest + len;
				if (len > 16UL)
				{
					if (len > 64UL)
					{
						if (len > 2048UL)
						{
							goto IL_10D;
						}
						ulong num = len >> 6;
						do
						{
							*(Buffer.Block64*)dest = *(Buffer.Block64*)src;
							dest += 64;
							src += 64;
							num -= 1UL;
						}
						while (num != 0UL);
						len %= 64UL;
						if (len <= 16UL)
						{
							*(Buffer.Block16*)(ptr2 - 16) = *(Buffer.Block16*)(ptr - 16);
							return;
						}
					}
					*(Buffer.Block16*)dest = *(Buffer.Block16*)src;
					if (len > 32UL)
					{
						*(Buffer.Block16*)(dest + 16) = *(Buffer.Block16*)(src + 16);
						if (len > 48UL)
						{
							*(Buffer.Block16*)(dest + 32) = *(Buffer.Block16*)(src + 32);
						}
					}
					*(Buffer.Block16*)(ptr2 - 16) = *(Buffer.Block16*)(ptr - 16);
					return;
				}
				if ((len & 24UL) != 0UL)
				{
					*(long*)dest = *(long*)src;
					*(long*)(ptr2 - 8) = *(long*)(ptr - 8);
					return;
				}
				if ((len & 4UL) != 0UL)
				{
					*(int*)dest = *(int*)src;
					*(int*)(ptr2 - 4) = *(int*)(ptr - 4);
					return;
				}
				if (len == 0UL)
				{
					return;
				}
				*dest = *src;
				if ((len & 2UL) == 0UL)
				{
					return;
				}
				*(short*)(ptr2 - 2) = *(short*)(ptr - 2);
				return;
			}
			IL_10D:
			Buffer._Memmove(dest, src, len);
		}

		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[MethodImpl(MethodImplOptions.NoInlining)]
		private unsafe static void _Memmove(byte* dest, byte* src, ulong len)
		{
			Buffer.__Memmove(dest, src, len);
		}

		[SuppressUnmanagedCodeSecurity]
		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("QCall", CharSet = CharSet.Unicode)]
		private unsafe static extern void __Memmove(byte* dest, byte* src, ulong len);

		[SecurityCritical]
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void MemoryCopy(void* source, void* destination, long destinationSizeInBytes, long sourceBytesToCopy)
		{
			if (sourceBytesToCopy > destinationSizeInBytes)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.sourceBytesToCopy);
			}
			Buffer.Memmove((byte*)destination, (byte*)source, checked((ulong)sourceBytesToCopy));
		}

		[SecurityCritical]
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void MemoryCopy(void* source, void* destination, ulong destinationSizeInBytes, ulong sourceBytesToCopy)
		{
			if (sourceBytesToCopy > destinationSizeInBytes)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.sourceBytesToCopy);
			}
			Buffer.Memmove((byte*)destination, (byte*)source, sourceBytesToCopy);
		}

		private struct Block16
		{
		}

		private struct Block64
		{
		}
	}
}
