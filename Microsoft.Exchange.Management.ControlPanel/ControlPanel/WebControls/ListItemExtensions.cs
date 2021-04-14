using System;
using System.Security.Principal;
using System.Web.UI.WebControls;

namespace Microsoft.Exchange.Management.ControlPanel.WebControls
{
	public static class ListItemExtensions
	{
		public static bool IsAccessibleToUser(this ListItem listItem, IPrincipal user)
		{
			string text = listItem.Attributes["Roles"];
			return string.IsNullOrEmpty(text) || LoginUtil.IsInRoles(user, text.Split(new char[]
			{
				','
			}));
		}

		private const string Roles = "Roles";
	}
}
