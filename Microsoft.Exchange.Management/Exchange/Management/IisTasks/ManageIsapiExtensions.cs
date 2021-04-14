using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Management.Metabase;

namespace Microsoft.Exchange.Management.IisTasks
{
	internal static class ManageIsapiExtensions
	{
		internal static void SetStatus(string hostName, string groupID, string extensionBinary, bool allow)
		{
			using (IsapiExtensionList isapiExtensionList = new IsapiExtensionList(hostName))
			{
				List<int> list = isapiExtensionList.FindMatchingExtensions(groupID, extensionBinary);
				if (list.Count == 0)
				{
					throw new ManageIsapiExtensionCouldNotFindExtensionException(groupID, extensionBinary);
				}
				if (list.Count != 1)
				{
					StringBuilder stringBuilder = new StringBuilder();
					for (int i = 0; i < list.Count; i++)
					{
						stringBuilder.Append(isapiExtensionList[i].ToMetabaseString());
						stringBuilder.Append("\r\n");
					}
					throw new ManageIsapiExtensionFoundMoreThanOneExtensionException(groupID, extensionBinary, stringBuilder.ToString());
				}
				IsapiExtension isapiExtension = isapiExtensionList[list[0]];
				if (isapiExtension.Allow != allow)
				{
					isapiExtensionList[list[0]] = new IsapiExtension(isapiExtension.PhysicalPath, isapiExtension.GroupID, isapiExtension.Description, allow, isapiExtension.UIDeletable);
					isapiExtensionList.CommitChanges();
					IisUtility.CommitMetabaseChanges(hostName);
				}
			}
		}
	}
}
