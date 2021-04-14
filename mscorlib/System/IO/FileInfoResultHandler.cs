using System;
using System.Security;
using System.Security.Permissions;
using Microsoft.Win32;

namespace System.IO
{
	internal class FileInfoResultHandler : SearchResultHandler<FileInfo>
	{
		[SecurityCritical]
		internal override bool IsResultIncluded(ref Win32Native.WIN32_FIND_DATA findData)
		{
			return findData.IsFile;
		}

		[SecurityCritical]
		internal override FileInfo CreateObject(Directory.SearchData searchData, ref Win32Native.WIN32_FIND_DATA findData)
		{
			return FileInfoResultHandler.CreateFileInfo(searchData, ref findData);
		}

		[SecurityCritical]
		internal static FileInfo CreateFileInfo(Directory.SearchData searchData, ref Win32Native.WIN32_FIND_DATA findData)
		{
			string cFileName = findData.cFileName;
			string text = Path.CombineNoChecks(searchData.fullPath, cFileName);
			if (!CodeAccessSecurityEngine.QuickCheckForAllDemands())
			{
				new FileIOPermission(FileIOPermissionAccess.Read, new string[]
				{
					text
				}, false, false).Demand();
			}
			FileInfo fileInfo = new FileInfo(text, cFileName);
			fileInfo.InitializeFrom(ref findData);
			return fileInfo;
		}
	}
}
