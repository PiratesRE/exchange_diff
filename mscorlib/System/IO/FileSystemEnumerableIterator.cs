using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;

namespace System.IO
{
	internal class FileSystemEnumerableIterator<TSource> : Iterator<TSource>
	{
		[SecuritySafeCritical]
		internal FileSystemEnumerableIterator(string path, string originalUserPath, string searchPattern, SearchOption searchOption, SearchResultHandler<TSource> resultHandler, bool checkHost)
		{
			this.oldMode = Win32Native.SetErrorMode(1);
			this.searchStack = new List<Directory.SearchData>();
			string text = FileSystemEnumerableIterator<TSource>.NormalizeSearchPattern(searchPattern);
			if (text.Length == 0)
			{
				this.empty = true;
				return;
			}
			this._resultHandler = resultHandler;
			this.searchOption = searchOption;
			this.fullPath = Path.GetFullPathInternal(path);
			string fullSearchString = FileSystemEnumerableIterator<TSource>.GetFullSearchString(this.fullPath, text);
			this.normalizedSearchPath = Path.GetDirectoryName(fullSearchString);
			if (CodeAccessSecurityEngine.QuickCheckForAllDemands())
			{
				FileIOPermission.EmulateFileIOPermissionChecks(this.fullPath);
				FileIOPermission.EmulateFileIOPermissionChecks(this.normalizedSearchPath);
			}
			else
			{
				new FileIOPermission(FileIOPermissionAccess.PathDiscovery, new string[]
				{
					Directory.GetDemandDir(this.fullPath, true),
					Directory.GetDemandDir(this.normalizedSearchPath, true)
				}, false, false).Demand();
			}
			this._checkHost = checkHost;
			this.searchCriteria = FileSystemEnumerableIterator<TSource>.GetNormalizedSearchCriteria(fullSearchString, this.normalizedSearchPath);
			string directoryName = Path.GetDirectoryName(text);
			string path2 = originalUserPath;
			if (directoryName != null && directoryName.Length != 0)
			{
				path2 = Path.CombineNoChecks(path2, directoryName);
			}
			this.userPath = path2;
			this.searchData = new Directory.SearchData(this.normalizedSearchPath, this.userPath, searchOption);
			this.CommonInit();
		}

		[SecurityCritical]
		private void CommonInit()
		{
			string fileName = Path.InternalCombine(this.searchData.fullPath, this.searchCriteria);
			Win32Native.WIN32_FIND_DATA win32_FIND_DATA = default(Win32Native.WIN32_FIND_DATA);
			this._hnd = Win32Native.FindFirstFile(fileName, ref win32_FIND_DATA);
			if (this._hnd.IsInvalid)
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (lastWin32Error != 2 && lastWin32Error != 18)
				{
					this.HandleError(lastWin32Error, this.searchData.fullPath);
				}
				else
				{
					this.empty = (this.searchData.searchOption == SearchOption.TopDirectoryOnly);
				}
			}
			if (this.searchData.searchOption == SearchOption.TopDirectoryOnly)
			{
				if (this.empty)
				{
					this._hnd.Dispose();
					return;
				}
				if (this._resultHandler.IsResultIncluded(ref win32_FIND_DATA))
				{
					this.current = this._resultHandler.CreateObject(this.searchData, ref win32_FIND_DATA);
					return;
				}
			}
			else
			{
				this._hnd.Dispose();
				this.searchStack.Add(this.searchData);
			}
		}

		[SecuritySafeCritical]
		private FileSystemEnumerableIterator(string fullPath, string normalizedSearchPath, string searchCriteria, string userPath, SearchOption searchOption, SearchResultHandler<TSource> resultHandler, bool checkHost)
		{
			this.fullPath = fullPath;
			this.normalizedSearchPath = normalizedSearchPath;
			this.searchCriteria = searchCriteria;
			this._resultHandler = resultHandler;
			this.userPath = userPath;
			this.searchOption = searchOption;
			this._checkHost = checkHost;
			this.searchStack = new List<Directory.SearchData>();
			if (searchCriteria != null)
			{
				if (CodeAccessSecurityEngine.QuickCheckForAllDemands())
				{
					FileIOPermission.EmulateFileIOPermissionChecks(fullPath);
					FileIOPermission.EmulateFileIOPermissionChecks(normalizedSearchPath);
				}
				else
				{
					new FileIOPermission(FileIOPermissionAccess.PathDiscovery, new string[]
					{
						Directory.GetDemandDir(fullPath, true),
						Directory.GetDemandDir(normalizedSearchPath, true)
					}, false, false).Demand();
				}
				this.searchData = new Directory.SearchData(normalizedSearchPath, userPath, searchOption);
				this.CommonInit();
				return;
			}
			this.empty = true;
		}

		protected override Iterator<TSource> Clone()
		{
			return new FileSystemEnumerableIterator<TSource>(this.fullPath, this.normalizedSearchPath, this.searchCriteria, this.userPath, this.searchOption, this._resultHandler, this._checkHost);
		}

		[SecuritySafeCritical]
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (this._hnd != null)
				{
					this._hnd.Dispose();
				}
			}
			finally
			{
				Win32Native.SetErrorMode(this.oldMode);
				base.Dispose(disposing);
			}
		}

		[SecuritySafeCritical]
		public override bool MoveNext()
		{
			Win32Native.WIN32_FIND_DATA win32_FIND_DATA = default(Win32Native.WIN32_FIND_DATA);
			switch (this.state)
			{
			case 1:
				if (this.empty)
				{
					this.state = 4;
					goto IL_242;
				}
				if (this.searchData.searchOption == SearchOption.TopDirectoryOnly)
				{
					this.state = 3;
					if (this.current != null)
					{
						return true;
					}
					goto IL_173;
				}
				else
				{
					this.state = 2;
				}
				break;
			case 2:
				break;
			case 3:
				goto IL_173;
			case 4:
				goto IL_242;
			default:
				return false;
			}
			IL_156:
			while (this.searchStack.Count > 0)
			{
				this.searchData = this.searchStack[0];
				this.searchStack.RemoveAt(0);
				this.AddSearchableDirsToStack(this.searchData);
				string fileName = Path.InternalCombine(this.searchData.fullPath, this.searchCriteria);
				this._hnd = Win32Native.FindFirstFile(fileName, ref win32_FIND_DATA);
				if (this._hnd.IsInvalid)
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					if (lastWin32Error == 2 || lastWin32Error == 18 || lastWin32Error == 3)
					{
						continue;
					}
					this._hnd.Dispose();
					this.HandleError(lastWin32Error, this.searchData.fullPath);
				}
				this.state = 3;
				this.needsParentPathDiscoveryDemand = true;
				if (this._resultHandler.IsResultIncluded(ref win32_FIND_DATA))
				{
					if (this.needsParentPathDiscoveryDemand)
					{
						this.DoDemand(this.searchData.fullPath);
						this.needsParentPathDiscoveryDemand = false;
					}
					this.current = this._resultHandler.CreateObject(this.searchData, ref win32_FIND_DATA);
					return true;
				}
				goto IL_173;
			}
			this.state = 4;
			goto IL_242;
			IL_173:
			if (this.searchData != null && this._hnd != null)
			{
				while (Win32Native.FindNextFile(this._hnd, ref win32_FIND_DATA))
				{
					if (this._resultHandler.IsResultIncluded(ref win32_FIND_DATA))
					{
						if (this.needsParentPathDiscoveryDemand)
						{
							this.DoDemand(this.searchData.fullPath);
							this.needsParentPathDiscoveryDemand = false;
						}
						this.current = this._resultHandler.CreateObject(this.searchData, ref win32_FIND_DATA);
						return true;
					}
				}
				int lastWin32Error2 = Marshal.GetLastWin32Error();
				if (this._hnd != null)
				{
					this._hnd.Dispose();
				}
				if (lastWin32Error2 != 0 && lastWin32Error2 != 18 && lastWin32Error2 != 2)
				{
					this.HandleError(lastWin32Error2, this.searchData.fullPath);
				}
			}
			if (this.searchData.searchOption != SearchOption.TopDirectoryOnly)
			{
				this.state = 2;
				goto IL_156;
			}
			this.state = 4;
			IL_242:
			base.Dispose();
			return false;
		}

		[SecurityCritical]
		private void HandleError(int hr, string path)
		{
			base.Dispose();
			__Error.WinIOError(hr, path);
		}

		[SecurityCritical]
		private void AddSearchableDirsToStack(Directory.SearchData localSearchData)
		{
			string fileName = Path.InternalCombine(localSearchData.fullPath, "*");
			SafeFindHandle safeFindHandle = null;
			Win32Native.WIN32_FIND_DATA win32_FIND_DATA = default(Win32Native.WIN32_FIND_DATA);
			try
			{
				safeFindHandle = Win32Native.FindFirstFile(fileName, ref win32_FIND_DATA);
				if (safeFindHandle.IsInvalid)
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					if (lastWin32Error == 2 || lastWin32Error == 18 || lastWin32Error == 3)
					{
						return;
					}
					this.HandleError(lastWin32Error, localSearchData.fullPath);
				}
				int num = 0;
				do
				{
					if (win32_FIND_DATA.IsNormalDirectory)
					{
						string cFileName = win32_FIND_DATA.cFileName;
						string text = Path.CombineNoChecks(localSearchData.fullPath, cFileName);
						string text2 = Path.CombineNoChecks(localSearchData.userPath, cFileName);
						SearchOption searchOption = localSearchData.searchOption;
						Directory.SearchData item = new Directory.SearchData(text, text2, searchOption);
						this.searchStack.Insert(num++, item);
					}
				}
				while (Win32Native.FindNextFile(safeFindHandle, ref win32_FIND_DATA));
			}
			finally
			{
				if (safeFindHandle != null)
				{
					safeFindHandle.Dispose();
				}
			}
		}

		[SecurityCritical]
		internal void DoDemand(string fullPathToDemand)
		{
			string demandDir = Directory.GetDemandDir(fullPathToDemand, true);
			FileIOPermission.QuickDemand(FileIOPermissionAccess.PathDiscovery, demandDir, false, false);
		}

		private static string NormalizeSearchPattern(string searchPattern)
		{
			string text = searchPattern.TrimEnd(Path.TrimEndChars);
			if (text.Equals("."))
			{
				text = "*";
			}
			Path.CheckSearchPattern(text);
			return text;
		}

		private static string GetNormalizedSearchCriteria(string fullSearchString, string fullPathMod)
		{
			char c = fullPathMod[fullPathMod.Length - 1];
			string result;
			if (Path.IsDirectorySeparator(c))
			{
				result = fullSearchString.Substring(fullPathMod.Length);
			}
			else
			{
				result = fullSearchString.Substring(fullPathMod.Length + 1);
			}
			return result;
		}

		private static string GetFullSearchString(string fullPath, string searchPattern)
		{
			string text = Path.InternalCombine(fullPath, searchPattern);
			char c = text[text.Length - 1];
			if (Path.IsDirectorySeparator(c) || c == Path.VolumeSeparatorChar)
			{
				text += "*";
			}
			return text;
		}

		private const int STATE_INIT = 1;

		private const int STATE_SEARCH_NEXT_DIR = 2;

		private const int STATE_FIND_NEXT_FILE = 3;

		private const int STATE_FINISH = 4;

		private SearchResultHandler<TSource> _resultHandler;

		private List<Directory.SearchData> searchStack;

		private Directory.SearchData searchData;

		private string searchCriteria;

		[SecurityCritical]
		private SafeFindHandle _hnd;

		private bool needsParentPathDiscoveryDemand;

		private bool empty;

		private string userPath;

		private SearchOption searchOption;

		private string fullPath;

		private string normalizedSearchPath;

		private int oldMode;

		private bool _checkHost;
	}
}
