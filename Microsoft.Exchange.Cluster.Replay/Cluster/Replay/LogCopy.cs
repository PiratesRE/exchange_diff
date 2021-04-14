using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class LogCopy
	{
		public static SafeFileHandle OpenFile(string filename, bool openForRead, out int lastError)
		{
			return LogCopy.OpenFile(filename, openForRead, out lastError, false);
		}

		public static SafeFileHandle OpenFile(string filename, bool openForRead, out int lastError, bool cachedIo)
		{
			FileMode fileMode;
			FileShare fileShare;
			FileAccess fileAccess;
			if (openForRead)
			{
				fileMode = FileMode.Open;
				fileShare = FileShare.Read;
				fileAccess = FileAccess.Read;
			}
			else
			{
				fileMode = FileMode.Create;
				fileShare = FileShare.None;
				fileAccess = FileAccess.Write;
			}
			FileFlags fileFlags;
			if (cachedIo)
			{
				fileFlags = FileFlags.FILE_ATTRIBUTE_NORMAL;
			}
			else
			{
				fileFlags = FileFlags.FILE_FLAG_NO_BUFFERING;
			}
			if (RegistryParameters.LogCopyBufferedIo != 0)
			{
				fileFlags = FileFlags.FILE_ATTRIBUTE_NORMAL;
			}
			return LogCopy.LogCopyCreateFile(filename, fileAccess, fileShare, fileMode, fileFlags, out lastError);
		}

		public static SafeFileHandle LogCopyCreateFile(string filename, FileAccess fileAccess, FileShare fileShare, FileMode fileMode, FileFlags fileFlags, out int lastError)
		{
			lastError = 0;
			SafeFileHandle safeFileHandle = NativeMethods.CreateFile(filename, fileAccess, fileShare, IntPtr.Zero, fileMode, fileFlags, IntPtr.Zero);
			if (!safeFileHandle.IsInvalid)
			{
				return safeFileHandle;
			}
			lastError = Marshal.GetLastWin32Error();
			Win32Exception ex = new Win32Exception(lastError);
			ExTraceGlobals.LogCopyTracer.TraceError<string, Win32Exception>(0L, "LogCopyCreateFile({0}) fails: {1}", filename, ex);
			string message = ReplayStrings.FileOpenError(filename, ex.Message);
			if (lastError == 2)
			{
				throw new FileNotFoundException(message, filename, ex);
			}
			int hresult = FileOperations.ConvertWin32ToHResult(lastError);
			throw new IOException(message, hresult);
		}

		public static SafeFileHandle OpenLogForRead(string filename)
		{
			int num = 0;
			int num2 = 300;
			Exception ex = null;
			ExDateTime? exDateTime = null;
			FileFlags fileFlags = FileFlags.FILE_FLAG_NO_BUFFERING;
			if (RegistryParameters.LogCopyBufferedIo != 0)
			{
				fileFlags = FileFlags.FILE_ATTRIBUTE_NORMAL;
			}
			for (;;)
			{
				int num3 = 0;
				try
				{
					return LogCopy.LogCopyCreateFile(filename, FileAccess.Read, FileShare.Read, FileMode.Open, fileFlags, out num3);
				}
				catch (IOException ex2)
				{
					ex = ex2;
					if (num3 == 32)
					{
						if (num++ == 0)
						{
							exDateTime = new ExDateTime?(ExDateTime.Now.AddSeconds((double)RegistryParameters.FileInUseRetryLimitInSecs));
						}
						else
						{
							if (exDateTime.Value <= ExDateTime.Now)
							{
								break;
							}
							num2 = 1000;
						}
						ExTraceGlobals.LogCopyTracer.TraceError<string, int>(0L, "OpenLogForRead({0}) caught sharing violation. Will retry in {1}ms", filename, num2);
						Thread.Sleep(num2);
						continue;
					}
				}
				break;
			}
			ExTraceGlobals.LogCopyTracer.TraceError<string, Exception>(0L, "OpenLogForRead({0}) failed: {1}", filename, ex);
			throw ex;
		}

		public static FileStream OpenFileStream(SafeFileHandle fileHandle, bool openForRead)
		{
			FileAccess access = openForRead ? FileAccess.Read : FileAccess.Write;
			return new FileStream(fileHandle, access, RegistryParameters.IOSizeInBytes, false);
		}
	}
}
