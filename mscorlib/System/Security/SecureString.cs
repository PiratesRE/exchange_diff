using System;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Microsoft.Win32;

namespace System.Security
{
	public sealed class SecureString : IDisposable
	{
		[SecurityCritical]
		private static bool EncryptionSupported()
		{
			bool result = true;
			try
			{
				Win32Native.SystemFunction041(SafeBSTRHandle.Allocate(null, 16U), 16U, 0U);
			}
			catch (EntryPointNotFoundException)
			{
				result = false;
			}
			return result;
		}

		[SecurityCritical]
		internal SecureString(SecureString str)
		{
			this.AllocateBuffer(str.BufferLength);
			SafeBSTRHandle.Copy(str.m_buffer, this.m_buffer);
			this.m_length = str.m_length;
			this.m_encrypted = str.m_encrypted;
		}

		[SecuritySafeCritical]
		public SecureString()
		{
			this.CheckSupportedOnCurrentPlatform();
			this.AllocateBuffer(8);
			this.m_length = 0;
		}

		[SecurityCritical]
		[HandleProcessCorruptedStateExceptions]
		private unsafe void InitializeSecureString(char* value, int length)
		{
			this.CheckSupportedOnCurrentPlatform();
			this.AllocateBuffer(length);
			this.m_length = length;
			byte* ptr = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				this.m_buffer.AcquirePointer(ref ptr);
				Buffer.Memcpy(ptr, (byte*)value, length * 2);
			}
			catch (Exception)
			{
				this.ProtectMemory();
				throw;
			}
			finally
			{
				if (ptr != null)
				{
					this.m_buffer.ReleasePointer();
				}
			}
			this.ProtectMemory();
		}

		[SecurityCritical]
		[CLSCompliant(false)]
		public unsafe SecureString(char* value, int length)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException("length", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (length > 65536)
			{
				throw new ArgumentOutOfRangeException("length", Environment.GetResourceString("ArgumentOutOfRange_Length"));
			}
			this.InitializeSecureString(value, length);
		}

		public int Length
		{
			[SecuritySafeCritical]
			[MethodImpl(MethodImplOptions.Synchronized)]
			get
			{
				this.EnsureNotDisposed();
				return this.m_length;
			}
		}

		[SecuritySafeCritical]
		[HandleProcessCorruptedStateExceptions]
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void AppendChar(char c)
		{
			this.EnsureNotDisposed();
			this.EnsureNotReadOnly();
			this.EnsureCapacity(this.m_length + 1);
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				this.UnProtectMemory();
				this.m_buffer.Write<char>((ulong)(this.m_length * 2), c);
				this.m_length++;
			}
			catch (Exception)
			{
				this.ProtectMemory();
				throw;
			}
			finally
			{
				this.ProtectMemory();
			}
		}

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Clear()
		{
			this.EnsureNotDisposed();
			this.EnsureNotReadOnly();
			this.m_length = 0;
			this.m_buffer.ClearBuffer();
			this.m_encrypted = false;
		}

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.Synchronized)]
		public SecureString Copy()
		{
			this.EnsureNotDisposed();
			return new SecureString(this);
		}

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void Dispose()
		{
			if (this.m_buffer != null && !this.m_buffer.IsInvalid)
			{
				this.m_buffer.Close();
				this.m_buffer = null;
			}
		}

		[SecuritySafeCritical]
		[HandleProcessCorruptedStateExceptions]
		[MethodImpl(MethodImplOptions.Synchronized)]
		public unsafe void InsertAt(int index, char c)
		{
			if (index < 0 || index > this.m_length)
			{
				throw new ArgumentOutOfRangeException("index", Environment.GetResourceString("ArgumentOutOfRange_IndexString"));
			}
			this.EnsureNotDisposed();
			this.EnsureNotReadOnly();
			this.EnsureCapacity(this.m_length + 1);
			byte* ptr = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				this.UnProtectMemory();
				this.m_buffer.AcquirePointer(ref ptr);
				char* ptr2 = (char*)ptr;
				for (int i = this.m_length; i > index; i--)
				{
					ptr2[i] = ptr2[i - 1];
				}
				ptr2[index] = c;
				this.m_length++;
			}
			catch (Exception)
			{
				this.ProtectMemory();
				throw;
			}
			finally
			{
				this.ProtectMemory();
				if (ptr != null)
				{
					this.m_buffer.ReleasePointer();
				}
			}
		}

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.Synchronized)]
		public bool IsReadOnly()
		{
			this.EnsureNotDisposed();
			return this.m_readOnly;
		}

		[SecuritySafeCritical]
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void MakeReadOnly()
		{
			this.EnsureNotDisposed();
			this.m_readOnly = true;
		}

		[SecuritySafeCritical]
		[HandleProcessCorruptedStateExceptions]
		[MethodImpl(MethodImplOptions.Synchronized)]
		public unsafe void RemoveAt(int index)
		{
			this.EnsureNotDisposed();
			this.EnsureNotReadOnly();
			if (index < 0 || index >= this.m_length)
			{
				throw new ArgumentOutOfRangeException("index", Environment.GetResourceString("ArgumentOutOfRange_IndexString"));
			}
			byte* ptr = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				this.UnProtectMemory();
				this.m_buffer.AcquirePointer(ref ptr);
				char* ptr2 = (char*)ptr;
				for (int i = index; i < this.m_length - 1; i++)
				{
					ptr2[i] = ptr2[i + 1];
				}
				ref short ptr3 = ref *(short*)ptr2;
				int num = this.m_length - 1;
				this.m_length = num;
				*(ref ptr3 + (IntPtr)num * 2) = 0;
			}
			catch (Exception)
			{
				this.ProtectMemory();
				throw;
			}
			finally
			{
				this.ProtectMemory();
				if (ptr != null)
				{
					this.m_buffer.ReleasePointer();
				}
			}
		}

		[SecuritySafeCritical]
		[HandleProcessCorruptedStateExceptions]
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void SetAt(int index, char c)
		{
			if (index < 0 || index >= this.m_length)
			{
				throw new ArgumentOutOfRangeException("index", Environment.GetResourceString("ArgumentOutOfRange_IndexString"));
			}
			this.EnsureNotDisposed();
			this.EnsureNotReadOnly();
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				this.UnProtectMemory();
				this.m_buffer.Write<char>((ulong)(index * 2), c);
			}
			catch (Exception)
			{
				this.ProtectMemory();
				throw;
			}
			finally
			{
				this.ProtectMemory();
			}
		}

		private int BufferLength
		{
			[SecurityCritical]
			get
			{
				return this.m_buffer.Length;
			}
		}

		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		private void AllocateBuffer(int size)
		{
			uint alignedSize = SecureString.GetAlignedSize(size);
			this.m_buffer = SafeBSTRHandle.Allocate(null, alignedSize);
			if (this.m_buffer.IsInvalid)
			{
				throw new OutOfMemoryException();
			}
		}

		private void CheckSupportedOnCurrentPlatform()
		{
			if (!SecureString.supportedOnCurrentPlatform)
			{
				throw new NotSupportedException(Environment.GetResourceString("Arg_PlatformSecureString"));
			}
		}

		[SecurityCritical]
		private void EnsureCapacity(int capacity)
		{
			if (capacity > 65536)
			{
				throw new ArgumentOutOfRangeException("capacity", Environment.GetResourceString("ArgumentOutOfRange_Capacity"));
			}
			if (capacity <= this.m_buffer.Length)
			{
				return;
			}
			SafeBSTRHandle safeBSTRHandle = SafeBSTRHandle.Allocate(null, SecureString.GetAlignedSize(capacity));
			if (safeBSTRHandle.IsInvalid)
			{
				throw new OutOfMemoryException();
			}
			SafeBSTRHandle.Copy(this.m_buffer, safeBSTRHandle);
			this.m_buffer.Close();
			this.m_buffer = safeBSTRHandle;
		}

		[SecurityCritical]
		private void EnsureNotDisposed()
		{
			if (this.m_buffer == null)
			{
				throw new ObjectDisposedException(null);
			}
		}

		private void EnsureNotReadOnly()
		{
			if (this.m_readOnly)
			{
				throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ReadOnly"));
			}
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		private static uint GetAlignedSize(int size)
		{
			uint num = (uint)(size / 8 * 8);
			if (size % 8 != 0 || size == 0)
			{
				num += 8U;
			}
			return num;
		}

		[SecurityCritical]
		private unsafe int GetAnsiByteCount()
		{
			uint flags = 1024U;
			uint num = 63U;
			byte* ptr = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			int result;
			try
			{
				this.m_buffer.AcquirePointer(ref ptr);
				result = Win32Native.WideCharToMultiByte(0U, flags, (char*)ptr, this.m_length, null, 0, IntPtr.Zero, new IntPtr((void*)(&num)));
			}
			finally
			{
				if (ptr != null)
				{
					this.m_buffer.ReleasePointer();
				}
			}
			return result;
		}

		[SecurityCritical]
		private unsafe void GetAnsiBytes(byte* ansiStrPtr, int byteCount)
		{
			uint flags = 1024U;
			uint num = 63U;
			byte* ptr = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				this.m_buffer.AcquirePointer(ref ptr);
				Win32Native.WideCharToMultiByte(0U, flags, (char*)ptr, this.m_length, ansiStrPtr, byteCount - 1, IntPtr.Zero, new IntPtr((void*)(&num)));
				*(ansiStrPtr + byteCount - 1) = 0;
			}
			finally
			{
				if (ptr != null)
				{
					this.m_buffer.ReleasePointer();
				}
			}
		}

		[SecurityCritical]
		[ReliabilityContract(Consistency.MayCorruptInstance, Cer.MayFail)]
		private void ProtectMemory()
		{
			if (this.m_length == 0 || this.m_encrypted)
			{
				return;
			}
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
			}
			finally
			{
				int num = Win32Native.SystemFunction040(this.m_buffer, (uint)(this.m_buffer.Length * 2), 0U);
				if (num < 0)
				{
					throw new CryptographicException(Win32Native.LsaNtStatusToWinError(num));
				}
				this.m_encrypted = true;
			}
		}

		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[HandleProcessCorruptedStateExceptions]
		[MethodImpl(MethodImplOptions.Synchronized)]
		internal unsafe IntPtr ToBSTR()
		{
			this.EnsureNotDisposed();
			int length = this.m_length;
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2 = IntPtr.Zero;
			byte* ptr = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				RuntimeHelpers.PrepareConstrainedRegions();
				try
				{
				}
				finally
				{
					intPtr = Win32Native.SysAllocStringLen(null, length);
				}
				if (intPtr == IntPtr.Zero)
				{
					throw new OutOfMemoryException();
				}
				this.UnProtectMemory();
				this.m_buffer.AcquirePointer(ref ptr);
				Buffer.Memcpy((byte*)intPtr.ToPointer(), ptr, length * 2);
				intPtr2 = intPtr;
			}
			catch (Exception)
			{
				this.ProtectMemory();
				throw;
			}
			finally
			{
				this.ProtectMemory();
				if (intPtr2 == IntPtr.Zero && intPtr != IntPtr.Zero)
				{
					Win32Native.ZeroMemory(intPtr, (UIntPtr)((ulong)((long)(length * 2))));
					Win32Native.SysFreeString(intPtr);
				}
				if (ptr != null)
				{
					this.m_buffer.ReleasePointer();
				}
			}
			return intPtr2;
		}

		[SecurityCritical]
		[HandleProcessCorruptedStateExceptions]
		[MethodImpl(MethodImplOptions.Synchronized)]
		internal unsafe IntPtr ToUniStr(bool allocateFromHeap)
		{
			this.EnsureNotDisposed();
			int length = this.m_length;
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2 = IntPtr.Zero;
			byte* ptr = null;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				RuntimeHelpers.PrepareConstrainedRegions();
				try
				{
				}
				finally
				{
					if (allocateFromHeap)
					{
						intPtr = Marshal.AllocHGlobal((length + 1) * 2);
					}
					else
					{
						intPtr = Marshal.AllocCoTaskMem((length + 1) * 2);
					}
				}
				if (intPtr == IntPtr.Zero)
				{
					throw new OutOfMemoryException();
				}
				this.UnProtectMemory();
				this.m_buffer.AcquirePointer(ref ptr);
				Buffer.Memcpy((byte*)intPtr.ToPointer(), ptr, length * 2);
				char* ptr2 = (char*)intPtr.ToPointer();
				ptr2[length] = '\0';
				intPtr2 = intPtr;
			}
			catch (Exception)
			{
				this.ProtectMemory();
				throw;
			}
			finally
			{
				this.ProtectMemory();
				if (intPtr2 == IntPtr.Zero && intPtr != IntPtr.Zero)
				{
					Win32Native.ZeroMemory(intPtr, (UIntPtr)((ulong)((long)(length * 2))));
					if (allocateFromHeap)
					{
						Marshal.FreeHGlobal(intPtr);
					}
					else
					{
						Marshal.FreeCoTaskMem(intPtr);
					}
				}
				if (ptr != null)
				{
					this.m_buffer.ReleasePointer();
				}
			}
			return intPtr2;
		}

		[SecurityCritical]
		[HandleProcessCorruptedStateExceptions]
		[MethodImpl(MethodImplOptions.Synchronized)]
		internal unsafe IntPtr ToAnsiStr(bool allocateFromHeap)
		{
			this.EnsureNotDisposed();
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2 = IntPtr.Zero;
			int num = 0;
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
				this.UnProtectMemory();
				num = this.GetAnsiByteCount() + 1;
				RuntimeHelpers.PrepareConstrainedRegions();
				try
				{
				}
				finally
				{
					if (allocateFromHeap)
					{
						intPtr = Marshal.AllocHGlobal(num);
					}
					else
					{
						intPtr = Marshal.AllocCoTaskMem(num);
					}
				}
				if (intPtr == IntPtr.Zero)
				{
					throw new OutOfMemoryException();
				}
				this.GetAnsiBytes((byte*)intPtr.ToPointer(), num);
				intPtr2 = intPtr;
			}
			catch (Exception)
			{
				this.ProtectMemory();
				throw;
			}
			finally
			{
				this.ProtectMemory();
				if (intPtr2 == IntPtr.Zero && intPtr != IntPtr.Zero)
				{
					Win32Native.ZeroMemory(intPtr, (UIntPtr)((ulong)((long)num)));
					if (allocateFromHeap)
					{
						Marshal.FreeHGlobal(intPtr);
					}
					else
					{
						Marshal.FreeCoTaskMem(intPtr);
					}
				}
			}
			return intPtr2;
		}

		[SecurityCritical]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		private void UnProtectMemory()
		{
			if (this.m_length == 0)
			{
				return;
			}
			RuntimeHelpers.PrepareConstrainedRegions();
			try
			{
			}
			finally
			{
				if (this.m_encrypted)
				{
					int num = Win32Native.SystemFunction041(this.m_buffer, (uint)(this.m_buffer.Length * 2), 0U);
					if (num < 0)
					{
						throw new CryptographicException(Win32Native.LsaNtStatusToWinError(num));
					}
					this.m_encrypted = false;
				}
			}
		}

		[SecurityCritical]
		private SafeBSTRHandle m_buffer;

		private int m_length;

		private bool m_readOnly;

		private bool m_encrypted;

		private static bool supportedOnCurrentPlatform = SecureString.EncryptionSupported();

		private const int BlockSize = 8;

		private const int MaxLength = 65536;

		private const uint ProtectionScope = 0U;
	}
}
