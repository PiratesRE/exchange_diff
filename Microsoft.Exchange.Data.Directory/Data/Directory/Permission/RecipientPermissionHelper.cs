using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Reflection;

namespace Microsoft.Exchange.Data.Directory.Permission
{
	internal static class RecipientPermissionHelper
	{
		private static Dictionary<RecipientAccessRight, Guid> RecipientAccessRightGuidMap { get; set; } = new Dictionary<RecipientAccessRight, Guid>();

		static RecipientPermissionHelper()
		{
			foreach (FieldInfo fieldInfo in typeof(RecipientAccessRight).GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField))
			{
				RightGuidAttribute[] array = (RightGuidAttribute[])fieldInfo.GetCustomAttributes(typeof(RightGuidAttribute), false);
				RecipientAccessRight key = (RecipientAccessRight)fieldInfo.GetValue(null);
				RecipientPermissionHelper.RecipientAccessRightGuidMap[key] = array[0].Guid;
			}
		}

		internal static Guid GetRecipientAccessRightGuid(RecipientAccessRight right)
		{
			return RecipientPermissionHelper.RecipientAccessRightGuidMap[right];
		}

		internal static RecipientAccessRight? GetRecipientAccessRight(ActiveDirectoryAccessRule ace)
		{
			if ((ace.ActiveDirectoryRights & ActiveDirectoryRights.ExtendedRight) == ActiveDirectoryRights.ExtendedRight)
			{
				foreach (RecipientAccessRight recipientAccessRight in RecipientPermissionHelper.RecipientAccessRightGuidMap.Keys)
				{
					if (ace.ObjectType == RecipientPermissionHelper.RecipientAccessRightGuidMap[recipientAccessRight])
					{
						return new RecipientAccessRight?(recipientAccessRight);
					}
				}
			}
			return null;
		}
	}
}
