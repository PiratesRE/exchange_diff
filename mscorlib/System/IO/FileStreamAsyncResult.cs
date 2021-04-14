using System;
using System.Diagnostics.Tracing;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace System.IO
{
	internal sealed class FileStreamAsyncResult : IAsyncResult
	{
		internal unsafe NativeOverlapped* OverLapped
		{
			[SecurityCritical]
			get
			{
				return this._overlapped;
			}
		}

		internal bool IsAsync
		{
			[SecuritySafeCritical]
			get
			{
				return this._overlapped != null;
			}
		}

		internal int NumBytes
		{
			get
			{
				return this._numBytes;
			}
		}

		internal int ErrorCode
		{
			get
			{
				return this._errorCode;
			}
		}

		internal int NumBufferedBytes
		{
			get
			{
				return this._numBufferedBytes;
			}
		}

		internal int NumBytesRead
		{
			get
			{
				return this._numBytes + this._numBufferedBytes;
			}
		}

		internal bool IsWrite
		{
			get
			{
				return this._isWrite;
			}
		}

		[SecuritySafeCritical]
		internal FileStreamAsyncResult(int numBufferedBytes, byte[] bytes, SafeFileHandle handle, AsyncCallback userCallback, object userStateObject, bool isWrite)
		{
			this._userCallback = userCallback;
			this._userStateObject = userStateObject;
			this._isWrite = isWrite;
			this._numBufferedBytes = numBufferedBytes;
			this._handle = handle;
			ManualResetEvent waitHandle = new ManualResetEvent(false);
			this._waitHandle = waitHandle;
			Overlapped overlapped = new Overlapped(0, 0, IntPtr.Zero, this);
			if (userCallback != null)
			{
				IOCompletionCallback iocompletionCallback = FileStreamAsyncResult.s_IOCallback;
				if (iocompletionCallback == null)
				{
					iocompletionCallback = (FileStreamAsyncResult.s_IOCallback = new IOCompletionCallback(FileStreamAsyncResult.AsyncFSCallback));
				}
				this._overlapped = overlapped.Pack(iocompletionCallback, bytes);
				return;
			}
			this._overlapped = overlapped.UnsafePack(null, bytes);
		}

		internal static FileStreamAsyncResult CreateBufferedReadResult(int numBufferedBytes, AsyncCallback userCallback, object userStateObject, bool isWrite)
		{
			FileStreamAsyncResult fileStreamAsyncResult = new FileStreamAsyncResult(numBufferedBytes, userCallback, userStateObject, isWrite);
			fileStreamAsyncResult.CallUserCallback();
			return fileStreamAsyncResult;
		}

		private FileStreamAsyncResult(int numBufferedBytes, AsyncCallback userCallback, object userStateObject, bool isWrite)
		{
			this._userCallback = userCallback;
			this._userStateObject = userStateObject;
			this._isWrite = isWrite;
			this._numBufferedBytes = numBufferedBytes;
		}

		public object AsyncState
		{
			get
			{
				return this._userStateObject;
			}
		}

		public bool IsCompleted
		{
			get
			{
				return this._isComplete;
			}
		}

		public unsafe WaitHandle AsyncWaitHandle
		{
			[SecuritySafeCritical]
			get
			{
				if (this._waitHandle == null)
				{
					ManualResetEvent manualResetEvent = new ManualResetEvent(false);
					if (this._overlapped != null && this._overlapped->EventHandle != IntPtr.Zero)
					{
						manualResetEvent.SafeWaitHandle = new SafeWaitHandle(this._overlapped->EventHandle, true);
					}
					if (Interlocked.CompareExchange<ManualResetEvent>(ref this._waitHandle, manualResetEvent, null) == null)
					{
						if (this._isComplete)
						{
							this._waitHandle.Set();
						}
					}
					else
					{
						manualResetEvent.Close();
					}
				}
				return this._waitHandle;
			}
		}

		public bool CompletedSynchronously
		{
			get
			{
				return this._completedSynchronously;
			}
		}

		private void CallUserCallbackWorker()
		{
			this._isComplete = true;
			Thread.MemoryBarrier();
			if (this._waitHandle != null)
			{
				this._waitHandle.Set();
			}
			this._userCallback(this);
		}

		internal void CallUserCallback()
		{
			if (this._userCallback != null)
			{
				this._completedSynchronously = false;
				ThreadPool.QueueUserWorkItem(delegate(object state)
				{
					((FileStreamAsyncResult)state).CallUserCallbackWorker();
				}, this);
				return;
			}
			this._isComplete = true;
			Thread.MemoryBarrier();
			if (this._waitHandle != null)
			{
				this._waitHandle.Set();
			}
		}

		[SecurityCritical]
		internal void ReleaseNativeResource()
		{
			if (this._overlapped != null)
			{
				Overlapped.Free(this._overlapped);
			}
		}

		internal void Wait()
		{
			if (this._waitHandle != null)
			{
				try
				{
					this._waitHandle.WaitOne();
				}
				finally
				{
					this._waitHandle.Close();
				}
			}
		}

		[SecurityCritical]
		private unsafe static void AsyncFSCallback(uint errorCode, uint numBytes, NativeOverlapped* pOverlapped)
		{
			Overlapped overlapped = Overlapped.Unpack(pOverlapped);
			FileStreamAsyncResult fileStreamAsyncResult = (FileStreamAsyncResult)overlapped.AsyncResult;
			fileStreamAsyncResult._numBytes = (int)numBytes;
			if (FrameworkEventSource.IsInitialized && FrameworkEventSource.Log.IsEnabled(EventLevel.Informational, (EventKeywords)16L))
			{
				FrameworkEventSource.Log.ThreadTransferReceive(fileStreamAsyncResult.OverLapped, 2, string.Empty);
			}
			if (errorCode == 109U || errorCode == 232U)
			{
				errorCode = 0U;
			}
			fileStreamAsyncResult._errorCode = (int)errorCode;
			fileStreamAsyncResult._completedSynchronously = false;
			fileStreamAsyncResult._isComplete = true;
			Thread.MemoryBarrier();
			ManualResetEvent waitHandle = fileStreamAsyncResult._waitHandle;
			if (waitHandle != null && !waitHandle.Set())
			{
				__Error.WinIOError();
			}
			AsyncCallback userCallback = fileStreamAsyncResult._userCallback;
			if (userCallback != null)
			{
				userCallback(fileStreamAsyncResult);
			}
		}

		[SecuritySafeCritical]
		[HostProtection(SecurityAction.LinkDemand, ExternalThreading = true)]
		internal void Cancel()
		{
			if (this.IsCompleted)
			{
				return;
			}
			if (this._handle.IsInvalid)
			{
				return;
			}
			if (!Win32Native.CancelIoEx(this._handle, this._overlapped))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (lastWin32Error != 1168)
				{
					__Error.WinIOError(lastWin32Error, string.Empty);
				}
			}
		}

		private AsyncCallback _userCallback;

		private object _userStateObject;

		private ManualResetEvent _waitHandle;

		[SecurityCritical]
		private SafeFileHandle _handle;

		[SecurityCritical]
		private unsafe NativeOverlapped* _overlapped;

		internal int _EndXxxCalled;

		private int _numBytes;

		private int _errorCode;

		private int _numBufferedBytes;

		private bool _isWrite;

		private bool _isComplete;

		private bool _completedSynchronously;

		[SecurityCritical]
		private static IOCompletionCallback s_IOCallback;
	}
}
