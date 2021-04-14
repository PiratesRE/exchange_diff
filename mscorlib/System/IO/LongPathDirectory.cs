using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Security.Permissions;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace System.IO
{
	[ComVisible(false)]
	internal static class LongPathDirectory
	{
		[SecurityCritical]
		internal static void CreateDirectory(string path)
		{
			string fullPath = LongPath.NormalizePath(path);
			string demandDir = LongPathDirectory.GetDemandDir(fullPath, true);
			FileIOPermission.QuickDemand(FileIOPermissionAccess.Read, demandDir, false, false);
			LongPathDirectory.InternalCreateDirectory(fullPath, path, null);
		}

		[SecurityCritical]
		private unsafe static void InternalCreateDirectory(string fullPath, string path, object dirSecurityObj)
		{
			DirectorySecurity directorySecurity = (DirectorySecurity)dirSecurityObj;
			int num = fullPath.Length;
			if (num >= 2 && Path.IsDirectorySeparator(fullPath[num - 1]))
			{
				num--;
			}
			int rootLength = LongPath.GetRootLength(fullPath);
			if (num == 2 && Path.IsDirectorySeparator(fullPath[1]))
			{
				throw new IOException(Environment.GetResourceString("IO.IO_CannotCreateDirectory", new object[]
				{
					path
				}));
			}
			List<string> list = new List<string>();
			bool flag = false;
			if (num > rootLength)
			{
				int num2 = num - 1;
				while (num2 >= rootLength && !flag)
				{
					string text = fullPath.Substring(0, num2 + 1);
					if (!LongPathDirectory.InternalExists(text))
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
				if (text2.Length >= 32767)
				{
					throw new PathTooLongException(Environment.GetResourceString("IO.PathTooLong"));
				}
				flag2 = Win32Native.CreateDirectory(PathInternal.EnsureExtendedPrefix(text2), security_ATTRIBUTES);
				if (!flag2 && num4 == 0)
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					if (lastWin32Error != 183)
					{
						num4 = lastWin32Error;
					}
					else if (LongPathFile.InternalExists(text2) || (!LongPathDirectory.InternalExists(text2, out lastWin32Error) && lastWin32Error == 5))
					{
						num4 = lastWin32Error;
						try
						{
							FileIOPermission.QuickDemand(FileIOPermissionAccess.PathDiscovery, LongPathDirectory.GetDemandDir(text2, true), false, false);
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
				string path2 = LongPathDirectory.InternalGetDirectoryRoot(fullPath);
				if (!LongPathDirectory.InternalExists(path2))
				{
					__Error.WinIOError(3, LongPathDirectory.InternalGetDirectoryRoot(path));
				}
				return;
			}
			if (!flag2 && num4 != 0)
			{
				__Error.WinIOError(num4, maybeFullPath);
			}
		}

		[SecurityCritical]
		internal static void Move(string sourceDirName, string destDirName)
		{
			string text = LongPath.NormalizePath(sourceDirName);
			string demandDir = LongPathDirectory.GetDemandDir(text, false);
			if (demandDir.Length >= 32767)
			{
				throw new PathTooLongException(Environment.GetResourceString("IO.PathTooLong"));
			}
			string fullPath = LongPath.NormalizePath(destDirName);
			string demandDir2 = LongPathDirectory.GetDemandDir(fullPath, false);
			if (demandDir2.Length >= 32767)
			{
				throw new PathTooLongException(Environment.GetResourceString("IO.PathTooLong"));
			}
			FileIOPermission.QuickDemand(FileIOPermissionAccess.Read | FileIOPermissionAccess.Write, demandDir, false, false);
			FileIOPermission.QuickDemand(FileIOPermissionAccess.Write, demandDir2, false, false);
			if (string.Compare(demandDir, demandDir2, StringComparison.OrdinalIgnoreCase) == 0)
			{
				throw new IOException(Environment.GetResourceString("IO.IO_SourceDestMustBeDifferent"));
			}
			string pathRoot = LongPath.GetPathRoot(demandDir);
			string pathRoot2 = LongPath.GetPathRoot(demandDir2);
			if (string.Compare(pathRoot, pathRoot2, StringComparison.OrdinalIgnoreCase) != 0)
			{
				throw new IOException(Environment.GetResourceString("IO.IO_SourceDestMustHaveSameRoot"));
			}
			string src = PathInternal.EnsureExtendedPrefix(sourceDirName);
			string dst = PathInternal.EnsureExtendedPrefix(destDirName);
			if (!Win32Native.MoveFile(src, dst))
			{
				int num = Marshal.GetLastWin32Error();
				if (num == 2)
				{
					num = 3;
					__Error.WinIOError(num, text);
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

		[SecurityCritical]
		internal static void Delete(string path, bool recursive)
		{
			string fullPath = LongPath.NormalizePath(path);
			LongPathDirectory.InternalDelete(fullPath, path, recursive);
		}

		[SecurityCritical]
		private static void InternalDelete(string fullPath, string userPath, bool recursive)
		{
			string demandDir = LongPathDirectory.GetDemandDir(fullPath, !recursive);
			FileIOPermission.QuickDemand(FileIOPermissionAccess.Write, demandDir, false, false);
			string text = Path.AddLongPathPrefix(fullPath);
			Win32Native.WIN32_FILE_ATTRIBUTE_DATA win32_FILE_ATTRIBUTE_DATA = default(Win32Native.WIN32_FILE_ATTRIBUTE_DATA);
			int num = File.FillAttributeInfo(text, ref win32_FILE_ATTRIBUTE_DATA, false, true);
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
			LongPathDirectory.DeleteHelper(text, userPath, recursive, true);
		}

		[SecurityCritical]
		private static void DeleteHelper(string fullPath, string userPath, bool recursive, bool throwOnTopLevelDirectoryNotFound)
		{
			Exception ex = null;
			if (recursive)
			{
				Win32Native.WIN32_FIND_DATA win32_FIND_DATA = default(Win32Native.WIN32_FIND_DATA);
				string fileName;
				if (fullPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
				{
					fileName = fullPath + "*";
				}
				else
				{
					fileName = fullPath + Path.DirectorySeparatorChar.ToString() + "*";
				}
				int num;
				using (SafeFindHandle safeFindHandle = Win32Native.FindFirstFile(fileName, ref win32_FIND_DATA))
				{
					if (safeFindHandle.IsInvalid)
					{
						num = Marshal.GetLastWin32Error();
						__Error.WinIOError(num, userPath);
					}
					for (;;)
					{
						bool flag = (win32_FIND_DATA.dwFileAttributes & 16) != 0;
						if (!flag)
						{
							goto IL_180;
						}
						if (!win32_FIND_DATA.IsRelativeDirectory)
						{
							bool flag2 = (win32_FIND_DATA.dwFileAttributes & 1024) == 0;
							if (flag2)
							{
								string fullPath2 = LongPath.InternalCombine(fullPath, win32_FIND_DATA.cFileName);
								string userPath2 = LongPath.InternalCombine(userPath, win32_FIND_DATA.cFileName);
								try
								{
									LongPathDirectory.DeleteHelper(fullPath2, userPath2, recursive, false);
									goto IL_1BD;
								}
								catch (Exception ex2)
								{
									if (ex == null)
									{
										ex = ex2;
									}
									goto IL_1BD;
								}
							}
							if (win32_FIND_DATA.dwReserved0 == -1610612733)
							{
								string mountPoint = LongPath.InternalCombine(fullPath, win32_FIND_DATA.cFileName + Path.DirectorySeparatorChar.ToString());
								if (!Win32Native.DeleteVolumeMountPoint(mountPoint))
								{
									num = Marshal.GetLastWin32Error();
									if (num != 3)
									{
										try
										{
											__Error.WinIOError(num, win32_FIND_DATA.cFileName);
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
							string path = LongPath.InternalCombine(fullPath, win32_FIND_DATA.cFileName);
							if (!Win32Native.RemoveDirectory(path))
							{
								num = Marshal.GetLastWin32Error();
								if (num != 3)
								{
									try
									{
										__Error.WinIOError(num, win32_FIND_DATA.cFileName);
										goto IL_1BD;
									}
									catch (Exception ex4)
									{
										if (ex == null)
										{
											ex = ex4;
										}
										goto IL_1BD;
									}
									goto IL_180;
								}
							}
						}
						IL_1BD:
						if (!Win32Native.FindNextFile(safeFindHandle, ref win32_FIND_DATA))
						{
							break;
						}
						continue;
						IL_180:
						string path2 = LongPath.InternalCombine(fullPath, win32_FIND_DATA.cFileName);
						if (Win32Native.DeleteFile(path2))
						{
							goto IL_1BD;
						}
						num = Marshal.GetLastWin32Error();
						if (num != 2)
						{
							try
							{
								__Error.WinIOError(num, win32_FIND_DATA.cFileName);
							}
							catch (Exception ex5)
							{
								if (ex == null)
								{
									ex = ex5;
								}
							}
							goto IL_1BD;
						}
						goto IL_1BD;
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
				__Error.WinIOError(num, userPath);
			}
		}

		[SecurityCritical]
		internal static bool Exists(string path)
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
				string text = LongPath.NormalizePath(path);
				string demandDir = LongPathDirectory.GetDemandDir(text, true);
				FileIOPermission.QuickDemand(FileIOPermissionAccess.Read, demandDir, false, false);
				return LongPathDirectory.InternalExists(text);
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
			return LongPathDirectory.InternalExists(path, out num);
		}

		[SecurityCritical]
		internal static bool InternalExists(string path, out int lastError)
		{
			string path2 = Path.AddLongPathPrefix(path);
			return Directory.InternalExists(path2, out lastError);
		}

		private static string GetDemandDir(string fullPath, bool thisDirOnly)
		{
			fullPath = Path.RemoveLongPathPrefix(fullPath);
			string result;
			if (thisDirOnly)
			{
				if (fullPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal) || fullPath.EndsWith(Path.AltDirectorySeparatorChar.ToString(), StringComparison.Ordinal))
				{
					result = fullPath + ".";
				}
				else
				{
					result = fullPath + Path.DirectorySeparatorChar.ToString() + ".";
				}
			}
			else if (!fullPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal) && !fullPath.EndsWith(Path.AltDirectorySeparatorChar.ToString(), StringComparison.Ordinal))
			{
				result = fullPath + Path.DirectorySeparatorChar.ToString();
			}
			else
			{
				result = fullPath;
			}
			return result;
		}

		private static string InternalGetDirectoryRoot(string path)
		{
			if (path == null)
			{
				return null;
			}
			return path.Substring(0, LongPath.GetRootLength(path));
		}
	}
}
