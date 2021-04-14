using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Cluster.Replay.IO
{
	internal class DirectoryEnumerator : DisposeTrackableBase
	{
		public DirectoryEnumerator(DirectoryInfo path, bool recurse = false, bool includeRecycleBin = false)
		{
			this.m_path = path;
			this.m_recurse = recurse;
			this.m_includeRecycleBin = includeRecycleBin;
			this.m_realDirectories = new List<string>();
		}

		public bool ReturnBaseNames { get; set; }

		public IEnumerable<string> EnumerateFiles(string filter, Predicate<NativeMethods.WIN32_FIND_DATA> extendedFilter = null)
		{
			if (!string.Equals(filter, "*"))
			{
				if (this.m_recurse)
				{
					bool returnBaseNames = this.ReturnBaseNames;
					try
					{
						this.ReturnBaseNames = false;
						IEnumerable<string> directories = this.EnumerateDirectories("*", extendedFilter);
						foreach (string dir in directories)
						{
							using (DirectoryEnumerator dirEnum = new DirectoryEnumerator(new DirectoryInfo(dir), false, this.m_includeRecycleBin))
							{
								dirEnum.ReturnBaseNames = returnBaseNames;
								foreach (string item in dirEnum.GetItemsInternal(filter, DirectoryEnumerator.EnumerationType.Files, extendedFilter))
								{
									yield return item;
								}
							}
						}
					}
					finally
					{
						this.ReturnBaseNames = returnBaseNames;
					}
					goto IL_1CD;
				}
			}
			string itemName;
			NativeMethods.WIN32_FIND_DATA findData;
			while (this.GetNextItem(filter, DirectoryEnumerator.EnumerationType.Files, out itemName, out findData, extendedFilter))
			{
				yield return itemName;
			}
			IL_1CD:
			yield break;
		}

		public IEnumerable<string> EnumerateDirectories(string filter, Predicate<NativeMethods.WIN32_FIND_DATA> extendedFilter = null)
		{
			return this.GetItemsInternal(filter, DirectoryEnumerator.EnumerationType.Directories, extendedFilter);
		}

		public IEnumerable<string> EnumerateFilesAndDirectories(string filter, Predicate<NativeMethods.WIN32_FIND_DATA> extendedFilter = null)
		{
			return this.GetItemsInternal(filter, DirectoryEnumerator.EnumerationType.FilesOrDirectories, extendedFilter);
		}

		public IEnumerable<string> EnumerateFilesAndDirectoriesExcludingHiddenAndSystem(string filter)
		{
			return this.GetItemsInternal(filter, DirectoryEnumerator.EnumerationType.FilesOrDirectories, DirectoryEnumerator.ExcludeHiddenAndSystemFilter);
		}

		public bool GetNextFile(string filter, out string fileName)
		{
			return this.GetNextItem(filter, DirectoryEnumerator.EnumerationType.Files, out fileName);
		}

		public bool GetNextDirectory(string filter, out string directoryName)
		{
			return this.GetNextItem(filter, DirectoryEnumerator.EnumerationType.Directories, out directoryName);
		}

		public bool GetNextFileExtendedInfo(string filter, out string fileName, out NativeMethods.WIN32_FIND_DATA findData)
		{
			return this.GetNextItem(filter, DirectoryEnumerator.EnumerationType.Files, out fileName, out findData, null);
		}

		public bool GetNextDirectoryExtendedInfo(string filter, out string directoryName, out NativeMethods.WIN32_FIND_DATA findData)
		{
			return this.GetNextItem(filter, DirectoryEnumerator.EnumerationType.Directories, out directoryName, out findData, null);
		}

		protected virtual LocalizedString GetIOExceptionMessage(string directoryName, string apiName, string ioErrorMessage, int win32ErrorCode)
		{
			return ReplayStrings.DirectoryEnumeratorIOError(apiName, ioErrorMessage, win32ErrorCode, directoryName);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				this.ResetFindHandle();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<DirectoryEnumerator>(this);
		}

		private IEnumerable<string> GetItemsInternal(string filter, DirectoryEnumerator.EnumerationType enumType, Predicate<NativeMethods.WIN32_FIND_DATA> extendedFilter = null)
		{
			string itemName;
			NativeMethods.WIN32_FIND_DATA findData;
			while (this.GetNextItem(filter, enumType, out itemName, out findData, extendedFilter))
			{
				yield return itemName;
			}
			if (this.m_recurse)
			{
				foreach (string dir in this.m_realDirectories)
				{
					string fullPath = Path.Combine(this.m_path.FullName, dir);
					using (DirectoryEnumerator dirEnum = new DirectoryEnumerator(new DirectoryInfo(fullPath), this.m_recurse, this.m_includeRecycleBin))
					{
						foreach (string item in dirEnum.GetItemsInternal(filter, enumType, extendedFilter))
						{
							yield return item;
						}
					}
				}
			}
			yield break;
		}

		private bool GetNextItem(string filter, DirectoryEnumerator.EnumerationType enumType, out string itemName)
		{
			NativeMethods.WIN32_FIND_DATA win32_FIND_DATA;
			return this.GetNextItem(filter, enumType, out itemName, out win32_FIND_DATA, null);
		}

		private bool GetNextItem(string filter, DirectoryEnumerator.EnumerationType enumType, out string itemName, out NativeMethods.WIN32_FIND_DATA findData, Predicate<NativeMethods.WIN32_FIND_DATA> extendedFilter = null)
		{
			bool flag = false;
			bool flag2 = false;
			itemName = null;
			findData = default(NativeMethods.WIN32_FIND_DATA);
			findData.FileName = string.Empty;
			findData.FileSizeHigh = 0U;
			findData.FileSizeLow = 0U;
			bool flag3;
			do
			{
				if (this.m_safeFindHandle == null)
				{
					string fileName = Path.Combine(this.m_path.FullName, filter);
					this.m_safeFindHandle = NativeMethods.FindFirstFile(fileName, out findData);
					if (this.m_safeFindHandle == null || this.m_safeFindHandle.IsInvalid)
					{
						flag3 = false;
						int lastWin32Error = Marshal.GetLastWin32Error();
						if (lastWin32Error != 2)
						{
							this.ThrowIOException(lastWin32Error, "FindFirstFile");
						}
					}
					else
					{
						flag3 = true;
					}
				}
				else
				{
					flag3 = NativeMethods.FindNextFile(this.m_safeFindHandle, out findData);
					if (!flag3)
					{
						int lastWin32Error2 = Marshal.GetLastWin32Error();
						if (lastWin32Error2 != 18)
						{
							this.ThrowIOException(lastWin32Error2, "FindNextFile");
						}
					}
				}
				if (flag3)
				{
					if ((findData.FileAttributes & NativeMethods.FileAttributes.Directory) != NativeMethods.FileAttributes.None && !this.IsSpecialDirectoryName(findData.FileName) && (extendedFilter == null || extendedFilter(findData)))
					{
						flag2 = true;
					}
					switch (enumType)
					{
					case DirectoryEnumerator.EnumerationType.Files:
						if ((findData.FileAttributes & NativeMethods.FileAttributes.Directory) == NativeMethods.FileAttributes.None && NativeMethods.PathMatchSpec(findData.FileName, filter))
						{
							flag = true;
						}
						break;
					case DirectoryEnumerator.EnumerationType.Directories:
						if (flag2)
						{
							flag = true;
						}
						break;
					case DirectoryEnumerator.EnumerationType.FilesOrDirectories:
						if (flag2)
						{
							flag = true;
						}
						else if ((findData.FileAttributes & NativeMethods.FileAttributes.Directory) == NativeMethods.FileAttributes.None && NativeMethods.PathMatchSpec(findData.FileName, filter))
						{
							flag = true;
						}
						break;
					}
					if (this.m_recurse && flag2)
					{
						this.m_realDirectories.Add(findData.FileName);
					}
					if (flag && extendedFilter != null && !extendedFilter(findData))
					{
						flag = false;
					}
				}
			}
			while (flag3 && !flag);
			if (flag)
			{
				if (this.ReturnBaseNames)
				{
					itemName = findData.FileName;
				}
				else
				{
					itemName = Path.Combine(this.m_path.FullName, findData.FileName);
				}
			}
			else
			{
				this.ResetFindHandle();
			}
			return flag;
		}

		private bool IsSpecialDirectoryName(string name)
		{
			return StringUtil.IsEqualIgnoreCase(name, ".") || StringUtil.IsEqualIgnoreCase(name, "..") || (!this.m_includeRecycleBin && StringUtil.IsEqualIgnoreCase(name, "$RECYCLE.BIN"));
		}

		private void ResetFindHandle()
		{
			if (this.m_safeFindHandle != null)
			{
				this.m_safeFindHandle.Close();
				this.m_safeFindHandle = null;
			}
		}

		private void ThrowIOException(int win32ErrorCode, string apiName)
		{
			Exception ex = new Win32Exception(win32ErrorCode);
			throw new IOException(this.GetIOExceptionMessage(this.m_path.FullName, apiName, ex.Message, win32ErrorCode), NativeMethods.HRESULT_FROM_WIN32((uint)win32ErrorCode));
		}

		public const string FilterAll = "*";

		private const string CurrentDirectory = ".";

		private const string ParentDirectory = "..";

		private const string RecycleBin = "$RECYCLE.BIN";

		public static readonly Predicate<NativeMethods.WIN32_FIND_DATA> ExcludeHiddenAndSystemFilter = (NativeMethods.WIN32_FIND_DATA findData) => (findData.FileAttributes & NativeMethods.FileAttributes.Hidden) == NativeMethods.FileAttributes.None && (findData.FileAttributes & NativeMethods.FileAttributes.System) == NativeMethods.FileAttributes.None && (findData.FileAttributes & NativeMethods.FileAttributes.ReparsePoint) == NativeMethods.FileAttributes.None;

		private readonly DirectoryInfo m_path;

		private readonly bool m_recurse;

		private readonly bool m_includeRecycleBin;

		private SafeFindHandle m_safeFindHandle;

		private List<string> m_realDirectories;

		private enum EnumerationType
		{
			Files,
			Directories,
			FilesOrDirectories
		}
	}
}
