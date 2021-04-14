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
	public static class Directory
	{
		public static DirectoryInfo GetParent(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_PathEmpty"), "path");
			}
			string fullPathInternal = Path.GetFullPathInternal(path);
			string directoryName = Path.GetDirectoryName(fullPathInternal);
			if (directoryName == null)
			{
				return null;
			}
			return new DirectoryInfo(directoryName);
		}

		[SecuritySafeCritical]
		public static DirectoryInfo CreateDirectory(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_PathEmpty"));
			}
			return Directory.InternalCreateDirectoryHelper(path, true);
		}

		[SecurityCritical]
		internal static DirectoryInfo UnsafeCreateDirectory(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_PathEmpty"));
			}
			return Directory.InternalCreateDirectoryHelper(path, false);
		}

		[SecurityCritical]
		internal static DirectoryInfo InternalCreateDirectoryHelper(string path, bool checkHost)
		{
			string fullPathAndCheckPermissions = Directory.GetFullPathAndCheckPermissions(path, checkHost, FileSecurityStateAccess.Read);
			Directory.InternalCreateDirectory(fullPathAndCheckPermissions, path, null, checkHost);
			return new DirectoryInfo(fullPathAndCheckPermissions, false);
		}

		internal static string GetFullPathAndCheckPermissions(string path, bool checkHost, FileSecurityStateAccess access = FileSecurityStateAccess.Read)
		{
			string fullPathInternal = Path.GetFullPathInternal(path);
			Directory.CheckPermissions(path, fullPathInternal, checkHost, access);
			return fullPathInternal;
		}

		[SecuritySafeCritical]
		internal static void CheckPermissions(string displayPath, string fullPath, bool checkHost, FileSecurityStateAccess access = FileSecurityStateAccess.Read)
		{
			if (CodeAccessSecurityEngine.QuickCheckForAllDemands())
			{
				FileIOPermission.EmulateFileIOPermissionChecks(fullPath);
				return;
			}
			FileIOPermission.QuickDemand((FileIOPermissionAccess)access, Directory.GetDemandDir(fullPath, true), false, false);
		}

		[SecuritySafeCritical]
		public static DirectoryInfo CreateDirectory(string path, DirectorySecurity directorySecurity)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_PathEmpty"));
			}
			string fullPathAndCheckPermissions = Directory.GetFullPathAndCheckPermissions(path, true, FileSecurityStateAccess.Read);
			Directory.InternalCreateDirectory(fullPathAndCheckPermissions, path, directorySecurity);
			return new DirectoryInfo(fullPathAndCheckPermissions, false);
		}

		internal static string GetDemandDir(string fullPath, bool thisDirOnly)
		{
			string result;
			if (thisDirOnly)
			{
				if (fullPath.EndsWith(Path.DirectorySeparatorChar) || fullPath.EndsWith(Path.AltDirectorySeparatorChar))
				{
					result = fullPath + ".";
				}
				else
				{
					result = fullPath + "\\.";
				}
			}
			else if (!fullPath.EndsWith(Path.DirectorySeparatorChar) && !fullPath.EndsWith(Path.AltDirectorySeparatorChar))
			{
				result = fullPath + "\\";
			}
			else
			{
				result = fullPath;
			}
			return result;
		}

		internal static void InternalCreateDirectory(string fullPath, string path, object dirSecurityObj)
		{
			Directory.InternalCreateDirectory(fullPath, path, dirSecurityObj, false);
		}

		[SecuritySafeCritical]
		internal unsafe static void InternalCreateDirectory(string fullPath, string path, object dirSecurityObj, bool checkHost)
		{
			DirectorySecurity directorySecurity = (DirectorySecurity)dirSecurityObj;
			int num = fullPath.Length;
			if (num >= 2 && Path.IsDirectorySeparator(fullPath[num - 1]))
			{
				num--;
			}
			int rootLength = Path.GetRootLength(fullPath);
			if (num == 2 && Path.IsDirectorySeparator(fullPath[1]))
			{
				throw new IOException(Environment.GetResourceString("IO.IO_CannotCreateDirectory", new object[]
				{
					path
				}));
			}
			if (Directory.InternalExists(fullPath))
			{
				return;
			}
			List<string> list = new List<string>();
			bool flag = false;
			if (num > rootLength)
			{
				int num2 = num - 1;
				while (num2 >= rootLength && !flag)
				{
					string text = fullPath.Substring(0, num2 + 1);
					if (!Directory.InternalExists(text))
					{
						list.Add(text);
					}
					else
					{
						flag = true;
					}
					while (num2 > rootLength && fullPath[num2] != Path.DirectorySeparatorChar && fullPath[num2] != Path.AltDirectorySeparatorChar)
					{
						num2--;
					}
					num2--;
				}
			}
			int count = list.Count;
			if (list.Count != 0 && !CodeAccessSecurityEngine.QuickCheckForAllDemands())
			{
				string[] array = new string[list.Count];
				list.CopyTo(array, 0);
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array;
					int num3 = i;
					array2[num3] += "\\.";
				}
				AccessControlActions control = (directorySecurity == null) ? AccessControlActions.None : AccessControlActions.Change;
				FileIOPermission.QuickDemand(FileIOPermissionAccess.Write, control, array, false, false);
			}
			Win32Native.SECURITY_ATTRIBUTES security_ATTRIBUTES = null;
			if (directorySecurity != null)
			{
				security_ATTRIBUTES = new Win32Native.SECURITY_ATTRIBUTES();
				security_ATTRIBUTES.nLength = Marshal.SizeOf<Win32Native.SECURITY_ATTRIBUTES>(security_ATTRIBUTES);
				byte[] securityDescriptorBinaryForm = directorySecurity.GetSecurityDescriptorBinaryForm();
				byte* ptr = stackalloc byte[checked(unchecked((UIntPtr)securityDescriptorBinaryForm.Length) * 1)];
				Buffer.Memcpy(ptr, 0, securityDescriptorBinaryForm, 0, securityDescriptorBinaryForm.Length);
				security_ATTRIBUTES.pSecurityDescriptor = ptr;
			}
			bool flag2 = true;
			int num4 = 0;
			string maybeFullPath = path;
			while (list.Count > 0)
			{
				string text2 = list[list.Count - 1];
				list.RemoveAt(list.Count - 1);
				if (PathInternal.IsDirectoryTooLong(text2))
				{
					throw new PathTooLongException(Environment.GetResourceString("IO.PathTooLong"));
				}
				flag2 = Win32Native.CreateDirectory(text2, security_ATTRIBUTES);
				if (!flag2 && num4 == 0)
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					if (lastWin32Error != 183)
					{
						num4 = lastWin32Error;
					}
					else if (File.InternalExists(text2) || (!Directory.InternalExists(text2, out lastWin32Error) && lastWin32Error == 5))
					{
						num4 = lastWin32Error;
						try
						{
							Directory.CheckPermissions(string.Empty, text2, checkHost, FileSecurityStateAccess.PathDiscovery);
							maybeFullPath = text2;
						}
						catch (SecurityException)
						{
						}
					}
				}
			}
			if (count == 0 && !flag)
			{
				string path2 = Directory.InternalGetDirectoryRoot(fullPath);
				if (!Directory.InternalExists(path2))
				{
					__Error.WinIOError(3, Directory.InternalGetDirectoryRoot(path));
				}
				return;
			}
			if (!flag2 && num4 != 0)
			{
				__Error.WinIOError(num4, maybeFullPath);
			}
		}

		[SecuritySafeCritical]
		public static bool Exists(string path)
		{
			return Directory.InternalExistsHelper(path, true);
		}

		[SecurityCritical]
		internal static bool UnsafeExists(string path)
		{
			return Directory.InternalExistsHelper(path, false);
		}

		[SecurityCritical]
		internal static bool InternalExistsHelper(string path, bool checkHost)
		{
			if (path == null || path.Length == 0)
			{
				return false;
			}
			try
			{
				string fullPathAndCheckPermissions = Directory.GetFullPathAndCheckPermissions(path, checkHost, FileSecurityStateAccess.Read);
				return Directory.InternalExists(fullPathAndCheckPermissions);
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
			int num = 0;
			return Directory.InternalExists(path, out num);
		}

		[SecurityCritical]
		internal static bool InternalExists(string path, out int lastError)
		{
			Win32Native.WIN32_FILE_ATTRIBUTE_DATA win32_FILE_ATTRIBUTE_DATA = default(Win32Native.WIN32_FILE_ATTRIBUTE_DATA);
			lastError = File.FillAttributeInfo(path, ref win32_FILE_ATTRIBUTE_DATA, false, true);
			return lastError == 0 && win32_FILE_ATTRIBUTE_DATA.fileAttributes != -1 && (win32_FILE_ATTRIBUTE_DATA.fileAttributes & 16) != 0;
		}

		public static void SetCreationTime(string path, DateTime creationTime)
		{
			Directory.SetCreationTimeUtc(path, creationTime.ToUniversalTime());
		}

		[SecuritySafeCritical]
		public unsafe static void SetCreationTimeUtc(string path, DateTime creationTimeUtc)
		{
			using (SafeFileHandle safeFileHandle = Directory.OpenHandle(path))
			{
				Win32Native.FILE_TIME file_TIME = new Win32Native.FILE_TIME(creationTimeUtc.ToFileTimeUtc());
				if (!Win32Native.SetFileTime(safeFileHandle, &file_TIME, null, null))
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					__Error.WinIOError(lastWin32Error, path);
				}
			}
		}

		public static DateTime GetCreationTime(string path)
		{
			return File.GetCreationTime(path);
		}

		public static DateTime GetCreationTimeUtc(string path)
		{
			return File.GetCreationTimeUtc(path);
		}

		public static void SetLastWriteTime(string path, DateTime lastWriteTime)
		{
			Directory.SetLastWriteTimeUtc(path, lastWriteTime.ToUniversalTime());
		}

		[SecuritySafeCritical]
		public unsafe static void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
		{
			using (SafeFileHandle safeFileHandle = Directory.OpenHandle(path))
			{
				Win32Native.FILE_TIME file_TIME = new Win32Native.FILE_TIME(lastWriteTimeUtc.ToFileTimeUtc());
				if (!Win32Native.SetFileTime(safeFileHandle, null, null, &file_TIME))
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					__Error.WinIOError(lastWin32Error, path);
				}
			}
		}

		public static DateTime GetLastWriteTime(string path)
		{
			return File.GetLastWriteTime(path);
		}

		public static DateTime GetLastWriteTimeUtc(string path)
		{
			return File.GetLastWriteTimeUtc(path);
		}

		public static void SetLastAccessTime(string path, DateTime lastAccessTime)
		{
			Directory.SetLastAccessTimeUtc(path, lastAccessTime.ToUniversalTime());
		}

		[SecuritySafeCritical]
		public unsafe static void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc)
		{
			using (SafeFileHandle safeFileHandle = Directory.OpenHandle(path))
			{
				Win32Native.FILE_TIME file_TIME = new Win32Native.FILE_TIME(lastAccessTimeUtc.ToFileTimeUtc());
				if (!Win32Native.SetFileTime(safeFileHandle, null, &file_TIME, null))
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					__Error.WinIOError(lastWin32Error, path);
				}
			}
		}

		public static DateTime GetLastAccessTime(string path)
		{
			return File.GetLastAccessTime(path);
		}

		public static DateTime GetLastAccessTimeUtc(string path)
		{
			return File.GetLastAccessTimeUtc(path);
		}

		public static DirectorySecurity GetAccessControl(string path)
		{
			return new DirectorySecurity(path, AccessControlSections.Access | AccessControlSections.Owner | AccessControlSections.Group);
		}

		public static DirectorySecurity GetAccessControl(string path, AccessControlSections includeSections)
		{
			return new DirectorySecurity(path, includeSections);
		}

		[SecuritySafeCritical]
		public static void SetAccessControl(string path, DirectorySecurity directorySecurity)
		{
			if (directorySecurity == null)
			{
				throw new ArgumentNullException("directorySecurity");
			}
			string fullPathInternal = Path.GetFullPathInternal(path);
			directorySecurity.Persist(fullPathInternal);
		}

		public static string[] GetFiles(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			return Directory.InternalGetFiles(path, "*", SearchOption.TopDirectoryOnly);
		}

		public static string[] GetFiles(string path, string searchPattern)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			return Directory.InternalGetFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
		}

		public static string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			if (searchOption != SearchOption.TopDirectoryOnly && searchOption != SearchOption.AllDirectories)
			{
				throw new ArgumentOutOfRangeException("searchOption", Environment.GetResourceString("ArgumentOutOfRange_Enum"));
			}
			return Directory.InternalGetFiles(path, searchPattern, searchOption);
		}

		private static string[] InternalGetFiles(string path, string searchPattern, SearchOption searchOption)
		{
			return Directory.InternalGetFileDirectoryNames(path, path, searchPattern, true, false, searchOption, true);
		}

		[SecurityCritical]
		internal static string[] UnsafeGetFiles(string path, string searchPattern, SearchOption searchOption)
		{
			return Directory.InternalGetFileDirectoryNames(path, path, searchPattern, true, false, searchOption, false);
		}

		public static string[] GetDirectories(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			return Directory.InternalGetDirectories(path, "*", SearchOption.TopDirectoryOnly);
		}

		public static string[] GetDirectories(string path, string searchPattern)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			return Directory.InternalGetDirectories(path, searchPattern, SearchOption.TopDirectoryOnly);
		}

		public static string[] GetDirectories(string path, string searchPattern, SearchOption searchOption)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			if (searchOption != SearchOption.TopDirectoryOnly && searchOption != SearchOption.AllDirectories)
			{
				throw new ArgumentOutOfRangeException("searchOption", Environment.GetResourceString("ArgumentOutOfRange_Enum"));
			}
			return Directory.InternalGetDirectories(path, searchPattern, searchOption);
		}

		private static string[] InternalGetDirectories(string path, string searchPattern, SearchOption searchOption)
		{
			return Directory.InternalGetFileDirectoryNames(path, path, searchPattern, false, true, searchOption, true);
		}

		[SecurityCritical]
		internal static string[] UnsafeGetDirectories(string path, string searchPattern, SearchOption searchOption)
		{
			return Directory.InternalGetFileDirectoryNames(path, path, searchPattern, false, true, searchOption, false);
		}

		public static string[] GetFileSystemEntries(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			return Directory.InternalGetFileSystemEntries(path, "*", SearchOption.TopDirectoryOnly);
		}

		public static string[] GetFileSystemEntries(string path, string searchPattern)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			return Directory.InternalGetFileSystemEntries(path, searchPattern, SearchOption.TopDirectoryOnly);
		}

		public static string[] GetFileSystemEntries(string path, string searchPattern, SearchOption searchOption)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			if (searchOption != SearchOption.TopDirectoryOnly && searchOption != SearchOption.AllDirectories)
			{
				throw new ArgumentOutOfRangeException("searchOption", Environment.GetResourceString("ArgumentOutOfRange_Enum"));
			}
			return Directory.InternalGetFileSystemEntries(path, searchPattern, searchOption);
		}

		private static string[] InternalGetFileSystemEntries(string path, string searchPattern, SearchOption searchOption)
		{
			return Directory.InternalGetFileDirectoryNames(path, path, searchPattern, true, true, searchOption, true);
		}

		internal static string[] InternalGetFileDirectoryNames(string path, string userPathOriginal, string searchPattern, bool includeFiles, bool includeDirs, SearchOption searchOption, bool checkHost)
		{
			IEnumerable<string> collection = FileSystemEnumerableFactory.CreateFileNameIterator(path, userPathOriginal, searchPattern, includeFiles, includeDirs, searchOption, checkHost);
			List<string> list = new List<string>(collection);
			return list.ToArray();
		}

		public static IEnumerable<string> EnumerateDirectories(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			return Directory.InternalEnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly);
		}

		public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			return Directory.InternalEnumerateDirectories(path, searchPattern, SearchOption.TopDirectoryOnly);
		}

		public static IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			if (searchOption != SearchOption.TopDirectoryOnly && searchOption != SearchOption.AllDirectories)
			{
				throw new ArgumentOutOfRangeException("searchOption", Environment.GetResourceString("ArgumentOutOfRange_Enum"));
			}
			return Directory.InternalEnumerateDirectories(path, searchPattern, searchOption);
		}

		private static IEnumerable<string> InternalEnumerateDirectories(string path, string searchPattern, SearchOption searchOption)
		{
			return Directory.EnumerateFileSystemNames(path, searchPattern, searchOption, false, true);
		}

		public static IEnumerable<string> EnumerateFiles(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			return Directory.InternalEnumerateFiles(path, "*", SearchOption.TopDirectoryOnly);
		}

		public static IEnumerable<string> EnumerateFiles(string path, string searchPattern)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			return Directory.InternalEnumerateFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
		}

		public static IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			if (searchOption != SearchOption.TopDirectoryOnly && searchOption != SearchOption.AllDirectories)
			{
				throw new ArgumentOutOfRangeException("searchOption", Environment.GetResourceString("ArgumentOutOfRange_Enum"));
			}
			return Directory.InternalEnumerateFiles(path, searchPattern, searchOption);
		}

		private static IEnumerable<string> InternalEnumerateFiles(string path, string searchPattern, SearchOption searchOption)
		{
			return Directory.EnumerateFileSystemNames(path, searchPattern, searchOption, true, false);
		}

		public static IEnumerable<string> EnumerateFileSystemEntries(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			return Directory.InternalEnumerateFileSystemEntries(path, "*", SearchOption.TopDirectoryOnly);
		}

		public static IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			return Directory.InternalEnumerateFileSystemEntries(path, searchPattern, SearchOption.TopDirectoryOnly);
		}

		public static IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			if (searchOption != SearchOption.TopDirectoryOnly && searchOption != SearchOption.AllDirectories)
			{
				throw new ArgumentOutOfRangeException("searchOption", Environment.GetResourceString("ArgumentOutOfRange_Enum"));
			}
			return Directory.InternalEnumerateFileSystemEntries(path, searchPattern, searchOption);
		}

		private static IEnumerable<string> InternalEnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption)
		{
			return Directory.EnumerateFileSystemNames(path, searchPattern, searchOption, true, true);
		}

		private static IEnumerable<string> EnumerateFileSystemNames(string path, string searchPattern, SearchOption searchOption, bool includeFiles, bool includeDirs)
		{
			return FileSystemEnumerableFactory.CreateFileNameIterator(path, path, searchPattern, includeFiles, includeDirs, searchOption, true);
		}

		[SecuritySafeCritical]
		public static string[] GetLogicalDrives()
		{
			new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
			int logicalDrives = Win32Native.GetLogicalDrives();
			if (logicalDrives == 0)
			{
				__Error.WinIOError();
			}
			uint num = (uint)logicalDrives;
			int num2 = 0;
			while (num != 0U)
			{
				if ((num & 1U) != 0U)
				{
					num2++;
				}
				num >>= 1;
			}
			string[] array = new string[num2];
			char[] array2 = new char[]
			{
				'A',
				':',
				'\\'
			};
			num = (uint)logicalDrives;
			num2 = 0;
			while (num != 0U)
			{
				if ((num & 1U) != 0U)
				{
					array[num2++] = new string(array2);
				}
				num >>= 1;
				char[] array3 = array2;
				int num3 = 0;
				array3[num3] += '\u0001';
			}
			return array;
		}

		[SecuritySafeCritical]
		public static string GetDirectoryRoot(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			string fullPathInternal = Path.GetFullPathInternal(path);
			string text = fullPathInternal.Substring(0, Path.GetRootLength(fullPathInternal));
			Directory.CheckPermissions(path, text, true, FileSecurityStateAccess.PathDiscovery);
			return text;
		}

		internal static string InternalGetDirectoryRoot(string path)
		{
			if (path == null)
			{
				return null;
			}
			return path.Substring(0, Path.GetRootLength(path));
		}

		[SecuritySafeCritical]
		public static string GetCurrentDirectory()
		{
			return Directory.InternalGetCurrentDirectory(true);
		}

		[SecurityCritical]
		internal static string UnsafeGetCurrentDirectory()
		{
			return Directory.InternalGetCurrentDirectory(false);
		}

		[SecuritySafeCritical]
		private static string InternalGetCurrentDirectory(bool checkHost)
		{
			string text = AppContextSwitches.UseLegacyPathHandling ? Directory.LegacyGetCurrentDirectory() : Directory.NewGetCurrentDirectory();
			Directory.CheckPermissions(string.Empty, text, true, FileSecurityStateAccess.PathDiscovery);
			return text;
		}

		[SecurityCritical]
		private static string LegacyGetCurrentDirectory()
		{
			StringBuilder stringBuilder = StringBuilderCache.Acquire(261);
			if (Win32Native.GetCurrentDirectory(stringBuilder.Capacity, stringBuilder) == 0)
			{
				__Error.WinIOError();
			}
			string text = stringBuilder.ToString();
			if (text.IndexOf('~') >= 0)
			{
				int longPathName = Win32Native.GetLongPathName(text, stringBuilder, stringBuilder.Capacity);
				if (longPathName == 0 || longPathName >= 260)
				{
					int num = Marshal.GetLastWin32Error();
					if (longPathName >= 260)
					{
						num = 206;
					}
					if (num != 2 && num != 3 && num != 1 && num != 5)
					{
						__Error.WinIOError(num, string.Empty);
					}
				}
				text = stringBuilder.ToString();
			}
			StringBuilderCache.Release(stringBuilder);
			return text;
		}

		[SecurityCritical]
		private static string NewGetCurrentDirectory()
		{
			string result;
			using (StringBuffer stringBuffer = new StringBuffer(260U))
			{
				uint currentDirectoryW;
				while ((currentDirectoryW = Win32Native.GetCurrentDirectoryW(stringBuffer.CharCapacity, stringBuffer.GetHandle())) > stringBuffer.CharCapacity)
				{
					stringBuffer.EnsureCharCapacity(currentDirectoryW);
				}
				if (currentDirectoryW == 0U)
				{
					__Error.WinIOError();
				}
				stringBuffer.Length = currentDirectoryW;
				if (stringBuffer.Contains('~'))
				{
					result = LongPathHelper.GetLongPathName(stringBuffer);
				}
				else
				{
					result = stringBuffer.ToString();
				}
			}
			return result;
		}

		[SecuritySafeCritical]
		public static void SetCurrentDirectory(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("value");
			}
			if (path.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_PathEmpty"));
			}
			if (PathInternal.IsPathTooLong(path))
			{
				throw new PathTooLongException(Environment.GetResourceString("IO.PathTooLong"));
			}
			new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();
			string fullPathInternal = Path.GetFullPathInternal(path);
			if (!Win32Native.SetCurrentDirectory(fullPathInternal))
			{
				int num = Marshal.GetLastWin32Error();
				if (num == 2)
				{
					num = 3;
				}
				__Error.WinIOError(num, fullPathInternal);
			}
		}

		[SecuritySafeCritical]
		public static void Move(string sourceDirName, string destDirName)
		{
			Directory.InternalMove(sourceDirName, destDirName, true);
		}

		[SecurityCritical]
		internal static void UnsafeMove(string sourceDirName, string destDirName)
		{
			Directory.InternalMove(sourceDirName, destDirName, false);
		}

		[SecurityCritical]
		private static void InternalMove(string sourceDirName, string destDirName, bool checkHost)
		{
			if (sourceDirName == null)
			{
				throw new ArgumentNullException("sourceDirName");
			}
			if (sourceDirName.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyFileName"), "sourceDirName");
			}
			if (destDirName == null)
			{
				throw new ArgumentNullException("destDirName");
			}
			if (destDirName.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyFileName"), "destDirName");
			}
			string fullPathInternal = Path.GetFullPathInternal(sourceDirName);
			string demandDir = Directory.GetDemandDir(fullPathInternal, false);
			if (PathInternal.IsDirectoryTooLong(demandDir))
			{
				throw new PathTooLongException(Environment.GetResourceString("IO.PathTooLong"));
			}
			string fullPathInternal2 = Path.GetFullPathInternal(destDirName);
			string demandDir2 = Directory.GetDemandDir(fullPathInternal2, false);
			if (PathInternal.IsDirectoryTooLong(demandDir))
			{
				throw new PathTooLongException(Environment.GetResourceString("IO.PathTooLong"));
			}
			FileIOPermission.QuickDemand(FileIOPermissionAccess.Read | FileIOPermissionAccess.Write, demandDir, false, false);
			FileIOPermission.QuickDemand(FileIOPermissionAccess.Write, demandDir2, false, false);
			if (string.Compare(demandDir, demandDir2, StringComparison.OrdinalIgnoreCase) == 0)
			{
				throw new IOException(Environment.GetResourceString("IO.IO_SourceDestMustBeDifferent"));
			}
			string pathRoot = Path.GetPathRoot(demandDir);
			string pathRoot2 = Path.GetPathRoot(demandDir2);
			if (string.Compare(pathRoot, pathRoot2, StringComparison.OrdinalIgnoreCase) != 0)
			{
				throw new IOException(Environment.GetResourceString("IO.IO_SourceDestMustHaveSameRoot"));
			}
			if (!Win32Native.MoveFile(sourceDirName, destDirName))
			{
				int num = Marshal.GetLastWin32Error();
				if (num == 2)
				{
					num = 3;
					__Error.WinIOError(num, fullPathInternal);
				}
				if (num == 5)
				{
					throw new IOException(Environment.GetResourceString("UnauthorizedAccess_IODenied_Path", new object[]
					{
						sourceDirName
					}), Win32Native.MakeHRFromErrorCode(num));
				}
				__Error.WinIOError(num, string.Empty);
			}
		}

		[SecuritySafeCritical]
		public static void Delete(string path)
		{
			string fullPathInternal = Path.GetFullPathInternal(path);
			Directory.Delete(fullPathInternal, path, false, true);
		}

		[SecuritySafeCritical]
		public static void Delete(string path, bool recursive)
		{
			string fullPathInternal = Path.GetFullPathInternal(path);
			Directory.Delete(fullPathInternal, path, recursive, true);
		}

		[SecurityCritical]
		internal static void UnsafeDelete(string path, bool recursive)
		{
			string fullPathInternal = Path.GetFullPathInternal(path);
			Directory.Delete(fullPathInternal, path, recursive, false);
		}

		[SecurityCritical]
		internal static void Delete(string fullPath, string userPath, bool recursive, bool checkHost)
		{
			string demandDir = Directory.GetDemandDir(fullPath, !recursive);
			FileIOPermission.QuickDemand(FileIOPermissionAccess.Write, demandDir, false, false);
			Win32Native.WIN32_FILE_ATTRIBUTE_DATA win32_FILE_ATTRIBUTE_DATA = default(Win32Native.WIN32_FILE_ATTRIBUTE_DATA);
			int num = File.FillAttributeInfo(fullPath, ref win32_FILE_ATTRIBUTE_DATA, false, true);
			if (num != 0)
			{
				if (num == 2)
				{
					num = 3;
				}
				__Error.WinIOError(num, fullPath);
			}
			if ((win32_FILE_ATTRIBUTE_DATA.fileAttributes & 1024) != 0)
			{
				recursive = false;
			}
			Win32Native.WIN32_FIND_DATA win32_FIND_DATA = default(Win32Native.WIN32_FIND_DATA);
			Directory.DeleteHelper(fullPath, userPath, recursive, true, ref win32_FIND_DATA);
		}

		[SecurityCritical]
		private static void DeleteHelper(string fullPath, string userPath, bool recursive, bool throwOnTopLevelDirectoryNotFound, ref Win32Native.WIN32_FIND_DATA data)
		{
			Exception ex = null;
			if (recursive)
			{
				int num;
				using (SafeFindHandle safeFindHandle = Win32Native.FindFirstFile(fullPath + "\\*", ref data))
				{
					if (safeFindHandle.IsInvalid)
					{
						num = Marshal.GetLastWin32Error();
						__Error.WinIOError(num, fullPath);
					}
					for (;;)
					{
						bool flag = (data.dwFileAttributes & 16) != 0;
						if (!flag)
						{
							goto IL_12B;
						}
						if (!data.IsRelativeDirectory)
						{
							string cFileName = data.cFileName;
							bool flag2 = (data.dwFileAttributes & 1024) == 0;
							if (flag2)
							{
								string fullPath2 = Path.CombineNoChecks(fullPath, cFileName);
								string userPath2 = Path.CombineNoChecks(userPath, cFileName);
								try
								{
									Directory.DeleteHelper(fullPath2, userPath2, recursive, false, ref data);
									goto IL_163;
								}
								catch (Exception ex2)
								{
									if (ex == null)
									{
										ex = ex2;
									}
									goto IL_163;
								}
							}
							if (data.dwReserved0 == -1610612733)
							{
								string mountPoint = Path.CombineNoChecks(fullPath, cFileName + Path.DirectorySeparatorChar.ToString());
								if (!Win32Native.DeleteVolumeMountPoint(mountPoint))
								{
									num = Marshal.GetLastWin32Error();
									if (num != 3)
									{
										try
										{
											__Error.WinIOError(num, cFileName);
										}
										catch (Exception ex3)
										{
											if (ex == null)
											{
												ex = ex3;
											}
										}
									}
								}
							}
							string path = Path.CombineNoChecks(fullPath, cFileName);
							if (!Win32Native.RemoveDirectory(path))
							{
								num = Marshal.GetLastWin32Error();
								if (num != 3)
								{
									try
									{
										__Error.WinIOError(num, cFileName);
										goto IL_163;
									}
									catch (Exception ex4)
									{
										if (ex == null)
										{
											ex = ex4;
										}
										goto IL_163;
									}
									goto IL_12B;
								}
							}
						}
						IL_163:
						if (!Win32Native.FindNextFile(safeFindHandle, ref data))
						{
							break;
						}
						continue;
						IL_12B:
						string cFileName2 = data.cFileName;
						if (Win32Native.DeleteFile(Path.CombineNoChecks(fullPath, cFileName2)))
						{
							goto IL_163;
						}
						num = Marshal.GetLastWin32Error();
						if (num != 2)
						{
							try
							{
								__Error.WinIOError(num, cFileName2);
							}
							catch (Exception ex5)
							{
								if (ex == null)
								{
									ex = ex5;
								}
							}
							goto IL_163;
						}
						goto IL_163;
					}
					num = Marshal.GetLastWin32Error();
				}
				if (ex != null)
				{
					throw ex;
				}
				if (num != 0 && num != 18)
				{
					__Error.WinIOError(num, userPath);
				}
			}
			if (!Win32Native.RemoveDirectory(fullPath))
			{
				int num = Marshal.GetLastWin32Error();
				if (num == 2)
				{
					num = 3;
				}
				if (num == 5)
				{
					throw new IOException(Environment.GetResourceString("UnauthorizedAccess_IODenied_Path", new object[]
					{
						userPath
					}));
				}
				if (num == 3 && !throwOnTopLevelDirectoryNotFound)
				{
					return;
				}
				__Error.WinIOError(num, fullPath);
			}
		}

		[SecurityCritical]
		private static SafeFileHandle OpenHandle(string path)
		{
			string fullPathInternal = Path.GetFullPathInternal(path);
			string pathRoot = Path.GetPathRoot(fullPathInternal);
			if (pathRoot == fullPathInternal && pathRoot[1] == Path.VolumeSeparatorChar)
			{
				throw new ArgumentException(Environment.GetResourceString("Arg_PathIsVolume"));
			}
			FileIOPermission.QuickDemand(FileIOPermissionAccess.Write, Directory.GetDemandDir(fullPathInternal, true), false, false);
			SafeFileHandle safeFileHandle = Win32Native.SafeCreateFile(fullPathInternal, 1073741824, FileShare.Write | FileShare.Delete, null, FileMode.Open, 33554432, IntPtr.Zero);
			if (safeFileHandle.IsInvalid)
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				__Error.WinIOError(lastWin32Error, fullPathInternal);
			}
			return safeFileHandle;
		}

		private const int FILE_ATTRIBUTE_DIRECTORY = 16;

		private const int GENERIC_WRITE = 1073741824;

		private const int FILE_SHARE_WRITE = 2;

		private const int FILE_SHARE_DELETE = 4;

		private const int OPEN_EXISTING = 3;

		private const int FILE_FLAG_BACKUP_SEMANTICS = 33554432;

		internal sealed class SearchData
		{
			public SearchData(string fullPath, string userPath, SearchOption searchOption)
			{
				this.fullPath = fullPath;
				this.userPath = userPath;
				this.searchOption = searchOption;
			}

			public readonly string fullPath;

			public readonly string userPath;

			public readonly SearchOption searchOption;
		}
	}
}
