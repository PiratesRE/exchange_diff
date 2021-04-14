using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace System.IO
{
	internal sealed class __ConsoleStream : Stream
	{
		[SecurityCritical]
		internal __ConsoleStream(SafeFileHandle handle, FileAccess access, bool useFileAPIs)
		{
			this._handle = handle;
			this._canRead = ((access & FileAccess.Read) == FileAccess.Read);
			this._canWrite = ((access & FileAccess.Write) == FileAccess.Write);
			this._useFileAPIs = useFileAPIs;
			this._isPipe = (Win32Native.GetFileType(handle) == 3);
		}

		public override bool CanRead
		{
			get
			{
				return this._canRead;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this._canWrite;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override long Length
		{
			get
			{
				__Error.SeekNotSupported();
				return 0L;
			}
		}

		public override long Position
		{
			get
			{
				__Error.SeekNotSupported();
				return 0L;
			}
			set
			{
				__Error.SeekNotSupported();
			}
		}

		[SecuritySafeCritical]
		protected override void Dispose(bool disposing)
		{
			if (this._handle != null)
			{
				this._handle = null;
			}
			this._canRead = false;
			this._canWrite = false;
			base.Dispose(disposing);
		}

		[SecuritySafeCritical]
		public override void Flush()
		{
			if (this._handle == null)
			{
				__Error.FileNotOpen();
			}
			if (!this.CanWrite)
			{
				__Error.WriteNotSupported();
			}
		}

		public override void SetLength(long value)
		{
			__Error.SeekNotSupported();
		}

		[SecuritySafeCritical]
		public override int Read([In] [Out] byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException((offset < 0) ? "offset" : "count", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidOffLen"));
			}
			if (!this._canRead)
			{
				__Error.ReadNotSupported();
			}
			int result;
			int num = __ConsoleStream.ReadFileNative(this._handle, buffer, offset, count, this._useFileAPIs, this._isPipe, out result);
			if (num != 0)
			{
				__Error.WinIOError(num, string.Empty);
			}
			return result;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			__Error.SeekNotSupported();
			return 0L;
		}

		[SecuritySafeCritical]
		public override void Write(byte[] buffer, int offset, int count)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0 || count < 0)
			{
				throw new ArgumentOutOfRangeException((offset < 0) ? "offset" : "count", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidOffLen"));
			}
			if (!this._canWrite)
			{
				__Error.WriteNotSupported();
			}
			int num = __ConsoleStream.WriteFileNative(this._handle, buffer, offset, count, this._useFileAPIs);
			if (num != 0)
			{
				__Error.WinIOError(num, string.Empty);
			}
		}

		[SecurityCritical]
		private unsafe static int ReadFileNative(SafeFileHandle hFile, byte[] bytes, int offset, int count, bool useFileAPIs, bool isPipe, out int bytesRead)
		{
			if (bytes.Length - offset < count)
			{
				throw new IndexOutOfRangeException(Environment.GetResourceString("IndexOutOfRange_IORaceCondition"));
			}
			if (bytes.Length == 0)
			{
				bytesRead = 0;
				return 0;
			}
			__ConsoleStream.WaitForAvailableConsoleInput(hFile, isPipe);
			bool flag;
			if (useFileAPIs)
			{
				fixed (byte* ptr = bytes)
				{
					flag = (Win32Native.ReadFile(hFile, ptr + offset, count, out bytesRead, IntPtr.Zero) != 0);
				}
			}
			else
			{
				fixed (byte* ptr2 = bytes)
				{
					int num;
					flag = Win32Native.ReadConsoleW(hFile, ptr2 + offset, count / 2, out num, IntPtr.Zero);
					bytesRead = num * 2;
				}
			}
			if (flag)
			{
				return 0;
			}
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (lastWin32Error == 232 || lastWin32Error == 109)
			{
				return 0;
			}
			return lastWin32Error;
		}

		[SecurityCritical]
		private unsafe static int WriteFileNative(SafeFileHandle hFile, byte[] bytes, int offset, int count, bool useFileAPIs)
		{
			if (bytes.Length == 0)
			{
				return 0;
			}
			bool flag;
			if (useFileAPIs)
			{
				fixed (byte* ptr = bytes)
				{
					int num;
					flag = (Win32Native.WriteFile(hFile, ptr + offset, count, out num, IntPtr.Zero) != 0);
				}
			}
			else
			{
				fixed (byte* ptr2 = bytes)
				{
					int num2;
					flag = Win32Native.WriteConsoleW(hFile, ptr2 + offset, count / 2, out num2, IntPtr.Zero);
				}
			}
			if (flag)
			{
				return 0;
			}
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (lastWin32Error == 232 || lastWin32Error == 109)
			{
				return 0;
			}
			return lastWin32Error;
		}

		[SecurityCritical]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void WaitForAvailableConsoleInput(SafeFileHandle file, bool isPipe);

		private const int BytesPerWChar = 2;

		[SecurityCritical]
		private SafeFileHandle _handle;

		private bool _canRead;

		private bool _canWrite;

		private bool _useFileAPIs;

		private bool _isPipe;
	}
}
