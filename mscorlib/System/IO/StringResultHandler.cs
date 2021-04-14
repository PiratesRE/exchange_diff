using System;
using System.Security;
using Microsoft.Win32;

namespace System.IO
{
	internal class StringResultHandler : SearchResultHandler<string>
	{
		internal StringResultHandler(bool includeFiles, bool includeDirs)
		{
			this._includeFiles = includeFiles;
			this._includeDirs = includeDirs;
		}

		[SecurityCritical]
		internal override bool IsResultIncluded(ref Win32Native.WIN32_FIND_DATA findData)
		{
			return (this._includeFiles && findData.IsFile) || (this._includeDirs && findData.IsNormalDirectory);
		}

		[SecurityCritical]
		internal override string CreateObject(Directory.SearchData searchData, ref Win32Native.WIN32_FIND_DATA findData)
		{
			return Path.CombineNoChecks(searchData.userPath, findData.cFileName);
		}

		private bool _includeFiles;

		private bool _includeDirs;
	}
}
