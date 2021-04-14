using System;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Basic.Controls
{
	internal class RenderingFlags
	{
		public static bool EnableAllFolderNavigation(UserContext userContext)
		{
			return RenderingFlags.GetFlag(userContext, RenderingFlags.Flags.EnableAllFolderNavigation);
		}

		public static bool HideOutOfOfficeInfoBar(UserContext userContext)
		{
			return RenderingFlags.GetFlag(userContext, RenderingFlags.Flags.HideOutOfOfficeInfobar);
		}

		public static void EnableAllFolderNavigation(UserContext userContext, bool value)
		{
			RenderingFlags.SetFlag(userContext, RenderingFlags.Flags.EnableAllFolderNavigation, value);
		}

		public static void HideOutOfOfficeInfoBar(UserContext userContext, bool value)
		{
			RenderingFlags.SetFlag(userContext, RenderingFlags.Flags.HideOutOfOfficeInfobar, value);
		}

		private static bool GetFlag(UserContext userContext, RenderingFlags.Flags flag)
		{
			return (userContext.RenderingFlags & (int)flag) != 0;
		}

		private static void SetFlag(UserContext userContext, RenderingFlags.Flags flag, bool value)
		{
			if (value)
			{
				userContext.RenderingFlags |= (int)flag;
				return;
			}
			userContext.RenderingFlags &= (int)(~(int)flag);
		}

		[Flags]
		private enum Flags
		{
			EnableAllFolderNavigation = 1,
			HideOutOfOfficeInfobar = 2
		}
	}
}
