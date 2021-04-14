using System;
using System.Diagnostics.Tracing;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace System.IO
{
	[ComVisible(true)]
	public class FileStream : Stream
	{
		internal FileStream()
		{
		}

		[SecuritySafeCritical]
		public FileStream(string path, FileMode mode) : this(path, mode, (mode == FileMode.Append) ? FileAccess.Write : FileAccess.ReadWrite, FileShare.Read, 4096, FileOptions.None, Path.GetFileName(path), false)
		{
		}

		[SecuritySafeCritical]
		public FileStream(string path, FileMode mode, FileAccess access) : this(path, mode, access, FileShare.Read, 4096, FileOptions.None, Path.GetFileName(path), false)
		{
		}

		[SecuritySafeCritical]
		public FileStream(string path, FileMode mode, FileAccess access, FileShare share) : this(path, mode, access, share, 4096, FileOptions.None, Path.GetFileName(path), false)
		{
		}

		[SecuritySafeCritical]
		public FileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize) : this(path, mode, access, share, bufferSize, FileOptions.None, Path.GetFileName(path), false)
		{
		}

		[SecuritySafeCritical]
		public FileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options) : this(path, mode, access, share, bufferSize, options, Path.GetFileName(path), false)
		{
		}

		[SecuritySafeCritical]
		public FileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool useAsync) : this(path, mode, access, share, bufferSize, useAsync ? FileOptions.Asynchronous : FileOptions.None, Path.GetFileName(path), false)
		{
		}

		[SecuritySafeCritical]
		public FileStream(string path, FileMode mode, FileSystemRights rights, FileShare share, int bufferSize, FileOptions options, FileSecurity fileSecurity)
		{
			object obj;
			Win32Native.SECURITY_ATTRIBUTES secAttrs = FileStream.GetSecAttrs(share, fileSecurity, out obj);
			try
			{
				this.Init(path, mode, (FileAccess)0, (int)rights, true, share, bufferSize, options, secAttrs, Path.GetFileName(path), false, false, false);
			}
			finally
			{
				if (obj != null)
				{
					((GCHandle)obj).Free();
				}
			}
		}

		[SecuritySafeCritical]
		public FileStream(string path, FileMode mode, FileSystemRights rights, FileShare share, int bufferSize, FileOptions options)
		{
			Win32Native.SECURITY_ATTRIBUTES secAttrs = FileStream.GetSecAttrs(share);
			this.Init(path, mode, (FileAccess)0, (int)rights, true, share, bufferSize, options, secAttrs, Path.GetFileName(path), false, false, false);
		}

		[SecurityCritical]
		internal FileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options, string msgPath, bool bFromProxy)
		{
			Win32Native.SECURITY_ATTRIBUTES secAttrs = FileStream.GetSecAttrs(share);
			this.Init(path, mode, access, 0, false, share, bufferSize, options, secAttrs, msgPath, bFromProxy, false, false);
		}

		[SecurityCritical]
		internal FileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options, string msgPath, bool bFromProxy, bool useLongPath)
		{
			Win32Native.SECURITY_ATTRIBUTES secAttrs = FileStream.GetSecAttrs(share);
			this.Init(path, mode, access, 0, false, share, bufferSize, options, secAttrs, msgPath, bFromProxy, useLongPath, false);
		}

		[SecurityCritical]
		internal FileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options, string msgPath, bool bFromProxy, bool useLongPath, bool checkHost)
		{
			Win32Native.SECURITY_ATTRIBUTES secAttrs = FileStream.GetSecAttrs(share);
			this.Init(path, mode, access, 0, false, share, bufferSize, options, secAttrs, msgPath, bFromProxy, useLongPath, checkHost);
		}

		[SecuritySafeCritical]
		private void Init(string path, FileMode mode, FileAccess access, int rights, bool useRights, FileShare share, int bufferSize, FileOptions options, Win32Native.SECURITY_ATTRIBUTES secAttrs, string msgPath, bool bFromProxy, bool useLongPath, bool checkHost)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path", Environment.GetResourceString("ArgumentNull_Path"));
			}
			if (path.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyPath"));
			}
			this._fileName = msgPath;
			this._exposedHandle = false;
			FileShare fileShare = share & ~FileShare.Inheritable;
			string text = null;
			if (mode < FileMode.CreateNew || mode > FileMode.Append)
			{
				text = "mode";
			}
			else if (!useRights && (access < FileAccess.Read || access > FileAccess.ReadWrite))
			{
				text = "access";
			}
			else if (useRights && (rights < 1 || rights > 2032127))
			{
				text = "rights";
			}
			else if (fileShare < FileShare.None || fileShare > (FileShare.Read | FileShare.Write | FileShare.Delete))
			{
				text = "share";
			}
			if (text != null)
			{
				throw new ArgumentOutOfRangeException(text, Environment.GetResourceString("ArgumentOutOfRange_Enum"));
			}
			if (options != FileOptions.None && (options & (FileOptions)67092479) != FileOptions.None)
			{
				throw new ArgumentOutOfRangeException("options", Environment.GetResourceString("ArgumentOutOfRange_Enum"));
			}
			if (bufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("bufferSize", Environment.GetResourceString("ArgumentOutOfRange_NeedPosNum"));
			}
			if (((!useRights && (access & FileAccess.Write) == (FileAccess)0) || (useRights && (rights & 278) == 0)) && (mode == FileMode.Truncate || mode == FileMode.CreateNew || mode == FileMode.Create || mode == FileMode.Append))
			{
				if (!useRights)
				{
					throw new ArgumentException(Environment.GetResourceString("Argument_InvalidFileMode&AccessCombo", new object[]
					{
						mode,
						access
					}));
				}
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidFileMode&RightsCombo", new object[]
				{
					mode,
					(FileSystemRights)rights
				}));
			}
			else
			{
				if (useRights && mode == FileMode.Truncate)
				{
					if (rights != 278)
					{
						throw new ArgumentException(Environment.GetResourceString("Argument_InvalidFileModeTruncate&RightsCombo", new object[]
						{
							mode,
							(FileSystemRights)rights
						}));
					}
					useRights = false;
					access = FileAccess.Write;
				}
				int dwDesiredAccess;
				if (!useRights)
				{
					dwDesiredAccess = ((access == FileAccess.Read) ? int.MinValue : ((access == FileAccess.Write) ? 1073741824 : -1073741824));
				}
				else
				{
					dwDesiredAccess = rights;
				}
				int maxPathLength = useLongPath ? 32767 : (AppContextSwitches.BlockLongPaths ? 260 : 32767);
				string text2 = Path.NormalizePath(path, true, maxPathLength);
				this._fileName = text2;
				if ((!CodeAccessSecurityEngine.QuickCheckForAllDemands() || AppContextSwitches.UseLegacyPathHandling) && text2.StartsWith("\\\\.\\", StringComparison.Ordinal))
				{
					throw new ArgumentException(Environment.GetResourceString("Arg_DevicesNotSupported"));
				}
				bool flag = false;
				if ((!useRights && (access & FileAccess.Read) != (FileAccess)0) || (useRights && (rights & 131241) != 0))
				{
					if (mode == FileMode.Append)
					{
						throw new ArgumentException(Environment.GetResourceString("Argument_InvalidAppendMode"));
					}
					flag = true;
				}
				if (CodeAccessSecurityEngine.QuickCheckForAllDemands())
				{
					FileIOPermission.EmulateFileIOPermissionChecks(text2);
				}
				else
				{
					FileIOPermissionAccess fileIOPermissionAccess = FileIOPermissionAccess.NoAccess;
					if (flag)
					{
						fileIOPermissionAccess |= FileIOPermissionAccess.Read;
					}
					if ((!useRights && (access & FileAccess.Write) != (FileAccess)0) || (useRights && (rights & 852310) != 0) || (useRights && (rights & 1048576) != 0 && mode == FileMode.OpenOrCreate))
					{
						if (mode == FileMode.Append)
						{
							fileIOPermissionAccess |= FileIOPermissionAccess.Append;
						}
						else
						{
							fileIOPermissionAccess |= FileIOPermissionAccess.Write;
						}
					}
					AccessControlActions control = (secAttrs != null && secAttrs.pSecurityDescriptor != null) ? AccessControlActions.Change : AccessControlActions.None;
					FileIOPermission.QuickDemand(fileIOPermissionAccess, control, new string[]
					{
						text2
					}, false, false);
				}
				share &= ~FileShare.Inheritable;
				bool flag2 = mode == FileMode.Append;
				if (mode == FileMode.Append)
				{
					mode = FileMode.OpenOrCreate;
				}
				if ((options & FileOptions.Asynchronous) != FileOptions.None)
				{
					this._isAsync = true;
				}
				else
				{
					options &= ~FileOptions.Asynchronous;
				}
				int num = (int)options;
				num |= 1048576;
				int errorMode = Win32Native.SetErrorMode(1);
				try
				{
					string text3 = text2;
					if (useLongPath)
					{
						text3 = Path.AddLongPathPrefix(text3);
					}
					this._handle = Win32Native.SafeCreateFile(text3, dwDesiredAccess, share, secAttrs, mode, num, IntPtr.Zero);
					if (this._handle.IsInvalid)
					{
						int num2 = Marshal.GetLastWin32Error();
						if (num2 == 3 && text2.Equals(Directory.InternalGetDirectoryRoot(text2)))
						{
							num2 = 5;
						}
						bool flag3 = false;
						if (!bFromProxy)
						{
							try
							{
								FileIOPermission.QuickDemand(FileIOPermissionAccess.PathDiscovery, this._fileName, false, false);
								flag3 = true;
							}
							catch (SecurityException)
							{
							}
						}
						if (flag3)
						{
							__Error.WinIOError(num2, this._fileName);
						}
						else
						{
							__Error.WinIOError(num2, msgPath);
						}
					}
				}
				finally
				{
					Win32Native.SetErrorMode(errorMode);
				}
				int fileType = Win32Native.GetFileType(this._handle);
				if (fileType != 1)
				{
					this._handle.Close();
					throw new NotSupportedException(Environment.GetResourceString("NotSupported_FileStreamOnNonFiles"));
				}
				if (this._isAsync)
				{
					bool flag4 = false;
					new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Assert();
					try
					{
						flag4 = ThreadPool.BindHandle(this._handle);
					}
					finally
					{
						CodeAccessPermission.RevertAssert();
						if (!flag4)
						{
							this._handle.Close();
						}
					}
					if (!flag4)
					{
						throw new IOException(Environment.GetResourceString("IO.IO_BindHandleFailed"));
					}
				}
				if (!useRights)
				{
					this._canRead = ((access & FileAccess.Read) > (FileAccess)0);
					this._canWrite = ((access & FileAccess.Write) > (FileAccess)0);
				}
				else
				{
					this._canRead = ((rights & 1) != 0);
					this._canWrite = ((rights & 2) != 0 || (rights & 4) != 0);
				}
				this._canSeek = true;
				this._isPipe = false;
				this._pos = 0L;
				this._bufferSize = bufferSize;
				this._readPos = 0;
				this._readLen = 0;
				this._writePos = 0;
				if (flag2)
				{
					this._appendStart = this.SeekCore(0L, SeekOrigin.End);
					return;
				}
				this._appendStart = -1L;
				return;
			}
		}

		[Obsolete("This constructor has been deprecated.  Please use new FileStream(SafeFileHandle handle, FileAccess access) instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
		public FileStream(IntPtr handle, FileAccess access) : this(handle, access, true, 4096, false)
		{
		}

		[Obsolete("This constructor has been deprecated.  Please use new FileStream(SafeFileHandle handle, FileAccess access) instead, and optionally make a new SafeFileHandle with ownsHandle=false if needed.  http://go.microsoft.com/fwlink/?linkid=14202")]
		public FileStream(IntPtr handle, FileAccess access, bool ownsHandle) : this(handle, access, ownsHandle, 4096, false)
		{
		}

		[Obsolete("This constructor has been deprecated.  Please use new FileStream(SafeFileHandle handle, FileAccess access, int bufferSize) instead, and optionally make a new SafeFileHandle with ownsHandle=false if needed.  http://go.microsoft.com/fwlink/?linkid=14202")]
		public FileStream(IntPtr handle, FileAccess access, bool ownsHandle, int bufferSize) : this(handle, access, ownsHandle, bufferSize, false)
		{
		}

		[SecuritySafeCritical]
		[Obsolete("This constructor has been deprecated.  Please use new FileStream(SafeFileHandle handle, FileAccess access, int bufferSize, bool isAsync) instead, and optionally make a new SafeFileHandle with ownsHandle=false if needed.  http://go.microsoft.com/fwlink/?linkid=14202")]
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public FileStream(IntPtr handle, FileAccess access, bool ownsHandle, int bufferSize, bool isAsync) : this(new SafeFileHandle(handle, ownsHandle), access, bufferSize, isAsync)
		{
		}

		[SecuritySafeCritical]
		public FileStream(SafeFileHandle handle, FileAccess access) : this(handle, access, 4096, false)
		{
		}

		[SecuritySafeCritical]
		public FileStream(SafeFileHandle handle, FileAccess access, int bufferSize) : this(handle, access, bufferSize, false)
		{
		}

		[SecuritySafeCritical]
		[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
		public FileStream(SafeFileHandle handle, FileAccess access, int bufferSize, bool isAsync)
		{
			if (handle.IsInvalid)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_InvalidHandle"), "handle");
			}
			this._handle = handle;
			this._exposedHandle = true;
			if (access < FileAccess.Read || access > FileAccess.ReadWrite)
			{
				throw new ArgumentOutOfRangeException("access", Environment.GetResourceString("ArgumentOutOfRange_Enum"));
			}
			if (bufferSize <= 0)
			{
				throw new ArgumentOutOfRangeException("bufferSize", Environment.GetResourceString("ArgumentOutOfRange_NeedPosNum"));
			}
			int fileType = Win32Native.GetFileType(this._handle);
			this._isAsync = isAsync;
			this._canRead = ((access & FileAccess.Read) > (FileAccess)0);
			this._canWrite = ((access & FileAccess.Write) > (FileAccess)0);
			this._canSeek = (fileType == 1);
			this._bufferSize = bufferSize;
			this._readPos = 0;
			this._readLen = 0;
			this._writePos = 0;
			this._fileName = null;
			this._isPipe = (fileType == 3);
			if (this._isAsync)
			{
				bool flag = false;
				try
				{
					flag = ThreadPool.BindHandle(this._handle);
				}
				catch (ApplicationException)
				{
					throw new ArgumentException(Environment.GetResourceString("Arg_HandleNotAsync"));
				}
				if (!flag)
				{
					throw new IOException(Environment.GetResourceString("IO.IO_BindHandleFailed"));
				}
			}
			else if (fileType != 3)
			{
				this.VerifyHandleIsSync();
			}
			if (this._canSeek)
			{
				this.SeekCore(0L, SeekOrigin.Current);
				return;
			}
			this._pos = 0L;
		}

		[SecuritySafeCritical]
		private static Win32Native.SECURITY_ATTRIBUTES GetSecAttrs(FileShare share)
		{
			Win32Native.SECURITY_ATTRIBUTES security_ATTRIBUTES = null;
			if ((share & FileShare.Inheritable) != FileShare.None)
			{
				security_ATTRIBUTES = new Win32Native.SECURITY_ATTRIBUTES();
				security_ATTRIBUTES.nLength = Marshal.SizeOf<Win32Native.SECURITY_ATTRIBUTES>(security_ATTRIBUTES);
				security_ATTRIBUTES.bInheritHandle = 1;
			}
			return security_ATTRIBUTES;
		}

		[SecuritySafeCritical]
		private unsafe static Win32Native.SECURITY_ATTRIBUTES GetSecAttrs(FileShare share, FileSecurity fileSecurity, out object pinningHandle)
		{
			pinningHandle = null;
			Win32Native.SECURITY_ATTRIBUTES security_ATTRIBUTES = null;
			if ((share & FileShare.Inheritable) != FileShare.None || fileSecurity != null)
			{
				security_ATTRIBUTES = new Win32Native.SECURITY_ATTRIBUTES();
				security_ATTRIBUTES.nLength = Marshal.SizeOf<Win32Native.SECURITY_ATTRIBUTES>(security_ATTRIBUTES);
				if ((share & FileShare.Inheritable) != FileShare.None)
				{
					security_ATTRIBUTES.bInheritHandle = 1;
				}
				if (fileSecurity != null)
				{
					byte[] securityDescriptorBinaryForm = fileSecurity.GetSecurityDescriptorBinaryForm();
					pinningHandle = GCHandle.Alloc(securityDescriptorBinaryForm, GCHandleType.Pinned);
					fixed (byte* ptr = securityDescriptorBinaryForm)
					{
						security_ATTRIBUTES.pSecurityDescriptor = ptr;
					}
				}
			}
			return security_ATTRIBUTES;
		}

		[SecuritySafeCritical]
		private void VerifyHandleIsSync()
		{
			byte[] bytes = new byte[1];
			int num = 0;
			if (this.CanRead)
			{
				int num2 = this.ReadFileNative(this._handle, bytes, 0, 0, null, out num);
			}
			else if (this.CanWrite)
			{
				int num2 = this.WriteFileNative(this._handle, bytes, 0, 0, null, out num);
			}
			if (num == 87)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_HandleNotSync"));
			}
			if (num == 6)
			{
				__Error.WinIOError(num, "<OS handle>");
			}
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
				return this._canSeek;
			}
		}

		public virtual bool IsAsync
		{
			get
			{
				return this._isAsync;
			}
		}

		public override long Length
		{
			[SecuritySafeCritical]
			get
			{
				if (this._handle.IsClosed)
				{
					__Error.FileNotOpen();
				}
				if (!this.CanSeek)
				{
					__Error.SeekNotSupported();
				}
				int num = 0;
				int fileSize = Win32Native.GetFileSize(this._handle, out num);
				if (fileSize == -1)
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					if (lastWin32Error != 0)
					{
						__Error.WinIOError(lastWin32Error, string.Empty);
					}
				}
				long num2 = (long)num << 32 | (long)((ulong)fileSize);
				if (this._writePos > 0 && this._pos + (long)this._writePos > num2)
				{
					num2 = (long)this._writePos + this._pos;
				}
				return num2;
			}
		}

		public string Name
		{
			[SecuritySafeCritical]
			get
			{
				if (this._fileName == null)
				{
					return Environment.GetResourceString("IO_UnknownFileName");
				}
				FileIOPermission.QuickDemand(FileIOPermissionAccess.PathDiscovery, this._fileName, false, false);
				return this._fileName;
			}
		}

		internal string NameInternal
		{
			get
			{
				if (this._fileName == null)
				{
					return "<UnknownFileName>";
				}
				return this._fileName;
			}
		}

		public override long Position
		{
			[SecuritySafeCritical]
			get
			{
				if (this._handle.IsClosed)
				{
					__Error.FileNotOpen();
				}
				if (!this.CanSeek)
				{
					__Error.SeekNotSupported();
				}
				if (this._exposedHandle)
				{
					this.VerifyOSHandlePosition();
				}
				return this._pos + (long)(this._readPos - this._readLen + this._writePos);
			}
			set
			{
				if (value < 0L)
				{
					throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
				}
				if (this._writePos > 0)
				{
					this.FlushWrite(false);
				}
				this._readPos = 0;
				this._readLen = 0;
				this.Seek(value, SeekOrigin.Begin);
			}
		}

		[SecuritySafeCritical]
		public FileSecurity GetAccessControl()
		{
			if (this._handle.IsClosed)
			{
				__Error.FileNotOpen();
			}
			return new FileSecurity(this._handle, this._fileName, AccessControlSections.Access | AccessControlSections.Owner | AccessControlSections.Group);
		}

		[SecuritySafeCritical]
		public void SetAccessControl(FileSecurity fileSecurity)
		{
			if (fileSecurity == null)
			{
				throw new ArgumentNullException("fileSecurity");
			}
			if (this._handle.IsClosed)
			{
				__Error.FileNotOpen();
			}
			fileSecurity.Persist(this._handle, this._fileName);
		}

		[SecuritySafeCritical]
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (this._handle != null && !this._handle.IsClosed && this._writePos > 0)
				{
					this.FlushWrite(!disposing);
				}
			}
			finally
			{
				if (this._handle != null && !this._handle.IsClosed)
				{
					this._handle.Dispose();
				}
				this._canRead = false;
				this._canWrite = false;
				this._canSeek = false;
				base.Dispose(disposing);
			}
		}

		[SecuritySafeCritical]
		~FileStream()
		{
			if (this._handle != null)
			{
				this.Dispose(false);
			}
		}

		public override void Flush()
		{
			this.Flush(false);
		}

		[SecuritySafeCritical]
		public virtual void Flush(bool flushToDisk)
		{
			if (this._handle.IsClosed)
			{
				__Error.FileNotOpen();
			}
			this.FlushInternalBuffer();
			if (flushToDisk && this.CanWrite)
			{
				this.FlushOSBuffer();
			}
		}

		private void FlushInternalBuffer()
		{
			if (this._writePos > 0)
			{
				this.FlushWrite(false);
				return;
			}
			if (this._readPos < this._readLen && this.CanSeek)
			{
				this.FlushRead();
			}
		}

		[SecuritySafeCritical]
		private void FlushOSBuffer()
		{
			if (!Win32Native.FlushFileBuffers(this._handle))
			{
				__Error.WinIOError();
			}
		}

		private void FlushRead()
		{
			if (this._readPos - this._readLen != 0)
			{
				this.SeekCore((long)(this._readPos - this._readLen), SeekOrigin.Current);
			}
			this._readPos = 0;
			this._readLen = 0;
		}

		private void FlushWrite(bool calledFromFinalizer)
		{
			if (this._isAsync)
			{
				IAsyncResult asyncResult = this.BeginWriteCore(this._buffer, 0, this._writePos, null, null);
				if (!calledFromFinalizer)
				{
					this.EndWrite(asyncResult);
				}
			}
			else
			{
				this.WriteCore(this._buffer, 0, this._writePos);
			}
			this._writePos = 0;
		}

		[Obsolete("This property has been deprecated.  Please use FileStream's SafeFileHandle property instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
		public virtual IntPtr Handle
		{
			[SecurityCritical]
			[SecurityPermission(SecurityAction.InheritanceDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get
			{
				this.Flush();
				this._readPos = 0;
				this._readLen = 0;
				this._writePos = 0;
				this._exposedHandle = true;
				return this._handle.DangerousGetHandle();
			}
		}

		public virtual SafeFileHandle SafeFileHandle
		{
			[SecurityCritical]
			[SecurityPermission(SecurityAction.InheritanceDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
			get
			{
				this.Flush();
				this._readPos = 0;
				this._readLen = 0;
				this._writePos = 0;
				this._exposedHandle = true;
				return this._handle;
			}
		}

		[SecuritySafeCritical]
		public override void SetLength(long value)
		{
			if (value < 0L)
			{
				throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (this._handle.IsClosed)
			{
				__Error.FileNotOpen();
			}
			if (!this.CanSeek)
			{
				__Error.SeekNotSupported();
			}
			if (!this.CanWrite)
			{
				__Error.WriteNotSupported();
			}
			if (this._writePos > 0)
			{
				this.FlushWrite(false);
			}
			else if (this._readPos < this._readLen)
			{
				this.FlushRead();
			}
			this._readPos = 0;
			this._readLen = 0;
			if (this._appendStart != -1L && value < this._appendStart)
			{
				throw new IOException(Environment.GetResourceString("IO.IO_SetLengthAppendTruncate"));
			}
			this.SetLengthCore(value);
		}

		[SecuritySafeCritical]
		private void SetLengthCore(long value)
		{
			long pos = this._pos;
			if (this._exposedHandle)
			{
				this.VerifyOSHandlePosition();
			}
			if (this._pos != value)
			{
				this.SeekCore(value, SeekOrigin.Begin);
			}
			if (!Win32Native.SetEndOfFile(this._handle))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (lastWin32Error == 87)
				{
					throw new ArgumentOutOfRangeException("value", Environment.GetResourceString("ArgumentOutOfRange_FileLengthTooBig"));
				}
				__Error.WinIOError(lastWin32Error, string.Empty);
			}
			if (pos != value)
			{
				if (pos < value)
				{
					this.SeekCore(pos, SeekOrigin.Begin);
					return;
				}
				this.SeekCore(0L, SeekOrigin.End);
			}
		}

		[SecuritySafeCritical]
		public override int Read([In] [Out] byte[] array, int offset, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array", Environment.GetResourceString("ArgumentNull_Buffer"));
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (array.Length - offset < count)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidOffLen"));
			}
			if (this._handle.IsClosed)
			{
				__Error.FileNotOpen();
			}
			bool flag = false;
			int num = this._readLen - this._readPos;
			if (num == 0)
			{
				if (!this.CanRead)
				{
					__Error.ReadNotSupported();
				}
				if (this._writePos > 0)
				{
					this.FlushWrite(false);
				}
				if (!this.CanSeek || count >= this._bufferSize)
				{
					num = this.ReadCore(array, offset, count);
					this._readPos = 0;
					this._readLen = 0;
					return num;
				}
				if (this._buffer == null)
				{
					this._buffer = new byte[this._bufferSize];
				}
				num = this.ReadCore(this._buffer, 0, this._bufferSize);
				if (num == 0)
				{
					return 0;
				}
				flag = (num < this._bufferSize);
				this._readPos = 0;
				this._readLen = num;
			}
			if (num > count)
			{
				num = count;
			}
			Buffer.InternalBlockCopy(this._buffer, this._readPos, array, offset, num);
			this._readPos += num;
			if (!this._isPipe && num < count && !flag)
			{
				int num2 = this.ReadCore(array, offset + num, count - num);
				num += num2;
				this._readPos = 0;
				this._readLen = 0;
			}
			return num;
		}

		[SecuritySafeCritical]
		private int ReadCore(byte[] buffer, int offset, int count)
		{
			if (this._isAsync)
			{
				IAsyncResult asyncResult = this.BeginReadCore(buffer, offset, count, null, null, 0);
				return this.EndRead(asyncResult);
			}
			if (this._exposedHandle)
			{
				this.VerifyOSHandlePosition();
			}
			int num = 0;
			int num2 = this.ReadFileNative(this._handle, buffer, offset, count, null, out num);
			if (num2 == -1)
			{
				if (num == 109)
				{
					num2 = 0;
				}
				else
				{
					if (num == 87)
					{
						throw new ArgumentException(Environment.GetResourceString("Arg_HandleNotSync"));
					}
					__Error.WinIOError(num, string.Empty);
				}
			}
			this._pos += (long)num2;
			return num2;
		}

		[SecuritySafeCritical]
		public override long Seek(long offset, SeekOrigin origin)
		{
			if (origin < SeekOrigin.Begin || origin > SeekOrigin.End)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidSeekOrigin"));
			}
			if (this._handle.IsClosed)
			{
				__Error.FileNotOpen();
			}
			if (!this.CanSeek)
			{
				__Error.SeekNotSupported();
			}
			if (this._writePos > 0)
			{
				this.FlushWrite(false);
			}
			else if (origin == SeekOrigin.Current)
			{
				offset -= (long)(this._readLen - this._readPos);
			}
			if (this._exposedHandle)
			{
				this.VerifyOSHandlePosition();
			}
			long num = this._pos + (long)(this._readPos - this._readLen);
			long num2 = this.SeekCore(offset, origin);
			if (this._appendStart != -1L && num2 < this._appendStart)
			{
				this.SeekCore(num, SeekOrigin.Begin);
				throw new IOException(Environment.GetResourceString("IO.IO_SeekAppendOverwrite"));
			}
			if (this._readLen > 0)
			{
				if (num == num2)
				{
					if (this._readPos > 0)
					{
						Buffer.InternalBlockCopy(this._buffer, this._readPos, this._buffer, 0, this._readLen - this._readPos);
						this._readLen -= this._readPos;
						this._readPos = 0;
					}
					if (this._readLen > 0)
					{
						this.SeekCore((long)this._readLen, SeekOrigin.Current);
					}
				}
				else if (num - (long)this._readPos < num2 && num2 < num + (long)this._readLen - (long)this._readPos)
				{
					int num3 = (int)(num2 - num);
					Buffer.InternalBlockCopy(this._buffer, this._readPos + num3, this._buffer, 0, this._readLen - (this._readPos + num3));
					this._readLen -= this._readPos + num3;
					this._readPos = 0;
					if (this._readLen > 0)
					{
						this.SeekCore((long)this._readLen, SeekOrigin.Current);
					}
				}
				else
				{
					this._readPos = 0;
					this._readLen = 0;
				}
			}
			return num2;
		}

		[SecuritySafeCritical]
		private long SeekCore(long offset, SeekOrigin origin)
		{
			int num = 0;
			long num2 = Win32Native.SetFilePointer(this._handle, offset, origin, out num);
			if (num2 == -1L)
			{
				if (num == 6)
				{
					this._handle.Dispose();
				}
				__Error.WinIOError(num, string.Empty);
			}
			this._pos = num2;
			return num2;
		}

		private void VerifyOSHandlePosition()
		{
			if (!this.CanSeek)
			{
				return;
			}
			long pos = this._pos;
			long num = this.SeekCore(0L, SeekOrigin.Current);
			if (num != pos)
			{
				this._readPos = 0;
				this._readLen = 0;
				if (this._writePos > 0)
				{
					this._writePos = 0;
					throw new IOException(Environment.GetResourceString("IO.IO_FileStreamHandlePosition"));
				}
			}
		}

		[SecuritySafeCritical]
		public override void Write(byte[] array, int offset, int count)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array", Environment.GetResourceString("ArgumentNull_Buffer"));
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (array.Length - offset < count)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidOffLen"));
			}
			if (this._handle.IsClosed)
			{
				__Error.FileNotOpen();
			}
			if (this._writePos == 0)
			{
				if (!this.CanWrite)
				{
					__Error.WriteNotSupported();
				}
				if (this._readPos < this._readLen)
				{
					this.FlushRead();
				}
				this._readPos = 0;
				this._readLen = 0;
			}
			if (this._writePos > 0)
			{
				int num = this._bufferSize - this._writePos;
				if (num > 0)
				{
					if (num > count)
					{
						num = count;
					}
					Buffer.InternalBlockCopy(array, offset, this._buffer, this._writePos, num);
					this._writePos += num;
					if (count == num)
					{
						return;
					}
					offset += num;
					count -= num;
				}
				if (this._isAsync)
				{
					IAsyncResult asyncResult = this.BeginWriteCore(this._buffer, 0, this._writePos, null, null);
					this.EndWrite(asyncResult);
				}
				else
				{
					this.WriteCore(this._buffer, 0, this._writePos);
				}
				this._writePos = 0;
			}
			if (count >= this._bufferSize)
			{
				this.WriteCore(array, offset, count);
				return;
			}
			if (count == 0)
			{
				return;
			}
			if (this._buffer == null)
			{
				this._buffer = new byte[this._bufferSize];
			}
			Buffer.InternalBlockCopy(array, offset, this._buffer, this._writePos, count);
			this._writePos = count;
		}

		[SecuritySafeCritical]
		private void WriteCore(byte[] buffer, int offset, int count)
		{
			if (this._isAsync)
			{
				IAsyncResult asyncResult = this.BeginWriteCore(buffer, offset, count, null, null);
				this.EndWrite(asyncResult);
				return;
			}
			if (this._exposedHandle)
			{
				this.VerifyOSHandlePosition();
			}
			int num = 0;
			int num2 = this.WriteFileNative(this._handle, buffer, offset, count, null, out num);
			if (num2 == -1)
			{
				if (num == 232)
				{
					num2 = 0;
				}
				else
				{
					if (num == 87)
					{
						throw new IOException(Environment.GetResourceString("IO.IO_FileTooLongOrHandleNotSync"));
					}
					__Error.WinIOError(num, string.Empty);
				}
			}
			this._pos += (long)num2;
		}

		[SecuritySafeCritical]
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public override IAsyncResult BeginRead(byte[] array, int offset, int numBytes, AsyncCallback userCallback, object stateObject)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (numBytes < 0)
			{
				throw new ArgumentOutOfRangeException("numBytes", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (array.Length - offset < numBytes)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidOffLen"));
			}
			if (this._handle.IsClosed)
			{
				__Error.FileNotOpen();
			}
			if (!this._isAsync)
			{
				return base.BeginRead(array, offset, numBytes, userCallback, stateObject);
			}
			return this.BeginReadAsync(array, offset, numBytes, userCallback, stateObject);
		}

		[SecuritySafeCritical]
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		private FileStreamAsyncResult BeginReadAsync(byte[] array, int offset, int numBytes, AsyncCallback userCallback, object stateObject)
		{
			if (!this.CanRead)
			{
				__Error.ReadNotSupported();
			}
			if (this._isPipe)
			{
				if (this._readPos < this._readLen)
				{
					int num = this._readLen - this._readPos;
					if (num > numBytes)
					{
						num = numBytes;
					}
					Buffer.InternalBlockCopy(this._buffer, this._readPos, array, offset, num);
					this._readPos += num;
					return FileStreamAsyncResult.CreateBufferedReadResult(num, userCallback, stateObject, false);
				}
				return this.BeginReadCore(array, offset, numBytes, userCallback, stateObject, 0);
			}
			else
			{
				if (this._writePos > 0)
				{
					this.FlushWrite(false);
				}
				if (this._readPos == this._readLen)
				{
					if (numBytes < this._bufferSize)
					{
						if (this._buffer == null)
						{
							this._buffer = new byte[this._bufferSize];
						}
						IAsyncResult asyncResult = this.BeginReadCore(this._buffer, 0, this._bufferSize, null, null, 0);
						this._readLen = this.EndRead(asyncResult);
						int num2 = this._readLen;
						if (num2 > numBytes)
						{
							num2 = numBytes;
						}
						Buffer.InternalBlockCopy(this._buffer, 0, array, offset, num2);
						this._readPos = num2;
						return FileStreamAsyncResult.CreateBufferedReadResult(num2, userCallback, stateObject, false);
					}
					this._readPos = 0;
					this._readLen = 0;
					return this.BeginReadCore(array, offset, numBytes, userCallback, stateObject, 0);
				}
				else
				{
					int num3 = this._readLen - this._readPos;
					if (num3 > numBytes)
					{
						num3 = numBytes;
					}
					Buffer.InternalBlockCopy(this._buffer, this._readPos, array, offset, num3);
					this._readPos += num3;
					if (num3 >= numBytes)
					{
						return FileStreamAsyncResult.CreateBufferedReadResult(num3, userCallback, stateObject, false);
					}
					this._readPos = 0;
					this._readLen = 0;
					return this.BeginReadCore(array, offset + num3, numBytes - num3, userCallback, stateObject, num3);
				}
			}
		}

		[SecuritySafeCritical]
		private unsafe FileStreamAsyncResult BeginReadCore(byte[] bytes, int offset, int numBytes, AsyncCallback userCallback, object stateObject, int numBufferedBytesRead)
		{
			FileStreamAsyncResult fileStreamAsyncResult = new FileStreamAsyncResult(numBufferedBytesRead, bytes, this._handle, userCallback, stateObject, false);
			NativeOverlapped* overLapped = fileStreamAsyncResult.OverLapped;
			if (this.CanSeek)
			{
				long length = this.Length;
				if (this._exposedHandle)
				{
					this.VerifyOSHandlePosition();
				}
				if (this._pos + (long)numBytes > length)
				{
					if (this._pos <= length)
					{
						numBytes = (int)(length - this._pos);
					}
					else
					{
						numBytes = 0;
					}
				}
				overLapped->OffsetLow = (int)this._pos;
				overLapped->OffsetHigh = (int)(this._pos >> 32);
				this.SeekCore((long)numBytes, SeekOrigin.Current);
			}
			if (FrameworkEventSource.IsInitialized && FrameworkEventSource.Log.IsEnabled(EventLevel.Informational, (EventKeywords)16L))
			{
				FrameworkEventSource.Log.ThreadTransferSend(fileStreamAsyncResult.OverLapped, 2, string.Empty, false);
			}
			int num = 0;
			int num2 = this.ReadFileNative(this._handle, bytes, offset, numBytes, overLapped, out num);
			if (num2 == -1 && numBytes != -1)
			{
				if (num == 109)
				{
					overLapped->InternalLow = IntPtr.Zero;
					fileStreamAsyncResult.CallUserCallback();
				}
				else if (num != 997)
				{
					if (!this._handle.IsClosed && this.CanSeek)
					{
						this.SeekCore(0L, SeekOrigin.Current);
					}
					if (num == 38)
					{
						__Error.EndOfFile();
					}
					else
					{
						__Error.WinIOError(num, string.Empty);
					}
				}
			}
			return fileStreamAsyncResult;
		}

		[SecuritySafeCritical]
		public override int EndRead(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			if (!this._isAsync)
			{
				return base.EndRead(asyncResult);
			}
			FileStreamAsyncResult fileStreamAsyncResult = asyncResult as FileStreamAsyncResult;
			if (fileStreamAsyncResult == null || fileStreamAsyncResult.IsWrite)
			{
				__Error.WrongAsyncResult();
			}
			if (1 == Interlocked.CompareExchange(ref fileStreamAsyncResult._EndXxxCalled, 1, 0))
			{
				__Error.EndReadCalledTwice();
			}
			fileStreamAsyncResult.Wait();
			fileStreamAsyncResult.ReleaseNativeResource();
			if (fileStreamAsyncResult.ErrorCode != 0)
			{
				__Error.WinIOError(fileStreamAsyncResult.ErrorCode, string.Empty);
			}
			return fileStreamAsyncResult.NumBytesRead;
		}

		[SecuritySafeCritical]
		public override int ReadByte()
		{
			if (this._handle.IsClosed)
			{
				__Error.FileNotOpen();
			}
			if (this._readLen == 0 && !this.CanRead)
			{
				__Error.ReadNotSupported();
			}
			if (this._readPos == this._readLen)
			{
				if (this._writePos > 0)
				{
					this.FlushWrite(false);
				}
				if (this._buffer == null)
				{
					this._buffer = new byte[this._bufferSize];
				}
				this._readLen = this.ReadCore(this._buffer, 0, this._bufferSize);
				this._readPos = 0;
			}
			if (this._readPos == this._readLen)
			{
				return -1;
			}
			int result = (int)this._buffer[this._readPos];
			this._readPos++;
			return result;
		}

		[SecuritySafeCritical]
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public override IAsyncResult BeginWrite(byte[] array, int offset, int numBytes, AsyncCallback userCallback, object stateObject)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (numBytes < 0)
			{
				throw new ArgumentOutOfRangeException("numBytes", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (array.Length - offset < numBytes)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidOffLen"));
			}
			if (this._handle.IsClosed)
			{
				__Error.FileNotOpen();
			}
			if (!this._isAsync)
			{
				return base.BeginWrite(array, offset, numBytes, userCallback, stateObject);
			}
			return this.BeginWriteAsync(array, offset, numBytes, userCallback, stateObject);
		}

		[SecuritySafeCritical]
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		private FileStreamAsyncResult BeginWriteAsync(byte[] array, int offset, int numBytes, AsyncCallback userCallback, object stateObject)
		{
			if (!this.CanWrite)
			{
				__Error.WriteNotSupported();
			}
			if (this._isPipe)
			{
				if (this._writePos > 0)
				{
					this.FlushWrite(false);
				}
				return this.BeginWriteCore(array, offset, numBytes, userCallback, stateObject);
			}
			if (this._writePos == 0)
			{
				if (this._readPos < this._readLen)
				{
					this.FlushRead();
				}
				this._readPos = 0;
				this._readLen = 0;
			}
			int num = this._bufferSize - this._writePos;
			if (numBytes <= num)
			{
				if (this._writePos == 0)
				{
					this._buffer = new byte[this._bufferSize];
				}
				Buffer.InternalBlockCopy(array, offset, this._buffer, this._writePos, numBytes);
				this._writePos += numBytes;
				return FileStreamAsyncResult.CreateBufferedReadResult(numBytes, userCallback, stateObject, true);
			}
			if (this._writePos > 0)
			{
				this.FlushWrite(false);
			}
			return this.BeginWriteCore(array, offset, numBytes, userCallback, stateObject);
		}

		[SecuritySafeCritical]
		private unsafe FileStreamAsyncResult BeginWriteCore(byte[] bytes, int offset, int numBytes, AsyncCallback userCallback, object stateObject)
		{
			FileStreamAsyncResult fileStreamAsyncResult = new FileStreamAsyncResult(0, bytes, this._handle, userCallback, stateObject, true);
			NativeOverlapped* overLapped = fileStreamAsyncResult.OverLapped;
			if (this.CanSeek)
			{
				long length = this.Length;
				if (this._exposedHandle)
				{
					this.VerifyOSHandlePosition();
				}
				if (this._pos + (long)numBytes > length)
				{
					this.SetLengthCore(this._pos + (long)numBytes);
				}
				overLapped->OffsetLow = (int)this._pos;
				overLapped->OffsetHigh = (int)(this._pos >> 32);
				this.SeekCore((long)numBytes, SeekOrigin.Current);
			}
			if (FrameworkEventSource.IsInitialized && FrameworkEventSource.Log.IsEnabled(EventLevel.Informational, (EventKeywords)16L))
			{
				FrameworkEventSource.Log.ThreadTransferSend(fileStreamAsyncResult.OverLapped, 2, string.Empty, false);
			}
			int num = 0;
			int num2 = this.WriteFileNative(this._handle, bytes, offset, numBytes, overLapped, out num);
			if (num2 == -1 && numBytes != -1)
			{
				if (num == 232)
				{
					fileStreamAsyncResult.CallUserCallback();
				}
				else if (num != 997)
				{
					if (!this._handle.IsClosed && this.CanSeek)
					{
						this.SeekCore(0L, SeekOrigin.Current);
					}
					if (num == 38)
					{
						__Error.EndOfFile();
					}
					else
					{
						__Error.WinIOError(num, string.Empty);
					}
				}
			}
			return fileStreamAsyncResult;
		}

		[SecuritySafeCritical]
		public override void EndWrite(IAsyncResult asyncResult)
		{
			if (asyncResult == null)
			{
				throw new ArgumentNullException("asyncResult");
			}
			if (!this._isAsync)
			{
				base.EndWrite(asyncResult);
				return;
			}
			FileStreamAsyncResult fileStreamAsyncResult = asyncResult as FileStreamAsyncResult;
			if (fileStreamAsyncResult == null || !fileStreamAsyncResult.IsWrite)
			{
				__Error.WrongAsyncResult();
			}
			if (1 == Interlocked.CompareExchange(ref fileStreamAsyncResult._EndXxxCalled, 1, 0))
			{
				__Error.EndWriteCalledTwice();
			}
			fileStreamAsyncResult.Wait();
			fileStreamAsyncResult.ReleaseNativeResource();
			if (fileStreamAsyncResult.ErrorCode != 0)
			{
				__Error.WinIOError(fileStreamAsyncResult.ErrorCode, string.Empty);
			}
		}

		[SecuritySafeCritical]
		public override void WriteByte(byte value)
		{
			if (this._handle.IsClosed)
			{
				__Error.FileNotOpen();
			}
			if (this._writePos == 0)
			{
				if (!this.CanWrite)
				{
					__Error.WriteNotSupported();
				}
				if (this._readPos < this._readLen)
				{
					this.FlushRead();
				}
				this._readPos = 0;
				this._readLen = 0;
				if (this._buffer == null)
				{
					this._buffer = new byte[this._bufferSize];
				}
			}
			if (this._writePos == this._bufferSize)
			{
				this.FlushWrite(false);
			}
			this._buffer[this._writePos] = value;
			this._writePos++;
		}

		[SecuritySafeCritical]
		public virtual void Lock(long position, long length)
		{
			if (position < 0L || length < 0L)
			{
				throw new ArgumentOutOfRangeException((position < 0L) ? "position" : "length", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (this._handle.IsClosed)
			{
				__Error.FileNotOpen();
			}
			int offsetLow = (int)position;
			int offsetHigh = (int)(position >> 32);
			int countLow = (int)length;
			int countHigh = (int)(length >> 32);
			if (!Win32Native.LockFile(this._handle, offsetLow, offsetHigh, countLow, countHigh))
			{
				__Error.WinIOError();
			}
		}

		[SecuritySafeCritical]
		public virtual void Unlock(long position, long length)
		{
			if (position < 0L || length < 0L)
			{
				throw new ArgumentOutOfRangeException((position < 0L) ? "position" : "length", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (this._handle.IsClosed)
			{
				__Error.FileNotOpen();
			}
			int offsetLow = (int)position;
			int offsetHigh = (int)(position >> 32);
			int countLow = (int)length;
			int countHigh = (int)(length >> 32);
			if (!Win32Native.UnlockFile(this._handle, offsetLow, offsetHigh, countLow, countHigh))
			{
				__Error.WinIOError();
			}
		}

		[SecurityCritical]
		private unsafe int ReadFileNative(SafeFileHandle handle, byte[] bytes, int offset, int count, NativeOverlapped* overlapped, out int hr)
		{
			if (bytes.Length - offset < count)
			{
				throw new IndexOutOfRangeException(Environment.GetResourceString("IndexOutOfRange_IORaceCondition"));
			}
			if (bytes.Length == 0)
			{
				hr = 0;
				return 0;
			}
			int result = 0;
			int num;
			fixed (byte* ptr = bytes)
			{
				if (this._isAsync)
				{
					num = Win32Native.ReadFile(handle, ptr + offset, count, IntPtr.Zero, overlapped);
				}
				else
				{
					num = Win32Native.ReadFile(handle, ptr + offset, count, out result, IntPtr.Zero);
				}
			}
			if (num != 0)
			{
				hr = 0;
				return result;
			}
			hr = Marshal.GetLastWin32Error();
			if (hr == 109 || hr == 233)
			{
				return -1;
			}
			if (hr == 6)
			{
				this._handle.Dispose();
			}
			return -1;
		}

		[SecurityCritical]
		private unsafe int WriteFileNative(SafeFileHandle handle, byte[] bytes, int offset, int count, NativeOverlapped* overlapped, out int hr)
		{
			if (bytes.Length - offset < count)
			{
				throw new IndexOutOfRangeException(Environment.GetResourceString("IndexOutOfRange_IORaceCondition"));
			}
			if (bytes.Length == 0)
			{
				hr = 0;
				return 0;
			}
			int result = 0;
			int num;
			fixed (byte* ptr = bytes)
			{
				if (this._isAsync)
				{
					num = Win32Native.WriteFile(handle, ptr + offset, count, IntPtr.Zero, overlapped);
				}
				else
				{
					num = Win32Native.WriteFile(handle, ptr + offset, count, out result, IntPtr.Zero);
				}
			}
			if (num != 0)
			{
				hr = 0;
				return result;
			}
			hr = Marshal.GetLastWin32Error();
			if (hr == 232)
			{
				return -1;
			}
			if (hr == 6)
			{
				this._handle.Dispose();
			}
			return -1;
		}

		[ComVisible(false)]
		[SecuritySafeCritical]
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidOffLen"));
			}
			if (base.GetType() != typeof(FileStream))
			{
				return base.ReadAsync(buffer, offset, count, cancellationToken);
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCancellation<int>(cancellationToken);
			}
			if (this._handle.IsClosed)
			{
				__Error.FileNotOpen();
			}
			if (!this._isAsync)
			{
				return base.ReadAsync(buffer, offset, count, cancellationToken);
			}
			FileStream.FileStreamReadWriteTask<int> fileStreamReadWriteTask = new FileStream.FileStreamReadWriteTask<int>(cancellationToken);
			AsyncCallback asyncCallback = FileStream.s_endReadTask;
			if (asyncCallback == null)
			{
				asyncCallback = (FileStream.s_endReadTask = new AsyncCallback(FileStream.EndReadTask));
			}
			fileStreamReadWriteTask._asyncResult = this.BeginReadAsync(buffer, offset, count, asyncCallback, fileStreamReadWriteTask);
			if (fileStreamReadWriteTask._asyncResult.IsAsync && cancellationToken.CanBeCanceled)
			{
				Action<object> action = FileStream.s_cancelReadHandler;
				if (action == null)
				{
					action = (FileStream.s_cancelReadHandler = new Action<object>(FileStream.CancelTask<int>));
				}
				fileStreamReadWriteTask._registration = cancellationToken.Register(action, fileStreamReadWriteTask);
				if (fileStreamReadWriteTask._asyncResult.IsCompleted)
				{
					fileStreamReadWriteTask._registration.Dispose();
				}
			}
			return fileStreamReadWriteTask;
		}

		[ComVisible(false)]
		[SecuritySafeCritical]
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_NeedNonNegNum"));
			}
			if (buffer.Length - offset < count)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidOffLen"));
			}
			if (base.GetType() != typeof(FileStream))
			{
				return base.WriteAsync(buffer, offset, count, cancellationToken);
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCancellation(cancellationToken);
			}
			if (this._handle.IsClosed)
			{
				__Error.FileNotOpen();
			}
			if (!this._isAsync)
			{
				return base.WriteAsync(buffer, offset, count, cancellationToken);
			}
			FileStream.FileStreamReadWriteTask<VoidTaskResult> fileStreamReadWriteTask = new FileStream.FileStreamReadWriteTask<VoidTaskResult>(cancellationToken);
			AsyncCallback asyncCallback = FileStream.s_endWriteTask;
			if (asyncCallback == null)
			{
				asyncCallback = (FileStream.s_endWriteTask = new AsyncCallback(FileStream.EndWriteTask));
			}
			fileStreamReadWriteTask._asyncResult = this.BeginWriteAsync(buffer, offset, count, asyncCallback, fileStreamReadWriteTask);
			if (fileStreamReadWriteTask._asyncResult.IsAsync && cancellationToken.CanBeCanceled)
			{
				Action<object> action = FileStream.s_cancelWriteHandler;
				if (action == null)
				{
					action = (FileStream.s_cancelWriteHandler = new Action<object>(FileStream.CancelTask<VoidTaskResult>));
				}
				fileStreamReadWriteTask._registration = cancellationToken.Register(action, fileStreamReadWriteTask);
				if (fileStreamReadWriteTask._asyncResult.IsCompleted)
				{
					fileStreamReadWriteTask._registration.Dispose();
				}
			}
			return fileStreamReadWriteTask;
		}

		[SecuritySafeCritical]
		private static void CancelTask<T>(object state)
		{
			FileStream.FileStreamReadWriteTask<T> fileStreamReadWriteTask = state as FileStream.FileStreamReadWriteTask<T>;
			FileStreamAsyncResult asyncResult = fileStreamReadWriteTask._asyncResult;
			try
			{
				if (!asyncResult.IsCompleted)
				{
					asyncResult.Cancel();
				}
			}
			catch (Exception exceptionObject)
			{
				fileStreamReadWriteTask.TrySetException(exceptionObject);
			}
		}

		[SecuritySafeCritical]
		private static void EndReadTask(IAsyncResult iar)
		{
			FileStreamAsyncResult fileStreamAsyncResult = iar as FileStreamAsyncResult;
			FileStream.FileStreamReadWriteTask<int> fileStreamReadWriteTask = fileStreamAsyncResult.AsyncState as FileStream.FileStreamReadWriteTask<int>;
			try
			{
				if (fileStreamAsyncResult.IsAsync)
				{
					fileStreamAsyncResult.ReleaseNativeResource();
					fileStreamReadWriteTask._registration.Dispose();
				}
				if (fileStreamAsyncResult.ErrorCode == 995)
				{
					CancellationToken cancellationToken = fileStreamReadWriteTask._cancellationToken;
					fileStreamReadWriteTask.TrySetCanceled(cancellationToken);
				}
				else
				{
					fileStreamReadWriteTask.TrySetResult(fileStreamAsyncResult.NumBytesRead);
				}
			}
			catch (Exception exceptionObject)
			{
				fileStreamReadWriteTask.TrySetException(exceptionObject);
			}
		}

		[SecuritySafeCritical]
		private static void EndWriteTask(IAsyncResult iar)
		{
			FileStreamAsyncResult fileStreamAsyncResult = iar as FileStreamAsyncResult;
			FileStream.FileStreamReadWriteTask<VoidTaskResult> fileStreamReadWriteTask = iar.AsyncState as FileStream.FileStreamReadWriteTask<VoidTaskResult>;
			try
			{
				if (fileStreamAsyncResult.IsAsync)
				{
					fileStreamAsyncResult.ReleaseNativeResource();
					fileStreamReadWriteTask._registration.Dispose();
				}
				if (fileStreamAsyncResult.ErrorCode == 995)
				{
					CancellationToken cancellationToken = fileStreamReadWriteTask._cancellationToken;
					fileStreamReadWriteTask.TrySetCanceled(cancellationToken);
				}
				else
				{
					fileStreamReadWriteTask.TrySetResult(default(VoidTaskResult));
				}
			}
			catch (Exception exceptionObject)
			{
				fileStreamReadWriteTask.TrySetException(exceptionObject);
			}
		}

		[ComVisible(false)]
		[SecuritySafeCritical]
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		public override Task FlushAsync(CancellationToken cancellationToken)
		{
			if (base.GetType() != typeof(FileStream))
			{
				return base.FlushAsync(cancellationToken);
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCancellation(cancellationToken);
			}
			if (this._handle.IsClosed)
			{
				__Error.FileNotOpen();
			}
			try
			{
				this.FlushInternalBuffer();
			}
			catch (Exception exception)
			{
				return Task.FromException(exception);
			}
			if (this.CanWrite)
			{
				return Task.Factory.StartNew(delegate(object state)
				{
					((FileStream)state).FlushOSBuffer();
				}, this, cancellationToken, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
			}
			return Task.CompletedTask;
		}

		internal const int DefaultBufferSize = 4096;

		private const bool _canUseAsync = true;

		private byte[] _buffer;

		private string _fileName;

		private bool _isAsync;

		private bool _canRead;

		private bool _canWrite;

		private bool _canSeek;

		private bool _exposedHandle;

		private bool _isPipe;

		private int _readPos;

		private int _readLen;

		private int _writePos;

		private int _bufferSize;

		[SecurityCritical]
		private SafeFileHandle _handle;

		private long _pos;

		private long _appendStart;

		private static AsyncCallback s_endReadTask;

		private static AsyncCallback s_endWriteTask;

		private static Action<object> s_cancelReadHandler;

		private static Action<object> s_cancelWriteHandler;

		private const int FILE_ATTRIBUTE_NORMAL = 128;

		private const int FILE_ATTRIBUTE_ENCRYPTED = 16384;

		private const int FILE_FLAG_OVERLAPPED = 1073741824;

		internal const int GENERIC_READ = -2147483648;

		private const int GENERIC_WRITE = 1073741824;

		private const int FILE_BEGIN = 0;

		private const int FILE_CURRENT = 1;

		private const int FILE_END = 2;

		internal const int ERROR_BROKEN_PIPE = 109;

		internal const int ERROR_NO_DATA = 232;

		private const int ERROR_HANDLE_EOF = 38;

		private const int ERROR_INVALID_PARAMETER = 87;

		private const int ERROR_IO_PENDING = 997;

		private sealed class FileStreamReadWriteTask<T> : Task<T>
		{
			internal FileStreamReadWriteTask(CancellationToken cancellationToken)
			{
				this._cancellationToken = cancellationToken;
			}

			internal CancellationToken _cancellationToken;

			internal CancellationTokenRegistration _registration;

			internal FileStreamAsyncResult _asyncResult;
		}
	}
}
