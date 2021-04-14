using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Clients.Owa.Core;

namespace Microsoft.Exchange.Clients.Owa.Premium.Controls
{
	public sealed class DefaultFlagMenu : ContextMenu
	{
		public static DefaultFlagMenu Create(UserContext userContext)
		{
			return new DefaultFlagMenu(userContext);
		}

		private DefaultFlagMenu(UserContext userContext) : base("divDftFM", userContext, true)
		{
		}

		protected override void RenderMenuItems(TextWriter output)
		{
			if (output == null)
			{
				throw new ArgumentNullException("output");
			}
			base.RenderMenuItem(output, -367521373, (this.userContext.UserOptions.FlagAction == FlagAction.Today) ? ThemeFileId.Flag : ThemeFileId.None, "divDFA" + DefaultFlagMenu.today, "d" + DefaultFlagMenu.today);
			base.RenderMenuItem(output, 1854511297, (this.userContext.UserOptions.FlagAction == FlagAction.Tomorrow) ? ThemeFileId.Flag : ThemeFileId.None, "divDFA" + DefaultFlagMenu.tomorrow, "d" + DefaultFlagMenu.tomorrow);
			base.RenderMenuItem(output, -1821673574, (this.userContext.UserOptions.FlagAction == FlagAction.ThisWeek) ? ThemeFileId.Flag : ThemeFileId.None, "divDFA" + DefaultFlagMenu.thisWeek, "d" + DefaultFlagMenu.thisWeek);
			base.RenderMenuItem(output, 1433854051, (this.userContext.UserOptions.FlagAction == FlagAction.NextWeek) ? ThemeFileId.Flag : ThemeFileId.None, "divDFA" + DefaultFlagMenu.nextWeek, "d" + DefaultFlagMenu.nextWeek);
			base.RenderMenuItem(output, 689037121, (this.userContext.UserOptions.FlagAction == FlagAction.NoDate) ? ThemeFileId.Flag : ThemeFileId.None, "divDFA" + DefaultFlagMenu.noDate, "d" + DefaultFlagMenu.noDate);
		}

		private static readonly string today = 2.ToString(CultureInfo.InvariantCulture);

		private static readonly string tomorrow = 3.ToString(CultureInfo.InvariantCulture);

		private static readonly string thisWeek = 4.ToString(CultureInfo.InvariantCulture);

		private static readonly string nextWeek = 5.ToString(CultureInfo.InvariantCulture);

		private static readonly string noDate = 6.ToString(CultureInfo.InvariantCulture);
	}
}
