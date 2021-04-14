using System;
using System.Security;
using Microsoft.Win32;

namespace System.IO
{
	internal class FileSystemInfoResultHandler : SearchResultHandler<FileSystemInfo>
	{
		[SecurityCritical]
		internal override bool IsResultIncluded(ref Win32Native.WIN32_FIND_DATA findData)
		{
			return findData.IsFile || findData.IsNormalDirectory;
		}

		[SecurityCritical]
		internal override FileSystemInfo CreateObject(Directory.SearchData searchData, ref Win32Native.WIN32_FIND_DATA findData)
		{
			if (!findData.IsFile)
			{
				return DirectoryInfoResultHandler.CreateDirectoryInfo(searchData, ref findData);
			}
			return FileInfoResultHandler.CreateFileInfo(searchData, ref findData);
		}
	}
}
