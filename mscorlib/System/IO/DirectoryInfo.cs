using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;
using System.Security.AccessControl;
using System.Security.Permissions;
using Microsoft.Win32;

namespace System.IO
{
	[ComVisible(true)]
	[Serializable]
	public sealed class DirectoryInfo : FileSystemInfo
	{
		[SecuritySafeCritical]
		public DirectoryInfo(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			this.Init(path, true);
		}

		[SecurityCritical]
		private void Init(string path, bool checkHost)
		{
			if (path.Length == 2 && path[1] == ':')
			{
				this.OriginalPath = ".";
			}
			else
			{
				this.OriginalPath = path;
			}
			string fullPathAndCheckPermissions = Directory.GetFullPathAndCheckPermissions(path, checkHost, FileSecurityStateAccess.Read);
			this.FullPath = fullPathAndCheckPermissions;
			base.DisplayPath = DirectoryInfo.GetDisplayName(this.OriginalPath, this.FullPath);
		}

		internal DirectoryInfo(string fullPath, bool junk)
		{
			this.OriginalPath = Path.GetFileName(fullPath);
			this.FullPath = fullPath;
			base.DisplayPath = DirectoryInfo.GetDisplayName(this.OriginalPath, this.FullPath);
		}

		internal DirectoryInfo(string fullPath, string fileName)
		{
			this.OriginalPath = fileName;
			this.FullPath = fullPath;
			base.DisplayPath = DirectoryInfo.GetDisplayName(this.OriginalPath, this.FullPath);
		}

		[SecurityCritical]
		private DirectoryInfo(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			Directory.CheckPermissions(string.Empty, this.FullPath, false, FileSecurityStateAccess.Read);
			base.DisplayPath = DirectoryInfo.GetDisplayName(this.OriginalPath, this.FullPath);
		}

		public override string Name
		{
			get
			{
				return DirectoryInfo.GetDirName(this.FullPath);
			}
		}

		public override string FullName
		{
			[SecuritySafeCritical]
			get
			{
				Directory.CheckPermissions(string.Empty, this.FullPath, true, FileSecurityStateAccess.PathDiscovery);
				return this.FullPath;
			}
		}

		internal override string UnsafeGetFullName
		{
			[SecurityCritical]
			get
			{
				Directory.CheckPermissions(string.Empty, this.FullPath, false, FileSecurityStateAccess.PathDiscovery);
				return this.FullPath;
			}
		}

		public DirectoryInfo Parent
		{
			[SecuritySafeCritical]
			get
			{
				string text = this.FullPath;
				if (text.Length > 3 && text.EndsWith(Path.DirectorySeparatorChar))
				{
					text = this.FullPath.Substring(0, this.FullPath.Length - 1);
				}
				string directoryName = Path.GetDirectoryName(text);
				if (directoryName == null)
				{
					return null;
				}
				DirectoryInfo directoryInfo = new DirectoryInfo(directoryName, false);
				Directory.CheckPermissions(string.Empty, directoryInfo.FullPath, true, FileSecurityStateAccess.Read | FileSecurityStateAccess.PathDiscovery);
				return directoryInfo;
			}
		}

		public DirectoryInfo CreateSubdirectory(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			return this.CreateSubdirectory(path, null);
		}

		[SecuritySafeCritical]
		public DirectoryInfo CreateSubdirectory(string path, DirectorySecurity directorySecurity)
		{
			return this.CreateSubdirectoryHelper(path, directorySecurity);
		}

		[SecurityCritical]
		private DirectoryInfo CreateSubdirectoryHelper(string path, object directorySecurity)
		{
			string path2 = Path.InternalCombine(this.FullPath, path);
			string fullPathInternal = Path.GetFullPathInternal(path2);
			if (string.Compare(this.FullPath, 0, fullPathInternal, 0, this.FullPath.Length, StringComparison.OrdinalIgnoreCase) != 0)
			{
				string displayablePath = __Error.GetDisplayablePath(base.DisplayPath, false);
				throw new ArgumentException(Environment.GetResourceString("Argument_InvalidSubPath", new object[]
				{
					path,
					displayablePath
				}));
			}
			string fullPath = Directory.GetDemandDir(fullPathInternal, true);
			FileIOPermission.QuickDemand(FileIOPermissionAccess.Write, fullPath, false, false);
			Directory.InternalCreateDirectory(fullPathInternal, path, directorySecurity);
			return new DirectoryInfo(fullPathInternal);
		}

		public void Create()
		{
			Directory.InternalCreateDirectory(this.FullPath, this.OriginalPath, null, true);
		}

		public void Create(DirectorySecurity directorySecurity)
		{
			Directory.InternalCreateDirectory(this.FullPath, this.OriginalPath, directorySecurity, true);
		}

		public override bool Exists
		{
			[SecuritySafeCritical]
			get
			{
				bool result;
				try
				{
					if (this._dataInitialised == -1)
					{
						base.Refresh();
					}
					if (this._dataInitialised != 0)
					{
						result = false;
					}
					else
					{
						result = (this._data.fileAttributes != -1 && (this._data.fileAttributes & 16) != 0);
					}
				}
				catch
				{
					result = false;
				}
				return result;
			}
		}

		public DirectorySecurity GetAccessControl()
		{
			return Directory.GetAccessControl(this.FullPath, AccessControlSections.Access | AccessControlSections.Owner | AccessControlSections.Group);
		}

		public DirectorySecurity GetAccessControl(AccessControlSections includeSections)
		{
			return Directory.GetAccessControl(this.FullPath, includeSections);
		}

		public void SetAccessControl(DirectorySecurity directorySecurity)
		{
			Directory.SetAccessControl(this.FullPath, directorySecurity);
		}

		public FileInfo[] GetFiles(string searchPattern)
		{
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			return this.InternalGetFiles(searchPattern, SearchOption.TopDirectoryOnly);
		}

		public FileInfo[] GetFiles(string searchPattern, SearchOption searchOption)
		{
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			if (searchOption != SearchOption.TopDirectoryOnly && searchOption != SearchOption.AllDirectories)
			{
				throw new ArgumentOutOfRangeException("searchOption", Environment.GetResourceString("ArgumentOutOfRange_Enum"));
			}
			return this.InternalGetFiles(searchPattern, searchOption);
		}

		private FileInfo[] InternalGetFiles(string searchPattern, SearchOption searchOption)
		{
			IEnumerable<FileInfo> collection = FileSystemEnumerableFactory.CreateFileInfoIterator(this.FullPath, this.OriginalPath, searchPattern, searchOption);
			List<FileInfo> list = new List<FileInfo>(collection);
			return list.ToArray();
		}

		public FileInfo[] GetFiles()
		{
			return this.InternalGetFiles("*", SearchOption.TopDirectoryOnly);
		}

		public DirectoryInfo[] GetDirectories()
		{
			return this.InternalGetDirectories("*", SearchOption.TopDirectoryOnly);
		}

		public FileSystemInfo[] GetFileSystemInfos(string searchPattern)
		{
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			return this.InternalGetFileSystemInfos(searchPattern, SearchOption.TopDirectoryOnly);
		}

		public FileSystemInfo[] GetFileSystemInfos(string searchPattern, SearchOption searchOption)
		{
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			if (searchOption != SearchOption.TopDirectoryOnly && searchOption != SearchOption.AllDirectories)
			{
				throw new ArgumentOutOfRangeException("searchOption", Environment.GetResourceString("ArgumentOutOfRange_Enum"));
			}
			return this.InternalGetFileSystemInfos(searchPattern, searchOption);
		}

		private FileSystemInfo[] InternalGetFileSystemInfos(string searchPattern, SearchOption searchOption)
		{
			IEnumerable<FileSystemInfo> collection = FileSystemEnumerableFactory.CreateFileSystemInfoIterator(this.FullPath, this.OriginalPath, searchPattern, searchOption);
			List<FileSystemInfo> list = new List<FileSystemInfo>(collection);
			return list.ToArray();
		}

		public FileSystemInfo[] GetFileSystemInfos()
		{
			return this.InternalGetFileSystemInfos("*", SearchOption.TopDirectoryOnly);
		}

		public DirectoryInfo[] GetDirectories(string searchPattern)
		{
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			return this.InternalGetDirectories(searchPattern, SearchOption.TopDirectoryOnly);
		}

		public DirectoryInfo[] GetDirectories(string searchPattern, SearchOption searchOption)
		{
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			if (searchOption != SearchOption.TopDirectoryOnly && searchOption != SearchOption.AllDirectories)
			{
				throw new ArgumentOutOfRangeException("searchOption", Environment.GetResourceString("ArgumentOutOfRange_Enum"));
			}
			return this.InternalGetDirectories(searchPattern, searchOption);
		}

		private DirectoryInfo[] InternalGetDirectories(string searchPattern, SearchOption searchOption)
		{
			IEnumerable<DirectoryInfo> collection = FileSystemEnumerableFactory.CreateDirectoryInfoIterator(this.FullPath, this.OriginalPath, searchPattern, searchOption);
			List<DirectoryInfo> list = new List<DirectoryInfo>(collection);
			return list.ToArray();
		}

		public IEnumerable<DirectoryInfo> EnumerateDirectories()
		{
			return this.InternalEnumerateDirectories("*", SearchOption.TopDirectoryOnly);
		}

		public IEnumerable<DirectoryInfo> EnumerateDirectories(string searchPattern)
		{
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			return this.InternalEnumerateDirectories(searchPattern, SearchOption.TopDirectoryOnly);
		}

		public IEnumerable<DirectoryInfo> EnumerateDirectories(string searchPattern, SearchOption searchOption)
		{
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			if (searchOption != SearchOption.TopDirectoryOnly && searchOption != SearchOption.AllDirectories)
			{
				throw new ArgumentOutOfRangeException("searchOption", Environment.GetResourceString("ArgumentOutOfRange_Enum"));
			}
			return this.InternalEnumerateDirectories(searchPattern, searchOption);
		}

		private IEnumerable<DirectoryInfo> InternalEnumerateDirectories(string searchPattern, SearchOption searchOption)
		{
			return FileSystemEnumerableFactory.CreateDirectoryInfoIterator(this.FullPath, this.OriginalPath, searchPattern, searchOption);
		}

		public IEnumerable<FileInfo> EnumerateFiles()
		{
			return this.InternalEnumerateFiles("*", SearchOption.TopDirectoryOnly);
		}

		public IEnumerable<FileInfo> EnumerateFiles(string searchPattern)
		{
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			return this.InternalEnumerateFiles(searchPattern, SearchOption.TopDirectoryOnly);
		}

		public IEnumerable<FileInfo> EnumerateFiles(string searchPattern, SearchOption searchOption)
		{
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			if (searchOption != SearchOption.TopDirectoryOnly && searchOption != SearchOption.AllDirectories)
			{
				throw new ArgumentOutOfRangeException("searchOption", Environment.GetResourceString("ArgumentOutOfRange_Enum"));
			}
			return this.InternalEnumerateFiles(searchPattern, searchOption);
		}

		private IEnumerable<FileInfo> InternalEnumerateFiles(string searchPattern, SearchOption searchOption)
		{
			return FileSystemEnumerableFactory.CreateFileInfoIterator(this.FullPath, this.OriginalPath, searchPattern, searchOption);
		}

		public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos()
		{
			return this.InternalEnumerateFileSystemInfos("*", SearchOption.TopDirectoryOnly);
		}

		public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(string searchPattern)
		{
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			return this.InternalEnumerateFileSystemInfos(searchPattern, SearchOption.TopDirectoryOnly);
		}

		public IEnumerable<FileSystemInfo> EnumerateFileSystemInfos(string searchPattern, SearchOption searchOption)
		{
			if (searchPattern == null)
			{
				throw new ArgumentNullException("searchPattern");
			}
			if (searchOption != SearchOption.TopDirectoryOnly && searchOption != SearchOption.AllDirectories)
			{
				throw new ArgumentOutOfRangeException("searchOption", Environment.GetResourceString("ArgumentOutOfRange_Enum"));
			}
			return this.InternalEnumerateFileSystemInfos(searchPattern, searchOption);
		}

		private IEnumerable<FileSystemInfo> InternalEnumerateFileSystemInfos(string searchPattern, SearchOption searchOption)
		{
			return FileSystemEnumerableFactory.CreateFileSystemInfoIterator(this.FullPath, this.OriginalPath, searchPattern, searchOption);
		}

		public DirectoryInfo Root
		{
			[SecuritySafeCritical]
			get
			{
				int rootLength = Path.GetRootLength(this.FullPath);
				string text = this.FullPath.Substring(0, rootLength);
				string fullPath = Directory.GetDemandDir(text, true);
				FileIOPermission.QuickDemand(FileIOPermissionAccess.PathDiscovery, fullPath, false, false);
				return new DirectoryInfo(text);
			}
		}

		[SecuritySafeCritical]
		public void MoveTo(string destDirName)
		{
			if (destDirName == null)
			{
				throw new ArgumentNullException("destDirName");
			}
			if (destDirName.Length == 0)
			{
				throw new ArgumentException(Environment.GetResourceString("Argument_EmptyFileName"), "destDirName");
			}
			Directory.CheckPermissions(base.DisplayPath, this.FullPath, true, FileSecurityStateAccess.Read | FileSecurityStateAccess.Write);
			string text = Path.GetFullPathInternal(destDirName);
			if (!text.EndsWith(Path.DirectorySeparatorChar))
			{
				text += Path.DirectorySeparatorChar.ToString();
			}
			Directory.CheckPermissions(destDirName, text, true, FileSecurityStateAccess.Read | FileSecurityStateAccess.Write);
			string text2;
			if (this.FullPath.EndsWith(Path.DirectorySeparatorChar))
			{
				text2 = this.FullPath;
			}
			else
			{
				text2 = this.FullPath + Path.DirectorySeparatorChar.ToString();
			}
			if (string.Compare(text2, text, StringComparison.OrdinalIgnoreCase) == 0)
			{
				throw new IOException(Environment.GetResourceString("IO.IO_SourceDestMustBeDifferent"));
			}
			string pathRoot = Path.GetPathRoot(text2);
			string pathRoot2 = Path.GetPathRoot(text);
			if (string.Compare(pathRoot, pathRoot2, StringComparison.OrdinalIgnoreCase) != 0)
			{
				throw new IOException(Environment.GetResourceString("IO.IO_SourceDestMustHaveSameRoot"));
			}
			if (!Win32Native.MoveFile(this.FullPath, destDirName))
			{
				int num = Marshal.GetLastWin32Error();
				if (num == 2)
				{
					num = 3;
					__Error.WinIOError(num, base.DisplayPath);
				}
				if (num == 5)
				{
					throw new IOException(Environment.GetResourceString("UnauthorizedAccess_IODenied_Path", new object[]
					{
						base.DisplayPath
					}));
				}
				__Error.WinIOError(num, string.Empty);
			}
			this.FullPath = text;
			this.OriginalPath = destDirName;
			base.DisplayPath = DirectoryInfo.GetDisplayName(this.OriginalPath, this.FullPath);
			this._dataInitialised = -1;
		}

		[SecuritySafeCritical]
		public override void Delete()
		{
			Directory.Delete(this.FullPath, this.OriginalPath, false, true);
		}

		[SecuritySafeCritical]
		public void Delete(bool recursive)
		{
			Directory.Delete(this.FullPath, this.OriginalPath, recursive, true);
		}

		public override string ToString()
		{
			return base.DisplayPath;
		}

		private static string GetDisplayName(string originalPath, string fullPath)
		{
			string result;
			if (originalPath.Length == 2 && originalPath[1] == ':')
			{
				result = ".";
			}
			else
			{
				result = originalPath;
			}
			return result;
		}

		private static string GetDirName(string fullPath)
		{
			string result;
			if (fullPath.Length > 3)
			{
				string path = fullPath;
				if (fullPath.EndsWith(Path.DirectorySeparatorChar))
				{
					path = fullPath.Substring(0, fullPath.Length - 1);
				}
				result = Path.GetFileName(path);
			}
			else
			{
				result = fullPath;
			}
			return result;
		}

		private string[] demandDir;
	}
}
