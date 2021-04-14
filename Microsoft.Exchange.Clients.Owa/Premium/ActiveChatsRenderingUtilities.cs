using System;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium
{
	public static class ActiveChatsRenderingUtilities
	{
		public static void RenderActiveChatsMenu(UserContext userContext, TextWriter output)
		{
			NotificationRenderingUtilities.RenderNotificationMenu(userContext, output, "divActChts", null, -1947596443, -1018465893, null, 1414246128, null, 1414246128, "divBtnOpen", 197744374, null);
		}
	}
}
