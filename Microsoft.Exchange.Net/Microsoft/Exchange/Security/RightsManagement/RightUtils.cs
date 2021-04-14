using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Security.RightsManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class RightUtils
	{
		public static bool IsUsageRightGranted(this ContentRight grantedRights, ContentRight usageRight)
		{
			return (grantedRights & ContentRight.Owner) == ContentRight.Owner || (grantedRights & usageRight) == usageRight;
		}

		internal static ContentRight GetRightFromName(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			switch (name)
			{
			case "DOCEDIT":
				return ContentRight.DocumentEdit;
			case "EDIT":
				return ContentRight.Edit;
			case "EXPORT":
				return ContentRight.Export;
			case "EXTRACT":
				return ContentRight.Extract;
			case "FORWARD":
				return ContentRight.Forward;
			case "OBJMODEL":
				return ContentRight.ObjectModel;
			case "OWNER":
				return ContentRight.Owner;
			case "PRINT":
				return ContentRight.Print;
			case "VIEW":
				return ContentRight.View;
			case "VIEWRIGHTSDATA":
				return ContentRight.ViewRightsData;
			case "REPLY":
				return ContentRight.Reply;
			case "REPLYALL":
				return ContentRight.ReplyAll;
			case "SIGN":
				return ContentRight.Sign;
			case "EDITRIGHTSDATA":
				return ContentRight.EditRightsData;
			}
			return ContentRight.None;
		}

		internal static ICollection<string> GetIndividualRightNames(ContentRight rights)
		{
			List<string> list = new List<string>(14);
			if ((rights & ContentRight.DocumentEdit) == ContentRight.DocumentEdit)
			{
				list.Add("DOCEDIT");
			}
			if ((rights & ContentRight.Edit) == ContentRight.Edit)
			{
				list.Add("EDIT");
			}
			if ((rights & ContentRight.Export) == ContentRight.Export)
			{
				list.Add("EXPORT");
			}
			if ((rights & ContentRight.Extract) == ContentRight.Extract)
			{
				list.Add("EXTRACT");
			}
			if ((rights & ContentRight.Forward) == ContentRight.Forward)
			{
				list.Add("FORWARD");
			}
			if ((rights & ContentRight.ObjectModel) == ContentRight.ObjectModel)
			{
				list.Add("OBJMODEL");
			}
			if ((rights & ContentRight.Owner) == ContentRight.Owner)
			{
				list.Add("OWNER");
			}
			if ((rights & ContentRight.Print) == ContentRight.Print)
			{
				list.Add("PRINT");
			}
			if ((rights & ContentRight.View) == ContentRight.View)
			{
				list.Add("VIEW");
			}
			if ((rights & ContentRight.ViewRightsData) == ContentRight.ViewRightsData)
			{
				list.Add("VIEWRIGHTSDATA");
			}
			if ((rights & ContentRight.Reply) == ContentRight.Reply)
			{
				list.Add("REPLY");
			}
			if ((rights & ContentRight.ReplyAll) == ContentRight.ReplyAll)
			{
				list.Add("REPLYALL");
			}
			if ((rights & ContentRight.Sign) == ContentRight.Sign)
			{
				list.Add("SIGN");
			}
			if ((rights & ContentRight.EditRightsData) == ContentRight.EditRightsData)
			{
				list.Add("EDITRIGHTSDATA");
			}
			return list;
		}

		private const int KnownRightsCount = 14;
	}
}
