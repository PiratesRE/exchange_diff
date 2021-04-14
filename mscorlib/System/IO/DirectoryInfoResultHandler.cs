using System;
using System.Security;
using System.Security.Permissions;
using Microsoft.Win32;

namespace System.IO
{
	internal class DirectoryInfoResultHandler : SearchResultHandler<DirectoryInfo>
	{
		[SecurityCritical]
		internal override bool IsResultIncluded(ref Win32Native.WIN32_FIND_DATA findData)
		{
			return findData.IsNormalDirectory;
		}

		[SecurityCritical]
		internal override DirectoryInfo CreateObject(Directory.SearchData searchData, ref Win32Native.WIN32_FIND_DATA findData)
		{
			return DirectoryInfoResultHandler.CreateDirectoryInfo(searchData, ref findData);
		}

		[SecurityCritical]
		internal static DirectoryInfo CreateDirectoryInfo(Directory.SearchData searchData, ref Win32Native.WIN32_FIND_DATA findData)
		{
			string cFileName = findData.cFileName;
			string text = Path.CombineNoChecks(searchData.fullPath, cFileName);
			if (!CodeAccessSecurityEngine.QuickCheckForAllDemands())
			{
				new FileIOPermission(FileIOPermissionAccess.Read, new string[]
				{
					text + "\\."
				}, false, false).Demand();
			}
			DirectoryInfo directoryInfo = new DirectoryInfo(text, cFileName);
			directoryInfo.InitializeFrom(ref findData);
			return directoryInfo;
		}
	}
}
