using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using Microsoft.Win32;

namespace System.IO
{
	internal static class __Error
	{
		internal static void EndOfFile()
		{
			throw new EndOfStreamException(Environment.GetResourceString("IO.EOF_ReadBeyondEOF"));
		}

		internal static void FileNotOpen()
		{
			throw new ObjectDisposedException(null, Environment.GetResourceString("ObjectDisposed_FileClosed"));
		}

		internal static void StreamIsClosed()
		{
			throw new ObjectDisposedException(null, Environment.GetResourceString("ObjectDisposed_StreamClosed"));
		}

		internal static void MemoryStreamNotExpandable()
		{
			throw new NotSupportedException(Environment.GetResourceString("NotSupported_MemStreamNotExpandable"));
		}

		internal static void ReaderClosed()
		{
			throw new ObjectDisposedException(null, Environment.GetResourceString("ObjectDisposed_ReaderClosed"));
		}

		internal static void ReadNotSupported()
		{
			throw new NotSupportedException(Environment.GetResourceString("NotSupported_UnreadableStream"));
		}

		internal static void SeekNotSupported()
		{
			throw new NotSupportedException(Environment.GetResourceString("NotSupported_UnseekableStream"));
		}

		internal static void WrongAsyncResult()
		{
			throw new ArgumentException(Environment.GetResourceString("Arg_WrongAsyncResult"));
		}

		internal static void EndReadCalledTwice()
		{
			throw new ArgumentException(Environment.GetResourceString("InvalidOperation_EndReadCalledMultiple"));
		}

		internal static void EndWriteCalledTwice()
		{
			throw new ArgumentException(Environment.GetResourceString("InvalidOperation_EndWriteCalledMultiple"));
		}

		[SecurityCritical]
		internal static string GetDisplayablePath(string path, bool isInvalidPath)
		{
			if (string.IsNullOrEmpty(path))
			{
				return string.Empty;
			}
			if (path.Length < 2)
			{
				return path;
			}
			if (PathInternal.IsPartiallyQualified(path) && !isInvalidPath)
			{
				return path;
			}
			bool flag = false;
			try
			{
				if (!isInvalidPath)
				{
					FileIOPermission.QuickDemand(FileIOPermissionAccess.PathDiscovery, path, false, false);
					flag = true;
				}
			}
			catch (SecurityException)
			{
			}
			catch (ArgumentException)
			{
			}
			catch (NotSupportedException)
			{
			}
			if (!flag)
			{
				if (Path.IsDirectorySeparator(path[path.Length - 1]))
				{
					path = Environment.GetResourceString("IO.IO_NoPermissionToDirectoryName");
				}
				else
				{
					path = Path.GetFileName(path);
				}
			}
			return path;
		}

		[SecuritySafeCritical]
		internal static void WinIOError()
		{
			int lastWin32Error = Marshal.GetLastWin32Error();
			__Error.WinIOError(lastWin32Error, string.Empty);
		}

		[SecurityCritical]
		internal static void WinIOError(int errorCode, string maybeFullPath)
		{
			bool isInvalidPath = errorCode == 123 || errorCode == 161;
			string displayablePath = __Error.GetDisplayablePath(maybeFullPath, isInvalidPath);
			if (errorCode <= 80)
			{
				if (errorCode <= 15)
				{
					switch (errorCode)
					{
					case 2:
						if (displayablePath.Length == 0)
						{
							throw new FileNotFoundException(Environment.GetResourceString("IO.FileNotFound"));
						}
						throw new FileNotFoundException(Environment.GetResourceString("IO.FileNotFound_FileName", new object[]
						{
							displayablePath
						}), displayablePath);
					case 3:
						if (displayablePath.Length == 0)
						{
							throw new DirectoryNotFoundException(Environment.GetResourceString("IO.PathNotFound_NoPathName"));
						}
						throw new DirectoryNotFoundException(Environment.GetResourceString("IO.PathNotFound_Path", new object[]
						{
							displayablePath
						}));
					case 4:
						break;
					case 5:
						if (displayablePath.Length == 0)
						{
							throw new UnauthorizedAccessException(Environment.GetResourceString("UnauthorizedAccess_IODenied_NoPathName"));
						}
						throw new UnauthorizedAccessException(Environment.GetResourceString("UnauthorizedAccess_IODenied_Path", new object[]
						{
							displayablePath
						}));
					default:
						if (errorCode == 15)
						{
							throw new DriveNotFoundException(Environment.GetResourceString("IO.DriveNotFound_Drive", new object[]
							{
								displayablePath
							}));
						}
						break;
					}
				}
				else if (errorCode != 32)
				{
					if (errorCode == 80)
					{
						if (displayablePath.Length != 0)
						{
							throw new IOException(Environment.GetResourceString("IO.IO_FileExists_Name", new object[]
							{
								displayablePath
							}), Win32Native.MakeHRFromErrorCode(errorCode), maybeFullPath);
						}
					}
				}
				else
				{
					if (displayablePath.Length == 0)
					{
						throw new IOException(Environment.GetResourceString("IO.IO_SharingViolation_NoFileName"), Win32Native.MakeHRFromErrorCode(errorCode), maybeFullPath);
					}
					throw new IOException(Environment.GetResourceString("IO.IO_SharingViolation_File", new object[]
					{
						displayablePath
					}), Win32Native.MakeHRFromErrorCode(errorCode), maybeFullPath);
				}
			}
			else if (errorCode <= 183)
			{
				if (errorCode == 87)
				{
					throw new IOException(Win32Native.GetMessage(errorCode), Win32Native.MakeHRFromErrorCode(errorCode), maybeFullPath);
				}
				if (errorCode == 183)
				{
					if (displayablePath.Length != 0)
					{
						throw new IOException(Environment.GetResourceString("IO.IO_AlreadyExists_Name", new object[]
						{
							displayablePath
						}), Win32Native.MakeHRFromErrorCode(errorCode), maybeFullPath);
					}
				}
			}
			else
			{
				if (errorCode == 206)
				{
					throw new PathTooLongException(Environment.GetResourceString("IO.PathTooLong"));
				}
				if (errorCode == 995)
				{
					throw new OperationCanceledException();
				}
			}
			throw new IOException(Win32Native.GetMessage(errorCode), Win32Native.MakeHRFromErrorCode(errorCode), maybeFullPath);
		}

		[SecuritySafeCritical]
		internal static void WinIODriveError(string driveName)
		{
			int lastWin32Error = Marshal.GetLastWin32Error();
			__Error.WinIODriveError(driveName, lastWin32Error);
		}

		[SecurityCritical]
		internal static void WinIODriveError(string driveName, int errorCode)
		{
			if (errorCode == 3 || errorCode == 15)
			{
				throw new DriveNotFoundException(Environment.GetResourceString("IO.DriveNotFound_Drive", new object[]
				{
					driveName
				}));
			}
			__Error.WinIOError(errorCode, driveName);
		}

		internal static void WriteNotSupported()
		{
			throw new NotSupportedException(Environment.GetResourceString("NotSupported_UnwritableStream"));
		}

		internal static void WriterClosed()
		{
			throw new ObjectDisposedException(null, Environment.GetResourceString("ObjectDisposed_WriterClosed"));
		}

		internal const int ERROR_FILE_NOT_FOUND = 2;

		internal const int ERROR_PATH_NOT_FOUND = 3;

		internal const int ERROR_ACCESS_DENIED = 5;

		internal const int ERROR_INVALID_PARAMETER = 87;
	}
}
