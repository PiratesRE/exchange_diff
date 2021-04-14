using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Common.HA;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class FileOperations
	{
		public static string GetCurrentDateString()
		{
			return DateTime.UtcNow.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss");
		}

		public static void RemoveDirectory(string path)
		{
			if (Directory.Exists(path))
			{
				Directory.Delete(path, true);
			}
		}

		public static void CreateDirectoryIfNeeded(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

		internal static void TruncateFile(string fileName, long fileSize)
		{
			int num;
			using (SafeFileHandle safeFileHandle = LogCopy.LogCopyCreateFile(fileName, FileAccess.Write, FileShare.None, FileMode.Open, FileFlags.FILE_ATTRIBUTE_NORMAL, out num))
			{
				long num2;
				NativeMethods.SetFilePointerEx(safeFileHandle, fileSize, out num2, 0U);
				NativeMethods.SetEndOfFile(safeFileHandle);
			}
		}

		internal static NativeMethods.WIN32_FILE_ATTRIBUTE_DATA GetFileAttributesEx(string path, out Exception ex)
		{
			NativeMethods.WIN32_FILE_ATTRIBUTE_DATA result = default(NativeMethods.WIN32_FILE_ATTRIBUTE_DATA);
			ex = null;
			if (!NativeMethods.GetFileAttributesEx(path, NativeMethods.GET_FILEEX_INFO_LEVELS.GetFileExInfoStandard, out result))
			{
				ex = new Win32Exception();
				return result;
			}
			return result;
		}

		internal static uint GetSectorSize(string fileName)
		{
			return 512U;
		}

		internal static bool IsDiskFullException(IOException exception)
		{
			uint hrforException = (uint)Marshal.GetHRForException(exception);
			return hrforException == 2147942512U;
		}

		internal static void ThrowDiskFullException(string msg)
		{
			throw new IOException(msg, -2147024784);
		}

		internal static bool ConvertHResultToWin32(int hresult, out int winErrorCode)
		{
			if (((long)hresult & (long)((ulong)-65536)) == (long)((ulong)-2147024896))
			{
				winErrorCode = (hresult & 65535);
				return true;
			}
			winErrorCode = 0;
			return false;
		}

		internal static int ConvertWin32ToHResult(int winErrorCode)
		{
			return -2147024896 | (winErrorCode & 65535);
		}

		internal static bool IsFatalIOException(IOException exception, out int hresult)
		{
			bool result = false;
			int win32Code = 0;
			hresult = Marshal.GetHRForException(exception);
			if (FileOperations.ConvertHResultToWin32(hresult, out win32Code))
			{
				result = FileOperations.IsFatalIOErrorCode(win32Code);
			}
			return result;
		}

		internal static bool IsFatalIOErrorCode(int win32Code)
		{
			bool result = false;
			if (win32Code <= 23)
			{
				if (win32Code != 15)
				{
					switch (win32Code)
					{
					case 20:
					case 21:
					case 23:
						break;
					case 22:
						return result;
					default:
						return result;
					}
				}
			}
			else if (win32Code != 1117 && win32Code != 1392)
			{
				return result;
			}
			result = true;
			return result;
		}

		internal static bool IsCorruptedIOException(IOException exception, out int hresult)
		{
			bool result = false;
			int win32Code = 0;
			hresult = Marshal.GetHRForException(exception);
			if (FileOperations.ConvertHResultToWin32(hresult, out win32Code))
			{
				result = FileOperations.IsCorruptedIOErrorCode(win32Code);
			}
			return result;
		}

		internal static bool IsCorruptedIOErrorCode(int win32Code)
		{
			bool result = false;
			if (win32Code != 23 && win32Code != 1005)
			{
				switch (win32Code)
				{
				case 1392:
				case 1393:
					break;
				default:
					return result;
				}
			}
			result = true;
			return result;
		}

		internal static bool IsRetryableIOException(IOException exception)
		{
			bool result = false;
			int win32Code = 0;
			int hrforException = Marshal.GetHRForException(exception);
			if (FileOperations.IsVolumeLockedHResult(hrforException))
			{
				return true;
			}
			if (FileOperations.ConvertHResultToWin32(hrforException, out win32Code))
			{
				result = FileOperations.IsRetryableIOErrorCode(win32Code);
			}
			return result;
		}

		internal static bool IsVolumeLockedException(IOException exception)
		{
			int hrforException = Marshal.GetHRForException(exception);
			return FileOperations.IsVolumeLockedHResult(hrforException);
		}

		internal static bool IsVolumeLockedHResult(int hResult)
		{
			return hResult == -2143879145 || hResult == -2144272361;
		}

		internal static bool IsRetryableIOErrorCode(int win32Code)
		{
			bool result = false;
			if (win32Code == 32 || win32Code == 995 || win32Code == 1450)
			{
				result = true;
			}
			return result;
		}

		internal static bool Win32ErrorCodeToIOFailureTag(int win32code, FailureTag defaultFailureTag, out FailureTag failureTag)
		{
			failureTag = FailureTag.NoOp;
			if (win32code <= 80)
			{
				switch (win32code)
				{
				case 0:
				case 2:
					break;
				case 1:
				case 4:
					goto IL_77;
				case 3:
				case 5:
					failureTag = FailureTag.Configuration;
					return true;
				default:
					switch (win32code)
					{
					case 38:
						break;
					case 39:
						goto IL_72;
					default:
						if (win32code != 80)
						{
							goto IL_77;
						}
						break;
					}
					break;
				}
			}
			else if (win32code <= 112)
			{
				if (win32code != 87)
				{
					if (win32code != 112)
					{
						goto IL_77;
					}
					goto IL_72;
				}
			}
			else if (win32code != 183)
			{
				if (win32code != 206)
				{
					goto IL_77;
				}
				failureTag = FailureTag.Unrecoverable;
				return true;
			}
			failureTag = FailureTag.NoOp;
			return true;
			IL_72:
			failureTag = FailureTag.Space;
			return true;
			IL_77:
			if (FileOperations.IsRetryableIOErrorCode(win32code))
			{
				failureTag = FailureTag.NoOp;
				return true;
			}
			if (FileOperations.IsFatalIOErrorCode(win32code))
			{
				failureTag = FailureTag.IoHard;
				return true;
			}
			failureTag = defaultFailureTag;
			return false;
		}

		private const uint Win32ErrorDiskFull = 2147942512U;
	}
}
