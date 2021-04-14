using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Text;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace System.IO
{
	[ComVisible(true)]
	public static class File
	{
		public static StreamReader OpenText(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			return new StreamReader(path);
		}

		public static StreamWriter CreateText(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			return new StreamWriter(path, false);
		}

		public static StreamWriter AppendText(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			return new StreamWriter(path, true);
		}

		public static void Copy(string sourceFileName, string destFileName)
		{
			if (sourceFileName == null)
			{
				throw new ArgumentNullException("sourceFileName", Environment.GetResourceString("ArgumentNull_FileName"));
			}
			if (destFileName == null)
			{
				throw new ArgumentNullException("destFileName", Environment.GetResourceString("ArgumentNull_FileName"));
			}
			if (sourceFileName.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyFileName"), "sourceFileName");
			}
			if (destFileName.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyFileName"), "destFileName");
			}
			File.InternalCopy(sourceFileName, destFileName, false, true);
		}

		public static void Copy(string sourceFileName, string destFileName, bool overwrite)
		{
			if (sourceFileName == null)
			{
				throw new ArgumentNullException("sourceFileName", Environment.GetResourceString("ArgumentNull_FileName"));
			}
			if (destFileName == null)
			{
				throw new ArgumentNullException("destFileName", Environment.GetResourceString("ArgumentNull_FileName"));
			}
			if (sourceFileName.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyFileName"), "sourceFileName");
			}
			if (destFileName.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyFileName"), "destFileName");
			}
			File.InternalCopy(sourceFileName, destFileName, overwrite, true);
		}

		[SecurityCritical]
		internal static void UnsafeCopy(string sourceFileName, string destFileName, bool overwrite)
		{
			if (sourceFileName == null)
			{
				throw new ArgumentNullException("sourceFileName", Environment.GetResourceString("ArgumentNull_FileName"));
			}
			if (destFileName == null)
			{
				throw new ArgumentNullException("destFileName", Environment.GetResourceString("ArgumentNull_FileName"));
			}
			if (sourceFileName.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyFileName"), "sourceFileName");
			}
			if (destFileName.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyFileName"), "destFileName");
			}
			File.InternalCopy(sourceFileName, destFileName, overwrite, false);
		}

		[SecuritySafeCritical]
		internal static string InternalCopy(string sourceFileName, string destFileName, bool overwrite, bool checkHost)
		{
			string fullPathInternal = Path.GetFullPathInternal(sourceFileName);
			string fullPathInternal2 = Path.GetFullPathInternal(destFileName);
			FileIOPermission.QuickDemand(FileIOPermissionAccess.Read, fullPathInternal, false, false);
			FileIOPermission.QuickDemand(FileIOPermissionAccess.Write, fullPathInternal2, false, false);
			if (!Win32Native.CopyFile(fullPathInternal, fullPathInternal2, !overwrite))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				string maybeFullPath = destFileName;
				if (lastWin32Error != 80)
				{
					using (SafeFileHandle safeFileHandle = Win32Native.UnsafeCreateFile(fullPathInternal, int.MinValue, FileShare.Read, null, FileMode.Open, 0, IntPtr.Zero))
					{
						if (safeFileHandle.IsInvalid)
						{
							maybeFullPath = sourceFileName;
						}
					}
					if (lastWin32Error == 5 && Directory.InternalExists(fullPathInternal2))
					{
						throw new IOException(Environment.GetResourceString("Arg_FileIsDirectory_Name", new object[]
						{
							destFileName
						}), 5, fullPathInternal2);
					}
				}
				__Error.WinIOError(lastWin32Error, maybeFullPath);
			}
			return fullPathInternal2;
		}

		public static FileStream Create(string path)
		{
			return File.Create(path, 4096);
		}

		public static FileStream Create(string path, int bufferSize)
		{
			return new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None, bufferSize);
		}

		public static FileStream Create(string path, int bufferSize, FileOptions options)
		{
			return new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.None, bufferSize, options);
		}

		public static FileStream Create(string path, int bufferSize, FileOptions options, FileSecurity fileSecurity)
		{
			return new FileStream(path, FileMode.Create, FileSystemRights.ReadData | FileSystemRights.WriteData | FileSystemRights.AppendData | FileSystemRights.ReadExtendedAttributes | FileSystemRights.WriteExtendedAttributes | FileSystemRights.ReadAttributes | FileSystemRights.WriteAttributes | FileSystemRights.ReadPermissions, FileShare.None, bufferSize, options, fileSecurity);
		}

		[SecuritySafeCritical]
		public static void Delete(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			File.InternalDelete(path, true);
		}

		[SecurityCritical]
		internal static void UnsafeDelete(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			File.InternalDelete(path, false);
		}

		[SecurityCritical]
		internal static void InternalDelete(string path, bool checkHost)
		{
			string fullPathInternal = Path.GetFullPathInternal(path);
			FileIOPermission.QuickDemand(FileIOPermissionAccess.Write, fullPathInternal, false, false);
			if (!Win32Native.DeleteFile(fullPathInternal))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (lastWin32Error == 2)
				{
					return;
				}
				__Error.WinIOError(lastWin32Error, fullPathInternal);
			}
		}

		[SecuritySafeCritical]
		public static void Decrypt(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			string fullPathInternal = Path.GetFullPathInternal(path);
			FileIOPermission.QuickDemand(FileIOPermissionAccess.Read | FileIOPermissionAccess.Write, fullPathInternal, false, false);
			if (!Win32Native.DecryptFile(fullPathInternal, 0))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (lastWin32Error == 5)
				{
					DriveInfo driveInfo = new DriveInfo(Path.GetPathRoot(fullPathInternal));
					if (!string.Equals("NTFS", driveInfo.DriveFormat))
					{
						throw new NotSupportedException(Environment.GetResourceString("NotSupported_EncryptionNeedsNTFS"));
					}
				}
				__Error.WinIOError(lastWin32Error, fullPathInternal);
			}
		}

		[SecuritySafeCritical]
		public static void Encrypt(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			string fullPathInternal = Path.GetFullPathInternal(path);
			FileIOPermission.QuickDemand(FileIOPermissionAccess.Read | FileIOPermissionAccess.Write, fullPathInternal, false, false);
			if (!Win32Native.EncryptFile(fullPathInternal))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (lastWin32Error == 5)
				{
					DriveInfo driveInfo = new DriveInfo(Path.GetPathRoot(fullPathInternal));
					if (!string.Equals("NTFS", driveInfo.DriveFormat))
					{
						throw new NotSupportedException(Environment.GetResourceString("NotSupported_EncryptionNeedsNTFS"));
					}
				}
				__Error.WinIOError(lastWin32Error, fullPathInternal);
			}
		}

		[SecuritySafeCritical]
		public static bool Exists(string path)
		{
			return File.InternalExistsHelper(path, true);
		}

		[SecurityCritical]
		internal static bool UnsafeExists(string path)
		{
			return File.InternalExistsHelper(path, false);
		}

		[SecurityCritical]
		private static bool InternalExistsHelper(string path, bool checkHost)
		{
			try
			{
				if (path == null)
				{
					return false;
				}
				if (path.Length == 0)
				{
					return false;
				}
				path = Path.GetFullPathInternal(path);
				if (path.Length > 0 && Path.IsDirectorySeparator(path[path.Length - 1]))
				{
					return false;
				}
				FileIOPermission.QuickDemand(FileIOPermissionAccess.Read, path, false, false);
				return File.InternalExists(path);
			}
			catch (ArgumentException)
			{
			}
			catch (NotSupportedException)
			{
			}
			catch (SecurityException)
			{
			}
			catch (IOException)
			{
			}
			catch (UnauthorizedAccessException)
			{
			}
			return false;
		}

		[SecurityCritical]
		internal static bool InternalExists(string path)
		{
			Win32Native.WIN32_FILE_ATTRIBUTE_DATA win32_FILE_ATTRIBUTE_DATA = default(Win32Native.WIN32_FILE_ATTRIBUTE_DATA);
			return File.FillAttributeInfo(path, ref win32_FILE_ATTRIBUTE_DATA, false, true) == 0 && win32_FILE_ATTRIBUTE_DATA.fileAttributes != -1 && (win32_FILE_ATTRIBUTE_DATA.fileAttributes & 16) == 0;
		}

		public static FileStream Open(string path, FileMode mode)
		{
			return File.Open(path, mode, (mode == FileMode.Append) ? FileAccess.Write : FileAccess.ReadWrite, FileShare.None);
		}

		public static FileStream Open(string path, FileMode mode, FileAccess access)
		{
			return File.Open(path, mode, access, FileShare.None);
		}

		public static FileStream Open(string path, FileMode mode, FileAccess access, FileShare share)
		{
			return new FileStream(path, mode, access, share);
		}

		public static void SetCreationTime(string path, DateTime creationTime)
		{
			File.SetCreationTimeUtc(path, creationTime.ToUniversalTime());
		}

		[SecuritySafeCritical]
		public unsafe static void SetCreationTimeUtc(string path, DateTime creationTimeUtc)
		{
			SafeFileHandle hFile;
			using (File.OpenFile(path, FileAccess.Write, out hFile))
			{
				Win32Native.FILE_TIME file_TIME = new Win32Native.FILE_TIME(creationTimeUtc.ToFileTimeUtc());
				if (!Win32Native.SetFileTime(hFile, &file_TIME, null, null))
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					__Error.WinIOError(lastWin32Error, path);
				}
			}
		}

		[SecuritySafeCritical]
		public static DateTime GetCreationTime(string path)
		{
			return File.InternalGetCreationTimeUtc(path, true).ToLocalTime();
		}

		[SecuritySafeCritical]
		public static DateTime GetCreationTimeUtc(string path)
		{
			return File.InternalGetCreationTimeUtc(path, false);
		}

		[SecurityCritical]
		private static DateTime InternalGetCreationTimeUtc(string path, bool checkHost)
		{
			string fullPathInternal = Path.GetFullPathInternal(path);
			FileIOPermission.QuickDemand(FileIOPermissionAccess.Read, fullPathInternal, false, false);
			Win32Native.WIN32_FILE_ATTRIBUTE_DATA win32_FILE_ATTRIBUTE_DATA = default(Win32Native.WIN32_FILE_ATTRIBUTE_DATA);
			int num = File.FillAttributeInfo(fullPathInternal, ref win32_FILE_ATTRIBUTE_DATA, false, false);
			if (num != 0)
			{
				__Error.WinIOError(num, fullPathInternal);
			}
			return DateTime.FromFileTimeUtc(win32_FILE_ATTRIBUTE_DATA.ftCreationTime.ToTicks());
		}

		public static void SetLastAccessTime(string path, DateTime lastAccessTime)
		{
			File.SetLastAccessTimeUtc(path, lastAccessTime.ToUniversalTime());
		}

		[SecuritySafeCritical]
		public unsafe static void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc)
		{
			SafeFileHandle hFile;
			using (File.OpenFile(path, FileAccess.Write, out hFile))
			{
				Win32Native.FILE_TIME file_TIME = new Win32Native.FILE_TIME(lastAccessTimeUtc.ToFileTimeUtc());
				if (!Win32Native.SetFileTime(hFile, null, &file_TIME, null))
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					__Error.WinIOError(lastWin32Error, path);
				}
			}
		}

		[SecuritySafeCritical]
		public static DateTime GetLastAccessTime(string path)
		{
			return File.InternalGetLastAccessTimeUtc(path, true).ToLocalTime();
		}

		[SecuritySafeCritical]
		public static DateTime GetLastAccessTimeUtc(string path)
		{
			return File.InternalGetLastAccessTimeUtc(path, false);
		}

		[SecurityCritical]
		private static DateTime InternalGetLastAccessTimeUtc(string path, bool checkHost)
		{
			string fullPathInternal = Path.GetFullPathInternal(path);
			FileIOPermission.QuickDemand(FileIOPermissionAccess.Read, fullPathInternal, false, false);
			Win32Native.WIN32_FILE_ATTRIBUTE_DATA win32_FILE_ATTRIBUTE_DATA = default(Win32Native.WIN32_FILE_ATTRIBUTE_DATA);
			int num = File.FillAttributeInfo(fullPathInternal, ref win32_FILE_ATTRIBUTE_DATA, false, false);
			if (num != 0)
			{
				__Error.WinIOError(num, fullPathInternal);
			}
			return DateTime.FromFileTimeUtc(win32_FILE_ATTRIBUTE_DATA.ftLastAccessTime.ToTicks());
		}

		public static void SetLastWriteTime(string path, DateTime lastWriteTime)
		{
			File.SetLastWriteTimeUtc(path, lastWriteTime.ToUniversalTime());
		}

		[SecuritySafeCritical]
		public unsafe static void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
		{
			SafeFileHandle hFile;
			using (File.OpenFile(path, FileAccess.Write, out hFile))
			{
				Win32Native.FILE_TIME file_TIME = new Win32Native.FILE_TIME(lastWriteTimeUtc.ToFileTimeUtc());
				if (!Win32Native.SetFileTime(hFile, null, null, &file_TIME))
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					__Error.WinIOError(lastWin32Error, path);
				}
			}
		}

		[SecuritySafeCritical]
		public static DateTime GetLastWriteTime(string path)
		{
			return File.InternalGetLastWriteTimeUtc(path, true).ToLocalTime();
		}

		[SecuritySafeCritical]
		public static DateTime GetLastWriteTimeUtc(string path)
		{
			return File.InternalGetLastWriteTimeUtc(path, false);
		}

		[SecurityCritical]
		private static DateTime InternalGetLastWriteTimeUtc(string path, bool checkHost)
		{
			string fullPathInternal = Path.GetFullPathInternal(path);
			FileIOPermission.QuickDemand(FileIOPermissionAccess.Read, fullPathInternal, false, false);
			Win32Native.WIN32_FILE_ATTRIBUTE_DATA win32_FILE_ATTRIBUTE_DATA = default(Win32Native.WIN32_FILE_ATTRIBUTE_DATA);
			int num = File.FillAttributeInfo(fullPathInternal, ref win32_FILE_ATTRIBUTE_DATA, false, false);
			if (num != 0)
			{
				__Error.WinIOError(num, fullPathInternal);
			}
			return DateTime.FromFileTimeUtc(win32_FILE_ATTRIBUTE_DATA.ftLastWriteTime.ToTicks());
		}

		[SecuritySafeCritical]
		public static FileAttributes GetAttributes(string path)
		{
			string fullPathInternal = Path.GetFullPathInternal(path);
			FileIOPermission.QuickDemand(FileIOPermissionAccess.Read, fullPathInternal, false, false);
			Win32Native.WIN32_FILE_ATTRIBUTE_DATA win32_FILE_ATTRIBUTE_DATA = default(Win32Native.WIN32_FILE_ATTRIBUTE_DATA);
			int num = File.FillAttributeInfo(fullPathInternal, ref win32_FILE_ATTRIBUTE_DATA, false, true);
			if (num != 0)
			{
				__Error.WinIOError(num, fullPathInternal);
			}
			return (FileAttributes)win32_FILE_ATTRIBUTE_DATA.fileAttributes;
		}

		[SecuritySafeCritical]
		public static void SetAttributes(string path, FileAttributes fileAttributes)
		{
			string fullPathInternal = Path.GetFullPathInternal(path);
			FileIOPermission.QuickDemand(FileIOPermissionAccess.Write, fullPathInternal, false, false);
			if (!Win32Native.SetFileAttributes(fullPathInternal, (int)fileAttributes))
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (lastWin32Error == 87)
				{
					throw new ArgumentException(Environment.GetResourceString("Arg_InvalidFileAttrs"));
				}
				__Error.WinIOError(lastWin32Error, fullPathInternal);
			}
		}

		public static FileSecurity GetAccessControl(string path)
		{
			return File.GetAccessControl(path, AccessControlSections.Access | AccessControlSections.Owner | AccessControlSections.Group);
		}

		public static FileSecurity GetAccessControl(string path, AccessControlSections includeSections)
		{
			return new FileSecurity(path, includeSections);
		}

		[SecuritySafeCritical]
		public static void SetAccessControl(string path, FileSecurity fileSecurity)
		{
			if (fileSecurity == null)
			{
				throw new ArgumentNullException("fileSecurity");
			}
			string fullPathInternal = Path.GetFullPathInternal(path);
			fileSecurity.Persist(fullPathInternal);
		}

		public static FileStream OpenRead(string path)
		{
			return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
		}

		public static FileStream OpenWrite(string path)
		{
			return new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
		}

		[SecuritySafeCritical]
		public static string ReadAllText(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyPath"));
			}
			return File.InternalReadAllText(path, Encoding.UTF8, true);
		}

		[SecuritySafeCritical]
		public static string ReadAllText(string path, Encoding encoding)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyPath"));
			}
			return File.InternalReadAllText(path, encoding, true);
		}

		[SecurityCritical]
		internal static string UnsafeReadAllText(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyPath"));
			}
			return File.InternalReadAllText(path, Encoding.UTF8, false);
		}

		[SecurityCritical]
		private static string InternalReadAllText(string path, Encoding encoding, bool checkHost)
		{
			string result;
			using (StreamReader streamReader = new StreamReader(path, encoding, true, StreamReader.DefaultBufferSize, checkHost))
			{
				result = streamReader.ReadToEnd();
			}
			return result;
		}

		[SecuritySafeCritical]
		public static void WriteAllText(string path, string contents)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyPath"));
			}
			File.InternalWriteAllText(path, contents, StreamWriter.UTF8NoBOM, true);
		}

		[SecuritySafeCritical]
		public static void WriteAllText(string path, string contents, Encoding encoding)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyPath"));
			}
			File.InternalWriteAllText(path, contents, encoding, true);
		}

		[SecurityCritical]
		internal static void UnsafeWriteAllText(string path, string contents)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyPath"));
			}
			File.InternalWriteAllText(path, contents, StreamWriter.UTF8NoBOM, false);
		}

		[SecurityCritical]
		private static void InternalWriteAllText(string path, string contents, Encoding encoding, bool checkHost)
		{
			using (StreamWriter streamWriter = new StreamWriter(path, false, encoding, 1024, checkHost))
			{
				streamWriter.Write(contents);
			}
		}

		[SecuritySafeCritical]
		public static byte[] ReadAllBytes(string path)
		{
			return File.InternalReadAllBytes(path, true);
		}

		[SecurityCritical]
		internal static byte[] UnsafeReadAllBytes(string path)
		{
			return File.InternalReadAllBytes(path, false);
		}

		[SecurityCritical]
		private static byte[] InternalReadAllBytes(string path, bool checkHost)
		{
			byte[] array;
			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.None, Path.GetFileName(path), false, false, checkHost))
			{
				int num = 0;
				long length = fileStream.Length;
				if (length > 2147483647L)
				{
					throw new IOException(Environment.GetResourceString("IO.IO_FileTooLong2GB"));
				}
				int i = (int)length;
				array = new byte[i];
				while (i > 0)
				{
					int num2 = fileStream.Read(array, num, i);
					if (num2 == 0)
					{
						__Error.EndOfFile();
					}
					num += num2;
					i -= num2;
				}
			}
			return array;
		}

		[SecuritySafeCritical]
		public static void WriteAllBytes(string path, byte[] bytes)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path", Environment.GetResourceString("ArgumentNull_Path"));
			}
			if (path.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyPath"));
			}
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			File.InternalWriteAllBytes(path, bytes, true);
		}

		[SecurityCritical]
		internal static void UnsafeWriteAllBytes(string path, byte[] bytes)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path", Environment.GetResourceString("ArgumentNull_Path"));
			}
			if (path.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyPath"));
			}
			if (bytes == null)
			{
				throw new ArgumentNullException("bytes");
			}
			File.InternalWriteAllBytes(path, bytes, false);
		}

		[SecurityCritical]
		private static void InternalWriteAllBytes(string path, byte[] bytes, bool checkHost)
		{
			using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Read, 4096, FileOptions.None, Path.GetFileName(path), false, false, checkHost))
			{
				fileStream.Write(bytes, 0, bytes.Length);
			}
		}

		public static string[] ReadAllLines(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyPath"));
			}
			return File.InternalReadAllLines(path, Encoding.UTF8);
		}

		public static string[] ReadAllLines(string path, Encoding encoding)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyPath"));
			}
			return File.InternalReadAllLines(path, encoding);
		}

		private static string[] InternalReadAllLines(string path, Encoding encoding)
		{
			List<string> list = new List<string>();
			using (StreamReader streamReader = new StreamReader(path, encoding))
			{
				string item;
				while ((item = streamReader.ReadLine()) != null)
				{
					list.Add(item);
				}
			}
			return list.ToArray();
		}

		public static IEnumerable<string> ReadLines(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyPath"), "path");
			}
			return ReadLinesIterator.CreateIterator(path, Encoding.UTF8);
		}

		public static IEnumerable<string> ReadLines(string path, Encoding encoding)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyPath"), "path");
			}
			return ReadLinesIterator.CreateIterator(path, encoding);
		}

		public static void WriteAllLines(string path, string[] contents)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (contents == null)
			{
				throw new ArgumentNullException("contents");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyPath"));
			}
			File.InternalWriteAllLines(new StreamWriter(path, false, StreamWriter.UTF8NoBOM), contents);
		}

		public static void WriteAllLines(string path, string[] contents, Encoding encoding)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (contents == null)
			{
				throw new ArgumentNullException("contents");
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyPath"));
			}
			File.InternalWriteAllLines(new StreamWriter(path, false, encoding), contents);
		}

		public static void WriteAllLines(string path, IEnumerable<string> contents)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (contents == null)
			{
				throw new ArgumentNullException("contents");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyPath"));
			}
			File.InternalWriteAllLines(new StreamWriter(path, false, StreamWriter.UTF8NoBOM), contents);
		}

		public static void WriteAllLines(string path, IEnumerable<string> contents, Encoding encoding)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (contents == null)
			{
				throw new ArgumentNullException("contents");
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyPath"));
			}
			File.InternalWriteAllLines(new StreamWriter(path, false, encoding), contents);
		}

		private static void InternalWriteAllLines(TextWriter writer, IEnumerable<string> contents)
		{
			try
			{
				foreach (string value in contents)
				{
					writer.WriteLine(value);
				}
			}
			finally
			{
				if (writer != null)
				{
					((IDisposable)writer).Dispose();
				}
			}
		}

		public static void AppendAllText(string path, string contents)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyPath"));
			}
			File.InternalAppendAllText(path, contents, StreamWriter.UTF8NoBOM);
		}

		public static void AppendAllText(string path, string contents, Encoding encoding)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyPath"));
			}
			File.InternalAppendAllText(path, contents, encoding);
		}

		private static void InternalAppendAllText(string path, string contents, Encoding encoding)
		{
			using (StreamWriter streamWriter = new StreamWriter(path, true, encoding))
			{
				streamWriter.Write(contents);
			}
		}

		public static void AppendAllLines(string path, IEnumerable<string> contents)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (contents == null)
			{
				throw new ArgumentNullException("contents");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyPath"));
			}
			File.InternalWriteAllLines(new StreamWriter(path, true, StreamWriter.UTF8NoBOM), contents);
		}

		public static void AppendAllLines(string path, IEnumerable<string> contents, Encoding encoding)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (contents == null)
			{
				throw new ArgumentNullException("contents");
			}
			if (encoding == null)
			{
				throw new ArgumentNullException("encoding");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyPath"));
			}
			File.InternalWriteAllLines(new StreamWriter(path, true, encoding), contents);
		}

		[SecuritySafeCritical]
		public static void Move(string sourceFileName, string destFileName)
		{
			File.InternalMove(sourceFileName, destFileName, true);
		}

		[SecurityCritical]
		internal static void UnsafeMove(string sourceFileName, string destFileName)
		{
			File.InternalMove(sourceFileName, destFileName, false);
		}

		[SecurityCritical]
		private static void InternalMove(string sourceFileName, string destFileName, bool checkHost)
		{
			if (sourceFileName == null)
			{
				throw new ArgumentNullException("sourceFileName", Environment.GetResourceString("ArgumentNull_FileName"));
			}
			if (destFileName == null)
			{
				throw new ArgumentNullException("destFileName", Environment.GetResourceString("ArgumentNull_FileName"));
			}
			if (sourceFileName.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyFileName"), "sourceFileName");
			}
			if (destFileName.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyFileName"), "destFileName");
			}
			string fullPathInternal = Path.GetFullPathInternal(sourceFileName);
			string fullPathInternal2 = Path.GetFullPathInternal(destFileName);
			FileIOPermission.QuickDemand(FileIOPermissionAccess.Read | FileIOPermissionAccess.Write, fullPathInternal, false, false);
			FileIOPermission.QuickDemand(FileIOPermissionAccess.Write, fullPathInternal2, false, false);
			if (!File.InternalExists(fullPathInternal))
			{
				__Error.WinIOError(2, fullPathInternal);
			}
			if (!Win32Native.MoveFile(fullPathInternal, fullPathInternal2))
			{
				__Error.WinIOError();
			}
		}

		public static void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName)
		{
			if (sourceFileName == null)
			{
				throw new ArgumentNullException("sourceFileName");
			}
			if (destinationFileName == null)
			{
				throw new ArgumentNullException("destinationFileName");
			}
			File.InternalReplace(sourceFileName, destinationFileName, destinationBackupFileName, false);
		}

		public static void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors)
		{
			if (sourceFileName == null)
			{
				throw new ArgumentNullException("sourceFileName");
			}
			if (destinationFileName == null)
			{
				throw new ArgumentNullException("destinationFileName");
			}
			File.InternalReplace(sourceFileName, destinationFileName, destinationBackupFileName, ignoreMetadataErrors);
		}

		[SecuritySafeCritical]
		private static void InternalReplace(string sourceFileName, string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors)
		{
			string fullPathInternal = Path.GetFullPathInternal(sourceFileName);
			string fullPathInternal2 = Path.GetFullPathInternal(destinationFileName);
			string text = null;
			if (destinationBackupFileName != null)
			{
				text = Path.GetFullPathInternal(destinationBackupFileName);
			}
			if (CodeAccessSecurityEngine.QuickCheckForAllDemands())
			{
				FileIOPermission.EmulateFileIOPermissionChecks(fullPathInternal);
				FileIOPermission.EmulateFileIOPermissionChecks(fullPathInternal2);
				if (text != null)
				{
					FileIOPermission.EmulateFileIOPermissionChecks(text);
				}
			}
			else
			{
				FileIOPermission fileIOPermission = new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.Write, new string[]
				{
					fullPathInternal,
					fullPathInternal2
				});
				if (text != null)
				{
					fileIOPermission.AddPathList(FileIOPermissionAccess.Write, text);
				}
				fileIOPermission.Demand();
			}
			int num = 1;
			if (ignoreMetadataErrors)
			{
				num |= 2;
			}
			if (!Win32Native.ReplaceFile(fullPathInternal2, fullPathInternal, text, num, IntPtr.Zero, IntPtr.Zero))
			{
				__Error.WinIOError();
			}
		}

		[SecurityCritical]
		internal static int FillAttributeInfo(string path, ref Win32Native.WIN32_FILE_ATTRIBUTE_DATA data, bool tryagain, bool returnErrorOnNotFound)
		{
			int num = 0;
			if (tryagain)
			{
				Win32Native.WIN32_FIND_DATA win32_FIND_DATA = default(Win32Native.WIN32_FIND_DATA);
				string fileName = path.TrimEnd(new char[]
				{
					Path.DirectorySeparatorChar,
					Path.AltDirectorySeparatorChar
				});
				int errorMode = Win32Native.SetErrorMode(1);
				try
				{
					bool flag = false;
					SafeFindHandle safeFindHandle = Win32Native.FindFirstFile(fileName, ref win32_FIND_DATA);
					try
					{
						if (safeFindHandle.IsInvalid)
						{
							flag = true;
							num = Marshal.GetLastWin32Error();
							if ((num == 2 || num == 3 || num == 21) && !returnErrorOnNotFound)
							{
								num = 0;
								data.fileAttributes = -1;
							}
							return num;
						}
					}
					finally
					{
						try
						{
							safeFindHandle.Close();
						}
						catch
						{
							if (!flag)
							{
								__Error.WinIOError();
							}
						}
					}
				}
				finally
				{
					Win32Native.SetErrorMode(errorMode);
				}
				data.PopulateFrom(ref win32_FIND_DATA);
				return num;
			}
			bool flag2 = false;
			int errorMode2 = Win32Native.SetErrorMode(1);
			try
			{
				flag2 = Win32Native.GetFileAttributesEx(path, 0, ref data);
			}
			finally
			{
				Win32Native.SetErrorMode(errorMode2);
			}
			if (!flag2)
			{
				num = Marshal.GetLastWin32Error();
				if (num != 2 && num != 3 && num != 21)
				{
					return File.FillAttributeInfo(path, ref data, true, returnErrorOnNotFound);
				}
				if (!returnErrorOnNotFound)
				{
					num = 0;
					data.fileAttributes = -1;
				}
			}
			return num;
		}

		[SecurityCritical]
		private static FileStream OpenFile(string path, FileAccess access, out SafeFileHandle handle)
		{
			FileStream fileStream = new FileStream(path, FileMode.Open, access, FileShare.ReadWrite, 1);
			handle = fileStream.SafeFileHandle;
			if (handle.IsInvalid)
			{
				int num = Marshal.GetLastWin32Error();
				string fullPathInternal = Path.GetFullPathInternal(path);
				if (num == 3 && fullPathInternal.Equals(Directory.GetDirectoryRoot(fullPathInternal)))
				{
					num = 5;
				}
				__Error.WinIOError(num, path);
			}
			return fileStream;
		}

		private const int GetFileExInfoStandard = 0;

		private const int ERROR_INVALID_PARAMETER = 87;

		private const int ERROR_ACCESS_DENIED = 5;
	}
}
